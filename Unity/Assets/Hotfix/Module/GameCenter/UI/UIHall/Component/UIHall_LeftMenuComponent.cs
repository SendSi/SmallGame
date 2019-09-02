using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_LeftMenuComponentSystem : AwakeSystem<UIHall_LeftMenuComponent>
    {
        public override void Awake(UIHall_LeftMenuComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_LeftMenuComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {         
            //SetUpNav("名字", UIType.UIHall_LeftMenu);
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
            UIEventListener.Get(rc.Get<GameObject>("Button_Shop")).onClick = ClickButton_Shop;
            UIEventListener.Get(rc.Get<GameObject>("Button_Notic")).onClick = ClickButton_Notic;
            UIEventListener.Get(rc.Get<GameObject>("Button_Setting")).onClick = ClickButton_Setting;
            UIEventListener.Get(rc.Get<GameObject>("Button_Agency")).onClick = ClickButton_Agency;
            UIEventListener.Get(rc.Get<GameObject>("Button_Customer")).onClick = ClickButton_Customer;
        }

        private void ClickButton_Shop(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_Shop);
        }

        private void ClickButton_Notic(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_Notic);
        }

        private void ClickButton_Setting(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_Setting);
        }

        private void ClickButton_Agency(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_Agency);
        }

        private void ClickButton_Customer(GameObject go)
        {
            UIComponent.Instance.ShowNoAnimation(UIType.UIHall_Customer);
        }
    }
}

