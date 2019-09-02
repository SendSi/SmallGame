namespace ETModel
{
    [Event(EventIdType.InstallPacketBegin)]
    public class InstallPacketBeginEvent_CreateLoadingUI : AEvent
    {
        public override void Run()
        {
            InstallPacketDownloaderComponent installPacketDownloaderComponent = Game.Scene.GetComponent<InstallPacketDownloaderComponent>();
            if (null != installPacketDownloaderComponent)
                Game.Scene.GetComponent<UIComponent>().ShowNoAnimation(UIType.UILoading, installPacketDownloaderComponent);
        }
    }
}
