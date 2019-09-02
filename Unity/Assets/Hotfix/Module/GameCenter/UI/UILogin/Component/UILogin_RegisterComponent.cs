using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UILogin_RegisterComponentSystem : AwakeSystem<UILogin_RegisterComponent>
    {
        public override void Awake(UILogin_RegisterComponent self)
        {
            self.Awake();
        }
    }

    public class UILogin_RegisterComponent : UIBaseComponent
    {
        ReferenceCollector rc;

        private Dropdown dropdownArea;
        private InputField inputfieldAccount;
        private InputField inputfieldNickname;
        private InputField inputfieldPassword;
        private InputField inputfieldSecurityCode;
        private Text textSecurityCode;
        private Image imageShowPassword;
        Dictionary<string, Sprite> mDicStrSprite = new Dictionary<string, Sprite>();


        public void Awake()
        {
            InitUI();
        }

        protected virtual void InitUI()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIEventListener.Get(rc.Get<GameObject>("Button_Back")).onClick = ClickButton_Back;
            UIEventListener.Get(rc.Get<GameObject>("Button_Regist")).onClick = ClickButton_Regist;
            UIEventListener.Get(rc.Get<GameObject>("Button_ShowPasswrod")).onClick = ClickButton_ShowPasswrod;
            UIEventListener.Get(rc.Get<GameObject>("Button_SecurityCode")).onClick = ClickButton_SecurityCode;
            dropdownArea = rc.Get<GameObject>("RegistUi_Dropdown_Area").GetComponent<Dropdown>();
            inputfieldAccount = rc.Get<GameObject>("RegistUi_InputField_Account").GetComponent<InputField>();
            inputfieldNickname = rc.Get<GameObject>("RegistUi_InputField_Nickname").GetComponent<InputField>();
            inputfieldPassword = rc.Get<GameObject>("RegistUi_InputField_Password").GetComponent<InputField>();
            inputfieldSecurityCode = rc.Get<GameObject>("RegistUi_InputField_SecurityCode").GetComponent<InputField>();
            textSecurityCode = rc.Get<GameObject>("RegistUi_Text_SecurityCode").GetComponent<Text>();
            mDicStrSprite = new Dictionary<string, Sprite>() {
                { "eyeOpen",rc.Get<Sprite>("eyeOpen")},
                { "eyeClose",rc.Get<Sprite>("eyeClose")},
            };
            imageShowPassword = rc.Get<GameObject>("Button_ShowPasswrod").GetComponent<Image>();

            List<string> mArea = UILoginModel.mInstance.GetCountryNames();
            dropdownArea.AddOptions(mArea);
            dropdownArea.value = 0;
            dropdownArea.onValueChanged.AddListener(DropDownSelectEvent);
        }

        private void DropDownSelectEvent(int arg0)
        {
            Log.Debug(dropdownArea.value+"    "+dropdownArea.captionText.text);
            Log.Debug("电话 前缀码 " + UILoginModel.mInstance.GetAreaCode(dropdownArea.captionText.text));
        }

        private void ClickButton_Back(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UILogin_Register);
        }

        private void ClickButton_Regist(GameObject go)
        {
            string tAccount = inputfieldAccount.text.Trim();
            string tPwd = inputfieldPassword.text.Trim();
            string tNickname = inputfieldNickname.text.Trim();
            string tSecurityCode = inputfieldSecurityCode.text.Trim();

            if (string.IsNullOrEmpty(tAccount)) { UIComponent.Instance.Toast("账号不能为空"); return; }
            if (string.IsNullOrEmpty(tPwd)) { UIComponent.Instance.Toast("密码不能为空请重新输入"); return; }
            if (string.IsNullOrEmpty(tNickname)) { UIComponent.Instance.Toast("昵称不能为空请重新输入"); return; }
            if (string.IsNullOrEmpty(tSecurityCode)) { UIComponent.Instance.Toast("验证码不能为空请重新输入"); return; }
            if (tPwd.Length < 6) { UIComponent.Instance.Toast("密码长度要大于6位"); return; }

            Log.Debug(dropdownArea.value + "    " + dropdownArea.captionText.text);
            Log.Debug("注册事件");
        }


        private void ClickButton_ShowPasswrod(GameObject go)
        {
            inputfieldPassword.contentType = (inputfieldPassword.contentType == InputField.ContentType.Password) ? InputField.ContentType.Standard : InputField.ContentType.Password;
            imageShowPassword.sprite = (inputfieldPassword.contentType == InputField.ContentType.Password) ? mDicStrSprite["eyeOpen"] : mDicStrSprite["eyeClose"];
            imageShowPassword.SetNativeSize();
            inputfieldPassword.ForceLabelUpdate();
        }

        private void ClickButton_SecurityCode(GameObject go)
        {
            string tPhone = inputfieldAccount.text;
            if (string.IsNullOrEmpty(tPhone))
            {
                UIComponent.Instance.ToastLanguage("UILogin_1004");//请输入手机号
                return;
            }
            if (textSecurityCode.text.Equals(LanguageMgr.mInstance.GetLanguageForKey("UILogin_GetCode")) == false)
            {
                UIComponent.Instance.ToastLanguage("UILogin_1005");//("请稍等再发");
                return;
            }

            UILoginModel.mInstance.ShowTimes(textSecurityCode);

            //var tNumFirst = textPhoneFirst.text.Replace("+", "");
            //UILoginModel.mInstance.APIPHoneExist(tNumFirst + "-" + tPhone, pIsExite =>
            //{
            //    if (pIsExite)
            //    {
            //        UIComponent.Instance.ToastLanguage("UILogin_1006");//("此号码已注册");
            //        return;
            //    }
            //    mIsCanClickCode = false;

            //    UILoginModel.mInstance.APISendCode(tNumFirst, tPhone, 1, tDto =>
            //    {
            //        if (tDto.status == 0)
            //        {
            //            UIComponent.Instance.ToastLanguage("UILogin_1007");//("验证码已发送");
            //            UILoginModel.mInstance.ShowTimes(textCode);
            //        }
            //        else
            //        {
            //            UIComponent.Instance.Toast(tDto.msg);
            //        }
            //        mIsCanClickCode = true;
            //    });
            //});
        }

        public override void OnShow(object obj)
        {

        }

        public override void OnHide()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

}