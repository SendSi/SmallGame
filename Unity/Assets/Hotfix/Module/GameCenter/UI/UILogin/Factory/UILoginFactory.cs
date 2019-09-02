using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UILogin)]
    public class UILoginFactory : IUIFactoryExtend
    {
        private List<string> subs = new List<string>();

        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject login = UnityEngine.Object.Instantiate(bundleGameObject);
                login.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(login);

                ui.AddUIBaseComponent<UILoginComponent>();

                AddSubComponent(ui);
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void Remove(string type)
        {
            RemoveSubComponents();
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                await resourcesComponent.LoadBundleAsync($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject login = UnityEngine.Object.Instantiate(bundleGameObject);
                login.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(login);

                ui.AddUIBaseComponent<UILoginComponent>();

                AddSubComponent(ui);
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void AddSubComponent(UI ui)
        {
            if (subs == null)
                subs = new List<string>();
            else
                subs.Clear();

            // UIComponent uiComponent = Game.Scene.GetComponent<UIComponent>();
            // GameObject go = ui.GameObject.transform.Find(UIType.UILogin_Register).gameObject;
            // UI subUi = ComponentFactory.Create<UI, GameObject>(go);
            // subUi.AddUIBaseComponent<UILogin_RegisterComponent>();
            // uiComponent.Add(UIType.UILogin_Register, subUi);
            // subs.Add(UIType.UILogin_Register);
            //
            // go = ui.GameObject.transform.Find(UIType.UILogin_Forget).gameObject;
            // subUi = ComponentFactory.Create<UI, GameObject>(go);
            // subUi.AddUIBaseComponent<UILogin_ForgetComponent>();
            // uiComponent.Add(UIType.UILogin_Forget, subUi);
            // subs.Add(UIType.UILogin_Forget);
            //
            // go = ui.GameObject.transform.Find(UIType.UILogin_Local).gameObject;
            // subUi = ComponentFactory.Create<UI, GameObject>(go);
            // subUi.AddUIBaseComponent<UILogin_LocalComponent>();
            // uiComponent.Add(UIType.UILogin_Local, subUi);
            // subs.Add(UIType.UILogin_Local);
            //
            // go = ui.GameObject.transform.Find(UIType.UILogin_Language).gameObject;
            // subUi = ComponentFactory.Create<UI, GameObject>(go);
            // subUi.AddUIBaseComponent<UILogin_LanguageComponent>();
            // uiComponent.Add(UIType.UILogin_Language, subUi);
            // subs.Add(UIType.UILogin_Language);
        }

        public void RemoveSubComponents()
        {
            UIComponent uiComponent = Game.Scene.GetComponent<UIComponent>();
            for (int i = 0, n = subs.Count; i < n; i++)
            {
                uiComponent.RemoveSub(subs[i]);
            }
        }

    }

    // [UIFactory(UIType.UILogin_LoadingWeb)]
    // public class UILogin_LoadingWebFactory : IUIFactoryExtend
    // {
    //     public UI Create(Scene scene, string type, GameObject gameObject)
    //     {
    //         try
    //         {
    //             ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
    //             resourcesComponent.LoadBundle($"{type}.unity3d");
    //             GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
    //             GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
    //             go.layer = LayerMask.NameToLayer(LayerNames.UI);
    //             UI ui = ComponentFactory.Create<UI, GameObject>(go);
    //
    //             // ui.AddUIBaseComponent<UILogin_LoadingWebComponent>();
    //
    //             AddSubComponent(ui);
    //             return ui;
    //         }
    //         catch (Exception e)
    //         {
    //             Log.Error(e);
    //             return null;
    //         }
    //     }
    //
    //     public void Remove(string type)
    //     {
    //         RemoveSubComponents();
    //         ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
    //     }
    //
    //     public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
    //     {
    //         try
    //         {
    //             ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
    //             await resourcesComponent.LoadBundleAsync($"{type}.unity3d");
    //             GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
    //             GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
    //             go.layer = LayerMask.NameToLayer(LayerNames.UI);
    //             UI ui = ComponentFactory.Create<UI, GameObject>(go);
    //
    //             // ui.AddUIBaseComponent<UILogin_LoadingWebComponent>();
    //
    //             AddSubComponent(ui);
    //             return ui;
    //         }
    //         catch (Exception e)
    //         {
    //             Log.Error(e);
    //             return null;
    //         }
    //     }
    //
    //     public void AddSubComponent(UI ui)
    //     {
    //     }
    //
    //     public void RemoveSubComponents()
    //     {
    //     }
    //
    // }

    [UIFactory(UIType.UILogin_Line)]
    public class UILogin_LineFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
                go.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(go);

                // ui.AddUIBaseComponent<UILogin_LineComponent>();

                AddSubComponent(ui);
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void Remove(string type)
        {
            RemoveSubComponents();
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {

                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                await resourcesComponent.LoadBundleAsync($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
                go.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(go);

                // ui.AddUIBaseComponent<UILogin_LineComponent>();

                AddSubComponent(ui);
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void AddSubComponent(UI ui)
        {
        }

        public void RemoveSubComponents()
        {
        }

    }


    [UIFactory(UIType.UILogin_Register)]
    public class UILogin_RegisterFactory : IUIFactoryExtend
    {
        private List<string> subs = new List<string>();

        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UILogin_RegisterComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void Remove(string type)
        {
            SingleFactory.mInstance.Remove(type);
        }

        public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
        {
            try
            {
                UI ui = await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UILogin_RegisterComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void AddSubComponent(UI ui)
        {
        }
    }


    #region 
    [UIFactory(UIType.UILogin_Forget)]
    public class UILogin_ForgetFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject club = UnityEngine.Object.Instantiate(bundleGameObject);
                club.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(club);
                ui.AddUIBaseComponent<UILogin_ForgetComponent>();
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
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                await resourcesComponent.LoadBundleAsync($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject club = UnityEngine.Object.Instantiate(bundleGameObject);
                club.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(club);
                ui.AddUIBaseComponent<UILogin_ForgetComponent>();
                AddSubComponent(ui);
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void Remove(string type)
        {
            RemoveSubComponents();
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
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
        public void AddSubComponent(UI ui)
        {
            if (subs == null)
                subs = new List<string>();
            else
                subs.Clear();
        }
    }
    #endregion

    #region 
    [UIFactory(UIType.UIPrivacyPolicy)]
    public class UIPrivacyPolicyFactory : IUIFactoryExtend
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                var ui = SingleFactory.mInstance.SingleCreate(type);
                ui.AddUIBaseComponent<UIPrivacyPolicyComponent>();
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
                ui.AddUIBaseComponent<UIPrivacyPolicyComponent>();
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