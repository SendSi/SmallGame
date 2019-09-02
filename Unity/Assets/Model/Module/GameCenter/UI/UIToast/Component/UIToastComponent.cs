using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    [ObjectSystem]
    public class UIToastComponentAwakeSystem : AwakeSystem<UIToastComponent>
    {
        public override void Awake(UIToastComponent self)
        {
            self.Awake();
        }
    }

    public class UIToastComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        private Text textContent;



        public void Awake()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            textContent = rc.Get<GameObject>("Text_Content").GetComponent<Text>();

        }



        public override void OnShow(object obj)
        {
            if (null == obj)
                return;

            textContent.text = obj.ToString();
        }

       

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}
