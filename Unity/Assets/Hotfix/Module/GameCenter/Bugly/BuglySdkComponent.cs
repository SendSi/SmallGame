//bugly sdk 插件
//登录界面初始化时加载
using System;
using UnityEngine;
using UnityEngine.UI;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class BuglySdkComponentSystem : AwakeSystem<BuglySdkComponent>
    {
        public override void Awake(BuglySdkComponent self)
        {
            self.Awake();
        }
    }

    public class BuglySdkComponent : Component
    {

        string channel = "渠道01";
        //string version = "开发1.0.0";

        //BuglyAgent.LogCallbackDelegate logcall;
        public static BuglySdkComponent Instance;
        bool isRuntime = true;
        public void Awake()
        {
            if (Instance == null)
            {
                Log.Debug("bugly sdk init");
                Instance = this;
                if (Application.isEditor)
                {
                    isRuntime = false;
                    return;
                }
#if DEBUG
                //BuglyAgent.ConfigDebugMode(true);
#endif
                //logcall = RegisterCallBack;
                //BuglyAgent.RegisterLogCallback(logcall);
                BuglyAgent.ConfigDefault(channel, Application.version, Application.productName, 0);

                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    if (GlobalData.Instance.serverType == 2)
                    {

                        BuglyAgent.InitWithAppId("1bef60b664");//正式服
                    }
                    else if (GlobalData.Instance.serverType == 1)
                    {
                        BuglyAgent.InitWithAppId("809b83eaa2");//测试服
                    }

                }
                else if (Application.platform == RuntimePlatform.Android)
                {

                    if (GlobalData.Instance.serverType == 2)
                    {

                        BuglyAgent.InitWithAppId("b15be2578e");//正式服
                    }
                    else if (GlobalData.Instance.serverType == 1)
                    {
                        BuglyAgent.InitWithAppId("02cceece35");//测试服
                    }
                }
                BuglyAgent.EnableExceptionHandler();
                BuglyAgent.PrintLog(LogSeverity.LogInfo, "bugly sdk init");
            }
        }

        void RegisterCallBack(string condition, string stackTrace, UnityEngine.LogType type)
        {
            Log.Debug($"BuglySdk::RegisterCallBack  condition = {condition}  stackTrace = {stackTrace} type = {type}");
        }

        /// <summary>
        /// Configs the default.
        /// </summary>
        /// <param name="channel">Channel.</param>
        /// <param name="version">Version.</param>
        /// <param name="user">User.</param>
        /// <param name="delay">Delay.</param>
        //void ConfigDefault(string channel, string version, string user, long delay)
        //{
        //    BuglyAgent.ConfigDefault(channel, version, user, delay);
        //}

        /// <summary>
        /// Sets the user identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public void SetUserId(string userid)
        {
            if (!isRuntime) return;
            BuglyAgent.SetUserId(userid);
        }

        /// <summary>
        /// Reports the exception.
        /// </summary>
        /// <param name="e">E.</param>
        /// <param name="message">Message.</param>
        public void ReportException(System.Exception e, string mes)
        {

            if (!isRuntime) return;
            BuglyAgent.ReportException(e, mes);
        }

        /// <summary>
        /// Reports the exception.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Stack trace.</param>
        public void ReportException(string name, string message, string stackTrace)
        {
            if (!isRuntime) return;
            BuglyAgent.ReportException(name, message, stackTrace);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
