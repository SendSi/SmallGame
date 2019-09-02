using System;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using BestHTTP;

namespace ETHotfix
{

    [ObjectSystem]
    public class HttpRequestComponentSystem : AwakeSystem<HttpRequestComponent>
    {
        public override void Awake(HttpRequestComponent self)
        {
            self.Awake();
        }
    }

    public class HttpRequestComponent : Component
    {
        public sealed class CommonResponseData
        {
            public int status;
            public string msg;
        }

        public static HttpRequestComponent Instance;
        public Dictionary<HTTPRequest, int> requests;
        public void Awake()
        {
            requests = new Dictionary<HTTPRequest, int>();
            Instance = this;
        }

        #region http协议部分
        public async void Send(string _apiUrl, string jsonData, Action<string> _call, string customHost = null, Action pFinishedNoSuccess = null)
        {
            if (WebMessageHelper.IsNeedShowJuhua(_apiUrl))
            {
                UIComponent.Instance.Prompt();
            }
            string url = customHost == null ? $"{GlobalData.Instance.WebURL}{_apiUrl}" : $"{customHost}{_apiUrl}";
            Log.Debug($"send-> url = {url}");

            requests.Add(GameUtil.HttpsPost(url, jsonData, (request, response) =>
            {
                RemoveRequest(request);
                switch (request.State)
                {
                    case HTTPRequestStates.Finished:
                        if (response.IsSuccess)
                        {
                            Log.Debug($"response-> url = {url} jsonData = {jsonData} 结果= {response.DataAsText}");
                            CommonResponseData data = JsonHelper.FromJson<CommonResponseData>(response.DataAsText);
                            if (data.status == 105)
                            {
                                //维护弹窗
                                UIComponent.Instance.ShowNoAnimation(UIType.UIDialog,
                                     new UIDialogComponent.DialogData()
                                     {
                                         type = UIDialogComponent.DialogData.DialogType.Commit,
                                         title = "",
                                         content = string.Format(LanguageMgr.mInstance.GetLanguageForKey("ServerMaintenance"), data.msg),
                                         contentCommit = CPErrorCode.LanguageDescription(10008),
                                         actionCommit = null,
                                         actionCancel = null
                                     });
                            }
                            if (_call != null)
                                _call(response.DataAsText);
                        }
                        else
                        {
                            UIComponent.Instance.Toast(response.StatusCode);
                            if (pFinishedNoSuccess != null)
                            {//追加的.下拉列表 不成功时 收缩下(加载中...)
                                pFinishedNoSuccess();
                            }
                        }
                        break;
                    case HTTPRequestStates.Error:
                        string exception = request.Exception != null
                                ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception";
                        Log.Debug($"Request Finished with Error!  {exception}");
                        // UIComponent.Instance.Toast("请求错误");
                        if (pFinishedNoSuccess != null) pFinishedNoSuccess();
                        break;
                    case HTTPRequestStates.Aborted:
                        Log.Debug($"Request Aborted!");
                        // UIComponent.Instance.Toast("请求拒绝");
                        if (pFinishedNoSuccess != null) pFinishedNoSuccess();
                        break;
                    case HTTPRequestStates.ConnectionTimedOut:
                        Log.Debug($"Connection Timed Out!");
                        // UIComponent.Instance.Toast("连接超时");
                        if (pFinishedNoSuccess != null) pFinishedNoSuccess();
                        break;
                    case HTTPRequestStates.TimedOut:
                        Log.Debug($"Processing the request Timed Out!");
                        // UIComponent.Instance.Toast("网络超时");
                        if (pFinishedNoSuccess != null) pFinishedNoSuccess();
                        break;
                    default:
                        UIComponent.Instance.Toast($"未知错误 State = {request.State} ");
                        if (pFinishedNoSuccess != null) pFinishedNoSuccess();
                        break;
                }

            }), (int)HTTPRequestStates.Initial);
        }

        public async void SendPicUpload(string _apiUrl, byte[] data, Action<string> _call)
        {
            UIComponent.Instance.Prompt();
            string url = $"{GlobalData.Instance.UploadURL}{_apiUrl}";
            Log.Debug($"SendPicUpload send url = {url} | data = {data} | data.Length = {data.Length}");
            
            NetHttpComponent netHttp = ETModel.Game.Scene.GetComponent<NetHttpComponent>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("did", SystemInfoUtil.getDeviceUniqueIdentifier());
            dic.Add("md5at", GameCache.Instance.strToken);

            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("user_id", $"{GameCache.Instance.nUserId}");

            string res = await netHttp.PostUpload(url, dic, form, "file", data, "picture.png", "image/png");
            UIComponent.Instance.ClosePrompt();
            if (null != _call)
            {
                _call(res);
            }

            return;

            requests.Add(GameUtil.HttpsImagePost(url, data, (request, response) =>
            {
                RemoveRequest(request);
                switch (request.State)
                {
                    case HTTPRequestStates.Finished:
                        if (response.IsSuccess)
                        {
                            _call(response.DataAsText);
                        }
                        break;
                    case HTTPRequestStates.Error:
                        string exception = request.Exception != null
                                ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception";
                        Log.Debug($"Request Finished with Error!  {exception}");
                        UIComponent.Instance.Toast("网络错误");
                        break;
                    case HTTPRequestStates.Aborted:
                        Log.Debug($"Request Aborted!");
                        UIComponent.Instance.Toast("网络错误");
                        break;
                    case HTTPRequestStates.ConnectionTimedOut:
                        Log.Debug($"Connection Timed Out!");
                        UIComponent.Instance.Toast("网络超时");
                        break;
                    case HTTPRequestStates.TimedOut:
                        Log.Debug($"Processing the request Timed Out!");
                        UIComponent.Instance.Toast("网络超时");
                        break;
                }

            }), (int)HTTPRequestStates.Initial);
        }


        void RemoveRequest(HTTPRequest request)
        {

            requests.Remove(request);
            if (requests.Count == 0)
            {
                UIComponent.Instance.ClosePrompt();
            }
        }

        #endregion

        #region 下载图片
        public void TextureDownLoad(string _textureUrl, Texture2D _t2d, Action<Texture2D> _call)
        {
            var request = new HTTPRequest(new Uri(_textureUrl), (req, resp) =>
            {

                switch (req.State)
                {
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                        {
                            Texture2D tex = req.Tag as Texture2D;
                            tex.LoadImage(resp.Data);
                            _call(tex);
                        }
                        else
                        {
                            Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} http  {3}",
                                                            resp.StatusCode,
                                                            resp.Message,
                                                            resp.DataAsText, _textureUrl));
                        }
                        break;
                    case HTTPRequestStates.Error:
                        Log.Error("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                        break;
                    case HTTPRequestStates.Aborted:
                        Log.Warning("Request Aborted!");
                        break;
                    case HTTPRequestStates.ConnectionTimedOut:
                        Log.Error("Connection Timed Out!");
                        break;
                    case HTTPRequestStates.TimedOut:
                        Log.Error("Processing the request Timed Out!");
                        break;
                }
            });
            request.Tag = _t2d;
            request.Send();
        }
        #endregion
    }
}
