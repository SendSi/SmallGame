using System;
using System.Threading.Tasks;

namespace ETModel
{
    public class LocalResourcesComponent : Component
    {
        public async Task<UnityEngine.Object> LoadAsync(string path)
        {
            UnityEngine.Object obj = null;
            using (LocalResourcesLoaderAsync localResourcesLoaderAsync = ComponentFactory.Create<LocalResourcesLoaderAsync>())
            {
                obj = await localResourcesLoaderAsync.LoadAsync(path);
            }

            if (null == obj)
            {
                throw new Exception($"resources not found: {path}");
            }

            return obj;
        }



    }
}
