using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    [ObjectSystem]
    public class UIDialogComponentAwakeSystem : AwakeSystem<UIDialogComponent>
    {
        public override void Awake(UIDialogComponent self)
        {
            self.Awake();
        }
    }

    public class UIDialogComponent : UIBaseComponent
    {
        public sealed class DialogData
        {
            public enum DialogType
            {
                Commit,
                CommitCancel
            }

            public DialogType type;
            public string title;
            public string content;
            public string contentCommit;
            public string contentCancel;
            public Action actionCommit;
            public Action actionCancel;
        }

        private ReferenceCollector rc;
        private Button buttonCommit;
        private Button buttonCancel;
        private Text textCommit;
        private Text textCancel;
        private Text textContent;
        private Text textTitle;

        private DialogData curDialogData;

        public void Awake()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            buttonCommit = rc.Get<GameObject>("Button_Commit").GetComponent<Button>();
            buttonCancel = rc.Get<GameObject>("Button_Cancel").GetComponent<Button>();
            textCommit = rc.Get<GameObject>("Text_Commit").GetComponent<Text>();
            textCancel = rc.Get<GameObject>("Text_Cancel").GetComponent<Text>();
            textContent = rc.Get<GameObject>("Text_Content").GetComponent<Text>();
            textTitle = rc.Get<GameObject>("Text_Title").GetComponent<Text>();

            UIEventListener.Get(buttonCommit.gameObject).onClick = onClickCommit;
            UIEventListener.Get(buttonCancel.gameObject).onClick = onClickCancel;

        }

        public override void OnShow(object obj)
        {
            if (null != obj)
            {
                curDialogData = obj as DialogData;
                if (null != curDialogData)
                {
                    textContent.text = string.IsNullOrEmpty(curDialogData.content) ? $"" : curDialogData.content;
                    textCommit.text = string.IsNullOrEmpty(curDialogData.contentCommit) ? $"Commit" : curDialogData.contentCommit;
                    textCancel.text = string.IsNullOrEmpty(curDialogData.contentCancel) ? $"Cancel" : curDialogData.contentCancel;
                    textTitle.text = string.IsNullOrEmpty(curDialogData.title) ? $"" : curDialogData.title;

                    switch (curDialogData.type)
                    {
                        case DialogData.DialogType.Commit:
                            buttonCommit.transform.localPosition = new Vector3(0, -180f);
                            buttonCancel.gameObject.SetActive(false);
                            break;
                        case DialogData.DialogType.CommitCancel:
                            buttonCommit.transform.localPosition = new Vector3(240f, -175f);
                            buttonCancel.transform.localPosition = new Vector3(-240f, -175f);
                            buttonCancel.gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            base.Dispose();
        }

        private void onClickCancel(GameObject go)
        {
            if (null != curDialogData && curDialogData.type == DialogData.DialogType.CommitCancel && null != curDialogData.actionCancel)
            {
                curDialogData.actionCancel.Invoke();
            }

            UIComponent.Instance.HideNoAnimation(UIType.UIDialog);
        }

        private void onClickCommit(GameObject go)
        {
            if (null != curDialogData && null != curDialogData.actionCommit)
            {
                curDialogData.actionCommit.Invoke();
            }

            UIComponent.Instance.HideNoAnimation(UIType.UIDialog);
        }
    }
}
