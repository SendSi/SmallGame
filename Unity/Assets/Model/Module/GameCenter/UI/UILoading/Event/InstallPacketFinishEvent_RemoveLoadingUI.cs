namespace ETModel
{
    [Event(EventIdType.InstallPacketFinish)]
    public class InstallPacketFinishEvent_RemoveLoadingUI : AEvent
    {
        public override void Run()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILoading);
        }
    }
}
