using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_ServiceClauseComponentSystem : AwakeSystem<UIHall_ServiceClauseComponent>
    {
        public override void Awake(UIHall_ServiceClauseComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_ServiceClauseComponent : UIBaseComponent

    {
        private ReferenceCollector rc;
        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {         
            //SetUpNav("名字", UIType.UIHall_ServiceClause);
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
        }

        private void ClickButton_Close(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UIHall_ServiceClause);
        }
    }
}

 
