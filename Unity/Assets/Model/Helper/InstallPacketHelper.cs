using System;
using System.IO;
using System.Threading.Tasks;

namespace ETModel
{
    public static class InstallPacketHelper
    {
        public static async Task CheckInstallPacket()
        {
            if (Define.IsAsync)
            {
                try
                {
                    using (InstallPacketDownloaderComponent installPacketDownloaderComponent = Game.Scene.GetComponent<InstallPacketDownloaderComponent>() ?? Game.Scene.AddComponent<InstallPacketDownloaderComponent>())
                    {
                        await installPacketDownloaderComponent.StartAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static async Task DownloadInstallPacket()
        {
            if (Define.IsAsync)
            {
                try
                {
                    using (InstallPacketDownloaderComponent installPacketDownloaderComponent = Game.Scene.GetComponent<InstallPacketDownloaderComponent>())
                    {
                        if (null == installPacketDownloaderComponent)
                            return;

                        Game.EventSystem.Run(EventIdType.InstallPacketBegin);

                        await installPacketDownloaderComponent.DownloadApkAsync();
                    }

                    Game.EventSystem.Run(EventIdType.InstallPacketFinish);

                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static bool NeedInstallPacket()
        {

            // if (UnityEngine.Application.platform == RuntimePlatform.WindowsEditor || UnityEngine.Application.platform == RuntimePlatform.OSXEditor)
            // return false;

#if UNITY_EDITOR
            return false;
#endif

            InstallPacketDownloaderComponent installPacketDownloaderComponent = Game.Scene.GetComponent<InstallPacketDownloaderComponent>();
            if (null == installPacketDownloaderComponent)
                return false;

#if UNITY_ANDROID
            if (installPacketDownloaderComponent.remoteInstallPacketConfig.Android.Equals(UnityEngine.Application.version))
            {
                return false;
            }

#elif UNITY_IOS

#if APPStore
            // AppStore专用
            if (installPacketDownloaderComponent.remoteInstallPacketConfig.IOSAppStore.Equals(UnityEngine.Application.version))
            {
                return false;
            }
#else
            if (installPacketDownloaderComponent.remoteInstallPacketConfig.IOS.Equals(UnityEngine.Application.version))
            {
                return false;
            }
#endif
#endif

            return true;
        }

        public static string InstallPacketPath()
        {
            InstallPacketDownloaderComponent installPacketDownloaderComponent = Game.Scene.GetComponent<InstallPacketDownloaderComponent>();
            if (null != installPacketDownloaderComponent)
                return Path.Combine(PathHelper.AppHotfixResPath, installPacketDownloaderComponent.remoteInstallPacketConfig.ApkName);
            return Path.Combine(PathHelper.AppHotfixResPath, "crazypoker.apk");
        }
    }
}
