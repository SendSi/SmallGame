using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
	[ObjectSystem]
	public class UiLoadingComponentAwakeSystem : AwakeSystem<UILoadingComponent>
	{
		public override void Awake(UILoadingComponent self)
        {
            self.Awake();
        }
	}

    [ObjectSystem]
    public class UILoadingComponentUpdateSystem: UpdateSystem<UILoadingComponent>
    {
        public override void Update(UILoadingComponent self)
        {
            self.Update();
        }
    }

    public class UILoadingComponent : UIBaseComponent
	{
        private ReferenceCollector rc;
        private Text textContent;
        private Slider sliderLoading;


        private BundleDownloaderComponent mBundleDownloaderComponent;
        private InstallPacketDownloaderComponent mInstallPacketDownloaderComponent;

        public void Awake()
        {
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            textContent = rc.Get<GameObject>("Text_Content").GetComponent<Text>();
            sliderLoading = rc.Get<GameObject>("Slider_Loading").GetComponent<Slider>();

        }

        public void Update()
        {
            if (null != mBundleDownloaderComponent)
            {
                textContent.text = $"{mBundleDownloaderComponent.Progress}%";
                sliderLoading.value = mBundleDownloaderComponent.Progress;
            }
            if (null != mInstallPacketDownloaderComponent)
            {
                textContent.text = $"{mInstallPacketDownloaderComponent.Progress}%";
                sliderLoading.value = mInstallPacketDownloaderComponent.Progress;
            }
        }

        private async void StartLoading()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();
            long instanceId = this.InstanceId;
            while (true)
            {
                await timerComponent.WaitAsync(1000);
        
                if (this.InstanceId != instanceId)
                {
                    //Remove
                    break;
                }

                if (null == mBundleDownloaderComponent && null == mInstallPacketDownloaderComponent)
                {
                    //Hide
                    break;
                }
                    
        
                if (null != mBundleDownloaderComponent)
                {
                    textContent.text = $"{mBundleDownloaderComponent.Progress}%";
                }
                if (null != mInstallPacketDownloaderComponent)
                {
                    textContent.text = $"{mInstallPacketDownloaderComponent.Progress}%";
                }
                    
            }
        
            mBundleDownloaderComponent = null;
            mInstallPacketDownloaderComponent = null;
        }

        public override void OnShow(object obj)
        {
            mBundleDownloaderComponent = obj as BundleDownloaderComponent;
            mInstallPacketDownloaderComponent = obj as InstallPacketDownloaderComponent;

            // StartLoading();
        }

        public override void OnHide()
        {
            mBundleDownloaderComponent = null;
            mInstallPacketDownloaderComponent = null;
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            mBundleDownloaderComponent = null;
            mInstallPacketDownloaderComponent = null;

            base.Dispose();
        }
    }
}
