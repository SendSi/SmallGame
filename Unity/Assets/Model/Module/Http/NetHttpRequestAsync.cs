using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ETModel
{
    [ObjectSystem]
    public class NetHttpRequestAsyncUpdateSystem : UpdateSystem<NetHttpRequestAsync>
    {
        public override void Update(NetHttpRequestAsync self)
        {
            self.Update();
        }
    }

    public class NetHttpRequestAsync : Component
    {
        public UnityWebRequest Request;

        public bool isCancel;

        public TaskCompletionSource<string> tcs;

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.Request?.Dispose();
            this.Request = null;
            this.isCancel = false;
        }

        public float Progress
        {
            get
            {
                if (this.Request == null)
                {
                    return 0;
                }
                return this.Request.downloadProgress;
            }
        }

        public ulong ByteDownloaded
        {
            get
            {
                if (this.Request == null)
                {
                    return 0;
                }
                return this.Request.downloadedBytes;
            }
        }

        public void Update()
        {
            if (this.isCancel)
            {
                this.tcs.SetResult(string.Empty);
                return;
            }

            if (!this.Request.isDone)
            {
                return;
            }
            if (!string.IsNullOrEmpty(this.Request.error))
            {
                // this.tcs.SetException(new Exception($"request error: {this.Request.error}"));
                this.tcs.SetResult(string.Empty);
                return;
            }

            this.tcs.SetResult(Request.downloadHandler.text);
        }

        public Task<string> DownloadAsync(string url, Dictionary<string, string> dic, byte[] rawData)
        {
            this.tcs = new TaskCompletionSource<string>();

            url = url.Replace(" ", "%20");

            this.Request = new UnityWebRequest(url, "POST");
            this.Request.uploadHandler = new UploadHandlerRaw(rawData);
            this.Request.downloadHandler = new DownloadHandlerBuffer();

            foreach (KeyValuePair<string, string> keyValuePair in dic)
            {
                Request.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
            }

            this.Request.SendWebRequest();

            return this.tcs.Task;
        }

        public Task<string> UploadAsync(string url, Dictionary<string, string> dic, Dictionary<string, string> form, string fieldName, byte[] data, string fileName, string mimeType)
        {
            this.tcs = new TaskCompletionSource<string>();

            url = url.Replace(" ", "%20");

            WWWForm wwwForm = new WWWForm();
            foreach (KeyValuePair<string, string> keyValuePair in form)
            {
                wwwForm.AddField(keyValuePair.Key, keyValuePair.Value);
            }

            wwwForm.AddBinaryData(fieldName, data, fileName, mimeType);

            Request = UnityWebRequest.Post(url, wwwForm);
            foreach (KeyValuePair<string, string> keyValuePair in dic)
            {
                Request.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
            }

            this.Request.SendWebRequest();

            return this.tcs.Task;
        }
    }
}
