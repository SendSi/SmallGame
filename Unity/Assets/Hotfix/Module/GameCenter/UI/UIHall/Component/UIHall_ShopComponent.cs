using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_ShopComponentSystem : AwakeSystem<UIHall_ShopComponent>
    {
        public override void Awake(UIHall_ShopComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_ShopComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        private Image imageShopItem;
        private Transform transShopItems;
        private Text textCoin;
        private Toggle toggleAgency;
        private Toggle toggleAlipay;
        private Toggle toggleScene;
        Dictionary<string, Sprite> mDicStrSprite = new Dictionary<string, Sprite>();

        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {
            //SetUpNav("名字", UIType.UIHall_Shop);
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
            UIEventListener.Get(rc.Get<GameObject>("Button_Back")).onClick = ClickButton_Back;
            imageShopItem = rc.Get<GameObject>("Image_ShopItem").GetComponent<Image>();
            transShopItems = rc.Get<GameObject>("ShopItems").transform;
            textCoin = rc.Get<GameObject>("Text_Coin").GetComponent<Text>();
            toggleAgency = rc.Get<GameObject>("Toggle_Agency").GetComponent<Toggle>();
            toggleAlipay = rc.Get<GameObject>("Toggle_Alipay").GetComponent<Toggle>();
            toggleScene = rc.Get<GameObject>("Toggle_Scene").GetComponent<Toggle>();
            mDicStrSprite = new Dictionary<string, Sprite>() {
                { "shopdiamond",rc.Get<Sprite>("shopdiamond")},
            };
        }

        private void ClickButton_Back(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UIHall_Shop);
        }
    }
}


