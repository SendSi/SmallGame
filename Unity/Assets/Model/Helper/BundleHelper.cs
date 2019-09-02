using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public static class BundleHelper
    {
        public static async Task CheckDownloadBundle()
        {
            if (Define.IsAsync)
            {
                try
                {
                    using (BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.GetComponent<BundleDownloaderComponent>() ?? Game.Scene.AddComponent<BundleDownloaderComponent>())
                    {
                        await bundleDownloaderComponent.StartAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static bool Loading = false;

        public static async Task DownloadBundle()
        {
            if (Define.IsAsync)
            {
                try
                {
                    using (BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.GetComponent<BundleDownloaderComponent>())
                    {
                        Game.EventSystem.Run(EventIdType.LoadingBegin);

                        Loading = true;

                        await bundleDownloaderComponent.DownloadAsync();
                    }

                    // Game.EventSystem.Run(EventIdType.LoadingFinish);

                    DownloadBundleFinish();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static string GetBundleMD5(VersionConfig streamingVersionConfig, string bundleName)
        {
            if (streamingVersionConfig.FileInfoDict.ContainsKey(bundleName))
            {
                return streamingVersionConfig.FileInfoDict[bundleName].MD5;
            }

            string path = Path.Combine(PathHelper.AppHotfixResPath, bundleName);
            if (File.Exists(path))
            {
                return MD5Helper.FileMD5(path);
            }

            return "";
        }

        public static bool NeedDownloadBundle()
        {
            BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.GetComponent<BundleDownloaderComponent>();
            if (null == bundleDownloaderComponent)
                return false;

            if (bundleDownloaderComponent.bundles.Count == 0)
                return false;

            return true;
        }

        public static void DownloadBundleFinish()
        {
            if (Define.IsAsync)
            {
                Game.Scene.GetComponent<ResourcesComponent>().LoadOneBundle("StreamingAssets");
                ResourcesComponent.AssetBundleManifestObject = (AssetBundleManifest)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("StreamingAssets", "AssetBundleManifest");
            }
        }

        public static bool IsDownloadBundleFinish { get; set; }
    }
}
