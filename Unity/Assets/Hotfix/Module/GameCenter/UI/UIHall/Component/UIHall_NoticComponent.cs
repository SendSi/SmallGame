using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_NoticComponentSystem : AwakeSystem<UIHall_NoticComponent>
    {
        public override void Awake(UIHall_NoticComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_NoticComponent : UIBaseComponent
    {
        private ReferenceCollector rc;

        private Transform transContent;
        private RawImage rawimageNotice;

        private Toggle toggleNoticeItem;

        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {         
            //SetUpNav("名字", UIType.UIHall_Notic);
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
            UIEventListener.Get(rc.Get<GameObject>("Button_Close")).onClick = ClickButton_Close;
            transContent = rc.Get<GameObject>("NoticeUi_Content").transform;
            rawimageNotice = rc.Get<GameObject>("NoticeUi_RawImage_Notice").GetComponent<RawImage>();        
            toggleNoticeItem = rc.Get<GameObject>("NoticeUi_Toggle_NoticeItem").GetComponent<Toggle>();
        }

        private void ClickButton_Close(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UIHall_Notic);
        }
    }
}


