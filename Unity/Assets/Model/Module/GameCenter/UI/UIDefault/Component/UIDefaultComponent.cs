using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    [ObjectSystem]
    public class UIDefaultComponentAwakeSystem : AwakeSystem<UIDefaultComponent>
    {
        public override void Awake(UIDefaultComponent self)
        {
            self.Awake();
        }
    }

    public class UIDefaultComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        private Image imageLogo;
        private Text textLogo;
        private Transform transarmatureName;

        public void Awake()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            imageLogo = rc.Get<GameObject>("Image_Logo").GetComponent<Image>();
            textLogo = rc.Get<GameObject>("Text_Logo").GetComponent<Text>();
            transarmatureName = rc.Get<GameObject>("armatureName").transform;
        }



        public override void OnShow(object obj)
        {
            
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            base.Dispose();
        }

        public void ActiveLogoAnim(bool active)
        {
            if (null != transarmatureName)
                transarmatureName.gameObject.SetActive(active);
        }

        public void ActiveLogoContent(bool active)
        {
            if (null != textLogo)
                textLogo.gameObject.SetActive(active);
        }
    }
}
