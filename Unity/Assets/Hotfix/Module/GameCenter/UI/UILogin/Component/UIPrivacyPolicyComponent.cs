using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPrivacyPolicyComponentSystem : AwakeSystem<UIPrivacyPolicyComponent>
    {
        public override void Awake(UIPrivacyPolicyComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名:隐私政策 </summary>
    public class UIPrivacyPolicyComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {         
            //SetUpNav("名字", UIType.UIPrivacyPolicy);
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
        }  
    }
}


