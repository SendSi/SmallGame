using System;
using System.Collections;
using System.Net.NetworkInformation;
namespace ETHotfix
{
    public class SystemInfoUtil
    {
        //缓存是否模拟器信息
        static int sim = -1;

        static public string getLocalMac()
        {
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            string physicalAddress = "";
            foreach (NetworkInterface adaper in nis)
            {
                //UnityEngine.Debug.Log(adaper.Description);
                if (adaper.Description == "en0")
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    if (physicalAddress != "")
                    {
                        break;
                    };
                }
            }
            return physicalAddress;
        }
        /// <summary>         imei         </summary>
        static public string getDeviceUniqueIdentifier()
        {

            return UnityEngine.SystemInfo.deviceUniqueIdentifier;
        }

        static public string getDeviceMode()
        {
            return UnityEngine.SystemInfo.deviceModel;
        }

        static public string getOperatingSystem()
        {
            return UnityEngine.SystemInfo.operatingSystem;
        }

        static public int isSimulator
        {
            get
            {

                if (sim == -1)
                {

                    if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
                    {
                        bool isRoot = NativeManager.OnFuncCall<bool>("IsRoot");
                        sim = NativeManager.OnFuncCall<bool>("isSimulator") ? 1 : 0;
                        ETModel.Game.Hotfix.OnEmulatorInfo = (str) => {

                            //加入TD统计
                            NativeManager.EmulatorInfo enfo = JsonHelper.FromJson<NativeManager.EmulatorInfo>(str);
                            string sensorNum = $"[{enfo.sensorNum / 5 * 5}-{enfo.sensorNum / 5 * 5 + 5}]";

                            int bVer;
                            if (enfo.baseBandVersion != null)
                            {

                                bVer = enfo.baseBandVersion.Trim().Length > 0 ? 1 : 0;
                            }
                            else
                            {
                                bVer = 0;//不存在信息
                            }

                            string userAppNum;
                            if (enfo.userAppNum < 5)
                            {

                                userAppNum = "<5";
                            }
                            else if (enfo.userAppNum > 30)
                            {

                                userAppNum = ">30";
                            }
                            else if (enfo.userAppNum > 20)
                            {
                                userAppNum = "[20-30]";
                            }
                            else if (enfo.userAppNum > 10)
                            {
                                userAppNum = "[10-20]";
                            }
                            else
                            {
                                userAppNum = "[5-10]";
                            }
                            string sys = $"isSim:{sim} | senNum:{sensorNum} | appNum:{userAppNum} | bVer:{bVer} | root:{isRoot}";
                            Log.Debug(sys);
                            TalkingDataSdkComponent.Instance.UploadSimulatorAnalysis(sys);
                            ETModel.Game.Hotfix.OnEmulatorInfo = null;
                        };
                        if (isRoot) sim = 1;//若已经root  侧判断为模拟器
                    }
                    else
                    {
                        sim = 0;
                    }
                }
                return sim;
            }
        }
    }
}
