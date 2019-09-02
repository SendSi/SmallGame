using ETModel;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    public interface IUIFactoryExtend : IUIFactory
    {
        Task<UI> CreateAsync(Scene scene, string type, GameObject parent);
        void AddSubComponent(UI ui);
        // void RemoveSubComponents();
    }

    /// <summary>     单一的 一个组件   (也就一个prefab只挂一个ReferenceCollector.cs)     </summary>
    public class SingleFactory
    {
        private static SingleFactory _instance;

        public static SingleFactory mInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new SingleFactory();
                return _instance;
            }
        }


        public async Task<UI> SingleCreateAsync(string assetBundle)
        {
            var resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            await resourcesComponent.LoadBundleAsync(assetBundle + ".unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset(assetBundle + ".unity3d", assetBundle);
            GameObject dataUI = UnityEngine.Object.Instantiate(bundleGameObject);
            dataUI.layer = LayerMask.NameToLayer(LayerNames.UI);
            UI ui = ComponentFactory.Create<UI, GameObject>(dataUI);
            return ui;
        }

        public UI SingleCreate(string assetBundle)
        {
            var resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle(assetBundle + ".unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset(assetBundle + ".unity3d", assetBundle);
            GameObject dataUI = UnityEngine.Object.Instantiate(bundleGameObject);
            dataUI.layer = LayerMask.NameToLayer(LayerNames.UI);
            UI ui = ComponentFactory.Create<UI, GameObject>(dataUI);
            return ui;
        }

        /// <summary>         若有子结构,记得RemoveSub         </summary>
        public void Remove(string assetBundle)
        {
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(assetBundle + ".unity3d");
        }

    }
}
