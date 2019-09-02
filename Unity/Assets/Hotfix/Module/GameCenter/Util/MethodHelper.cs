using UnityEngine;

namespace ETHotfix
{
    public class MethodHelper
    {
        #region 简单的数据方法
        /// <summary> 是否手机号码    还有国外的手机号码,先不校验</summary>
        //public static bool IsPhoneNumber(string phoneNumber)
        //{
        //    return true;
        //    // return Regex.IsMatch(phoneNumber, @"^1(3[0-9]|5[0-9]|7[6-8]|8[0-9])[0-9]{8}$");
        //}

        /// <summary> true=全不为null"" </summary>
        public static bool IsStrNotNull(string[] pStrs)
        {
            if (pStrs == null || pStrs.Length == 0)
                return true;
            int countHas = 0;
            for (int i = 0; i < pStrs.Length; i++)
            {
                if (string.IsNullOrEmpty(pStrs[i]) == false) //有值
                {
                    countHas = countHas + 1;
                }
            }
            return countHas == pStrs.Length;
        }
        /// <summary> 输入 255的值   使用new color32() </summary>param>
        public static Color RGB255(float r, float g, float b)
        {
            return new Color(r / 255, g / 255, b / 255);
        }

        /// <summary> 输入 255的值  a值</summary>param>
        public static Color RGBA255(float r, float g, float b, float a)
        {
            return new Color(r / 255, g / 255, b / 255, a / 255);
        }
        #endregion
    }
}

