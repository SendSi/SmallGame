using System;
using UnityEngine;
/// <summary>
/// 对PlayerPrefs的管理 
/// </summary>
public class PlayerPrefsMgr
{
    private static PlayerPrefsMgr _instance;
    public static PlayerPrefsMgr mInstance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerPrefsMgr();
            return _instance;
        }
    }

    #region string的get与set
    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public string GetString(string key,string def="")
    {
        if (PlayerPrefs.HasKey(key))
        {
            var value = PlayerPrefs.GetString(key,def);
            return value;
        }
        return def;
    }
    #endregion

    #region int的get与set
    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public int GetInt(string key,int def=0)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var value = PlayerPrefs.GetInt(key,def);
            return value;
        }
        return def;
    }
    #endregion


}

/// <summary>
/// 对Key 所有定义
/// </summary>
public class PlayerPrefsKeys
{
    public static string Key1 = "Key1";

    public static string Key_Shake = "Key_Shake";//默认0开启  1关闭
    public static string Key_Bgm = "Key_Shake";//默认0开启  1关闭
    public static string Key_Sfx= "Key_Sfx";//默认0开启  1关闭

    public const string SERVER_TYPE = "serverType";
    public const string CURRENT_SERVER_USER_CHOICEID = "CurrentServerUserChoiceID_";
    public const string CURRENT_USING_SERVERID = "CurrentUsingServerID_";
    public const string CURRENT_SERVER_HTTP_DOMAIN = "CurrentServerHttpDomain_";
    public const string CURRENT_SERVER_SCK_DOMAIN = "CurrentServerSckDomain_";
    public const string CURRENT_SERVER_RES_DOMAIN = "CurrentServerResDomain_";
    public const string NET_LINE_SWITCH_INFO = "NetLineSwithInfo_";
    public const string CURRENT_VIP_DNS_SERVERID = "CurrentVipDNSServerID_";

    public const string KEY_USERID = "USER_ID";
    public const string KEY_TOKEN = "USER_TOKEN";
    public const string KEY_PWD = "USER_PWD";
}


