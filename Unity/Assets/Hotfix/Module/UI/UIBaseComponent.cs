using UnityEngine;
using System;

namespace ETHotfix
{
    public class UIBaseComponent : Component
    {
        public virtual void OnShow(object obj)
        {
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }

        public void SetUpNav(string title, string type, Action rightAction = null, Action leftAction = null)
        {
            // Transform navTransform = this.GetParent<UI>().GameObject.transform.Find(UIType.UINavigationBar);
            // if (null != navTransform)
            // {
            //     GameObject navGo = navTransform.gameObject;
            //     UI navUi = ComponentFactory.Create<UI, GameObject>(navGo);
            //     navUi.AddUIBaseComponent<UINavigationBarComponent>();
            //     if (null == leftAction)
            //     {
            //         leftAction = () => { UIComponent.Instance.Remove(type); };
            //     }
            //     navUi.UiBaseComponent.OnShow(new UINavigationBarComponent.NavigationBarInfo()
            //     {
            //         Title = title,
            //         OnClickBack = (go) => { leftAction.Invoke(); },
            //         OnClickMenu = (go) => { rightAction.Invoke(); }
            //     });
            // }
        }
        /// <summary>        /// 多语言  title在*.txt配        /// </summary>
        public void SetUpNav(string type, Action rightAction = null, Action leftAction = null)
        {
            var title = LanguageMgr.mInstance.GetLanguageForKey(type);
            SetUpNav(title, type, rightAction, leftAction);
        }

        /// <summary>
        /// initUI之后 设置Text的文字
        /// </summary>
        public virtual void InitLanguage()
        {
        }
    }
}
