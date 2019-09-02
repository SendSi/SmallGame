using System.Runtime.InteropServices;
using ETModel;
using UnityEngine;
using System.Collections;

public class NativeManager : MonoBehaviour
{
    public static NativeManager Instance;

    private void Awake()
    {
        Instance = this;

#if UNITY_ANDROID

        if (Application.platform == RuntimePlatform.Android)
        {
            using(AndroidJavaClass sdkserver = new AndroidJavaClass(SDK_JAVA_CLASS)) {

                sdkserver.CallStatic("Init");
            }
        }
#endif
    }

    public SafeArea safeArea = new SafeArea()
    {
        top = 0,
        bottom = 0,
        left = 0,
        right = 0,
        width = 750
    };

    private const string SDK_JAVA_CLASS = "com.hbkj.gamecenter.sdk.SdkServer";

    public static void OnFuncCall(string funcName, params object[] args)
    {
        using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
        {
            cls.CallStatic(funcName, args);
        }
    }

    public static T OnFuncCall<T>(string funcName, params object[] args)
    {
        using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
        {
            return cls.CallStatic<T>(funcName, args);
        }
    }

    public static void OnClassFuncCall(string javaCalss, string funcName, params object[] args)
    {
        using (AndroidJavaClass cls = new AndroidJavaClass(javaCalss))
        {
            cls.CallStatic(funcName, args);
        }
    }

    public static T OnClassFuncCall<T>(string javaCalss, string funcName, params object[] args)
    {
        using (AndroidJavaClass cls = new AndroidJavaClass(javaCalss))
        {
            return cls.CallStatic<T>(funcName, args);
        }
    }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void CallByUnity(string jsonContent);

    [DllImport("__Internal")]
    public static extern void IMLogin(string userID, string token, int appID, string systemSender);

    [DllImport("__Internal")]
    public static extern void IMLoginOut();

    [DllImport("__Internal")]
    public static extern bool StartRecord();

    [DllImport("__Internal")]
    public static extern long StopRecord(bool isSend);

    [DllImport("__Internal")]
    public static extern void PlayVoice(string path);

    [DllImport("__Internal")]
    public static extern void StopVoice();

    [DllImport("__Internal")]
    public static extern bool IsPlayingVoice();

    [DllImport("__Internal")]
    public static extern void SetVolume(float volume);

    [DllImport("__Internal")]
    public static extern void IMSendMessage(string str);

    [DllImport("__Internal")]
    public static extern void SetConversation(string groupId);

    [DllImport("__Internal")]
    public static extern void ClearConversation();

    [DllImport("__Internal")]
    public static extern int GetImState();

    [DllImport("__Internal")]
    public static extern void ApplyJoinGroup(string groupId);

    [DllImport("__Internal")]
    public static extern void UpdateShowStatusBar(bool show);

    [DllImport("__Internal")]
    public static extern void SaveImageToNative(byte[] bytes, long length);

    [DllImport("__Internal")]
    public static extern void KeFuInit(string siteid, string sdkkey);

    [DllImport("__Internal")]
    public static extern int KeFuLogin(string userId, string userName);

    [DllImport("__Internal")]
    public static extern void KeFuLoginOut();

    [DllImport("__Internal")]
    public static extern int StartChat(string settingid, string groupName, string strbody);

#endif

#if UNITY_ANDROID
    public static void IMLogin(string userID, string token, int appID, string systemSender) { }
    public static void IMLoginOut() { }
    public static bool StartRecord() { return false; }
    public static long StopRecord(bool isSend) { return 0; }
    public static void PlayVoice(string path) { }
    public static void StopVoice() { }
    public static bool IsPlayingVoice() { return false; }
    public static void SetVolume(float volume) { }
    public static void IMSendMessage(string str) { }
    public static void SetConversation(string groupId) { }
    public static void ClearConversation() { }
    public static int GetImState() { return 0; }
    public static void ApplyJoinGroup(string groupId) { }
    public static void UpdateShowStatusBar(bool show) { }
    public static void SaveImageToNative(byte[] bytes, long length) { }
    public static void KeFuInit(string siteid, string sdkkey) { }
    public static int KeFuLogin(string userId, string userName) { return 0; }
    public static void KeFuLoginOut() { }
    public static int StartChat(string settingid, string groupName, string strbody) { return 0; }
#endif

#if UNITY_STANDALONE
    public static void IMLogin(string userID, string token, int appID, string systemSender) { }
    public static void IMLoginOut() { }
    public static bool StartRecord() { return false; }
    public static long StopRecord(bool isSend) { return 0; }
    public static void PlayVoice(string path) { }
    public static void StopVoice() { }
    public static bool IsPlayingVoice() { return false; }
    public static void SetVolume(float volume) { }
    public static void IMSendMessage(string str) { }
    public static void SetConversation(string groupId) { }
    public static void ClearConversation() { }
    public static int GetImState() { return 0; }
    public static void ApplyJoinGroup(string groupId) { }
    public static void UpdateShowStatusBar(bool show) { }
    public static void SaveImageToNative(byte[] bytes, long length) { }
    public static void KeFuInit(string siteid, string sdkkey) { }
    public static int KeFuLogin(string userId, string userName) { return 0; }
    public static void KeFuLoginOut() { }
    public static int StartChat(string settingid, string groupName, string strbody) { return 0; }
#endif

    public static void CallNative(string jsonContent)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;

#if UNITY_ANDROID
        OnFuncCall("CallByUnity", jsonContent);
#elif UNITY_IOS
        CallByUnity(jsonContent);
#endif
    }

    public void NativeCallUnity(string jsonContent)
    {
        ResolverForNativeMsg(jsonContent);
    }

    //解析原生返回数据
    public static void ResolverForNativeMsg(string content)
    {
        // Debug.Log($"ResolverForNativeMsg::{content}");
        NativeMessage nativeMessage = JsonHelper.FromJson<NativeMessage>(content);
        NativeCallType type = (NativeCallType)nativeMessage.type;

        switch (type)
        {
            case NativeCallType.GetSafeArea:
                //适配刘海屏
                SafeArea safeArea = JsonHelper.FromJson<SafeArea>(nativeMessage.content);
                NativeManager.Instance.safeArea = safeArea;
                break;
            case NativeCallType.AwakeByURL:
                //URL唤醒
                string awakeContent = nativeMessage.content;
                // Debug.Log($"游戏被唤起，参数为:{awakeContent}");
                Game.Hotfix.OnAwakeByURL?.Invoke(awakeContent);
                break;
            case NativeCallType.GetGPS:
                //获取GPS定位
                GPSInfo gPSInfo = JsonHelper.FromJson<GPSInfo>(nativeMessage.content);
                // Debug.Log($"获取GPS信息返回:statue={gPSInfo.status},lan={gPSInfo.latitude},long={gPSInfo.longitude},name={gPSInfo.locationName}");
                Game.Hotfix.OnGetGPS?.Invoke(string.Format("{0},{1},{2},{3}", gPSInfo.status, gPSInfo.longitude, gPSInfo.latitude, gPSInfo.locationName));
                break;
            case NativeCallType.TakeHeadImage:
                //获取头像图片路径
                // Debug.Log("ongetHead");
                Game.Hotfix.OnGetHeadImagePath?.Invoke(nativeMessage.content);
                break;
            case NativeCallType.InAppPurchase:
                InAppPurchaseInfo purchaseInfo = JsonHelper.FromJson<InAppPurchaseInfo>(nativeMessage.content);
                Game.Hotfix.OnInAppPurchaseCallBack?.Invoke(string.Format("{0},{1},{2},{3},{4}", purchaseInfo.status, purchaseInfo.productId, purchaseInfo.orderId, purchaseInfo.transactionId, purchaseInfo.receiptData));
                break;
            case NativeCallType.IMMes:
                Game.Hotfix.OnGroupMes?.Invoke(nativeMessage.content);
                break;
            case NativeCallType.IMOpearteMes:
                Game.Hotfix.OnOpearteMes?.Invoke(nativeMessage.content);
                break;
            case NativeCallType.IMServerMes:
                Game.Hotfix.OnServerMes?.Invoke(nativeMessage.content);
                break;
            case NativeCallType.EMULATORINFO:
                Game.Hotfix.OnEmulatorInfo?.Invoke(nativeMessage.content);
                break;
            default:
                Debug.Log($"error nativeMessage.type {nativeMessage.type} content = {nativeMessage.content}");
                break;
        }
    }

    public enum NativeCallType
    {
        GetSafeArea = 1,            //获取异形屏设备信息
        AwakeByURL = 2,             //URL唤醒
        GetGPS = 3,                 //获取GPS定位
        TakeHeadImage = 4,          //获取头像图片
        InAppPurchase = 5,          //iOS内购

        EMULATORINFO = 20,          //模拟器信息

        IMMes = 100,                //牌局的聊天信息
        IMOpearteMes = 101,          //IM操作信息
        IMServerMes = 200,          //服务器发送来的信息
    }

    public sealed class NativeMessage
    {
        public int type;
        public string content;
    }

    public sealed class SafeArea
    {
        public float top;
        public float bottom;
        public float left;
        public float right;
        public float width;//屏幕宽度，必须与top是同一个单位
    }

    public sealed class GPSInfo
    {
        public int status;      //状态 0 成功 1用户关闭GPS权限 2其他错误
        public double latitude;
        public double longitude;
        public string locationName; //地址的名称
    }

    public sealed class InAppPurchaseInfo
    {
        public int status;      //0 成功 1 默认错误 2 网络错误 3 取消支付 4 等待上一个支付 5 内购不可用 6 无法获取产品信息 7 提交订单失败 8 等待交易中
        public string productId;
        public string orderId;
        public string transactionId;
        public string receiptData;
    }

    #region 向原生发起请求
    public static void GetDeviceSafeArea()
    {
        NativeManager.CallNative($"{{\"type\":\"{(int)NativeManager.NativeCallType.GetSafeArea}\"}}");
    }

    public static void IntAwakeByURLObserver()
    {
        NativeManager.CallNative($"{{\"type\":\"{(int)NativeManager.NativeCallType.AwakeByURL}\"}}");
    }

    public static void GetGPSLocation()
    {
        NativeManager.CallNative($"{{\"type\":\"{(int)NativeManager.NativeCallType.GetGPS}\"}}");
    }

    public static void TakeHeadImage(string type) //1拍照 2相册
    {
        NativeManager.CallNative($"{{\"type\":\"{(int)NativeManager.NativeCallType.TakeHeadImage}\",\"content\":\"{type}\"}}");
    }

    public static void InAppPurchase(string productID, string orderID)
    {
        NativeManager.CallNative($"{{\"type\":\"{(int)NativeManager.NativeCallType.InAppPurchase}\",\"content\":{{\"productID\":\"{productID}\",\"orderID\":\"{orderID}\"}}}}");
    }

    public static bool SaveImage(byte[] bytes)
    {
        return OnFuncCall<bool>("SaveImage", bytes);
    }

    public static bool SaveImageScreen(byte[] bytes)
    {
        return OnFuncCall<bool>("SaveImageScreen", bytes);
    }

    /// <summary>
    /// 临时接口  调起apk
    /// </summary>
    /// <param name="path"></param>
    public static void OpenApk(string path)
    {

        OnFuncCall("InstallApk", path);
    }

    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotIEmu());
    }

    public IEnumerator CaptureScreenshotIEmu()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        Game.Hotfix.OnGetScreenShot?.Invoke(texture);
    }



    #endregion


    public sealed class EmulatorInfo
    {
        public int sensorNum;
        public int userAppNum;
        public string baseBandVersion;
        public int isSimulator;
    }
}
