using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class UiBundleDownloaderComponentAwakeSystem : AwakeSystem<BundleDownloaderComponent>
    {
        public override void Awake(BundleDownloaderComponent self)
        {
            self.bundles = new Queue<string>();
            self.downloadedBundles = new HashSet<string>();
            self.downloadingBundle = "";
        }
    }

    /// <summary>
    /// 用来对比web端的资源，比较md5，对比下载资源
    /// </summary>
    public class BundleDownloaderComponent : Component
    {
        /// <summary>   远程   </summary>
        private VersionConfig remoteVersionConfig;
        /// <summary>   本地   </summary>
        private VersionConfig streamingVersionConfig = null;

        public Queue<string> bundles;
        public long TotalSize;
        public HashSet<string> downloadedBundles;
        public string downloadingBundle;
        public UnityWebRequestAsync webRequest;
        public async Task StartAsync()
        {
            // 获取远程的Version.txt
            string versionUrl = "";
            try
            {
                using (UnityWebRequestAsync webRequestAsync = ComponentFactory.Create<UnityWebRequestAsync>())
                {
                    versionUrl = GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/Version.txt";
                    Log.Error("server version=" + versionUrl);
                    await webRequestAsync.DownloadAsync(versionUrl);
                    remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(webRequestAsync.Request.downloadHandler.text);
                    Log.Warning("server version json=" + JsonHelper.ToJson(remoteVersionConfig));
                }
            }
            catch (Exception e)
            {
                throw new Exception($"url: {versionUrl}", e);
            }
            // 获取HotfixResPath目录的Version.txt
            string versionPath = Path.Combine($"file://{PathHelper.AppHotfixResPath}", "Version.txt");
            Log.Debug("local version=" + versionPath);

            using (UnityWebRequestAsync request = ComponentFactory.Create<UnityWebRequestAsync>())//未下载的
            {
                try
                {
                    await request.DownloadAsync(versionPath);
                    streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(request.Request.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Log.Debug($"获取本地目录的Version.txt 失败! Message: {e.Message}");

                }
            }

            // 获取streaming目录的Version.txt
            if (null == streamingVersionConfig)
            {
                // string versionPath = Path.Combine(PathHelper.AppResPath4Web, "Version.txt");
                versionPath = Path.Combine($"file://{PathHelper.AppResPath}", "Version.txt");
                using (UnityWebRequestAsync request = ComponentFactory.Create<UnityWebRequestAsync>())
                {
                    try
                    {
                        await request.DownloadAsync(versionPath);
                        streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(request.Request.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"获取本地目录的Version.txt 失败! Message: {e.Message}");
                        streamingVersionConfig = new VersionConfig();
                    }
                }
                var verLocal = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");//创建路径
                if (Directory.Exists(PathHelper.AppHotfixResPath) == false)
                    Directory.CreateDirectory(PathHelper.AppHotfixResPath);
                using (FileStream fs = new FileStream(verLocal, FileMode.Create))//创建
                {
                    VersionConfig t = new VersionConfig() { Version = remoteVersionConfig.Version, TotalSize = 0, ABSecurity = remoteVersionConfig.ABSecurity, FileInfoDict = new Dictionary<string, FileVersionInfo>() };
                    var json = JsonHelper.ToJson(t);
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            // 删掉远程不存在的文件
            DirectoryInfo directoryInfo = new DirectoryInfo(PathHelper.AppHotfixResPath);
            if (directoryInfo.Exists)
            {
                FileInfo[] fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (remoteVersionConfig.FileInfoDict.ContainsKey(fileInfo.Name))
                    {
                        continue;
                    }
                    if (fileInfo.Name == "Version.txt")
                    {
                        continue;
                    }
                    fileInfo.Delete();
                }
            }
            else
            {
                directoryInfo.Create();
            }

            // 对比MD5
            foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values)
            {
                // 对比md5
                string localFileMD5 = BundleHelper.GetBundleMD5(streamingVersionConfig, fileVersionInfo.File);
                if (fileVersionInfo.MD5 == localFileMD5 || fileVersionInfo.File == "Version.txt")//txt不去对比md5
                {
                    continue;
                }
                bundles.Enqueue(fileVersionInfo.File);
                TotalSize += fileVersionInfo.Size;
            }
        }

        public int Progress
        {
            get
            {
                if (TotalSize == 0)
                {
                    return 0;
                }

                long alreadyDownloadBytes = 0;
                foreach (string downloadedBundle in downloadedBundles)
                {
                    long size = remoteVersionConfig.FileInfoDict[downloadedBundle].Size;
                    alreadyDownloadBytes += size;
                }
                if (webRequest != null)
                {
                    alreadyDownloadBytes += (long)webRequest.Request.downloadedBytes;
                }
                return (int)(alreadyDownloadBytes * 100f / TotalSize);
            }
        }

        public async Task DownloadAsync()
        {
            if (bundles.Count == 0 && downloadingBundle == "")
            {
                return;
            }

            try
            {
                while (true)
                {
                    if (bundles.Count == 0)
                    {
                        break;
                    }

                    downloadingBundle = bundles.Dequeue();

                    while (true)
                    {
                        try
                        {
                            using (webRequest = ComponentFactory.Create<UnityWebRequestAsync>())
                            {
                                await webRequest.DownloadAsync(GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + downloadingBundle);
                                byte[] data = webRequest.Request.downloadHandler.data;

                                string path = Path.Combine(PathHelper.AppHotfixResPath, downloadingBundle);

                                if (path.Contains("Version.txt") == false)
                                {
                                    var versionLocalPath = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");//本地文件
                                    var versionLocalson = JsonHelper.FromJson<VersionConfig>(File.ReadAllText(versionLocalPath));
                                    versionLocalson.FileInfoDict[downloadingBundle] = remoteVersionConfig.FileInfoDict[downloadingBundle];
                                    versionLocalson.Version = remoteVersionConfig.Version;
                                    versionLocalson.ABSecurity = remoteVersionConfig.ABSecurity;

                                    var content = JsonHelper.ToJson(versionLocalson);
                                    StreamWriter sw = new StreamWriter(versionLocalPath);
                                    sw.Write(content);
                                    sw.Close();

                                   var pathPP= Path.GetDirectoryName(path);
                                    if (Directory.Exists(pathPP) == false)
                                        Directory.CreateDirectory(pathPP);

                                    using (FileStream fs = new FileStream(path, FileMode.Create))
                                    {
                                        fs.Write(data, 0, data.Length);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"download bundle error: {downloadingBundle}\n{e}");
                            continue;
                        }

                        break;
                    }
                    downloadedBundles.Add(downloadingBundle);
                    downloadingBundle = "";
                    webRequest = null;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
