using System;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class NetLineSwitchComponentAwakeSystem : AwakeSystem<NetLineSwitchComponent>
    {
        public override void Awake(NetLineSwitchComponent self)
        {
            self.Awake();
        }
    }

    public class NetLineSwitchComponent : Component
    {
        public static NetLineSwitchComponent Instance;

        public void Awake()
        {
            Instance = this;
            Game.Hotfix.OnApplicationPauseFalse += OnApplicationPauseFalse;
            Refresh();
        }

        public sealed class ResponseData
        {
            public int status;
            public string data;
        }

        public sealed class NetLineSwitchData
        {
            public List<LineInfo> list;
            public int selector;
        }

        public sealed class LineInfo
        {
            public int id;
            public string http;
            public string sck;
            public string res;
        }

        private void OnApplicationPauseFalse()
        {
            Refresh();
        }

        public async void Refresh()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                TimerComponent iOSTimerComponent = Game.Scene.GetComponent<TimerComponent>();
                while (true)
                {
                    if (UnityEngine.Application.internetReachability != NetworkReachability.NotReachable)
                    {
                        break;
                    }
                    else
                    {
                        await iOSTimerComponent.WaitAsync(200);
                    }
                }
            }

            string switchUrl = GlobalConfigComponent.Instance.GlobalProto.NetLineSwitchUrl;
            if (!string.IsNullOrEmpty(switchUrl))
            {
                HTTPRequest httpRequest = new HTTPRequest(new Uri(switchUrl), HTTPMethods.Post, ResponseDelegate);
                httpRequest.UseAlternateSSL = true;
                httpRequest.AddHeader("Content-Type", "application/json");
                httpRequest.Send();
            }
        }

        private void ResponseDelegate(HTTPRequest originalRequest, HTTPResponse response)
        {
            switch (originalRequest.State)
            {
                case HTTPRequestStates.Finished:
                    Log.Debug($"url = {originalRequest.Uri} 结果=  " + response.DataAsText);
                    if (response.IsSuccess)
                    {
                        ResponseData responseData = JsonHelper.FromJson<ResponseData>(response.DataAsText);
                        if (responseData.status == 0)
                        {
                            if (null != Game.Hotfix.OnGetNetLineSwith)
                            {
                                //热更层网络控件已初始化，由热更层处理
                                Game.Hotfix.OnGetNetLineSwith.Invoke(responseData.data);
                            }
                            else
                            {
                                //热更层未初始化，直接保存新域名
                                int serverType = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.SERVER_TYPE, 2);
                                PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.NET_LINE_SWITCH_INFO}{serverType}", responseData.data); 
                                PlayerPrefs.Save();
                                NetLineSwitchData lineSwitchData = JsonHelper.FromJson<NetLineSwitchData>(responseData.data);
                                UpdateNetLineWithInfo(lineSwitchData);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateNetLineWithInfo(NetLineSwitchData lineSwitchData)
        {
            int serverType = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.SERVER_TYPE, 2);
            int selectID = lineSwitchData.selector;
            string userChoice = PlayerPrefsMgr.mInstance.GetString($"{PlayerPrefsKeys.CURRENT_SERVER_USER_CHOICEID}{serverType}", string.Empty);
            if (!string.IsNullOrEmpty(userChoice))
            {
                selectID = Convert.ToInt32(userChoice);
            }
            PlayerPrefsMgr.mInstance.SetInt($"{PlayerPrefsKeys.CURRENT_USING_SERVERID}{serverType}", selectID);

            foreach (LineInfo lineInfo in lineSwitchData.list)
            {
                if (lineInfo.id == selectID)
                {
                    if (!string.IsNullOrEmpty(lineInfo.http))
                    {
                        PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.CURRENT_SERVER_HTTP_DOMAIN}{serverType}", lineInfo.http);
                    }
                    if (!string.IsNullOrEmpty(lineInfo.sck))
                    {
                        PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.CURRENT_SERVER_SCK_DOMAIN}{serverType}", lineInfo.sck);
                    }
                    if (!string.IsNullOrEmpty(lineInfo.res))
                    {
                        PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.CURRENT_SERVER_RES_DOMAIN}{serverType}", lineInfo.res);
                    }
                }
            }
            PlayerPrefs.Save();
        }

        public string CurrentServerResDomain()
        {
            int serverType = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.SERVER_TYPE, 2);
            string defaultRes = GlobalConfigComponent.Instance.GlobalProto.AssetBundleServerUrl;
            return PlayerPrefsMgr.mInstance.GetString($"{PlayerPrefsKeys.CURRENT_SERVER_RES_DOMAIN}{serverType}", defaultRes);
        }

    }
}