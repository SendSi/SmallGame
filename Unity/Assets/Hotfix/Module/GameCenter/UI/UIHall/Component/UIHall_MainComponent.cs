using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_MainComponentSystem : AwakeSystem<UIHall_MainComponent>
    {
        public override void Awake(UIHall_MainComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_MainComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        private Button buttonAddCoin;
        private Button buttonLeftArrow;
        private Button buttonRanking;
        private Button buttonRecharge;
        private Button buttonRightArrow;
        private Transform transContent;
        private Image imageHead;
        private Image imageHeadFrame;
        private Image imageMarquee;
        private Image imageRed;
        private Scrollbar scrollbarGames;
        private Text textCoin;
        private Text textId;
        private Text textMarquee;
        private Text textNickname;
        private Transform transbaijiale;
        private Transform transbuyu;
        private Transform transerbagang;
        private Transform transhaochehui;
        private Transform transhongheidazhan;
        private Transform translonghu;
        private Transform transniuniubairen;
        private Transform transniuniuqz;
        private Transform transsenlin;
        private Transform transshuiguoji;
        private Transform transtoubao;
        private Transform transzhajinhua;

        public void Awake()
        {
            InitUI();
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
        protected virtual void InitUI()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            buttonAddCoin = rc.Get<GameObject>("HallUi_Button_AddCoin").GetComponent<Button>();
            buttonLeftArrow = rc.Get<GameObject>("HallUi_Button_LeftArrow").GetComponent<Button>();
            buttonRanking = rc.Get<GameObject>("HallUi_Button_Ranking").GetComponent<Button>();
            buttonRecharge = rc.Get<GameObject>("HallUi_Button_Recharge").GetComponent<Button>();
            buttonRightArrow = rc.Get<GameObject>("HallUi_Button_RightArrow").GetComponent<Button>();
            transContent = rc.Get<GameObject>("HallUi_Content").transform;
            imageHead = rc.Get<GameObject>("HallUi_Image_Head").GetComponent<Image>();
            imageHeadFrame = rc.Get<GameObject>("HallUi_Image_HeadFrame").GetComponent<Image>();
            imageMarquee = rc.Get<GameObject>("HallUi_Image_Marquee").GetComponent<Image>();
            imageRed = rc.Get<GameObject>("HallUi_Notice_Image_Red").GetComponent<Image>();
            scrollbarGames = rc.Get<GameObject>("HallUi_Scrollbar_Games").GetComponent<Scrollbar>();
            textCoin = rc.Get<GameObject>("HallUi_Text_Coin").GetComponent<Text>();
            textId = rc.Get<GameObject>("HallUi_Text_Id").GetComponent<Text>();
            textMarquee = rc.Get<GameObject>("HallUi_Text_Marquee").GetComponent<Text>();
            textNickname = rc.Get<GameObject>("HallUi_Text_Nickname").GetComponent<Text>();
            transbaijiale = rc.Get<GameObject>("HallUi_baijiale").transform;
            transbuyu = rc.Get<GameObject>("HallUi_buyu").transform;
            transerbagang = rc.Get<GameObject>("HallUi_erbagang").transform;
            transhaochehui = rc.Get<GameObject>("HallUi_haochehui").transform;
            transhongheidazhan = rc.Get<GameObject>("HallUi_hongheidazhan").transform;
            translonghu = rc.Get<GameObject>("HallUi_longhu").transform;
            transniuniubairen = rc.Get<GameObject>("HallUi_niuniubairen").transform;
            transniuniuqz = rc.Get<GameObject>("HallUi_niuniuqz").transform;
            transsenlin = rc.Get<GameObject>("HallUi_senlin").transform;
            transshuiguoji = rc.Get<GameObject>("HallUi_shuiguoji").transform;
            transtoubao = rc.Get<GameObject>("HallUi_toubao").transform;
            transzhajinhua = rc.Get<GameObject>("HallUi_zhajinhua").transform;
            UIEventListener.Get(rc.Get<GameObject>("Image_InfoFrame")).onClick = ClickImage_InfoFrame;

            SetTopLeftInfo();
        }

        private void ClickImage_InfoFrame(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_SelectHead);
        }

        void SetTopLeftInfo()
        {
            textId.text = "123456";
            textNickname.text = "昵称名";
            textCoin.text = "9999";

        }
    }
}


