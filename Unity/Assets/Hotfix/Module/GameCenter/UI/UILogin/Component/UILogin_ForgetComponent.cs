using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UILogin_ForgetComponentSystem : AwakeSystem<UILogin_ForgetComponent>
    {
        public override void Awake(UILogin_ForgetComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名:忘记密码 </summary>
    public class UILogin_ForgetComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        private Dropdown dropdownArea;
        private InputField inputfieldAccount;
        private InputField inputfieldPassword;
        private InputField inputfieldSecurityCode;
        private Image imageShowPassword;
        private Text textSecurityCode;
        Dictionary<string, Sprite> mDicStrSprite = new Dictionary<string, Sprite>();

        public void Awake()
        {
            InitUI();
        }
        protected virtual void InitUI()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            dropdownArea = rc.Get<GameObject>("RetrievePasswordUi_Dropdown_Area").GetComponent<Dropdown>();
            inputfieldAccount = rc.Get<GameObject>("RetrievePasswordUi_InputField_Account").GetComponent<InputField>();
            inputfieldPassword = rc.Get<GameObject>("RetrievePasswordUi_InputField_Password").GetComponent<InputField>();
            inputfieldSecurityCode = rc.Get<GameObject>("RetrievePasswordUi_InputField_SecurityCode").GetComponent<InputField>();
            textSecurityCode = rc.Get<GameObject>("RetrievePasswordUi_Text_SecurityCode").GetComponent<Text>();

            UIEventListener.Get(rc.Get<GameObject>("Button_Back")).onClick = ClickButton_Back;
            UIEventListener.Get(rc.Get<GameObject>("Button_Retrieve")).onClick = ClickButton_Retrieve;
            UIEventListener.Get(rc.Get<GameObject>("Button_SecurityCode")).onClick = ClickButton_SecurityCode;
            UIEventListener.Get(rc.Get<GameObject>("Button_ShowPasswrod")).onClick = ClickButton_ShowPasswrod;

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

        private void ClickButton_ShowPasswrod(GameObject go)
        {
            inputfieldPassword.contentType = (inputfieldPassword.contentType == InputField.ContentType.Password) ? InputField.ContentType.Standard : InputField.ContentType.Password;
            imageShowPassword.sprite = (inputfieldPassword.contentType == InputField.ContentType.Password) ? mDicStrSprite["eyeOpen"] : mDicStrSprite["eyeClose"];
            imageShowPassword.SetNativeSize();
            inputfieldPassword.ForceLabelUpdate();
        }

        private void ClickButton_SecurityCode(GameObject go)
        {
            string tPhone = inputfieldAccount.text.Trim();
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

        /// <summary>         确认修改密码         </summary>
        private void ClickButton_Retrieve(GameObject go)
        {
            string tAccount = inputfieldAccount.text.Trim();
            string tPwd = inputfieldPassword.text.Trim();
            string tSecurityCode = inputfieldSecurityCode.text.Trim();

            if (string.IsNullOrEmpty(tAccount)) { UIComponent.Instance.Toast("账号不能为空"); return; }
            if (string.IsNullOrEmpty(tPwd)) { UIComponent.Instance.Toast("密码不能为空请重新输入"); return; }
            if (string.IsNullOrEmpty(tSecurityCode)) { UIComponent.Instance.Toast("验证码不能为空请重新输入"); return; }
            if (tPwd.Length < 6) { UIComponent.Instance.Toast("密码长度要大于6位"); return; }

            Log.Debug(dropdownArea.value + "    " + dropdownArea.captionText.text);
            Log.Debug("注册事件");
        }

        private void ClickButton_Back(GameObject go)
        {
            if (textSecurityCode.text != LanguageMgr.mInstance.GetLanguageForKey("UILogin_GetCode"))//若还在走走时间
            {
                //int t = int.Parse(textSecurityCode.text.Replace("S", ""));
                //UILoginModel.mInstance.CalcRemindTime(t,UIType.UILogin_Forget);
                UIComponent.Instance.Hide(UIType.UILogin_Forget,null,0);
            }
            else
            {
                UIComponent.Instance.Remove(UIType.UILogin_Forget);
            }
        }

        private void DropDownSelectEvent(int arg0)
        {
            Log.Debug(dropdownArea.value + "    " + dropdownArea.captionText.text);
            Log.Debug("电话 前缀码 " + UILoginModel.mInstance.GetAreaCode(dropdownArea.captionText.text));
        }

        public override void OnShow(object obj)
        {
            //SetUpNav("名字", UIType.UILogin_Forget);
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


