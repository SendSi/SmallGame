using System;
using System.Threading;
using UnityEngine;

namespace ETModel
{
    public class Init : MonoBehaviour
    {
        private void Start()
        {
            this.StartAsync().Coroutine();
        }

        private async ETVoid StartAsync()
        {
            try
            {
                Application.runInBackground = true;
#if UNITY_IOS || UNITY_ANDROID
                // Application.runInBackground = true;
                // Application.targetFrameRate = 60;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

#if LOG_ZSX
                var nodes = GameObject.Find("LoginCanvas");
                if (nodes != null)
                {
                    var node = nodes.GetComponentsInChildren<RectTransform>();
                    for (int i = 0; i < node.Length; i++)
                    {
                        node[i].gameObject.SetActive(false);
                    }
                    nodes.SetActive(true);
                }
#endif

                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

                DontDestroyOnLoad(gameObject);
                Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

                Game.Scene.AddComponent<TimerComponent>();
                Game.Scene.AddComponent<GlobalConfigComponent>();
                Game.Scene.AddComponent<NetOuterComponent>();
                Game.Scene.AddComponent<ResourcesComponent>();
                Game.Scene.AddComponent<LocalResourcesComponent>();
                // Game.Scene.AddComponent<PlayerComponent>();
                // Game.Scene.AddComponent<UnitComponent>();

                Game.Scene.AddComponent<NetHttpComponent>();

                Game.Scene.AddComponent<UIComponent>();
                Game.Scene.AddComponent<NetLineSwitchComponent>();

#if UNITY_IOS
                TimerComponent iOSTimerComponent = Game.Scene.GetComponent<TimerComponent>();
                while (true)
                {
                    await iOSTimerComponent.WaitAsync(200);
                    if (UnityEngine.Application.internetReachability != NetworkReachability.NotReachable)
                        break;
                }
#endif

                // 安装包更新
                await InstallPacketHelper.CheckInstallPacket();
                if (InstallPacketHelper.NeedInstallPacket())
                {
                    InstallPacketDownloaderComponent mInstallPacketDownloaderComponent = Game.Scene.GetComponent<InstallPacketDownloaderComponent>();

                    if (null != mInstallPacketDownloaderComponent)
                    {
                        bool mWaitting = true;

                        string[] mArr = mInstallPacketDownloaderComponent.remoteInstallPacketConfig.Msg.Split(new string[] { "@%" }, StringSplitOptions.None);
                        int mTmpLanguage = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.KEY_LANGUAGE, 2);
                        if (mTmpLanguage < 0)
                            mTmpLanguage = 0;
                        int mGroup = mArr.Length / 3;
                        if (mTmpLanguage < mGroup)
                        {
                            UIComponent.Instance.ShowNoAnimation(UIType.UIDialog,
                                                                 new UIDialogComponent.DialogData()
                                                                 {
                                                                     type = UIDialogComponent.DialogData.DialogType.Commit,
                                                                     title = mArr[mTmpLanguage],
                                                                     content = mArr[mTmpLanguage + mGroup],
                                                                     contentCommit = mArr[mTmpLanguage + 2 * mGroup],
                                                                     contentCancel = string.Empty,
                                                                     actionCommit = () => { mWaitting = false; },
                                                                     actionCancel = null
                                                                 });

                            TimerComponent mUpdateTimerComponent = Game.Scene.GetComponent<TimerComponent>();
                            while (mWaitting)
                            {
                                await mUpdateTimerComponent.WaitAsync(200);
                            }
                            UIComponent.Instance.Remove(UIType.UIDialog);
                        }
                    }

#if UNITY_ANDROID
                    switch (UnityEngine.Application.internetReachability)
                    {
                        case NetworkReachability.ReachableViaCarrierDataNetwork:
                            // "当前为运行商网络(2g、3g、4g)"
                            string mTmpMsg = string.Empty;
                            int mTmpLanguage = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.KEY_LANGUAGE, 2);
                            switch (mTmpLanguage)
                            {
                                case 0:
                                    mTmpMsg = $"当前使用移动数据,需要消耗{mInstallPacketDownloaderComponent.remoteInstallPacketConfig.ApkSize / (1024 * 1024)}M流量,是否继续下载?";
                                    break;
                                case 1:
                                    mTmpMsg = $"The current use of mobile data, need to consume {mInstallPacketDownloaderComponent.remoteInstallPacketConfig.ApkSize / (1024 * 1024)} M, whether to download?";
                                    break;
                                case 2:
                                    mTmpMsg = $"當前使用移動數據,需要消耗{mInstallPacketDownloaderComponent.remoteInstallPacketConfig.ApkSize / (1024 * 1024)}M流量,是否繼續下載?";
                                    break;
                            }
                            Game.Scene.GetComponent<UIComponent>().Show(UIType.UICarrierDataNetwork, new UICarrierDataNetworkComponent.UICarrierDataNetworkData()
                            {
                                Msg = mTmpMsg,
                                Callback = async () =>
                                {
                                    await InstallPacketHelper.DownloadInstallPacket();
                                    // Log.Debug($"下载完啦!");
                                    // todo 调起安装
                                    NativeManager.OpenApk(InstallPacketHelper.InstallPacketPath());
                                }
                            }, null, 0);
                            break;
                        case NetworkReachability.ReachableViaLocalAreaNetwork:
                            // wifi网络
                            await InstallPacketHelper.DownloadInstallPacket();
                            // todo 调起安装
                            NativeManager.OpenApk(InstallPacketHelper.InstallPacketPath());
                            return;
                    }

#elif UNITY_IOS
                    UnityEngine.Application.OpenURL(mInstallPacketDownloaderComponent.remoteInstallPacketConfig.IOSUrl);
                    Application.Quit();
#endif
                    return;
                }






                // 下载ab包
                await BundleHelper.CheckDownloadBundle();
                BundleHelper.IsDownloadBundleFinish = false;
                if (BundleHelper.NeedDownloadBundle())
                {
#if UNITY_IOS
                    bool AppStorePack = false;
#if APPStore
                    AppStorePack = true;
#endif
                    InstallPacketDownloaderComponent mInstallPacketDownloaderComponent = Game.Scene.GetComponent<InstallPacketDownloaderComponent>();
                    if (!mInstallPacketDownloaderComponent.remoteInstallPacketConfig.CheckRes && AppStorePack)
                    {
                        BundleHelper.DownloadBundleFinish();
                    }
                    else
#endif
                    {
                        BundleDownloaderComponent mBundleDownloaderComponent = Game.Scene.GetComponent<BundleDownloaderComponent>();
                        switch (UnityEngine.Application.internetReachability)
                        {
                            case NetworkReachability.ReachableViaCarrierDataNetwork:
                                // "当前为运行商网络(2g、3g、4g)"
                                string mTmpMsg = string.Empty;
                                int mTmpLanguage = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.KEY_LANGUAGE, 2);
                                switch (mTmpLanguage)
                                {
                                    case 0:
                                        mTmpMsg = $"当前使用移动数据,需要消耗{mBundleDownloaderComponent.TotalSize / (1024 * 1024)}M流量,是否继续下载?";
                                        break;
                                    case 1:
                                        mTmpMsg = $"The current use of mobile data, need to consume {mBundleDownloaderComponent.TotalSize / (1024 * 1024)} M, whether to download?";
                                        break;
                                    case 2:
                                        mTmpMsg = $"當前使用移動數據,需要消耗{mBundleDownloaderComponent.TotalSize / (1024 * 1024)}M流量,是否繼續下載?";
                                        break;
                                }
                                Game.Scene.GetComponent<UIComponent>().ShowNoAnimation(UIType.UICarrierDataNetwork, new UICarrierDataNetworkComponent.UICarrierDataNetworkData()
                                {
                                    Msg = mTmpMsg,
                                    Callback = async () =>
                                    {
                                        await BundleHelper.DownloadBundle();
                                        BundleHelper.IsDownloadBundleFinish = true;
                                    },
                                });
                                TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();
                                while (true)
                                {
                                    await timerComponent.WaitAsync(1000);
                                    if (BundleHelper.IsDownloadBundleFinish)
                                        break;
                                }
                                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICarrierDataNetwork);
                                break;
                            case NetworkReachability.ReachableViaLocalAreaNetwork:
                                // wifi网络
                                await BundleHelper.DownloadBundle();
                                break;
                        }
                    }
                }
                else
                {
                    BundleHelper.DownloadBundleFinish();
                }

                Game.Hotfix.LoadHotfixAssembly();

                // 加载配置
                Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
                Game.Scene.AddComponent<ConfigComponent>();
                Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
                Game.Scene.AddComponent<OpcodeTypeComponent>();
                Game.Scene.AddComponent<MessageDispatcherComponent>();

                Game.Hotfix.GotoHotfix();

                // Game.EventSystem.Run(EventIdType.TestHotfixSubscribMonoEvent, "TestHotfixSubscribMonoEvent");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void Update()
        {
            OneThreadSynchronizationContext.Instance.Update();
            Game.Hotfix.Update?.Invoke();
            Game.EventSystem.Update();
        }

        private void LateUpdate()
        {
            Game.Hotfix.LateUpdate?.Invoke();
            Game.EventSystem.LateUpdate();
        }

        private void OnApplicationQuit()
        {
            Game.Hotfix.OnApplicationQuit?.Invoke();
            Game.Close();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                //this.LogInfo("游戏进入了后台=》  游戏暂停 一切停止");  // Home到桌面或者打进电话等不适前台的时候触Game.Hotfix.OnApplicationPauseTrue?.Invoke();          
                Game.Hotfix.OnApplicationPauseTrue?.Invoke();
            }
            else
            {
                //this.LogInfo("游戏回到了前台  继续监听");  //回到游戏前台的时候触发 最后执行是Game.Hotfix.OnApplicationPauseFalse?.Invoke();
                Game.Hotfix.OnApplicationPauseFalse?.Invoke();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Game.Hotfix.OnApplicationFocusTrue?.Invoke();
            }
            else
            {
                Game.Hotfix.OnApplicationFocusFalse?.Invoke();
            }
        }
    }
}