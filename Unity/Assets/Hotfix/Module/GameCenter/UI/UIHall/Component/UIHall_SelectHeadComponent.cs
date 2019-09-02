using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_SelectHeadComponentSystem : AwakeSystem<UIHall_SelectHeadComponent>
    {
        public override void Awake(UIHall_SelectHeadComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_SelectHeadComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {         
            //SetUpNav("名字", UIType.UIHall_SelectHead);
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
            UIEventListener.Get(rc.Get<GameObject>("Center_XuanzetouxiangCloseBtn")).onClick = ClickCenterCloseBtn;
        }

        private void ClickCenterCloseBtn(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UIHall_SelectHead);
        }
    }
}

