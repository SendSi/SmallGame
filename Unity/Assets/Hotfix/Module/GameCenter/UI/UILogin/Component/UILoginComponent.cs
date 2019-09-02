// #define ReleaseEnv //外网线上版本，不可切换环境

using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UILoginComponentSystem : AwakeSystem<UILoginComponent>
    {
        public override void Awake(UILoginComponent self)
        {
            self.Awake();
        }
    }

    public class UILoginComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        private InputField inputfieldAccount;
        private InputField inputfieldPassword;
        private Button buttonGuestLogin;
        private Button buttonLogin;

        public void Awake()
        {
            InitUI();
        }

        protected virtual void InitUI()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            inputfieldAccount = rc.Get<GameObject>("InputField_Account").GetComponent<InputField>();
            inputfieldPassword = rc.Get<GameObject>("InputField_Password").GetComponent<InputField>();
            buttonGuestLogin = rc.Get<GameObject>("Button_GuestLogin").GetComponent<Button>();
            buttonLogin = rc.Get<GameObject>("Button_Login").GetComponent<Button>();

            UIEventListener.Get(buttonLogin.gameObject).onClick = ClickButton_Login;
            UIEventListener.Get(buttonGuestLogin.gameObject).onClick = ClickButton_GuestLogin;
            UIEventListener.Get(rc.Get<GameObject>("Button_RetrievePassword")).onClick = ClickButton_RetrievePassword;
            UIEventListener.Get(rc.Get<GameObject>("Button_Register")).onClick = ClickButton_Register;

            var tTextAsset = rc.Get<TextAsset>(LanguageMgr.mInstance.GetCurLanguageTxtName());
            LanguageMgr.mInstance.StartSetDicLanguage(tTextAsset.text);
        }

        private void ClickButton_Login(GameObject go)
        {
            string mAccount = inputfieldAccount.text;
            string mPassword = inputfieldPassword.text;
#if LOG_ZSX
            if (mAccount == "1")
                mAccount = inputfieldAccount.text = "13543403844";
            inputfieldPassword.text = mPassword = "123456";
#endif
            if (MethodHelper.IsStrNotNull(new string[] { mAccount, mPassword }) == false)
            {
                UIComponent.Instance.Toast(100001);
                return;
            }

            if (mPassword.Length < 6)
            {
                UIComponent.Instance.Toast(100002);
                return;
            }
            Log.Debug(mAccount);
        }
        /// <summary> 注册 </summary>
        private void ClickButton_Register(GameObject go)
        {
            if (UIComponent.Instance.Get(UIType.UILogin_Register) != null)//验证码的计时器
            {
                UIComponent.Instance.Show(UIType.UILogin_Register, null, null, 0);
            }
            else
            {
                UIComponent.Instance.ShowNoAnimation(UIType.UILogin_Register);
            }
        }
        /// <summary> 忘记密码 </summary>
        private void ClickButton_RetrievePassword(GameObject go)
        {
            if (UIComponent.Instance.Get(UIType.UILogin_Forget) != null)//验证码的计时器
            {
                UIComponent.Instance.Show(UIType.UILogin_Forget, null, null, 0);
            }
            else
            {
                UIComponent.Instance.ShowNoAnimation(UIType.UILogin_Forget);
            }
        }

        private void ClickButton_GuestLogin(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_Main);
            UIComponent.Instance.Remove(UIType.UILogin);
            UIComponent.Instance.Remove(UIType.UILogin_Forget);
            UIComponent.Instance.Remove(UIType.UILogin_Register);
            UIComponent.Instance.Toast("直接进入大厅");
        }



        #region 为防止报错
        public bool IsAutoLogin { get; set; }
        public ReferenceCollector RC { get { return rc; } }
        public void UsingSprite()
        {
            //ImageMask.sprite = mDicSprite["LoginBigBg"];
            //buttonLogin.GetComponent<Image>().sprite = mDicSprite["LoginRedBtn"];
            //ImageLogo.sprite = mDicSprite["LoginLogo"];
        }
        #endregion
    }
}



/*
private Button buttonForget;
private Button buttonLogin;
private Button buttonLine;
private Button buttonRegister;
private Button buttonSecret;
private Button buttonContact;
private InputField inputfieldAccount;
private InputField inputfieldPassword;
private Text textLocal;
private Button buttonLanguage;
private Text textLanguage;
private Text textPhoneFirst;
private Button buttonChangeServer;
private Image ImageSecret;

private Transform transSystemId;
private Text textSystemId;
private Button buttonCopySystemId;

#region 切换服务器

private Transform transUILogiChangeServerGM;
private Button buttonGMClose;
private InputField inputfieldGM;

private const string GM_PWD_Test = "10101000";   // 测试服
private const string GM_PWD_Beta = "allin168000"; // 正式服
#endregion

//store key
private const string KEY_USERID = "USER_ID";
public const string KEY_TOKEN = "USER_TOKEN";
private const string KEY_PHONE = "USER_PHONE";
private const string KEY_LANGUAGE = "KEY_LANGUAGE";//中文ZH,英文EN
private const string KEY_PHONE_FIRST = "KEY_PHONE_FIRST"; //用户登录手机前缀(例"86")
private const string KEY_INFODEFAULT = "KEY_INFODEFAULT";//牌局内 个人信息 默认页  德州=1,奥马哈=2
private const string KEY_COUNTRYCOD = "KEY_COUNTRYCOD";//国家码
private Dictionary<string, Sprite> mDicSprite;
private Image ImageMask;
private Image ImageLogo;


public ReferenceCollector RC { get { return rc; } }

public void Awake()
{
    rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
    buttonLogin = rc.Get<GameObject>("Button_Login").GetComponent<Button>();
    UIEventListener.Get(buttonLogin.gameObject).onClick = onClickLogin;





    // buttonForget = rc.Get<GameObject>("Button_Forget").GetComponent<Button>();
    // buttonLogin = rc.Get<GameObject>("Button_Login").GetComponent<Button>();
    // buttonLine = rc.Get<GameObject>("Button_Line").GetComponent<Button>();
    // buttonRegister = rc.Get<GameObject>("Button_Register").GetComponent<Button>();
    // buttonSecret = rc.Get<GameObject>("Button_Secret").GetComponent<Button>();
    // ImageSecret = rc.Get<GameObject>("Button_Secret").GetComponent<Image>();
    // inputfieldAccount = rc.Get<GameObject>("InputField_Account").GetComponent<InputField>();
    // inputfieldPassword = rc.Get<GameObject>("InputField_Password").GetComponent<InputField>();
    // textLocal = rc.Get<GameObject>("Text_Local").GetComponent<Text>();
    // buttonLanguage = rc.Get<GameObject>("Button_Language").GetComponent<Button>();
    // textLanguage = rc.Get<GameObject>("Text_Language").GetComponent<Text>();
    // textPhoneFirst = rc.Get<GameObject>("Text_PhoneFirst").GetComponent<Text>();
    // buttonChangeServer = rc.Get<GameObject>("Button_ChangeServer").GetComponent<Button>();
    // buttonContact = rc.Get<GameObject>("Button_Contact").GetComponent<Button>();
    // mDicSprite = new Dictionary<string, Sprite>() {
    //     { "eye_open",rc.Get<Sprite>("eye_open")},
    //     { "eye_close",rc.Get<Sprite>("eye_close")},
    //     { "LoginLogo",rc.Get<Sprite>("LoginLogo")},
    //     { "LoginRedBtn",rc.Get<Sprite>("LoginRedBtn")},
    //     { "LoginBigBg",rc.Get<Sprite>("LoginBigBg")}
    // };
    //
    // transSystemId = rc.Get<GameObject>("SystemId").transform;
    // textSystemId = rc.Get<GameObject>("Text_SystemId").GetComponent<Text>();
    // this.buttonCopySystemId = rc.Get<GameObject>("Button_CopySystemId").GetComponent<Button>();
    //
    // UIEventListener.Get(buttonForget.gameObject).onClick = onClickForget;
    // UIEventListener.Get(buttonLogin.gameObject).onClick = onClickLogin;
    // UIEventListener.Get(buttonLine.gameObject).onClick = onClickLine;
    // UIEventListener.Get(buttonRegister.gameObject).onClick = onClickRegister;
    // UIEventListener.Get(buttonSecret.gameObject).onClick = onClickSecret;
    // UIEventListener.Get(rc.Get<GameObject>("Button_LocalArrow")).onClick = onClickLocalArrow;
    // UIEventListener.Get(buttonLanguage.gameObject).onClick = onClickSelectLanguage;
    // UIEventListener.Get(buttonChangeServer.gameObject).onClick = onClickChangeServer;
    // UIEventListener.Get(buttonContact.gameObject).onClick = onClickContact;
    // UIEventListener.Get(this.buttonCopySystemId.gameObject).onClick = onClickCopySystemId;
    // ImageMask = rc.Get<GameObject>("Image_Mask").GetComponent<Image>();
    // ImageLogo = rc.Get<GameObject>("Image_Logo").GetComponent<Image>();
    //
    // registerHandler();
    //
    // //获取本地缓存信息
    // GameCache.Instance.strPhone = PlayerPrefs.GetString(KEY_PHONE, string.Empty);
    // GameCache.Instance.strPhoneFirst = PlayerPrefs.GetString(KEY_PHONE_FIRST, "86");//前缀  86    没加号
    // GameCache.Instance.nUserId = PlayerPrefs.GetInt(KEY_USERID, 0);
    // GameCache.Instance.strToken = PlayerPrefs.GetString(KEY_TOKEN, string.Empty);
    // GameCache.Instance.CurInfoRoomPath = PlayerPrefs.GetInt(KEY_INFODEFAULT, 0);
    //
    // LanguageMgr.mInstance.mCurLanguage = PlayerPrefs.GetInt(KEY_LANGUAGE, 2);
    //
    // var tTextAsset = rc.Get<TextAsset>(LanguageUtility.LanguageIdToName(LanguageMgr.mInstance.mCurLanguage));
    // LanguageMgr.mInstance.StartSetDicLanguage(tTextAsset.text);
    //
    // inputfieldAccount.text = GameCache.Instance.strPhone;
    //
    // var tLanguage = LanguageMgr.mInstance.mCurLanguage;
    // textLanguage.text = LanguageMgr.mInstance.GetLanguageForKeyMoreValue("UserLanguage", tLanguage.GetHashCode());
    //
    // //登录或者注销登录都需要清理
    // JPushSdkComponent.Instance.DeleteAlias();
    // IMSdkComponent.Instance.LoginOut();
    //
    // Button_MoreClick = rc.Get<GameObject>("Button_MoreClick");
    // UIEventListener.Get(Button_MoreClick).onClick = ClickMoreClick;
    //
    // SetCountryCodeName();
    //
    //
    // transUILogiChangeServerGM = rc.Get<GameObject>("UILogin_ChangeServerGM").transform;
    // buttonGMClose = rc.Get<GameObject>("Button_GMClose").GetComponent<Button>();
    // inputfieldGM = rc.Get<GameObject>("InputField_GM").GetComponent<InputField>();
    // UIEventListener.Get(buttonGMClose.gameObject).onClick = onClickGMClose;
    //
    // CheckYaoYao();
}

private void onClickCopySystemId(GameObject go)
{
    UniClipboard.SetText(SystemInfoUtil.getDeviceUniqueIdentifier());
}

private void CheckYaoYao()
{
    var tYaoLe = GameObject.Find("Camera_Yaoyaole(Clone)");
    if (tYaoLe != null)
    {
        GameObject.DestroyImmediate(tYaoLe);
    }
}

/// <summary>
/// 设置 国家前缀码+86  中国
/// </summary>
private void SetCountryCodeName()
{
    textPhoneFirst.text = "+" + GameCache.Instance.strPhoneFirst;
    var tStr = UILoginModel.mInstance.GetDicCountryKeyName(textPhoneFirst.text);
    textLocal.text = tStr;
}

int mCountAll = 0;
long mTime0 = 0;
long mTime4 = 0;
private void ClickMoreClick(GameObject go)
{
    if (mCountAll == 0)
        mTime0 = TimeHelper.ClientNow();

    if (mCountAll == 5)
        mTime4 = TimeHelper.ClientNow();

    mCountAll++;

    if (mCountAll == 6)
    {
        if (mTime4 - mTime0 < 2500)
        {
            // buttonChangeServer.gameObject.SetActive(true);
            inputfieldGM.text = $"version: {UnityEngine.Application.version}";
            transUILogiChangeServerGM.gameObject.SetActive(true);
            Button_MoreClick.gameObject.SetActive(false);
        }
        else
        {
            mCountAll = -1;
            mTime0 = 0;
            mTime4 = 0;
        }
    }
}

private void onClickGMClose(GameObject go)
{
    if (string.IsNullOrEmpty(inputfieldGM.text))
    {
        transUILogiChangeServerGM.gameObject.SetActive(false);
        Button_MoreClick.gameObject.SetActive(true);
        mCountAll = 0;
        mTime0 = 0;
        mTime4 = 0;
    }
    else
    {
        if (inputfieldGM.text.Equals(GlobalData.Instance.serverType.Equals(2) ? GM_PWD_Beta : GM_PWD_Test))
        {
            transUILogiChangeServerGM.gameObject.SetActive(false);
            buttonChangeServer.gameObject.SetActive(true);

            transSystemId.gameObject.SetActive(true);
            textSystemId.text = SystemInfoUtil.getDeviceUniqueIdentifier();
        }
        else
        {
            transUILogiChangeServerGM.gameObject.SetActive(false);
            Button_MoreClick.gameObject.SetActive(true);
            mCountAll = 0;
            mTime0 = 0;
            mTime4 = 0;
        }
    }
}

private GameObject Button_MoreClick;

private void onClickLocalArrow(GameObject go)
{
    string label = textLocal.GetComponent<Text>().text;
    // UIComponent.Instance.ShowNoAnimation(UIType.UILogin_Local, new UILogin_LocalComponent.LocalCountryData() { mStrShow = label, successDelegate = LocalArrowChangeText });
}
private void LocalArrowChangeText(string pStrKey)
{
    string nums;
    switch (LanguageMgr.mInstance.mCurLanguage)
    {
        case 1:
            if (UILoginModel.mInstance.mDicCountryNum_EN.TryGetValue(pStrKey, out nums))
            {
                this.textLocal.text = pStrKey;
                this.textPhoneFirst.text = UILoginModel.mInstance.mDicCountryNum_EN[pStrKey];
            }
            break;
        case 2:
            if (UILoginModel.mInstance.mDicCountryNum_TW.TryGetValue(pStrKey, out nums))
            {
                this.textLocal.text = pStrKey;
                this.textPhoneFirst.text = UILoginModel.mInstance.mDicCountryNum_TW[pStrKey];
            }
            break;
        default:
            if (UILoginModel.mInstance.mDicCountryNum.TryGetValue(pStrKey, out nums))
            {
                this.textLocal.text = pStrKey;
                this.textPhoneFirst.text = UILoginModel.mInstance.mDicCountryNum[pStrKey];
            }
            break;
    }
}

private void onClickSelectLanguage(GameObject go)
{
    // UIComponent.Instance.ShowNoAnimation(UIType.UILogin_Language, new UILogin_LanguageComponent.UILanguageData()
    // {
    //     successDelegate = LanguageChangeText
    // });
}
private void LanguageChangeText()
{
    var tLanguage = LanguageMgr.mInstance.mCurLanguage;
    var tContent = rc.Get<TextAsset>(UILoginModel.mInstance.mLanguages[tLanguage]);
    LanguageMgr.mInstance.SettingSetDicLanguage(tContent.text);
    textLanguage.text = LanguageMgr.mInstance.GetLanguageForKeyMoreValue("UserLanguage", tLanguage);

    var tStr = UILoginModel.mInstance.GetDicCountryKeyName(textPhoneFirst.text);
    textLocal.text = tStr;
}


private async void onClickChangeServer(GameObject go)
{
    // await UIComponent.Instance.ShowAsyncNoAnimation(UIType.UIActionSheet, new UIActionSheetComponent.ActionSheetData()
    // {
    //     titles = new string[] { "开发服", "测试服", "正式服", "提审用测试服" },
    //     actionDelegate = (index) =>
    //     {
    //         GlobalData.Instance.serverType = index;
    //         GlobalData.Instance.Refresh();
    //         UpdateChangeServerBtn();
    //         UIComponent.Instance.ShowNoAnimation(UIType.UIDialog, new UIDialogComponent.DialogData()
    //         {
    //             type = UIDialogComponent.DialogData.DialogType.CommitCancel,
    //             title = $"切换环境",
    //             content = $"切换环境成功，如果想切换到对应环境的资源包，需要重启应用。",
    //             contentCommit = "重启",
    //             contentCancel = "不重启",
    //             actionCommit = () =>
    //             {
    //                 Application.Quit();
    //             },
    //             actionCancel = null
    //         });
    //     }
    // });
}

//联系客服
private void onClickContact(GameObject go)
{
    // UIMineModel.mInstance.ShowSDKCustom();
}

private void UpdateChangeServerBtn()
{
    switch (GlobalData.Instance.serverType)
    {
        case 0:
            buttonChangeServer.gameObject.transform.Find("Text").GetComponent<Text>().text = "开发服";
            break;
        case 1:
            buttonChangeServer.gameObject.transform.Find("Text").GetComponent<Text>().text = "测试服";
            break;
        case 2:
            buttonChangeServer.gameObject.transform.Find("Text").GetComponent<Text>().text = "正式服";
            break;
        case 3:
            buttonChangeServer.gameObject.transform.Find("Text").GetComponent<Text>().text = "提审用测试服";
            break;
        default:
            buttonChangeServer.gameObject.transform.Find("Text").GetComponent<Text>().text = "环境切换";
            break;
    }
}

private void onClickSecret(GameObject go)
{
    inputfieldPassword.contentType = inputfieldPassword.contentType ==
        InputField.ContentType.Password ? InputField.ContentType.Standard : InputField.ContentType.Password;
    inputfieldPassword.ForceLabelUpdate();
    ImageSecret.sprite = inputfieldPassword.contentType ==
        InputField.ContentType.Password ? mDicSprite["eye_close"] : mDicSprite["eye_open"];
}

private void onClickRegister(GameObject go)
{
    UIComponent.Instance.ShowNoAnimation(UIType.UILogin_Register);
}

private void onClickLine(GameObject go)
{
    UIComponent.Instance.ShowNoAnimation(UIType.UILogin_Line);
}

private void onClickForget(GameObject go)
{
    UIComponent.Instance.ShowNoAnimation(UIType.UILogin_Forget);
}

private void onClickLogin(GameObject go)
{
    Log.Debug($"{go.name}");

    return;
    string mAccount = inputfieldAccount.text;
    string mPassword = inputfieldPassword.text;

#if LOG_ZSX
    if (string.IsNullOrEmpty(mAccount))
        mPassword = inputfieldPassword.text = "13543403844";
    inputfieldPassword.text = mPassword = "123456";
#endif

    if (string.IsNullOrEmpty(mAccount) && MethodHelper.IsPhoneNumber(mAccount) == false)
    {
        UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10329));
        return;
    }

    if (mPassword.Length < 6)
    {
        UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10330));
        return;
    }

    IsAutoLogin = false;

    // REQ_LOGIN res_login = new REQ_LOGIN
    // {
    //     userType = 12,
    //     userUUID = mAccount,
    //     macAddress = SystemInfoUtil.getLocalMac(),
    //     imsi = SystemInfoUtil.getDeviceUniqueIdentifier(),
    //     HeighthAndWidth = $"{Screen.width}*{Screen.height}",
    //     model = SystemInfoUtil.isSimulator == 0 ? SystemInfoUtil.getDeviceMode() : "simulator",
    //     system_version = SystemInfoUtil.getOperatingSystem(),
    //     sessionKey = MD5Helper.StringMD5(mPassword),
    //     nickname = string.Empty,
    //     countryCode = textPhoneFirst.text.Replace("+", ""),
    //     version = "3.9.7",
    //     channel = 3
    // };
    // REQ_LOGIN(res_login);
}

private void registerHandler()
{
    // CPMessageDispatherComponent.Instance.RegisterHandler(HotfixOpcode.REQ_LOGIN, REQ_LOGIN_HANDLER);
    // CPMessageDispatherComponent.Instance.RegisterHandler(HotfixOpcode.REQ_LOGIN_RESOURCES, REQ_LOGIN_RESOURCES_HANDLER);
}

private void removeHandler()
{
    // CPMessageDispatherComponent.Instance.RemoveHandler(HotfixOpcode.REQ_LOGIN, REQ_LOGIN_HANDLER);
    // CPMessageDispatherComponent.Instance.RemoveHandler(HotfixOpcode.REQ_LOGIN_RESOURCES, REQ_LOGIN_RESOURCES_HANDLER);
}

public override void OnShow(object obj)
{
    return;

    if (UILoginModel.mInstance.IsOnceShowLanguagePanel())
    {
        onClickSelectLanguage(null);
        UILoginModel.mInstance.SetShowedLanguage();
    }

    if (null == obj)
    {
        if (GameCache.Instance.nUserId != 0 && !string.IsNullOrEmpty(GameCache.Instance.strToken))
        {
            // IsAutoLogin = true;
            // rc.gameObject.SetActive(false);
            // REQ_LOGIN req = new REQ_LOGIN
            // {
            //     userType = 10,
            //     userUUID = GameCache.Instance.strToken,
            //     macAddress = SystemInfoUtil.getLocalMac(),
            //     imsi = SystemInfoUtil.getDeviceUniqueIdentifier(),
            //     HeighthAndWidth = $"{Screen.width}*{Screen.height}",
            //     model = SystemInfoUtil.isSimulator == 0 ? SystemInfoUtil.getDeviceMode() : "simulator",
            //     system_version = SystemInfoUtil.getOperatingSystem(),
            //     sessionKey = string.Empty,
            //     nickname = string.Empty,
            //     countryCode = string.Empty,
            //     version = "3.9.7",
            //     channel = 3
            // };
            // REQ_LOGIN(req);
        }
        else
        {
            UsingSprite();
        }
    }
    else
    {
        UsingSprite();
    }
    UpdateChangeServerBtn();
}

public void UsingSprite()
{
    ImageMask.sprite = mDicSprite["LoginBigBg"];
    buttonLogin.GetComponent<Image>().sprite = mDicSprite["LoginRedBtn"];
    ImageLogo.sprite = mDicSprite["LoginLogo"];
}

public override void Dispose()
{
    if (IsDisposed)
    {
        return;
    }
    // mCountAll = 0;
    // mTime0 = 0;
    // mTime4 = 0;
    // removeHandler();
    //
    // ImageLogo.sprite = mDicSprite["eye_open"];
    // rc = null;
    // buttonForget = null;
    // buttonLogin = null;
    // buttonLine = null;
    // buttonRegister = null;
    // buttonSecret = null;
    // inputfieldAccount = null;
    // inputfieldPassword = null;
    // textLocal = null;
    // buttonLanguage = null;
    // textLanguage = null;
    // textPhoneFirst = null;
    // buttonChangeServer = null;
    // ImageSecret = null;
    // mDicSprite.Clear();
    // mDicSprite = null;
    //
    // ImageMask.sprite = null;
    // ImageMask = null;
    //
    // ImageLogo.sprite = null;
    // ImageLogo = null;
    //
    // IsAutoLogin = false;

    base.Dispose();

    System.GC.Collect();
    Resources.UnloadUnusedAssets();
}

public void SetInputfieldAccount(string mobile, string mobileFirst)
{
    inputfieldAccount.text = mobile;
    textPhoneFirst.text = mobileFirst;
}

// private void REQ_LOGIN_HANDLER(ICPResponse response)
// {
//     REQ_LOGIN rec = response as REQ_LOGIN;
//     if (null == rec)
//         return;
//
//     if (rec.status != 0)
//     {
//         rc.gameObject.SetActive(true);
//     }
//
//     if (rec.status > 0)
//     {
//         UsingSprite();
//     }
//
//     GameCache.Instance.kDouNum = 0;
//
//     switch (rec.status)
//     {
//         case 0:
//             Log.Debug("登录成功");
//             if (rec.user_type == 12 || rec.user_type == 10)
//             {
//                 PlayerPrefs.SetInt(KEY_USERID, rec.user_id);
//                 PlayerPrefs.SetString(KEY_TOKEN, rec.token);
//                 PlayerPrefs.SetString(KEY_PHONE, inputfieldAccount.text);
//                 PlayerPrefs.SetString(KEY_PHONE_FIRST, textPhoneFirst.text.Replace("+", ""));
//                 //PlayerPrefs.SetString(KEY_PWD, inputfieldPassword.text);
//                 PlayerPrefs.Save();
//                 GameCache.Instance.nUserId = rec.user_id;
//                 GameCache.Instance.strToken = rec.token;
//                 GameCache.Instance.resIP = rec.IP;
//                 GameCache.Instance.resport = rec.port;
//                 GameCache.Instance.isfirstLogin = rec.isfirstLogin;
//                 GameCache.Instance.strPhone = inputfieldAccount.text;
//                 GameCache.Instance.isActivity = rec.isActivity;
//                 GameCache.Instance.kDouNum = rec.kDouNum;
//
//                 // 连接Res服务器
//                 string[] mArrAddress = NetHelper.GetAddressIPs(GlobalData.Instance.LoginHost, GlobalData.Instance.UseDNS);
//                 ETModel.Session mSession = Game.Scene.ModelScene.GetComponent<NetOuterComponent>()
//                         .Create(new IPEndPoint(IPAddress.Parse(mArrAddress[0]), GameCache.Instance.resport));
//                 CPResSessionComponent mCpResSessionComponent =
//                         Game.Scene.GetComponent<CPResSessionComponent>() ?? Game.Scene.AddComponent<CPResSessionComponent>();
//                 mCpResSessionComponent.HotfixSession = ComponentFactory.Create<Session, ETModel.Session>(mSession);
//                 mCpResSessionComponent.HotfixSession.SetProtrolVersionInHead(1);
//                 mCpResSessionComponent.HotfixSession.SetUserIdInHead(GameCache.Instance.nUserId);
//                 mCpResSessionComponent.HotfixSession.SetLanguageInHead(1);
//                 byte mPlatform = 2;
//                 if (Application.platform == RuntimePlatform.Android)
//                     mPlatform = 1;
//                 mCpResSessionComponent.HotfixSession.SetPlatformInHead(mPlatform);
//                 mCpResSessionComponent.HotfixSession.SetBuildVersionInHead(254);
//                 mCpResSessionComponent.HotfixSession.SetChannelInHead(14);
//                 mCpResSessionComponent.HotfixSession.SetProductIdInHead(1002);
//                 mCpResSessionComponent.HotfixSession.SetTokenInHead(GameCache.Instance.strToken);
//                 mCpResSessionComponent.HotfixSession.Send(new REQ_LOGIN_RESOURCES() { key = rec.key });
//             }
//             break;
//         case 1:
//             Log.Debug("用户不存在");
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10144));
//             break;
//         case 2:
//             Log.Debug("密码不对");
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10145));
//             break;
//         case 3:
//             Log.Debug("一般错误");
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10146));
//             break;
//         case 4:
//             Log.Debug("版本太低");
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10147));
//             break;
//         case 5:
//             Log.Debug("token不对");
//             UIComponent.Instance.Toast(CPErrorCode.RoomErrorDescription(6, 5));
//             break;
//         case 6:
//             Log.Debug("账户或密码错误");
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10148));
//             break;
//         case 7:
//             Log.Debug("账号被封禁");
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10149));
//             break;
//         case 8:
//             Log.Debug("手机异常，无法登录");//账号被冻结
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(20068));
//             break;
//         case 9:
//             Log.Debug("玩家非指定机器码登录");//玩家非指定机器码登录
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(20079));
//             break;
//         default:
//             Log.Debug("未知错误");
//             UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(10052));
//             break;
//     }
// }

// private void REQ_LOGIN(REQ_LOGIN req)
// {
//     var mNetOuterComponent = Game.Scene.ModelScene.GetComponent<NetOuterComponent>();
//     if (null == mNetOuterComponent)
//         return;
//     try
//     {
//         // GlobalData.Instance.LoginHost = "127.0.0.1";
//         // GlobalData.Instance.UseDNS = false;
//         string[] mArrAddress = NetHelper.GetAddressIPs(GlobalData.Instance.LoginHost, GlobalData.Instance.UseDNS);
//         ETModel.Session mSession = mNetOuterComponent.Create(new IPEndPoint(IPAddress.Parse(mArrAddress[0]), GlobalData.Instance.LoginPort));
//         var mCPLoginNetworkComponent =
//                 Game.Scene.GetComponent<CPLoginSessionComponent>() ?? Game.Scene.AddComponent<CPLoginSessionComponent>();
//         mCPLoginNetworkComponent.HotfixSession = ComponentFactory.Create<Session, ETModel.Session>(mSession);
//         mCPLoginNetworkComponent.HotfixSession.SetProtrolVersionInHead(1);
//         mCPLoginNetworkComponent.HotfixSession.SetUserIdInHead(GameCache.Instance.nUserId);
//         mCPLoginNetworkComponent.HotfixSession.SetLanguageInHead(1);
//         byte mPlatform = 2;
//         if (Application.platform == RuntimePlatform.Android)
//             mPlatform = 1;
//         mCPLoginNetworkComponent.HotfixSession.SetPlatformInHead(mPlatform);
//         mCPLoginNetworkComponent.HotfixSession.SetBuildVersionInHead(254);
//         mCPLoginNetworkComponent.HotfixSession.SetChannelInHead(14);
//         mCPLoginNetworkComponent.HotfixSession.SetProductIdInHead(1002);
//         mCPLoginNetworkComponent.HotfixSession.SetTokenInHead(GameCache.Instance.strToken);
//         mCPLoginNetworkComponent.HotfixSession.Send(req);
//     }
//     catch (Exception e)
//     {
//         UIComponent.Instance.ClosePrompt();
//         Log.Error(e);
//     }
// }

// private void REQ_LOGIN_RESOURCES_HANDLER(ICPResponse response)
// {
//     REQ_LOGIN_RESOURCES rec = response as REQ_LOGIN_RESOURCES;
//     if (null == rec)
//         return;
//
//     if (rec.status != 0)
//     {
//         UsingSprite();
//         rc.gameObject.SetActive(true);
//         string mErrorMsg = string.Empty;
//         if (rec.status == 7)
//             mErrorMsg = CPErrorCode.LanguageDescription(10149);
//         else
//             mErrorMsg = CPErrorCode.LanguageDescription(10150);
//         UIComponent.Instance.Toast(mErrorMsg);
//         return;
//     }
//
//     GameCache.Instance.sex = rec.sex;
//     GameCache.Instance.msg_count = rec.msg_count;
//     GameCache.Instance.nick = rec.nick;
//     GameCache.Instance.headPic = rec.head_picName;
//     GameCache.Instance.gold = rec.gold;
//     GameCache.Instance.idou = rec.idou;
//     GameCache.Instance.vipLevel = rec.vipLevel;
//     GameCache.Instance.vipEndDate = rec.vipEndDate;
//
//     // 添加Res服务器心跳组件
//     CPResSessionComponent mCpResSessionComponent = Game.Scene.GetComponent<CPResSessionComponent>();
//     CPResHeartbeatComponent mCpResHeartbeatComponent = mCpResSessionComponent.HotfixSession.GetComponent<CPResHeartbeatComponent>();
//     if (null != mCpResHeartbeatComponent)
//         mCpResSessionComponent.HotfixSession.RemoveComponent<CPResHeartbeatComponent>();
//     mCpResHeartbeatComponent = mCpResSessionComponent.HotfixSession.AddComponent<CPResHeartbeatComponent>();
//     mCpResHeartbeatComponent.StartSending();
//
//     Game.EventSystem.Run(EventIdType.LoginSucess);//登录成功后  设置SDK内容
//     loadUILobby();
// }

// async void loadUILobby()
// {
//     UI mUILobbyMenu = await UIComponent.Instance.ShowAsyncNoAnimation(UIType.UILobby_Menu);
//     ((UILobby_MenuComponent)mUILobbyMenu.UiBaseComponent).ChangeView(UILobby_MenuComponent.ePageType.MATCH);
//     Game.Scene.RemoveComponent<CPLoginSessionComponent>();
//
//     UIComponent.Instance.Remove(UIType.UILogin);
// }


*/
