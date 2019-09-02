namespace ETModel
{
    [Event(EventIdType.LoadingBegin)]
    public class LoadingBeginEvent_CreateLoadingUI : AEvent
    {
        public override void Run()
        {
            BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.GetComponent<BundleDownloaderComponent>();
            if(null != bundleDownloaderComponent)
                Game.Scene.GetComponent<UIComponent>().ShowNoAnimation(UIType.UILoading, bundleDownloaderComponent);
        }
    }
}
