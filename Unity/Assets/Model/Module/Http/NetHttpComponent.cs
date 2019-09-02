using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class NetHttpComponentAwakeSystem : AwakeSystem<NetHttpComponent>
    {
        public override void Awake(NetHttpComponent self)
        {
            self.Awake();
        }
    }

    public class NetHttpComponent : Component
    {
        public void Awake()
        {

        }

        public async Task<string> PostAsync(string url, Dictionary<string, string> dic, byte[] rawBytes)
        {
            try
            {
                using (NetHttpRequestAsync webRequestAsync = ComponentFactory.Create<NetHttpRequestAsync>())
                {
                    string success = await webRequestAsync.DownloadAsync(url, dic, rawBytes);
                    return success;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"url: {url}", e);
            }
        }

        public async Task<string> PostUpload(
        string url, Dictionary<string, string> dic, Dictionary<string, string> form, string fieldName, byte[] data, string fileName,
        string mimeType)
        {
            try
            {
                using (NetHttpRequestAsync webRequestAsync = ComponentFactory.Create<NetHttpRequestAsync>())
                {
                    string success = await webRequestAsync.UploadAsync(url, dic, form, fieldName, data, fileName, mimeType);
                    return success;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"url: {url}", e);
            }
        }
    }
}
