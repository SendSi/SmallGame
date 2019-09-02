using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UiComponentAwakeSystem : AwakeSystem<UIComponent>
    {
        public override void Awake(UIComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class UiComponentLoadSystem : LoadSystem<UIComponent>
    {
        public override void Load(UIComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 管理所有UI
    /// </summary>
    public class UIComponent : Component
    {
        public static UIComponent Instance;

        private GameObject Root;
        private readonly Dictionary<string, IUIFactoryExtend> UiTypes = new Dictionary<string, IUIFactoryExtend>();
        private readonly Dictionary<string, UI> uis = new Dictionary<string, UI>();

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            foreach (string type in uis.Keys.ToArray())
            {
                UI ui;
                if (!uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                uis.Remove(type);
                ui.Dispose();
            }

            this.UiTypes.Clear();
            this.uis.Clear();

            Instance = null;

            base.Dispose();
        }

        public void Awake()
        {
            this.Root = GameObject.Find("Global/UI/");
            this.Load();

            if (Screen.width / (Screen.height * 1f) >= 3 / 4f)
            {
                for (int i = 0, n = this.Root.transform.childCount; i < n; i++)
                {
                    this.Root.transform.GetChild(i).GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
                }
            }

            Instance = this;
        }

        public void Load()
        {
            UiTypes.Clear();

            List<Type> types = Game.EventSystem.GetTypes();

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UIFactoryAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                UIFactoryAttribute attribute = attrs[0] as UIFactoryAttribute;
                if (UiTypes.ContainsKey(attribute.Type))
                {
                    Log.Debug($"已经存在同类UI Factory: {attribute.Type}");
                    throw new Exception($"已经存在同类UI Factory: {attribute.Type}");
                }
                object o = Activator.CreateInstance(type);
                IUIFactoryExtend factory = o as IUIFactoryExtend;
                if (factory == null)
                {
                    Log.Error($"{o.GetType().FullName} 没有继承 IUIFactory");
                    continue;
                }
                this.UiTypes.Add(attribute.Type, factory);
            }
        }

        public UI Create(string type)
        {
            try
            {
                UI ui = UiTypes[type].Create(this.GetParent<Scene>(), type, Root);
                uis.Add(type, ui);

                // 设置canvas
                string cavasName = ui.GameObject.GetComponent<CanvasConfig>().CanvasName;
                ui.GameObject.transform.SetParent(this.Root.Get<GameObject>(cavasName).transform, false);
                return ui;
            }
            catch (Exception e)
            {
                throw new Exception($"{type} UI 错误: {e}");
            }
        }

        public async Task<UI> CreateAsync(string type)
        {
            try
            {
                UI ui = await this.UiTypes[type].CreateAsync(this.GetParent<Scene>(), type, this.Root);
                this.uis.Add(type, ui);

                // 设置canvas
                string cavasName = ui.GameObject.GetComponent<CanvasConfig>().CanvasName;
                ui.GameObject.transform.SetParent(this.Root.Get<GameObject>(cavasName).transform, false);
                return ui;
            }
            catch (Exception e)
            {
                throw new Exception($"Async {type} UI 错误: {e}");
            }
        }

        public void Add(string type, UI ui)
        {
            this.uis.Add(type, ui);
        }

        public void RemoveSub(string type)
        {
            UI ui;
            if (!this.uis.TryGetValue(type, out ui))
            {
                return;
            }

            this.uis.Remove(type);
            ui.Dispose();
        }

        public void Remove(string type)
        {
            UI ui;
            if (!uis.TryGetValue(type, out ui))
            {
                return;
            }
            UiTypes[type].Remove(type);
            uis.Remove(type);
            ui.Dispose();
        }

        public void RemoveAll(List<string> filter = null)
        {
            foreach (string type in this.uis.Keys.ToArray())
            {
                if (null != filter && filter.Contains(type))
                    continue;

                UI ui;
                if (!this.uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                this.uis.Remove(type);
                ui.Dispose();
            }
        }

        public UI Get(string type)
        {
            UI ui;
            this.uis.TryGetValue(type, out ui);
            return ui;
        }

        public List<string> GetUITypeList()
        {
            return new List<string>(this.uis.Keys);
        }

        public IUIFactoryExtend GetUIFactory(string type)
        {
            IUIFactoryExtend uiFactory = null;
            this.UiTypes.TryGetValue(type, out uiFactory);
            return uiFactory;
        }

        public UI ShowNoAnimation(string type, object param = null, Action action = null)
        {
            return Show(type, param, action, 0);
        }

        public UI Show(string type, object param = null, Action action = null, float time = 0.25f, Ease ease = Ease.OutBack)
        {
            UI ui = Get(type);
            if (null == ui)
            {
                ui = Create(type);
            }
            else
            {
                if (ui.GameObject.activeInHierarchy)
                {
                    ui.GameObject.transform.localScale = Vector3.one;
                }
                else
                {
                    ui.GameObject.SetActive(true);
                }
            }

            ui.UiBaseComponent?.OnShow(param);

            if (time == 0)
            {
                action?.Invoke();
            }
            else
            {
                if (null == action)
                {
                    ShowAnimation(ui, null, time, ease);
                }
                else
                {
                    ShowAnimation(ui, () => { action.Invoke(); }, time, ease);
                }
            }

            return ui;
        }

        public async Task<UI> ShowAsyncNoAnimation(string type, object param = null, Action action = null)
        {
            return await ShowAsync(type, param, action, 0);
        }

        public async Task<UI> ShowAsync(string type, object param = null, Action action = null, float time = 0.25f, Ease ease = Ease.OutBack)
        {
            UI ui = Get(type);
            if (null == ui)
            {
                ui = await CreateAsync(type);
            }
            else
            {
                if (ui.GameObject.activeInHierarchy)
                {
                    // ui.GameObject.transform.localScale = Vector3.one;
                }
                else
                {
                    ui.GameObject.SetActive(true);
                }
                ui.GameObject.transform.localScale = Vector3.one;
            }

            ui.UiBaseComponent?.OnShow(param);

            if (time == 0)
            {
                action?.Invoke();
            }
            else
            {
                if (null == action)
                {
                    ShowAnimation(ui, null, time, ease);
                }
                else
                {
                    ShowAnimation(ui, () => { action.Invoke(); }, time, ease);
                }
            }

            return ui;
        }

        public void RemoveAnimated(string type, Action action = null)
        {
            Hide(type, () =>
            {
                Remove(type);
                action.Invoke();
            });
        }

        public void HideNoAnimation(string type, Action action = null)
        {
            Hide(type, action, 0);
        }

        public void Hide(string type, Action action = null, float time = 0.25f, Ease ease = Ease.InBack)
        {
            UI ui = Get(type);
            if (null != ui)
            {
                ui.UiBaseComponent?.OnHide();
                if (null != action)
                {
                    if (time == 0)
                    {
                        action.Invoke();
                        ui.GameObject.SetActive(false); // 可以考虑不隐藏
                        // ui.GameObject.transform.localScale = Vector3.zero;
                    }
                    else
                    {
                        BackAnimation(ui, () =>
                        {
                            action.Invoke();
                            ui.GameObject.SetActive(false); // 可以考虑不隐藏
                            // ui.GameObject.transform.localScale = Vector3.zero;
                        }, time, ease);
                    }
                }
                else
                {
                    if (time == 0)
                    {
                        ui.GameObject.SetActive(false); // 可以考虑设置为Vector3.zero
                        // ui.GameObject.transform.localScale = Vector3.zero;
                    }
                    else
                    {
                        BackAnimation(ui, () =>
                        {
                            ui.GameObject.SetActive(false); // 可以考虑设置为Vector3.zero
                            // ui.GameObject.transform.localScale = Vector3.zero;
                        }, time, ease);
                    }
                }
            }
        }

        public void Toast(string content)
        {
            ShowNoAnimation(UIType.UIToast, content);
        }

        public void ToastLanguage(string key)
        {
            var tStr = LanguageMgr.mInstance.GetLanguageForKey(key);
            if (string.IsNullOrEmpty(tStr) == false)
                ShowNoAnimation(UIType.UIToast, tStr);
            else
            {
                ShowNoAnimation(UIType.UIToast, "多语言:未找到呀value,key=" + key);
            }
        }
        public void ToastFormat1(string key, string value1)
        {
            var tStr = LanguageMgr.mInstance.GetLanguageForKey(key);
            if (string.IsNullOrEmpty(tStr) == false)
            {
                ShowNoAnimation(UIType.UIToast, string.Format(tStr, value1));
            }
            else
            {
                ShowNoAnimation(UIType.UIToast, "多语言:未找到呀value,key=" + key);
            }
        }


        public void Toast(int status)
        {
            var tStr = LanguageMgr.mInstance.GetLanguageForKey("error" + status.ToString());
            if (string.IsNullOrEmpty(tStr) == false)
                ShowNoAnimation(UIType.UIToast, tStr);
            else
            {
                tStr = LanguageMgr.mInstance.GetLanguageForKey("errorDefault");
                ShowNoAnimation(UIType.UIToast, tStr);
            }
        }

        public void Toast(int status, string msg)
        {
            ShowNoAnimation(UIType.UIToast, "错误码:" + status.ToString() + "," + msg);
        }

        public void Prompt()
        {
            ShowNoAnimation(UIType.UIPrompt);
        }

        public void ClosePrompt()
        {
            HideNoAnimation(UIType.UIPrompt);
        }


        private void ShowAnimation(UI ui, TweenCallback callback, float time = 0.25f, Ease ease = Ease.OutBack)
        {
            ui.GameObject.transform.localScale = Vector3.zero;
            if (null != callback)
                ui.GameObject.transform.DOScale(Vector3.one, time).SetEase(ease).OnComplete(callback);
            else
                ui.GameObject.transform.DOScale(Vector3.one, time).SetEase(ease);
        }

        private void BackAnimation(UI ui, TweenCallback callback, float time = 0.25f, Ease ease = Ease.InBack)
        {
            ui.GameObject.transform.localScale = Vector3.one;
            if (null != callback)
                ui.GameObject.transform.DOScale(Vector3.zero, time).SetEase(ease).OnComplete(callback);
            else
                ui.GameObject.transform.DOScale(Vector3.zero, time).SetEase(ease);
        }
    }
}