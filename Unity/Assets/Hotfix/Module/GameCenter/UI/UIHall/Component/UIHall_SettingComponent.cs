using System;
using System.Collections.Generic;
using DG.Tweening;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_SettingComponentSystem : AwakeSystem<UIHall_SettingComponent>
    {
        public override void Awake(UIHall_SettingComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_SettingComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        Dictionary<string, Sprite> mDicStrSprite = new Dictionary<string, Sprite>();
        private Button buttonBgm;
        private Button buttonChangeScene;
        private Button buttonPrivacyPolicy;
        private Button buttonSfx;
        private Button buttonShake;
        private Button buttonTermsOfService;
        private Button buttonUseragree;
        private Image imageBgmBall;
        private Image imageShakeBall;
        private Image imageSfxBall;
        private Image imageHead;
        private Image imageScene;
        private Text textBgmContent;
        private Text textId;
        private Text textNickname;
        private Text textSceneName;
        private Text textSfxContent;
        private Text textShakeContent;

        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {
            //SetUpNav("名字", UIType.UIHall_Setting);
        }

        public override void OnHide()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        protected virtual void InitUI()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            buttonBgm = rc.Get<GameObject>("SettingUi_Button_Bgm").GetComponent<Button>();
            UIEventListener.Get(rc.Get<GameObject>("SettingUi_Button_ChangeAccount")).onClick = ClickChangeAccount;
            buttonChangeScene = rc.Get<GameObject>("SettingUi_Button_ChangeScene").GetComponent<Button>();
            UIEventListener.Get(rc.Get<GameObject>("Button_Close")).onClick = ClickButton_Close;
            buttonPrivacyPolicy = rc.Get<GameObject>("SettingUi_Button_PrivacyPolicy").GetComponent<Button>();
            buttonSfx = rc.Get<GameObject>("SettingUi_Button_Sfx").GetComponent<Button>();
            buttonShake = rc.Get<GameObject>("SettingUi_Button_Shake").GetComponent<Button>();
            buttonTermsOfService = rc.Get<GameObject>("SettingUi_Button_TermsOfService").GetComponent<Button>();
            buttonUseragree = rc.Get<GameObject>("SettingUi_Button_Useragree").GetComponent<Button>();
            imageBgmBall = rc.Get<GameObject>("SettingUi_Image_BgmBall").GetComponent<Image>();
            imageShakeBall = rc.Get<GameObject>("SettingUi_Image_ShakeBall").GetComponent<Image>();
            imageSfxBall = rc.Get<GameObject>("SettingUi_Image_SfxBall").GetComponent<Image>();
            imageHead = rc.Get<GameObject>("SettingUi_Image_Head").GetComponent<Image>();
            imageScene = rc.Get<GameObject>("SettingUi_Image_Scene").GetComponent<Image>();
            textBgmContent = rc.Get<GameObject>("SettingUi_Text_BgmContent").GetComponent<Text>();
            textId = rc.Get<GameObject>("SettingUi_Text_Id").GetComponent<Text>();
            textNickname = rc.Get<GameObject>("SettingUi_Text_Nickname").GetComponent<Text>();
            textSceneName = rc.Get<GameObject>("SettingUi_Text_SceneName").GetComponent<Text>();
            textSfxContent = rc.Get<GameObject>("SettingUi_Text_SfxContent").GetComponent<Text>();
            textShakeContent = rc.Get<GameObject>("SettingUi_Text_ShakeContent").GetComponent<Text>();

            UIEventListener.Get(buttonSfx.gameObject).onClick = ClickButtonSfx;
            UIEventListener.Get(buttonBgm.gameObject).onClick = ClickButtonBgm;
            UIEventListener.Get(buttonShake.gameObject).onClick = ClickButtonShake;
            UIEventListener.Get(buttonPrivacyPolicy.gameObject).onClick = ClickButtonPrivacyPolicy;
            UIEventListener.Get(buttonTermsOfService.gameObject).onClick = ClickButtonTermsOfService;
            UIEventListener.Get(buttonUseragree.gameObject).onClick = ClickButtonUseragree;

            mDicStrSprite = new Dictionary<string, Sprite>() {
                { "noselect",rc.Get<Sprite>("noselect")},
                { "select",rc.Get<Sprite>("select")},
                { "ballNoSelect",rc.Get<Sprite>("ballNoSelect")},
                { "ballSelect",rc.Get<Sprite>("ballSelect")},
            };

            var bgm = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.Key_Bgm);
            var sfx = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.Key_Sfx);
            var shake = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.Key_Shake);
            GetButtonToggle(bgm, textBgmContent, imageBgmBall);
            GetButtonToggle(sfx, textSfxContent, imageSfxBall);
            GetButtonToggle(shake, textShakeContent, imageShakeBall);
        }

        void GetButtonToggle(int state, Text text, Image image)
        {
            if (state == 0)//开启了
            {
                text.text = "开";
                image.sprite = mDicStrSprite["ballSelect"];
                image.transform.localPosition = new Vector3(50, 0, 0);
            }
            else
            {
                text.text = "关";
                image.sprite = mDicStrSprite["ballNoSelect"];
                image.transform.localPosition = new Vector3(-50, 0, 0);
            }
        }


        private void ClickButtonPrivacyPolicy(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_PrivacyPolicy);
            UIComponent.Instance.Remove(UIType.UIHall_Setting);
        }

        private void ClickButtonTermsOfService(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_ServiceClause);
            UIComponent.Instance.Remove(UIType.UIHall_Setting);
        }

        private void ClickButtonUseragree(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_Upgrade);
            UIComponent.Instance.Remove(UIType.UIHall_Setting);
        }

        int SetButtonToggle(int oldState, Text text, Image image)
        {
            if (oldState == 0)//开启了
            {
                image.transform.DOLocalMoveX(-50, 0.05f).OnComplete(() =>
                {
                    text.text = "关";
                    image.sprite = mDicStrSprite["ballNoSelect"];
                });
                return 1;
            }
            else
            {
                image.transform.DOLocalMoveX(50, 0.05f).OnComplete(() =>
                {
                    text.text = "开";
                    image.sprite = mDicStrSprite["ballSelect"];
                });
                return 0;
            }
        }

        private void ClickButtonShake(GameObject go)
        {
            var shake = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.Key_Shake);
            var newShake = SetButtonToggle(shake, textShakeContent, imageShakeBall);
            PlayerPrefsMgr.mInstance.SetInt(PlayerPrefsKeys.Key_Shake, newShake);
        }

        private void ClickButtonBgm(GameObject go)
        {
            var bgm = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.Key_Bgm);
            var newBgm = SetButtonToggle(bgm, textBgmContent, imageBgmBall);
            PlayerPrefsMgr.mInstance.SetInt(PlayerPrefsKeys.Key_Bgm, newBgm);
        }

        private void ClickButtonSfx(GameObject go)
        {
            var sfx = PlayerPrefsMgr.mInstance.GetInt(PlayerPrefsKeys.Key_Sfx);
            var newSfx = SetButtonToggle(sfx, textSfxContent, imageSfxBall);
            PlayerPrefsMgr.mInstance.SetInt(PlayerPrefsKeys.Key_Sfx, newSfx);
        }

        private void ClickChangeAccount(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UILogin);
            UIComponent.Instance.Remove(UIType.UIHall_Main);
            UIComponent.Instance.Remove(UIType.UIHall_Setting);
        }

        private void ClickButton_Close(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UIHall_Setting);
        }
    }
}


