using System;
using System.Collections.Generic;
using UnityEngine;

//用于jpush sdk 的事件接收
public class JPushReceiver : MonoBehaviour
{

    public delegate void JPushDelegate(string str);
    public JPushDelegate jpushDelegate_mess;
    public JPushDelegate jpushDelegate_noti;
    public JPushDelegate jpushDelegate_opennoti;
    public JPushDelegate jpushDelegate_tag;
    public JPushDelegate jpushDelegate_atlas;
    public JPushDelegate jpushDelegate_Reg;

    /* data format
     {
        "message": "hhh",
        "extras": {
            "f": "fff",
            "q": "qqq",
            "a": "aaa"
        }
     }
     */
    // 开发者自己处理由 JPush 推送下来的消息。
    void OnReceiveMessage(string jsonStr)
    {
        Debug.Log("recv----message-----" + jsonStr);
        if (jpushDelegate_mess != null) jpushDelegate_mess(jsonStr);
    }

    /**
     * {
     *	 "title": "notiTitle",
     *   "content": "content",
     *   "extras": {
     *		"key1": "value1",
     *       "key2": "value2"
     * 	}
     * }
     */
    // 获取的是 json 格式数据，开发者根据自己的需要进行处理。
    void OnReceiveNotification(string jsonStr)
    {
        Debug.Log("recv---notification---" + jsonStr);
        if (jpushDelegate_noti != null) jpushDelegate_noti(jsonStr);
    }

    //开发者自己处理点击通知栏中的通知
    void OnOpenNotification(string jsonStr)
    {
        Debug.Log("recv---openNotification---" + jsonStr);
        if (jpushDelegate_opennoti != null) jpushDelegate_opennoti(jsonStr);
    }

    /// <summary>
    /// JPush 的 tag 操作回调。
    /// </summary>
    /// <param name="result">操作结果，为 json 字符串。</param>
    void OnJPushTagOperateResult(string result)
    {
        Debug.Log("JPush tag operate result: " + result);
        if (jpushDelegate_tag != null) jpushDelegate_tag(result);
    }

    /// <summary>
    /// JPush 的 alias 操作回调。
    /// </summary>
    /// <param name="result">操作结果，为 json 字符串。</param>
    void OnJPushAliasOperateResult(string result)
    {
        Debug.Log("JPush alias operate result: " + result);
        if (jpushDelegate_atlas != null) jpushDelegate_atlas(result);
    }

    void OnGetRegistrationId(string result)
    {
        Debug.Log("JPush on get registration Id: " + result);
        if (jpushDelegate_Reg != null) jpushDelegate_Reg(result);
    }
}

