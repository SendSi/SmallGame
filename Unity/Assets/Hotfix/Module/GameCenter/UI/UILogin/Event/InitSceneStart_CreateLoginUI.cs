using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[Event(EventIdType.InitSceneStart)]
	public class InitSceneStart_CreateLoginUI: AEvent
	{
		public override void Run()
		{
            UIComponent.Instance.ShowNoAnimation(UIType.UILogin);
            // UIComponent.Instance.ShowNoAnimation(UIType.UIMarquee);
        }
	}

    [Event(EventIdType.LoginSucess)]
    public class LoginSucess_CreateLoginUI : AEvent
    {
        public override void Run()
        {

            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android
                || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer)
            {

                TalkingDataSdkComponent.Instance.UploadSdkAnalysis("sdk - init");

                //登录成功后配置sdk信息
                BuglySdkComponent.Instance.SetUserId($"user {GameCache.Instance.nUserId}");
                TalkingDataSdkComponent.Instance.SetAccount(GameCache.Instance.nUserId.ToString());
                TalkingDataSdkComponent.Instance.SetAccountType(AccountType.ANONYMOUS);
                //设置识别号 serverType为服务器重要属性，用于区别开发服、测试服和正式服的极光推送功能
                JPushSdkComponent.Instance.SetAlias($"{GlobalData.Instance.serverType}_{GameCache.Instance.nUserId.ToString()}");
                IMSdkComponent.Instance.Login();
                KeFuSdkComponent.Instance.Login(GameCache.Instance.nUserId.ToString(), GameCache.Instance.nick);

                //jpush获取ID发送给服务器
                int device;
                if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
                {
                    device = 0;
                }
                else device = 1;

                // WEB2_push_uinfo.RequestData push_req = new WEB2_push_uinfo.RequestData()
                // {
                //     regId = GameCache.Instance.nUserId.ToString(),
                //     lanType = LanguageMgr.mInstance.mCurLanguage,//语音类别 
                //     deviceType = device
                // };

                // HttpRequestComponent.Instance.Send(WEB2_push_uinfo.API, WEB2_push_uinfo.Request(push_req),
                //     (resData) => {
                //
                //         WEB2_push_uinfo.ResponseData push_res = WEB2_push_uinfo.Response(resData);
                //         if (push_res.status == 0)
                //         {
                //             //返回成功
                //             Log.Debug($"注册推送成功 regId = {GameCache.Instance.nUserId}");
                //             TalkingDataSdkComponent.Instance.UploadSdkAnalysis("JPush - upload succ");
                //         }
                //         else
                //         {
                //             //接口错误时上传到bugly
                //             BuglySdkComponent.Instance.ReportException("sdk jpush", $"error api WEB2_push_uinfo status = {push_res.status}", "");
                //             TalkingDataSdkComponent.Instance.UploadSdkAnalysis("JPush - upload false");
                //             UIComponent.Instance.Toast(push_res.status);
                //         }
                //     });

            }
        }

        [Event(EventIdType.LoginError)]
        public class LoginError_CreateLoginUI : AEvent<int>
        {
            public override void Run(int errorStatus)
            {
                UI mUILogin = UIComponent.Instance.Get(UIType.UILogin);
                if (null != mUILogin && null != mUILogin.GameObject)
                {
                    UILoginComponent mUILoginComponent = mUILogin.UiBaseComponent as UILoginComponent;
                    if (null != mUILoginComponent)
                    {
                        if (mUILoginComponent.IsAutoLogin)
                        {
                            mUILoginComponent.UsingSprite();
                            mUILoginComponent.RC.gameObject.SetActive(true);
                            GameObject defaultObj = GameObject.Find("Global/UI/LobbyCanvas/UIDefault(Clone)");
                            if (null != defaultObj)
                            {
                                GameObject.Destroy(defaultObj);
                            }
                        }
                        else
                        {
                            UIComponent.Instance.ShowNoAnimation(UIType.UIDialog, new UIDialogComponent.DialogData()
                            {
                                type = UIDialogComponent.DialogData.DialogType.Commit,
                                // title = $"温馨提示",
                                title = CPErrorCode.LanguageDescription(10007),
                                // content = $"当前线路不稳定，已为您切换线路重新连接",
                                content = CPErrorCode.LanguageDescription(20071),
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
        }
    }
}
