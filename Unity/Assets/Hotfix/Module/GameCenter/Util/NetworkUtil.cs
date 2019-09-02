using System;
using System.Collections.Generic;
using BestHTTP;
using ETModel;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace ETHotfix
{
    public static class NetworkUtil
    {
        public static void RemoveAllSessionComponent()
        {
            // CPLoginSessionComponent mCPLoginSessionComponent = Game.Scene.GetComponent<CPLoginSessionComponent>();
            // if (null != mCPLoginSessionComponent)
            //     Game.Scene.RemoveComponent<CPLoginSessionComponent>();
            // CPResSessionComponent mCPResSessionComponent = Game.Scene.GetComponent<CPResSessionComponent>();
            // if (null != mCPResSessionComponent)
            //     Game.Scene.RemoveComponent<CPResSessionComponent>();
            // CPGameSessionComponent mCPGameSessionComponent = Game.Scene.GetComponent<CPGameSessionComponent>();
            // if (null != mCPGameSessionComponent)
            //     Game.Scene.RemoveComponent<CPGameSessionComponent>();
        }


        public static void LogoutClear()
        {
            GameCache.Instance.strPwd = "";
            GameCache.Instance.nUserId = 0;
            GameCache.Instance.strToken = "";
            GameCache.Instance.headPic = "";
            GameCache.Instance.nick = "";
            PlayerPrefsMgr.mInstance.SetString(PlayerPrefsKeys.KEY_PWD, string.Empty);
            PlayerPrefsMgr.mInstance.SetInt(PlayerPrefsKeys.KEY_USERID, 0);
            PlayerPrefsMgr.mInstance.SetString(PlayerPrefsKeys.KEY_TOKEN, string.Empty);
            // UIMineModel.mInstance.Dispose();
            // UIMatchModel.mInstance.Dispose();
        }
    }
}
