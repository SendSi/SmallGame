using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_CustomerComponentSystem : AwakeSystem<UIHall_CustomerComponent>
    {
        public override void Awake(UIHall_CustomerComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_CustomerComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {         
            //SetUpNav("名字", UIType.UIHall_Customer);
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
        }

        private void ClickButton_Back(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UIHall_Customer);
        }
    }
}


