using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class LanguageMgr
{
    private static StringBuilder mBuilder;
    private static LanguageMgr instance;
    public static LanguageMgr mInstance { get { if (instance == null) instance = new LanguageMgr(); return instance; } }

    /// <summary>     当前的语言 0简中 1英文    2繁中     </summary>
    public int mCurLanguage = 0;


    /// <summary>     得到当前语言的 txt 名字     </summary>
    public string GetCurLanguageTxtName()
    {
        if (mCurLanguage == 0)//简
        {
            return "Language_ZH";
        }
        else if (mCurLanguage == 1)//英
        {
            return "Language_EN";
        }
        else if (mCurLanguage == 2)//繁体
        {
            return "Language_TW";
        }
        return "Language_ZH";
    }


    private Dictionary<string, string> mDicKeyLanguage = new Dictionary<string, string>();
    public event Action EActTextChange;

    /// <summary>     初始化 从component传内容     </summary>
    public void StartSetDicLanguage(string pLanguage)
    {
        mDicKeyLanguage.Clear();
        string[] lines = pLanguage.Split(new string[] { "\n" }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (line == null || line.Contains("=") == false) continue;

            string[] keyAndValue = line.Split('=');
            if (keyAndValue.Length <= 2)
            {
                mDicKeyLanguage[keyAndValue[0]] = keyAndValue[1].Trim();
            }
            else
            {
                if (null == mBuilder)
                    mBuilder = new StringBuilder();
                if (mBuilder.Length > 0)
                    mBuilder.Clear();
                for (int i = 1; i < keyAndValue.Length; i++)
                {
                    mBuilder.Append(keyAndValue[i]);
                    if (i < keyAndValue.Length - 1)
                    {
                        mBuilder.Append("=");
                    }
                }
                mDicKeyLanguage[keyAndValue[0]] = mBuilder.ToString().Trim();
            }
        }
    }

    /// <summary>     手动改语言.将发事件给Text 们     </summary>
    public void SettingSetDicLanguage(string pLanguage)
    {
        PlayerPrefsMgr.mInstance.SetInt(PlayerPrefsKeys.KEY_LANGUAGE, mCurLanguage.GetHashCode());
        StartSetDicLanguage(pLanguage);
        EActTextChange();
    }

    /// <summary>     根据key 去取值     </summary>
    public string GetLanguageForKey(string key)
    {
        string value;
        if (mDicKeyLanguage.TryGetValue(key, out value))
        {
            //#if LOG_ZSX
            //            return value.Replace("\\n", "\n") + "T";
            //#endif
            return value.Replace("\\n", "\n");
        }
        return "";
    }

    /// <summary>     根据key 去取值   value又有多个值 用^分隔开  index从0开始 </summary>
    public string GetLanguageForKeyMoreValue(string key, int index)
    {
        string value;
        if (mDicKeyLanguage.TryGetValue(key, out value))
        {
            var tOthers = value.Split('^');
            //#if LOG_ZSX
            //            return tOthers[index] + "T";
            //#endif
            return tOthers[index];
        }
        return "";
    }

    public string[] GetLanguageForKeyMoreValues(string key)
    {
        string value;
        if (mDicKeyLanguage.TryGetValue(key, out value))
        {
            var tOthers = value.Split('^');
            //#if LOG_ZSX
            //            for (int i = 0; i < tOthers.Length; i++)
            //            {
            //                tOthers[i] = tOthers[i] + "T";
            //            }
            //#endif
            return tOthers;
        }
        return new string[] { };
    }

}