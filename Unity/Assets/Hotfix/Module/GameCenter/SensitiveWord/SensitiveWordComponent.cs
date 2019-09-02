using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using ETModel;
using System.Text.RegularExpressions;

namespace ETHotfix
{
    [ObjectSystem]
    public class SensitiveWordComponentAwakeSystem : AwakeSystem<SensitiveWordComponent>
    {
        public override void Awake(SensitiveWordComponent self)
        {
            self.Awake();
        }
    }

    public class SensitiveWordComponent : Component
    {
        public static SensitiveWordComponent Instance;
        public string[] txtString;

        public void Awake()
        {
            //if (instance == null)
            //{
            Instance = this;
            //获取敏感字txt文件
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle("sensitiveword.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("sensitiveword.unity3d", "SensitiveWord");

            TextAsset txt = bundleGameObject.GetComponent<ReferenceCollector>().Get<TextAsset>("all_sensitiveword");
            txtString = txt.text.Split(',');
            //
        }
        /// <summary>
        /// 是否包含敏感字
        /// </summary>
        public bool SensitiveWordJudge(string SensitiveWord_type, string SensitiveWord_Str)
        {
            if (null == SensitiveWord_type)
                return false;
            if (null == SensitiveWord_Str)
                return false;
            string senstr = Regex.Replace(SensitiveWord_Str, "[ \\[ \\] \\^ \\-_*×――(^)|'$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "").ToUpper();
            //遍历判定是否含有敏感字
            foreach (string str in txtString)
            {
                if (str.Trim().Length == 0) continue;
                if (senstr.IndexOf(str.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    UIComponent.Instance.Toast(CPErrorCode.LanguageDescription(20075, new List<object>() { SensitiveWord_type }));
                    return true;
                }
            }

            return false;
        }


    }
}

