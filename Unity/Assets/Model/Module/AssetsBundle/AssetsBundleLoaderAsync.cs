using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class AssetsBundleLoaderAsyncSystem : UpdateSystem<AssetsBundleLoaderAsync>
    {
        public override void Update(AssetsBundleLoaderAsync self)
        {
            self.Update();
        }
    }

    public class AssetsBundleLoaderAsync : Component
    {
        private AssetBundleCreateRequest request;

        private TaskCompletionSource<AssetBundle> tcs;

        public void Update()
        {
            if (!this.request.isDone)
            {
                return;
            }

            TaskCompletionSource<AssetBundle> t = tcs;
            t.SetResult(this.request.assetBundle);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
        }


        /// <summary>         0未知,去load下, 1不使用加密   ,2加密            若0则使用1         </summary>
        private int mABSecurity = 0;
        void TryABSecurity()//只执行了一次
        {
            try
            {
                if (mABSecurity == 0)
                {
                    var versionTxt = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");
                    var tAllTxt = File.ReadAllText(versionTxt);
                    var json = JsonHelper.FromJson<VersionConfig>(tAllTxt);
                    mABSecurity = json.ABSecurity;
                }
            }
            catch (Exception)
            {
                mABSecurity = 1;
                throw;
            }
        }


        public Task<AssetBundle> LoadAsync(string path)
        {
            this.tcs = new TaskCompletionSource<AssetBundle>();
            TryABSecurity();
            if (Define.IsAsync && Define.IsILRuntime && mABSecurity == 2)
            {
                this.request = AssetBundle.LoadFromFileAsync(path, 0, 128);
            }
            else
            {
                this.request = AssetBundle.LoadFromFileAsync(path);
            }
            return this.tcs.Task;
        }

    }
}
