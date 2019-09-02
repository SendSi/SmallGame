using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    //关于 api接口 往这边迁移
    public class UILoginModel
    {

        private static UILoginModel _instance;
        public static UILoginModel mInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new UILoginModel();
                return _instance;
            }
        }

        #region 手机前缀码 多语言
        /// <summary>         根据国家手机前缀码 得到国家         </summary>
        public string GetDicCountryKeyName(string pValue)
        {
            Dictionary<string, string> tDicValue = null;
            if (LanguageMgr.mInstance.mCurLanguage == 0)//简中
                tDicValue = mDicCountryNum_ZH;
            else if (LanguageMgr.mInstance.mCurLanguage == 1)//英文
                tDicValue = mDicCountryNum_EN;
            else if (LanguageMgr.mInstance.mCurLanguage == 2)//繁体中文
                tDicValue = mDicCountryNum_TW;

            string tCountry = string.Empty;
            foreach (var item in tDicValue)
            {
                if (pValue == item.Value)
                {
                    tCountry = item.Key;
                    break;
                }
            }
            return tCountry;
        }
        /// <summary> 得到国家s </summary>
        public List<string> GetCountryNames()
        {
            Dictionary<string, string> tDicValue = null;
            if (LanguageMgr.mInstance.mCurLanguage == 0)//简中
                tDicValue = mDicCountryNum_ZH;
            else if (LanguageMgr.mInstance.mCurLanguage == 1)//英文
                tDicValue = mDicCountryNum_EN;
            else if (LanguageMgr.mInstance.mCurLanguage == 2)//繁体中文
                tDicValue = mDicCountryNum_TW;
            return tDicValue.Keys.ToList();
        }
        /// <summary>         得到 电话码   区号      </summary>
        public string GetAreaCode(string contryName)
        {
            Dictionary<string, string> tDicValue = null;
            if (LanguageMgr.mInstance.mCurLanguage == 0)//简中
                tDicValue = mDicCountryNum_ZH;
            else if (LanguageMgr.mInstance.mCurLanguage == 1)//英文
                tDicValue = mDicCountryNum_EN;
            else if (LanguageMgr.mInstance.mCurLanguage == 2)//繁体中文
                tDicValue = mDicCountryNum_TW;

            string code = "+86";
            if (tDicValue.TryGetValue(contryName, out code))
            {
                return code;
            }
            return code;
        }

        public Dictionary<string, string> mDicCountryNum_ZH = new Dictionary<string, string>()
        {
                {"中国","+86"},
                {"美国","+1"},
                {"加拿大","+1"},
                {"英国","+44"},
                {"法国","+33"},
                {"德国","+49"},
                {"意大利","+39"},
                {"西班牙","+34"},
                {"葡萄牙","+351"},
                {"澳大利亚","+61"},
                {"荷兰","+31"},
                {"新西兰","+64"},
                {"香港","+852"},
                {"澳门","+853"},
                {"菲律宾	","+63"},
                {"韩国","+82"},
                {"柬埔寨","+855"},
                {"马来西亚	","+60"},
                {"日本","+81"},
                {"泰国","+66"},
                {"台湾","+886"},
                {"文莱","+673"},
                {"新加坡","+65"},
                {"印度","+91"},
                {"印度尼西亚","+62"},
                {"越南","+84"},
                {"俄罗斯","+7"}
        };
        public Dictionary<string, string> mDicCountryNum_TW = new Dictionary<string, string>()
        {
                {"中國","+86"},
                {"美國","+1"},
                {"加拿大","+1"},
                {"英國","+44"},
                {"法國","+33"},
                {"德國","+49"},
                {"意大利","+39"},
                {"西班牙","+34"},
                {"葡萄牙","+351"},
                {"澳大利亞","+61"},
                {"荷蘭","+31"},
                {"新西蘭","+64"},
                {"香港","+852"},
                {"澳門","+853"},
                {"菲律賓","+63"},
                {"韓國","+82"},
                {"柬埔寨","+855"},
                {"馬來西亞  ","+60"},
                {"日本","+81"},
                {"泰國","+66"},
                {"臺灣","+886"},
                {"文萊","+673"},
                {"新加坡","+65"},
                {"印度","+91"},
                {"印度尼西亞","+62"},
                {"越南","+84"},
                {"俄羅斯","+7"}
        };
        public Dictionary<string, string> mDicCountryNum_EN = new Dictionary<string, string>()
        {
            {"China", "+86"},
             {"United States", "+1"},
             {"Canada", "+1"},
             {"British", "+44"},
             {"France", "+33"},
             {"Germany", "+49"},
             {"Italy", "+39"},
             {"Spain", "+34"},
             {"Portugal", "+351"},
             {"Australia", "+61"},
             {"Netherlands", "+31"},
             {"New Zealand", "+64"},
             {"Hong Kong", "+852"},
             {"Macau", "+853"},
             {"Philippines", "+63"},
             {"Korea", "+82"},
             {"Cambodia", "+855"},
             {"Malaysia", "+60"},
             {"Japan", "+81"},
             {"Thailand", "+66"},
             {"Taiwan", "+886"},
             {"Brunei", "+673"},
             {"Singapore", "+65"},
             {"India", "+91"},
             {"Indonesia", "+62"},
             {"Vietnamese", "+84"},
             {"Russia", "+7"}
        };
        #endregion

        #region 验证码 倒计时
        /// <summary>     传入控件    Text        </summary>
        public async void ShowTimes(Text textCode)
        {
            mTimerCountRemind = TimeHelper.GetCodeTime();
            textCode.text = mTimerCountRemind.ToString() + "S";
            TimerComponent mTC = ETModel.Game.Scene.GetComponent<TimerComponent>();
            while (mTimerCountRemind >= 0)
            {
                await mTC.WaitAsync(1000);
                if (textCode == null) return;
                textCode.text = mTimerCountRemind.ToString() + "S";
                mTimerCountRemind--;
            }
            mTimerCountRemind = TimeHelper.GetCodeTime();
            textCode.text = LanguageMgr.mInstance.GetLanguageForKey("UILogin_GetCode"); //"获取验证码";
        }
        float mTimerCountRemind = 0; 
        #endregion

        public void Test()
        {
            PlayerPrefsMgr.mInstance.GetString(PlayerPrefsKeys.Key1);
        }




        /*
        /// <summary>是否打开面板          true 打开选择面板 首次自动打开选择语言面板</summary>
        public bool IsOnceShowLanguagePanel()
        {
            bool tShow = true;
            if (PlayerPrefs.HasKey("OnceShowLanguage"))
            {
                tShow = false;
            }
            return tShow;
        }
        / <summary>
        / 首次 调用下 打开面板 
        / </summary>
        public void SetShowedLanguage()
        {
            PlayerPrefs.SetInt("OnceShowLanguage", 1);
        }


        / <summary>   号码是否已存在 pMoblie 传 86-13543403844        回调,true已注册的号码</summary>
        public void APIPHoneExist(string pMoblieNum, Action<bool> pAct)
        {
            var tReq = new WEB2_user_exist_phone.RequestData()
            {
                phone = pMoblieNum
            };
            HttpRequestComponent.Instance.Send(WEB2_user_exist_phone.API, WEB2_user_exist_phone.Request(tReq), s =>
            {
                var tResp = WEB2_user_exist_phone.Response(s);
                if (tResp.status == 0)
                {
                    if (pAct != null)
                        pAct(tResp.data);
                }
                else
                {
                    UIComponent.Instance.Toast(tResp.status);
                }
            });
        }

        /// <summary>         发送验证码   pFirst前缀,例86      pPhoneNum手机号码,例13543403844  pType1 = 注册    2 = 找回密码   3 = 设置二级密码 4 = 找回二级密码     5 = 提豆 </summary>
        public void APISendCode(string pFirst, string pPhoneNum, int pType, Action<WEB2_sms_sendCode.ResponseData> pAct)
        {
            var tReq = new WEB2_sms_sendCode.RequestData()
            {
                countryCode = pFirst,
                sendCodeType = pType,//1注册   2找回密码
                phoneNum = pPhoneNum,
                lang = 0
            };
            HttpRequestComponent.Instance.Send(WEB2_sms_sendCode.API, WEB2_sms_sendCode.Request(tReq), s =>
            {
                var tResp = WEB2_sms_sendCode.Response(s);
                if (pAct != null)
                    pAct(tResp);
            });
        }

        / <summary>         pMoblieNum例86-13543403844          </summary>
         public void APISendRegister(string pMoblieNum, string pCode, string pWD, string pInvitationCode, Action<WEB2_user_register.ResponseData> pAct)
        {
            var tMd5Pwd = MD5Helper.StringMD5(pWD);
            var req_user_register = new WEB2_user_register.RequestData()
            {
                phone = pMoblieNum,
                password = tMd5Pwd,
                channelId = 0,
                area = "cn",
                imei = SystemInfoUtil.getDeviceUniqueIdentifier(),
                code = pCode,
                isSimulator = SystemInfoUtil.isSimulator,
                invitationCode = pInvitationCode
            };
            HttpRequestComponent.Instance.Send(
                WEB2_user_register.API,
                WEB2_user_register.Request(req_user_register), s =>
                {
                    var rec = WEB2_user_register.Response(s);
                    if (pAct != null)
                        pAct(rec);
                });
        }

        /// <summary>
        /// pMoblieNum例86-13543403844
        /// </summary>
         public void APISendModifyPW(string pMoblieNum, string pCode, string pPwd, Action<WEB2_user_mod_pwd.ResponseData> pAct)
        {
            HttpRequestComponent.Instance.Send(WEB2_user_mod_pwd.API,
                                   WEB2_user_mod_pwd.Request(new WEB2_user_mod_pwd.RequestData()
                                   {
                                       phone = pMoblieNum,
                                       code = pCode,
                                       password = MD5Helper.StringMD5(pPwd)
                                   }), s =>
                                   {
                                       var tDto = WEB2_user_mod_pwd.Response(s);
                                       if (pAct != null)
                                           pAct(tDto);
                                   });
        }
        */



    }
}