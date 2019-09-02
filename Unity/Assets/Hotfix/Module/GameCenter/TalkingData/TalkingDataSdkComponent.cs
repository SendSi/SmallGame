//TalkingData sdk 插件
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class TalkingDataSdkComponentSystem : AwakeSystem<TalkingDataSdkComponent>
    {
        public override void Awake(TalkingDataSdkComponent self)
        {
            self.Awake();
        }
    }

    public class TalkingDataSdkComponent : Component
    {

        string appid;
        public static TalkingDataSdkComponent Instance;

        public TDGAAccount account;
        bool isRuntime = true;

        public void Awake()
        {
            if (Instance == null)
            {

                Instance = this;
                if (Application.isEditor)
                {
                    isRuntime = false;
                    return;
                }
                Log.Debug("talkingData sdk init");
                if (GlobalData.Instance.serverType == 2) appid = "5428ABD5D22B4D01B2EE139C192FFF03";//正式服
                else appid = "B35CF2D45DE4486BAEA407A7148F631D";//测试服
                TalkingDataGA.OnStart(appid, GlobalData.versionChannel);
                Log.Debug("talkingData sdk init completed ");
            }
        }

        /// <summary>
        /// 设置用户
        /// </summary>
        /// <param name="strUser"></param>
        public void SetAccount(string strUser)
        {

            if (!isRuntime) return;
            account = TDGAAccount.SetAccount(strUser);
        }

        /// <summary>
        /// 设置用户的类型
        /// </summary>
        /// <param name="at"></param>
        public void SetAccountType(AccountType at)
        {

            if (!isRuntime) return;
            if (account != null)
            {
                account.SetAccountType(at);
            }
            else Log.Error("error SetAccountType::you need to setAccount frist");
        }

        /// <summary>
        /// 用户发起充值请求时调用
        /// </summary>
        /// <param name="strOrder"></param>
        /// <param name="currencyAmount"></param>
        /// <param name="paymentType"></param>
        public void OnChargeRequest(string strOrder, int currencyAmount, string paymentType)
        {

            if (!isRuntime) return;
            TDGAVirtualCurrency.OnChargeRequest(strOrder, "iap", currencyAmount, "CH", currencyAmount, paymentType);
        }

        /// <summary>
        /// 充值成功时调用
        /// </summary>
        /// <param name="strOrder"></param>
        public void OnChargeSuccess(string strOrder)
        {
            if (!isRuntime) return;
            TDGAVirtualCurrency.OnChargeSuccess(strOrder);
        }

        /// <summary>
        /// 游戏内获得物品时调用
        /// </summary>
        /// <param name="rewardNum"></param>
        /// <param name="rsason"></param>
        public void OnReward(int rewardNum, string strRsason)
        {

            if (!isRuntime) return;
            TDGAVirtualCurrency.OnReward(rewardNum, strRsason);
        }

        /// <summary>
        /// 游戏内使用道具
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemNumber"></param>
        public void OnUse(string itemName, int itemNumber)
        {

            if (!isRuntime) return;
            TDGAItem.OnUse(itemName, itemNumber);
        }

        /// <summary>
        /// 用于游戏内虚拟道具的购买
        /// </summary>
        /// <param name="itemName">道具ID或者名称</param>
        /// <param name="itemNumber">数量</param>
        /// <param name="priceInVirtualCurrency">道具的价格</param>
        public void OnPurchase(string itemName, int itemNumber, int priceInVirtualCurrency)
        {

            if (!isRuntime) return;
            TDGAItem.OnPurchase(itemName, itemNumber, priceInVirtualCurrency);
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="missionName">任务名称</param>
        public void OnBeginMission(string missionName)
        {

            if (!isRuntime) return;
            TDGAMission.OnBegin(missionName);
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="missionName">任务名称</param>
        public void OnCompletedMission(string missionName)
        {
            if (!isRuntime) return;
            TDGAMission.OnCompleted(missionName);
        }

        /// <summary>
        /// 简单的行为事件调用
        /// </summary>
        /// <param name="strActionId"></param>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        public void OnEvent(string strActionId, string eventName, object value)
        {
            if (!isRuntime) return;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(eventName, value);
            TalkingDataGA.OnEvent(strActionId, dic);
        }

        /// <summary>
        /// 多项的行为事件调用
        /// </summary>
        /// <param name="strActionId"></param>
        /// <param name="dic"></param>
        public void OnEvent(string strActionId, Dictionary<string, object> dic)
        {
            if (!isRuntime) return;
            TalkingDataGA.OnEvent(strActionId, dic);
        }


        public override void Dispose()
        {
            base.Dispose();
            if (!isRuntime) return;
            TalkingDataGA.OnEnd();
        }


        //自定义统计事件
        public void UploadSdkAnalysis(string sis)
        {
            TalkingDataSdkComponent.Instance.OnEvent("sdk", "init_event", sis);
        }

        //统计手机设备信息(用于区分模拟器)
        public void UploadSimulatorAnalysis(string sis)
        {

            TalkingDataSdkComponent.Instance.OnEvent("sdk", "simulator_event", sis);
        }
    }
}
