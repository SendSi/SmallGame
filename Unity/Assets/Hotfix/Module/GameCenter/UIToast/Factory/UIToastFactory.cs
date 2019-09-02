using System;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[UIFactory(UIType.UIToast)]
	public class UIToastFactory : IUIFactoryExtend
	{
		public UI Create(Scene scene, string type, GameObject gameObject)
		{
			try
			{
                //ResourcesComponent resourcesComponent = Game.Scene.ModelScene.GetComponent<ResourcesComponent>();
                //resourcesComponent.LoadBundle($"{type}.unity3d");
                //GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                //GameObject login = UnityEngine.Object.Instantiate(bundleGameObject);
                //login.layer = LayerMask.NameToLayer(LayerNames.UI);
                //UI ui = ComponentFactory.Create<UI, GameObject>(login);

                UI ui =  SingleFactory.mInstance.SingleCreate(type);
				ui.AddUIBaseComponent<UIToastComponent>();

				this.AddSubComponent(ui);

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
			ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
		}

		public async Task<UI> CreateAsync(Scene scene, string type, GameObject parent)
		{
			try
			{
                //ResourcesComponent resourcesComponent = Game.Scene.ModelScene.GetComponent<ResourcesComponent>();
                //await resourcesComponent.LoadBundleAsync($"{type}.unity3d");
                //GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                //GameObject login = UnityEngine.Object.Instantiate(bundleGameObject);
                //login.layer = LayerMask.NameToLayer(LayerNames.UI);
                //UI ui = ComponentFactory.Create<UI, GameObject>(login);
                UI ui =await SingleFactory.mInstance.SingleCreateAsync(type);
                ui.AddUIBaseComponent<UIToastComponent>();

				this.AddSubComponent(ui);

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
}