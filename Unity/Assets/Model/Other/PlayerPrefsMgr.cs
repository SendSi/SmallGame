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


    #region float的get与set
    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public float GetFloat(string key, float def = 0.0f)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var value = PlayerPrefs.GetFloat(key, def);
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

    public const string SERVER_TYPE = "serverType";
    public const string CURRENT_SERVER_USER_CHOICEID = "CurrentServerUserChoiceID_";
    public const string CURRENT_USING_SERVERID = "CurrentUsingServerID_";
    public const string CURRENT_SERVER_HTTP_DOMAIN = "CurrentServerHttpDomain_";
    public const string CURRENT_SERVER_SCK_DOMAIN = "CurrentServerSckDomain_";
    public const string CURRENT_SERVER_RES_DOMAIN = "CurrentServerResDomain_";
    public const string NET_LINE_SWITCH_INFO = "NetLineSwithInfo_";
    public const string CURRENT_VIP_DNS_SERVERID = "CurrentVipDNSServerID_";

    public const string BgMusicVolKey = "BGMVol";
    public const string SoundFxVolKey = "SFXVol";
    public const string BgMusicMuteKey = "BGMMute";
    public const string SoundFxMuteKey = "SFXMute";

    public const string KEY_LANGUAGE = "KEY_LANGUAGE";
    // 0简中 1英文 2繁中


}


