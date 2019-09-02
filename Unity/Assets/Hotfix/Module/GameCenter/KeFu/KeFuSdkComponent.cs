using System;
using UnityEngine;
using UnityEngine.UI;
using ETModel;

namespace ETHotfix
{
    //对应原生结构内容
    public class ChatParamsBody
    {
        public String erpParam = "";
        public String matchstr = "";
        public String startPageTitle = "";
        public String startPageUrl = "";
        public String headurl = "";
        public String headlocaldir = "";
        public int clickurltoshow_type = 0;
        public String chatWindowClassName;
        public String kfuid = "";
        public String kfname = "";
        public String sendMsg;
    }

    [ObjectSystem]
    public class KeFuSdkComponentAwakeSystem : AwakeSystem<KeFuSdkComponent>
    {
        public override void Awake(KeFuSdkComponent self)
        {
            self.Awake();
        }
    }

    public class KeFuSdkComponent : Component
    {
        public static KeFuSdkComponent Instance;
        public void Awake()
        {
            if (Instance == null)
            {
                Log.Debug("kefu sdk init");
                Instance = this;
                Init();
            }
        }

        /// <summary>
        /// 初始化SDK
        /// </summary>
        void Init()
        {

            String siteid = "kf_10535";
            String sdkkey = "c5403fda-4a16-4fbe-85e7-cf7041928e12";//
            if (Application.platform == RuntimePlatform.Android)
            {
                Log.Debug($"siteid = {siteid} , sdkkey = {sdkkey}");
                NativeManager.OnFuncCall("KeFuInit", siteid, sdkkey);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                NativeManager.KeFuInit(siteid, sdkkey);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void Login(string userId, string uersName)
        {

            string uname = $"{uersName}({userId})";
            int status = 0;
            if (Application.platform == RuntimePlatform.Android)
            {
                status = NativeManager.OnFuncCall<int>("KeFuLogin", userId, uname);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                status = NativeManager.KeFuLogin(userId, uname);
            }
            Log.Debug($"KeFuLogin status = {status}");
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void LoginOut()
        {

            if (Application.platform == RuntimePlatform.Android)
            {
                NativeManager.OnFuncCall("KeFuLoginOut");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                NativeManager.KeFuLoginOut();
            }
        }

        /// <summary>
        /// 调起聊天窗口
        /// </summary>
        public void StarChat(ChatParamsBody chatParams)
        {

            String settingid = "kf_10535_1558941800319";
            String groupName = "客服链接中...";
            string strbody = JsonHelper.ToJson(chatParams);
            int status = 0;
            if (Application.platform == RuntimePlatform.Android)
            {

                status = NativeManager.OnFuncCall<int>("StartChat", settingid, groupName, strbody);////
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                status = NativeManager.StartChat(settingid, groupName, strbody);
            }
            Log.Debug($"StartChat :: status = {status}");
        }
    }
}
