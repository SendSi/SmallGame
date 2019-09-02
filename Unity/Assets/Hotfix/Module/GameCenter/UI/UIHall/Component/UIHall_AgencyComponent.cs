using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHall_AgencyComponentSystem : AwakeSystem<UIHall_AgencyComponent>
    {
        public override void Awake(UIHall_AgencyComponent self)
        {
            self.Awake();
        }
    }

    /// <summary> 页面名: </summary>
    public class UIHall_AgencyComponent : UIBaseComponent
    {
        private ReferenceCollector rc;
        private List<GameObject> viewIndexs;



        public void Awake()
        {
            InitUI();
        }

        public override void OnShow(object obj)
        {         
            //SetUpNav("名字", UIType.UIHall_Agency);
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
            viewIndexs = new List<GameObject>();
            for (int i = 0; i < 6; i++)
            {
                viewIndexs.Add(rc.Get<GameObject>("view_index_"+(i.ToString())));
                UIEventListener.Get(rc.Get<GameObject>("btn_view_"+(i.ToString())),i).onIntClick=ClickIntBtnView;
            }
        }

        private void ClickIntBtnView(GameObject go, int index)
        {
            for (int i = 0; i < viewIndexs.Count; i++)
            {
                viewIndexs[i].SetActive(false);
            }
            viewIndexs[index].SetActive(true);
        }

        private void ClickButton_Back(GameObject go)
        {
            UIComponent.Instance.Remove(UIType.UIHall_Agency);
        }
    }
}


