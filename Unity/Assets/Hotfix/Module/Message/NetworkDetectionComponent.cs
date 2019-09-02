using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class NetworkDetectionAwakeSystem : AwakeSystem<NetworkDetectionComponent>
    {
        public override void Awake(NetworkDetectionComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class NetworkDetectionUpdateSystem : UpdateSystem<NetworkDetectionComponent>
    {
        public override void Update(NetworkDetectionComponent self)
        {
            self.Update();
        }
    }

    public class NetworkDetectionComponent : Component
    {
        public static NetworkDetectionComponent Instance;

        private string LOG_TAG = "Hotfix_NetworkDetectionComponent";

        public float sendInterval = 1f;
        private float recordDeltaTime = 0f;

        public float beginReconnectTime = 0f;
        public float reconnectTimeOut = 5f;

        private bool netAvailable = true;  // 网络是否可用
        private bool fromBack = false;

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            ETModel.Game.Hotfix.OnApplicationPauseTrue -= OnApplicationPauseTrue;
            ETModel.Game.Hotfix.OnApplicationPauseFalse -= OnApplicationPauseFalse;

            base.Dispose();
        }

        public void Awake()
        {
            Instance = this;
            ETModel.Game.Hotfix.OnApplicationPauseTrue += OnApplicationPauseTrue;
            ETModel.Game.Hotfix.OnApplicationPauseFalse += OnApplicationPauseFalse;

            recordDeltaTime = 0;
        }

        public void Update()
        {
            // 每秒检测一次
            if (!(Time.time - recordDeltaTime > sendInterval))
            {
                return;
            }
            recordDeltaTime = Time.time;

            bool isReconentTimeOut = false;
            if (beginReconnectTime > 0 && Time.time - beginReconnectTime > reconnectTimeOut)
            {
                isReconentTimeOut = true;
            }

            if (UnityEngine.Application.internetReachability != 0)
            {
                if (!netAvailable || fromBack || isReconentTimeOut)
                {
                    UIComponent.Instance.Prompt();
                    if (!netAvailable)
                    {
                        UI mPromptWifi = UIComponent.Instance.Get(UIType.UIPromptWifi);
                        if (null != mPromptWifi)
                        {
                            UIComponent.Instance.Remove(UIType.UIPromptWifi);
                        }
                    }
                    netAvailable = true;
                    fromBack = false;
                    beginReconnectTime = Time.time;

                    NetworkUtil.RemoveAllSessionComponent();

                    // 登录界面不需要重连
                    UI mUILogin = UIComponent.Instance.Get(UIType.UILogin);
                    if (null != mUILogin && mUILogin.GameObject.activeInHierarchy)
                    {
                        UIComponent.Instance.ClosePrompt();
                        beginReconnectTime = 0;
                        return;
                    }

                    // Login服
                    // todo 登录
                }
            }
            else
            {
                if (netAvailable)
                {
                    netAvailable = false;
                    NetworkUtil.RemoveAllSessionComponent();

                    // UIComponent.Instance.Toast("请检查网络");
                    UIComponent.Instance.ShowNoAnimation(UIType.UIPromptWifi);
                }
            }
        }

        private void OnApplicationPauseTrue()
        {
            // 暂停
            if (Application.platform == RuntimePlatform.Android)
            {
                NetworkUtil.RemoveAllSessionComponent();
                fromBack = false;
            }
        }

        private void OnApplicationPauseFalse()
        {
            // 恢复
            if (Application.platform == RuntimePlatform.Android)
            {
                fromBack = true;
            }
        }

        public void Reconnect()
        {
            NetworkUtil.RemoveAllSessionComponent();
            fromBack = true;
        }

        private void GotoLoginPage()
        {
            NetworkUtil.LogoutClear();
            UIComponent.Instance.RemoveAll(new List<string>() { UIType.UIMarquee });
            UIComponent.Instance.ShowNoAnimation(UIType.UILogin, "NoAutoLogin");
        }
    }
}
