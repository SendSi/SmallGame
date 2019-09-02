using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	public static class Init
	{
		public static void Start()
		{
#if ILRuntime
			if (!Define.IsILRuntime)
			{
				Log.Error("mono层是mono模式, 但是Hotfix层是ILRuntime模式");
			}
#else
			if (Define.IsILRuntime)
			{
				Log.Error("mono层是ILRuntime模式, Hotfix层是mono模式");
			}
#endif
			
			try
			{
				// 注册热更层回调
				ETModel.Game.Hotfix.Update = () => { Update(); };
				ETModel.Game.Hotfix.LateUpdate = () => { LateUpdate(); };
				ETModel.Game.Hotfix.OnApplicationQuit = () => { OnApplicationQuit(); };
                ETModel.Game.Hotfix.OnApplicationPauseTrue = () => { OnApplicationPauseTrue(); };
                ETModel.Game.Hotfix.OnApplicationPauseFalse = () => { OnApplicationPauseFalse(); };
                ETModel.Game.Hotfix.OnApplicationFocusTrue = () => { OnApplicationFocusTrue(); };
                ETModel.Game.Hotfix.OnApplicationFocusFalse = () => { OnApplicationFocusFalse(); };
                ETModel.Game.Hotfix.OnGetHeadImagePath = (imagePath) => { };
                ETModel.Game.Hotfix.OnGetGPS = (gpsInfo) => { OnGetGPS(gpsInfo); };
                ETModel.Game.Hotfix.OnAwakeByURL = (gpsInfo) => { OnAwakeByURL(gpsInfo); };

                Game.Scene.AddComponent<UIComponent>();
				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatcherComponent>();
				Game.Scene.AddComponent<SoundComponent>();
				Game.Scene.AddComponent<HttpRequestComponent>();

                //添加sdk
                //Game.Scene.AddComponent<BuglySdkComponent>();
                //Game.Scene.AddComponent<TalkingDataSdkComponent>();
                //Game.Scene.AddComponent<JPushSdkComponent>();
                //Game.Scene.AddComponent<IMSdkComponent>();
                //Game.Scene.AddComponent<KeFuSdkComponent>();


                // 加载热更配置
                ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
				Game.Scene.AddComponent<ConfigComponent>();
				ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");

                Game.Scene.AddComponent<DBAreaComponent>();

                Game.Scene.AddComponent<SensitiveWordComponent>();

                Game.EventSystem.Run(EventIdType.InitSceneStart);

                Game.Scene.AddComponent<NetworkDetectionComponent>();

                if (!Application.isEditor)
                {
                    NativeManager.GetDeviceSafeArea();
                    NativeManager.IntAwakeByURLObserver();
                }

                ObtainConfigMsg();
            }
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void Update()
		{
			try
			{
				Game.EventSystem.Update();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void LateUpdate()
		{
			try
			{
				Game.EventSystem.LateUpdate();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void OnApplicationQuit()
		{
			Game.Close();
		}

        public static void OnApplicationPauseTrue()
        {

        }

        public static void OnApplicationPauseFalse()
        {
            ObtainConfigMsg();
        }

        public static void OnApplicationFocusTrue()
        {

        }

        public static void OnApplicationFocusFalse()
        {

        }

        public static void ObtainConfigMsg()
        {
            //刷新系统配置信息
            // HttpRequestComponent.Instance.Send(
            //     WEB2_system_config_query.API,
            //     WEB2_system_config_query.Request(new WEB2_system_config_query.RequestData()), json =>
            //     {
            //         var tDto = WEB2_system_config_query.Response(json);
            //         // Log.Debug($"{tDto}");
            //     });
        }

        public static void NativeCallUnity(string jsonContent)
        {
            NativeManager.ResolverForNativeMsg(jsonContent);
        }

        public static void OnGetGPS(string gpsInfo)
        {
            string[] mInfns = gpsInfo.Split(new string[] { "," }, StringSplitOptions.None);
            string status = mInfns[0]; // 状态 0 成功 1用户关闭GPS权限 2其他错误
            string longitude = mInfns[1];
            string latitude = mInfns[2];
            string locationName = mInfns[3];

            if (status.Equals("0"))
            {
                GameCache.Instance.longitude = longitude;
                GameCache.Instance.latitude = latitude;
                GameCache.Instance.locationName = locationName;
            }
            else
            {
                UIComponent.Instance.Toast($"请开启GPS");
                GameCache.Instance.longitude = string.Empty;
                GameCache.Instance.latitude = string.Empty;
                GameCache.Instance.locationName = string.Empty;
            }

            Game.EventSystem.Run(EventIdType.GPSCallbackSitdown, status);
        }

        public static void OnAwakeByURL(string awakeContent)
        {

        }
    }
}