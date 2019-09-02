using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    [ObjectSystem]
    public class UICarrierDataNetworkComponentAwakeSystem : AwakeSystem<UICarrierDataNetworkComponent>
    {
        public override void Awake(UICarrierDataNetworkComponent self)
        {
            self.Awake();
        }
    }

    public class UICarrierDataNetworkComponent : UIBaseComponent
    {
        public sealed class UICarrierDataNetworkData
        {
            public string Msg;
            public Action Callback;
        }

        private ReferenceCollector rc;
        private Button buttonCommit;
        private Button buttonCancel;
        private Text textContent;

        private UICarrierDataNetworkData data; 

        public void Awake()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            buttonCommit = rc.Get<GameObject>("Button_Commit").GetComponent<Button>();
            buttonCancel = rc.Get<GameObject>("Button_Cancel").GetComponent<Button>();
            textContent = rc.Get<GameObject>("Text_Content").GetComponent<Text>();

            UIEventListener.Get(this.buttonCommit.gameObject).onClick = onClickCommit;
            UIEventListener.Get(this.buttonCancel.gameObject).onClick = onClickCancel;
        }

        private async void onClickCancel(GameObject go)
        {
            UnityEngine.Application.Quit();
        }

        private async void onClickCommit(GameObject go)
        {
            if (null != data)
            {
                data.Callback();
            }
        }

        public override void OnShow(object obj)
        {
            data = obj as UICarrierDataNetworkData;
            if (data?.Msg != null)
            {
                textContent.text = data.Msg;
            }
        }

        public override void OnHide()
        {
            data = null;
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            data = null;

            base.Dispose();
        }
    }
}
