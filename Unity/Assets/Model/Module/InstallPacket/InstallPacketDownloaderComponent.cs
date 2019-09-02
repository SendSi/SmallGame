using System;
using System.IO;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class InstallPacketDownloaderComponentAwakeSystem : AwakeSystem<InstallPacketDownloaderComponent>
    {
        public override void Awake(InstallPacketDownloaderComponent self)
        {
            self.Awake();
        }
    }

    public class InstallPacketDownloaderComponent : Component
    {
        public InstallPacketConfig remoteInstallPacketConfig;

        public UnityWebRequestAsync webRequest;

        public void Awake()
        {

        }

        public async Task StartAsync()
        {
            string installPacketUrl = "";
            try
            {
                using (UnityWebRequestAsync webRequestAsync = ComponentFactory.Create<UnityWebRequestAsync>())
                {
                    installPacketUrl = $"{NetLineSwitchComponent.Instance.CurrentServerResDomain()}InstallPacket.txt";
                    await webRequestAsync.DownloadAsync(installPacketUrl);
                    remoteInstallPacketConfig = JsonHelper.FromJson<InstallPacketConfig>(webRequestAsync.Request.downloadHandler.text);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"url: {installPacketUrl}", e);
            }
        }

        public async Task DownloadApkAsync()
        {
            using (webRequest = ComponentFactory.Create<UnityWebRequestAsync>())
            {
                await webRequest.DownloadAsync(remoteInstallPacketConfig.AndroidUrl);
                FileHelper.WriteFile(Path.Combine(PathHelper.AppHotfixResPath, remoteInstallPacketConfig.ApkName), webRequest.Request.downloadHandler.data);
            }
        }

        public int Progress
        {
            get
            {
                return (int)Math.Ceiling(webRequest.Progress * 100);
            }
        }
    }
}
