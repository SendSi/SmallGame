using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    #region 
    [UIFactory(UIType.UIHall_Main)]
    public class UIHall_MainFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_MainComponent>();
                AddSubComponent(ui);
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_MainComponent>();
                AddSubComponent(ui);
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            RemoveSubComponents();
            SingleFactory.mInstance.Remove(type);
        }

        public void RemoveSubComponents()
        {
            UIComponent uiComponent = Game.Scene.GetComponent<UIComponent>();
            for (int i = 0, n = subs.Count; i < n; i++)
            {
                uiComponent.RemoveSub(subs[i]);
            }
        }
        private List<string> subs = new List<string>();
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
            if (subs == null)
                subs = new List<string>();
            else
                subs.Clear();
            UIComponent uiComponent = Game.Scene.GetComponent<UIComponent>();
            GameObject go = ui.GameObject.transform.Find(UIType.UIHall_LeftMenu).gameObject;
            UI subUi = ComponentFactory.Create<UI, GameObject>(go);
            subUi.AddUIBaseComponent<UIHall_LeftMenuComponent>();
            uiComponent.Add(UIType.UIHall_LeftMenu, subUi);
            subs.Add(UIType.UIHall_LeftMenu);
        }
    }
    #endregion

    #region 
    [UIFactory(UIType.UIHall_Notic)]
    public class UIHall_NoticFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_NoticComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_NoticComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion

    #region 
    [UIFactory(UIType.UIHall_Shop)]
    public class UIHall_ShopFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_ShopComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_ShopComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion
    #region 
    [UIFactory(UIType.UIHall_Setting)]
    public class UIHall_SettingFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_SettingComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_SettingComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion
    #region 
    [UIFactory(UIType.UIHall_Agency)]
    public class UIHall_AgencyFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_AgencyComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_AgencyComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion

    #region 
    [UIFactory(UIType.UIHall_Customer)]
    public class UIHall_CustomerFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_CustomerComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_CustomerComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion

    #region 
    [UIFactory(UIType.UIHall_SelectHead)]
    public class UIHall_SelectHeadFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_SelectHeadComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_SelectHeadComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion


    #region 
    [UIFactory(UIType.UIHall_PrivacyPolicy)]
    public class UIHall_PrivacyPolicyFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_PrivacyPolicyComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_PrivacyPolicyComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion

    #region 
    [UIFactory(UIType.UIHall_ServiceClause)]
    public class UIHall_ServiceClauseFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_ServiceClauseComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_ServiceClauseComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion

    #region 
    [UIFactory(UIType.UIHall_Upgrade)]
    public class UIHall_UpgradeFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIHall_UpgradeComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIHall_UpgradeComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        //若有子结构,记得RemoveSub
        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }
        //若有子结构,在add完base下再调用 此方法
        public void AddSubComponent(UI ui)
        {
        }
    }
    #endregion
}
