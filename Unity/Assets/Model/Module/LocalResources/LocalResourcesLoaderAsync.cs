using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class LocalResourcesLoaderAsyncUpdateSystem : UpdateSystem<LocalResourcesLoaderAsync>
    {
        public override void Update(LocalResourcesLoaderAsync self)
        {
            self.Update();
        }
    }

    public class LocalResourcesLoaderAsync : Component
    {
        private ResourceRequest request;

        private TaskCompletionSource<UnityEngine.Object> tcs;

        public void Update()
        {
            if (!request.isDone)
            {
                return;
            }

            TaskCompletionSource<UnityEngine.Object> t = tcs;
            t.SetResult(request.asset);
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            base.Dispose();
        }

        public Task<UnityEngine.Object> LoadAsync(string path)
        {
            tcs = new TaskCompletionSource<UnityEngine.Object>();
            request = Resources.LoadAsync(path);
            return tcs.Task;
        }
    }
}
