// #define DevEnv // 开发服 0
// #define TestEnv // 测试服 1
// #define ReleaseEnv // 正式服 2，不可切换环境
// #define AppStoreEnv // 提审服 3
// #define SwitchServer // 可选服

using UnityEngine;
using ETModel;
using System;

namespace ETHotfix
{
    public class GlobalData
    {


        private static GlobalData _instance;

        public static GlobalData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalData();
                    _instance.Refresh();
                    ETModel.Game.Hotfix.OnGetNetLineSwith = _instance.OnGetNetLineSwithInfo;
                }
                return _instance;
            }
        }

        public int serverType
        {
            get
            {
                return PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.SERVER_TYPE, 2);
            }
            set
            {
                PlayerPrefsMgr.mInstance.SetInt(PlayerPrefsKeys.SERVER_TYPE, value);
                PlayerPrefs.Save();
            }
        }

        //收到IM切换消息
        public void OnGetNetLineSwithIMMessage(string info)
        {
            PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.NET_LINE_SWITCH_INFO}{serverType}", info);
            PlayerPrefs.Save();
            OnCheckUpdateSwithInfo(info, true, false);
        }

        //收到切换API响应
        private void OnGetNetLineSwithInfo(string info)
        {
            PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.NET_LINE_SWITCH_INFO}{serverType}", info);
            PlayerPrefs.Save();
            OnCheckUpdateSwithInfo(info, false, false);
        }

        //用户主动切换
        public void UserSwitchServer(int serverID)
        {
            PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.CURRENT_SERVER_USER_CHOICEID}{serverType}", $"{serverID}");
            PlayerPrefs.Save();
            string info = PlayerPrefsMgr.mInstance.GetString($"{PlayerPrefsKeys.NET_LINE_SWITCH_INFO}{serverType}", string.Empty);
            if (!string.IsNullOrEmpty(info))
            {
                OnCheckUpdateSwithInfo(info, false, true);
            }
        }

        public int CurrentUsingServerID()
        {
            return PlayerPrefsMgr.mInstance.GetInt($"{PlayerPrefsKeys.CURRENT_USING_SERVERID}{serverType}", 0);
        }

        public string NameForServerID(int serverID)
        {
            if (serverID == 1)
            {
                return LanguageMgr.mInstance.GetLanguageForKey("UILogin_HKLine");//"香港(主线路)";
            }
            if (serverID == 2)
            {
                return LanguageMgr.mInstance.GetLanguageForKey("UILogin_EastNosLine");//"东南亚";
            }
            if (serverID == 3)
            {
                return LanguageMgr.mInstance.GetLanguageForKey("UILogin_OtherLine");//"日韩欧美澳";
            }
            return $"Line{serverID}";
        }


        private void OnCheckUpdateSwithInfo(string info, bool isIM, bool isUserChoice)
        {
            NetLineSwitchComponent.NetLineSwitchData lineSwitchData = JsonHelper.FromJson<NetLineSwitchComponent.NetLineSwitchData>(info);

            int selectID;

            string userChoice = PlayerPrefsMgr.mInstance.GetString($"{PlayerPrefsKeys.CURRENT_SERVER_USER_CHOICEID}{serverType}", null);
            if (!string.IsNullOrEmpty(userChoice))
            {
                //优先使用用户选择的线路
                selectID = Convert.ToInt32(userChoice);
            }
            else
            {
                if (isIM)
                {
                    //IM切换的，不要使用接口返回的默认线路，使用上次的线路即可，如无则用线路1
                    selectID = PlayerPrefsMgr.mInstance.GetInt($"{PlayerPrefsKeys.CURRENT_VIP_DNS_SERVERID}{serverType}", 1);
                }
                else
                {
                    selectID = lineSwitchData.selector;
                    PlayerPrefsMgr.mInstance.SetInt($"{PlayerPrefsKeys.CURRENT_VIP_DNS_SERVERID}{serverType}", selectID);
                }
            }

            PlayerPrefsMgr.mInstance.SetInt($"{PlayerPrefsKeys.CURRENT_USING_SERVERID}{serverType}", selectID);
            PlayerPrefs.Save();

            foreach (NetLineSwitchComponent.LineInfo lineInfo in lineSwitchData.list)
            {
                if (lineInfo.id == selectID)
                {
                    bool hadChange = false;
                    if (!string.IsNullOrEmpty(lineInfo.http))
                    {
                        if (lineInfo.http != HTTP)
                        {
                            hadChange = true;
                        }
                        PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.CURRENT_SERVER_HTTP_DOMAIN}{serverType}", lineInfo.http);
                        PlayerPrefs.Save();
                    }
                    if (!string.IsNullOrEmpty(lineInfo.sck))
                    {
                        if (lineInfo.sck != LoginHost)
                        {
                            hadChange = true;
                        }
                        PlayerPrefsMgr.mInstance.SetString($"{PlayerPrefsKeys.CURRENT_SERVER_SCK_DOMAIN}{serverType}", lineInfo.sck);
                        PlayerPrefs.Save();
                    }
                    Refresh();
                    if (hadChange && GameCache.Instance.nUserId > 0 && !isUserChoice)
                    {
                        //重连
                        NetworkDetectionComponent mNetworkDetectionComponent = Game.Scene.GetComponent<NetworkDetectionComponent>();
                        if (null != mNetworkDetectionComponent)
                            Game.Scene.GetComponent<NetworkDetectionComponent>().Reconnect();
                        // UIComponent.Instance.ShowNoAnimation(UIType.UIDialog, new UIDialogComponent.DialogData()
                        // {
                        //     type = UIDialogComponent.DialogData.DialogType.Commit,
                        //     title = $"温馨提示",
                        //     content = $"当前线路不稳定，已为您切换线路重新连接。",
                        //     contentCommit = "好的",
                        //     actionCommit = null
                        // });
                        UIComponent.Instance.ShowNoAnimation(UIType.UIDialog, new UIDialogComponent.DialogData()
                        {
                            type = UIDialogComponent.DialogData.DialogType.Commit,
                            // title = $"温馨提示",
                            title = CPErrorCode.LanguageDescription(10007),
                            // content = $"当前线路不稳定，已为您切换线路重新连接",
                            content = CPErrorCode.LanguageDescription(20072),
                            // contentCommit = "知道了",
                            contentCommit = CPErrorCode.LanguageDescription(10024),
                            contentCancel = "",
                            actionCommit = null,
                            actionCancel = null
                        });
                    }
                }
            }

        }


        public const string versionChannel = "dev1.0.0";//版本渠道

        private string HTTP;
        public string WebHost;
        public string LoginHost;
        public int APIPort;
        public int PayPort;
        public int LoginPort;
        public int HeadPort;
        public int PaipuPort;  //牌谱
        public int UploadPort; //头像上传
        public bool UseDNS = false;

        public string WebURL;
        public string PayURL;
        public string HeadUrl;
        public string BannerImageUrl;
        public string UploadURL;
        public string PaipuBaseUrl;
        /// <summary>关于我们</summary>
        public string AboutWeURL;
        /// <summary>用户协议</summary>
        public string UserAgentURL;
        /// <summary>数据分析</summary>       
        public string DataAnalysURL;

        public void Refresh()
        {
#if ReleaseEnv
            serverType = 2;
#endif
            string httpKey = $"{PlayerPrefsKeys.CURRENT_SERVER_HTTP_DOMAIN}{serverType}";
            string sckKey = $"{PlayerPrefsKeys.CURRENT_SERVER_SCK_DOMAIN}{serverType}";

            switch (serverType)
            {
                case 0:
                    //开发服
                    HTTP = "192.168.0.10"; // 开发服
                    WebHost = $"http://{HTTP}";
                    LoginHost = "192.168.0.10"; // 开发服
                    APIPort = 5050;//8089;//5050;//5000正常服
                    PayPort = 8504;
                    LoginPort = 8500;
                    HeadPort = 8600;
                    PaipuPort = 8600;  //牌谱
                    UploadPort = 8009; //头像上传
                    UseDNS = false;
                    AboutWeURL = "https://tres.shzc.mobi/staticPage/aboutUs/index.html";
                    UserAgentURL = "https://tres.shzc.mobi/staticPage/userLicense/index.html";
                    DataAnalysURL = "https://tres.shzc.mobi/staticPage/dataAnalys/index.html";
                    break;
                case 1:
                    //测试服
                    HTTP = PlayerPrefsMgr.mInstance.GetString(httpKey, "thp.wdjjsc.biz");
                    WebHost = $"https://{HTTP}";
                    LoginHost = LoginHost = PlayerPrefsMgr.mInstance.GetString(sckKey, "tsk.wdjjsc.biz");
                    APIPort = 4445;
                    // APIPort = 443;
                    PayPort = 8057;
                    LoginPort = 8058;
                    HeadPort = 8004;
                    PaipuPort = 7443;  //牌谱
                    UploadPort = 9443; //头像上传
                    UseDNS = true;
                    AboutWeURL = "https://tres.shzc.mobi/staticPage/aboutUs/index.html";
                    UserAgentURL = "https://tres.shzc.mobi/staticPage/userLicense/index.html";
                    DataAnalysURL = "https://tres.shzc.mobi/staticPage/dataAnalys/index.html";
                    break;
                case 2:
                    //正式服
                    HTTP = PlayerPrefsMgr.mInstance.GetString(httpKey, "fhp.umy.mobi");
                    WebHost = $"https://{HTTP}";
                    LoginHost = PlayerPrefsMgr.mInstance.GetString(sckKey, "fsk.umy.mobi");
                    APIPort = 4443;
                    PayPort = 8443;
                    LoginPort = 8090;
                    HeadPort = 6443;
                    PaipuPort = 5443;  //牌谱
                    UploadPort = 9443; //头像上传
                    UseDNS = true;
                    AboutWeURL = "https://fhp.umy.mobi:5443/staticPage/aboutUs/index.html";
                    UserAgentURL = "https://fhp.umy.mobi:5443/staticPage/userLicense/index.html";
                    DataAnalysURL = "https://fhp.umy.mobi:5443/staticPage/dataAnalys/index.html";
                    break;
                case 3:
                    //测试服（提审用）
                    HTTP = "store.wdjjsc.biz";
                    WebHost = $"https://{HTTP}";
                    LoginHost = "storesk.wdjjsc.biz";
                    APIPort = 443;
                    PayPort = 8057;
                    LoginPort = 8058;
                    HeadPort = 8003;
                    PaipuPort = 7443;  //牌谱
                    UploadPort = 9443; //头像上传
                    UseDNS = true;
                    AboutWeURL = "https://tres.shzc.mobi/staticPage/aboutUs/index.html";
                    UserAgentURL = "https://tres.shzc.mobi/staticPage/userLicense/index.html";
                    DataAnalysURL = "https://tres.shzc.mobi/staticPage/dataAnalys/index.html";
                    break;
            }

            WebURL = $"{WebHost}:{APIPort}";
            PayURL = $"{WebHost}:{PayPort}";
            // HeadUrl = $"http://{LoginHost}:{HeadPort}/dzfile/img/";
            HeadUrl = $"{WebHost}:{HeadPort}/dzfile/img/";
            // BannerImageUrl = $"http://{LoginHost}:{HeadPort}";
            BannerImageUrl = $"{WebHost}:{HeadPort}";
            UploadURL = $"{WebHost}:{UploadPort}";
            PaipuBaseUrl = $"{WebHost}:{PaipuPort}/?lan=zh&info_id=";
        }

        public static string WebVersion = UnityEngine.Application.version;
        public static int WebChannelId = 3;

        public int WebCliType
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return 1;
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return 2;
                }
                return 2;
            }
        }
    }
}
