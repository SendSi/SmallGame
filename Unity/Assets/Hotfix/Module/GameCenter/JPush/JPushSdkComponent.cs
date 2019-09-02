//jpush sdk 插件
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETModel;
using JPush;
using LitJson;

namespace ETHotfix
{

    /**消息通知结构
 * {
 *	 "title": "notiTitle",
 *   "content": "content",
 *   "extras": {
 *		"key1": "value1",
 *       "key2": "value2"
 * 	}
 * }
 */

    public class JPushMes
    {
        public string title;
        public string content;
    }

    [ObjectSystem]
    public class JPushSdkComponentSystem : AwakeSystem<JPushSdkComponent>
    {
        public override void Awake(JPushSdkComponent self)
        {
            self.Awake();
        }
    }

    //[ObjectSystem]
    //public class JPushSdkComponentUpdateSystem : UpdateSystem<JPushSdkComponent>
    //{
    //    public override void Update(JPushSdkComponent self)
    //    {
    //        self.Update();
    //    }
    //}

    public class JPushSdkComponent : Component
    {

        public class OperateResult
        {
            public int sequence;
            public int code;
        }

        public static JPushSdkComponent Instance;
        List<string> tags = new List<string>();
        int sequence;
        bool isRuntime = true;
        bool isRegistration = false;//是否SDK已经注册标识
        int setAliasSeq = -1;//当前设置的用户标识
        public void Awake()
        {

            if (Instance == null)
            {
                Log.Debug("jpush sdk init");
                Instance = this;
                if (Application.isEditor)
                {
                    isRuntime = false;
                    return;
                }
                GameObject obj = new GameObject();
                JPushReceiver receiver = obj.AddComponent<JPushReceiver>();
                receiver.jpushDelegate_mess = OnReceiveMessage;
                receiver.jpushDelegate_noti = OnReceiveNotification;
                receiver.jpushDelegate_opennoti = OnOpenNotification;
                receiver.jpushDelegate_tag = OnJPushTagOperateResult;
                receiver.jpushDelegate_atlas = OnJPushAliasOperateResult;
                receiver.jpushDelegate_Reg = OnGetRegistrationId;
                obj.name = "JPushSdk";
                JPushBinding.Init(obj.name);
                JPushBinding.SetDebug(false);
                tags.Add("crazypoker");
                if (Application.platform == RuntimePlatform.Android)
                {
                    tags.Add("android");
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {

                    tags.Add("ios");
                }

                JPushBinding.SetTags(sequence++, tags);
                Log.Debug($"GetRegistrationId : {JPushBinding.GetRegistrationId()}");
                Log.Debug("jpush sdk complete");
                //JPushBinding.ClearLocalNotifications();
            }
        }

        /// <summary>
        /// 停止推送
        /// </summary>
        public void StopPush()
        {
            //            if (!isRuntime) return;
            //#if UNITY_ANDROID
            //            JPushBinding.StopPush();
            //#endif
        }

        /// <summary>
        /// 恢复推送
        /// </summary>
        public void ResumePush()
        {
            //            if (!isRuntime) return;
            //#if UNITY_ANDROID
            //            JPushBinding.ResumePush();
            //#endif
        }

        /// <summary>
        /// 本地推送
        /// </summary>
        /// <param name="builderId"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="nId"></param>
        /// <param name="broadcastTime"></param>
        public void AddLocalNotification(int builderId, string content, string title, int nId, int broadcastTime)
        {
            //            if (!isRuntime) return;
            //#if UNITY_ANDROID
            //            JPushBinding.AddLocalNotification(builderId, content, title, nId, broadcastTime , "");
            //#elif UNITY_IPHONE || UNITY_IOS
            //            JsonData args = new JsonData();
            //            args["title"] = title;
            //            args["id"] = builderId;
            //            args["content"] = content;
            //            args["badge"] = 10;
            //            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            //            //long ret = Convert.ToInt64(ts.TotalSeconds) + 3;
            //            args["fireTime"] = broadcastTime;
            //            args["subtitle"] = "the subtitle";

            //            JPushBinding.SendLocalNotification(args.ToJson());
            //#endif

        }

        /// <summary>
        /// 本地推送
        /// </summary>
        /// <param name="builderId"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="nId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        public void AddLocalNotificationByDate(int builderId, string content, string title, int nId,
                int year, int month, int day, int hour, int minute)
        {
            //            if (!isRuntime) return;
            //#if UNITY_ANDROID
            //            JPushBinding.AddLocalNotificationByDate(builderId, content, title, nId, year, month, day, hour, minute, 0, "");
            //#endif
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="mts"></param>
        public void AddTags(List<string> mts)
        {
            if (!isRuntime) return;
            JPushBinding.AddTags(sequence++, mts);
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="mts"></param>
        public void DeleteTags(List<string> mts)
        {
            if (!isRuntime) return;
            JPushBinding.DeleteTags(sequence++, mts);
        }

        /// <summary>
        /// 返回注册号
        /// </summary>
        /// <returns></returns>
        public string GetRegistrationId()
        {
            if (!isRuntime) return string.Empty;
            return JPushBinding.GetRegistrationId();
        }

        /// <summary>
        /// 设置识别号
        /// </summary>
        /// <param name="alias"></param>
        public void SetAlias(string alias)
        {
            if (!isRuntime) return;
            JPushBinding.SetAlias(sequence++, alias);
            setAliasSeq = sequence;
        }


        /// <summary>
        /// 删除识别号  退出APP  或登录时候用
        /// </summary>
        public void DeleteAlias()
        {

            if (!isRuntime) return;
            JPushBinding.DeleteAlias(sequence++);
        }

        // 开发者自己处理由 JPush 推送下来的消息。
        void OnReceiveMessage(string jsonStr)
        {
            //Debug.Log("recv----message-----" + jsonStr);
        }


        // 获取的是 json 格式数据，开发者根据自己的需要进行处理。
        void OnReceiveNotification(string jsonStr)
        {
            //Debug.Log("recv---notification---" + jsonStr);
            //Log.Debug("OnReceiveNotification");
            //JPushMes mes = JsonHelper.FromJson<JPushMes>(jsonStr);
        }

        //开发者自己处理点击通知栏中的通知
        void OnOpenNotification(string jsonStr)
        {
            //Debug.Log("recv---openNotification---" + jsonStr);
        }

        /// <summary>
        /// JPush 的 tag 操作回调。
        /// </summary>
        /// <param name="result">操作结果，为 json 字符串。</param>
        void OnJPushTagOperateResult(string result)
        {
            //Debug.Log("JPush tag operate result: " + result);
        }

        /// <summary>
        /// JPush 的 alias 操作回调。
        /// </summary>
        /// <param name="result">操作结果，为 json 字符串。</param>
        void OnJPushAliasOperateResult(string result)
        {
            try
            {
                OperateResult oRes = JsonHelper.FromJson<OperateResult>(result);
                //Log.Debug($"JPush alias oRes.sequence = {oRes.sequence}");
                if (oRes.sequence == setAliasSeq - 1)
                {
                    Log.Debug("JPush upload sequence on TD");
                    TalkingDataSdkComponent.Instance.UploadSdkAnalysis("JPush - OnJPushAliasOperateResult");
                }
            }
            catch (Exception e)
            {

                Log.Debug(e.ToString());
            }
        }

        void OnGetRegistrationId(string result)
        {
            Log.Debug("JPush on get registration Id: " + result);
        }
    }
}