//im sdk 插件
using System;
using UnityEngine;
using UnityEngine.UI;
using ETModel;
using BestHTTP;
using System.Threading;
using System.Threading.Tasks;

namespace ETHotfix
{

    public enum IMOpearteType
    {
        joinGroup = 0,
    }

    public class IMOpearteMes
    {
        public int opearteType;
    }

    public class IMMes
    {

        public string sender;//发送者
        public int mesType;//0-文本信息 1-音效
        public string mesContent;//{mesType=0 mesContent=文本} {mesType=1 mesContent=soundPath}
        public long duration;
    }

    [ObjectSystem]
    public class IMSdkComponentAwakeSystem : AwakeSystem<IMSdkComponent>
    {
        public override void Awake(IMSdkComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class IMSdkComponentUpdateSystem : UpdateSystem<IMSdkComponent>
    {
        public override void Update(IMSdkComponent self)
        {
            self.Update();
        }
    }

    public class IMSdkComponent : Component
    {
        public static int SUCCESS = 1000;
        public static int E_NOSDCARD = 1001;
        public static int E_STATE_RECODING = 1002;
        public static int E_UNKOWN = 1003;

        public static IMSdkComponent Instance;
        public bool isRecored;

        private string cacheToken;
        private string cacheGid;//若存在数据，则发送给对应接口

        const float pt = 5;//5秒检测一次
        private float protectTimer;
        private bool isKeepAlive = false;

        public void Awake()
        {

            if (Instance == null)
            {
                Log.Debug("im sdk init");
                Instance = this;
                isRecored = false;
                ETModel.Game.Hotfix.OnOpearteMes = OnOpearteMes;
            }
        }

        void OnOpearteMes(string mes)
        {

            IMOpearteMes opMes = JsonHelper.FromJson<IMOpearteMes>(mes);
            if (opMes.opearteType == (int)IMOpearteType.joinGroup)
            {

                UpImInfo();//告知服务器加入group成功
            }
        }

        public void Update()
        {

            //监听IM的状态
            if (isKeepAlive)
            {
                protectTimer += Time.deltaTime;
                if (protectTimer > pt)
                {
                    protectTimer = 0;
                    int state = GetImState();
                    if (state <= 0)
                    {
                        //重连
                        NativeCallLogin(cacheToken);
                    }
                }
            }
        }

        /// <summary>
        /// 和服务器请求token后登录IM
        /// </summary>
        public void Login()
        {

            // TalkingDataSdkComponent.Instance.UploadSdkAnalysis("im - token req");
            // WEB2_im_gentoken.RequestData im_req = new WEB2_im_gentoken.RequestData()
            // {
            //     userId = GameCache.Instance.nUserId
            // };
            // HttpRequestComponent.Instance.Send(WEB2_im_gentoken.API, WEB2_im_gentoken.Request(im_req),
            //     (resData) => {
            //
            //         WEB2_im_gentoken.ResponseData im_res = WEB2_im_gentoken.Response(resData);
            //         if (im_res.status == 0)
            //         {
            //             TalkingDataSdkComponent.Instance.UploadSdkAnalysis("im - token res");
            //             //IM登录
            //             NativeCallLogin(im_res.data.token);
            //             cacheToken = im_res.data.token;
            //             cacheGid = im_res.data.gid;
            //             //gid 不为空时 加入对话组
            //             if (!im_res.data.gid.Equals("") && !im_res.data.gid.Equals(string.Empty))
            //             {
            //
            //                 ApplyJoinGroup(im_res.data.gid);
            //             }
            //             isKeepAlive = true;
            //         }
            //         else
            //         {
            //             Log.Debug($"im_res.msg = {im_res.msg}");
            //             UIComponent.Instance.Toast(im_res.status);
            //         }
            //     });
        }

        /// <summary>
        /// 新账号加入唯一对话组后需告知服务器
        /// </summary>
        public void UpImInfo()
        {

            // WEB2_im_upim_info.RequestData upim_req = new WEB2_im_upim_info.RequestData()
            // {
            //     userId = GameCache.Instance.nUserId,
            //     groupId = cacheGid
            // };
            // HttpRequestComponent.Instance.Send(WEB2_im_upim_info.API, WEB2_im_upim_info.Request(upim_req),
            //     (resData) => {
            //
            //         WEB2_im_upim_info.ResponseData upim_res = WEB2_im_upim_info.Response(resData);
            //         if (upim_res.status == 0)
            //         {
            //             //do nothing
            //         }
            //         else
            //         {
            //             Log.Debug($"im_res.msg = {upim_res.msg}");
            //             UIComponent.Instance.Toast(upim_res.status);
            //         }
            //     });
        }

        /// <summary>
        /// 调用原生登录接口
        /// </summary>
        /// <param name="t"></param>
        void NativeCallLogin(string t)
        {

            TalkingDataSdkComponent.Instance.UploadSdkAnalysis("im - login call");
            int appid = 0;
            if (GlobalData.Instance.serverType == 0) appid = 1400154930;
            else if (GlobalData.Instance.serverType == 2) appid = 1400154934;
            else appid = 1400154932;

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("IMLogin", GameCache.Instance.nUserId.ToString(), t, appid, "crazyAdmin");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.IMLogin(GameCache.Instance.nUserId.ToString(), t, appid, "crazyAdmin");
            }

            int state = GetImState();
            if (state < 0)
            {

                //接口错误时上传到bugly
                BuglySdkComponent.Instance.ReportException("sdk im", "error im init ", "");
            }
            else if (state == 0)
            {
                //接口错误时上传到bugly
                BuglySdkComponent.Instance.ReportException("sdk im", "error im login ", "");
                TalkingDataSdkComponent.Instance.UploadSdkAnalysis("im - init succ");
            }
            else
            {
                TalkingDataSdkComponent.Instance.UploadSdkAnalysis("im - init succ");
                TalkingDataSdkComponent.Instance.UploadSdkAnalysis("im - login succ");
            }
        }

        /// <summary>
        /// 调用原生登录退出
        /// </summary>
        public void LoginOut()
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("IMLoginOut");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.IMLoginOut();
            }
            isKeepAlive = false;
        }

        /// <summary>
        /// 返回IM当前状态 -1未实例化 0-未登录 , 1-登录中, 2已登录
        /// </summary>
        /// <returns></returns>
        public int GetImState()
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                return NativeManager.OnFuncCall<int>("GetImState");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return NativeManager.GetImState();

            }
            return 0;
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        public bool StartRecord()
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                return NativeManager.OnFuncCall<bool>("StartRecord");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                return NativeManager.StartRecord();
            }
            return false;
        }

        /// <summary>
        /// 停止录音  
        /// </summary>
        /// <param name="isSend">是否发送到聊天组</param>
        /// <returns>音频的毫秒数，如果返回0， 表示录制失败, 或者时间不够</returns>
        public long StopRecord(bool isSend)
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                return NativeManager.OnFuncCall<long>("StopRecord", isSend);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                return NativeManager.StopRecord(isSend);
            }
            return 0;
        }

        public void PauseRecord()
        {

            //if (Application.platform == RuntimePlatform.Android)
            //{

            //    NativeManager.OnFuncCall("PauseRecord");
            //}
            //else if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{

            //}
        }

        public void ContinueRecord()
        {

            //if (Application.platform == RuntimePlatform.Android)
            //{

            //    NativeManager.OnFuncCall("ContinueRecord");
            //}
            //else if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{

            //}
        }

        /// <summary>
        /// 播放录音音效
        /// </summary>
        public void PlayVoice(string path)
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("PlayVoice", path);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.PlayVoice(path);
            }
        }

        /// <summary>
        /// 停止播放录音音效
        /// </summary>
        public void StopVoice()
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("StopVoice");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.StopVoice();
            }
        }

        /// <summary>
        /// 停止播放录音音效
        /// </summary>
        public bool IsPlayingVoice()
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                return NativeManager.OnFuncCall<bool>("IsPlayingVoice");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                return NativeManager.IsPlayingVoice();
            }
            return false;
        }

        /// <summary>
        /// 设置播放音量  同步牌局音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("setVolume", volume);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.SetVolume(volume);
            }
        }

        /// <summary>
        /// 发送文本信息
        /// </summary>
        /// <param name="str"></param>
        public void SendMessage(string str)
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("IMSendMessage", str);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.IMSendMessage(str);
            }
        }

        /// <summary>
        /// 加入对话组 不带发送操作
        /// </summary>
        /// <param name="groupId"></param>
        public void ApplyJoinGroup(string groupId)
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("ApplyJoinGroup", groupId);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                NativeManager.ApplyJoinGroup(groupId);
            }
        }

        /// <summary>
        /// 设置对话组
        /// </summary>
        /// <param name="groupId"></param>
        public void SetConversation(string groupId)
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("IMSetConversation", groupId);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.SetConversation(groupId);
            }
        }

        /// <summary>
        /// 离开对话组
        /// </summary>
        public void ClearConversation()
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                NativeManager.OnFuncCall("IMLeaveConversation");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                NativeManager.ClearConversation();
            }
        }
    }

}
