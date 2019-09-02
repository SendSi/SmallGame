using System.Collections.Generic;

namespace ETHotfix
{
    public static class WebMessageHelper
    {
        //需要加载菊花的请求在这里配置
        private static readonly HashSet<string> ShowJuhuaSet = new HashSet<string>()
        {
            // WEB2_room_dz_enter.API,
            // WEB2_wallet_recharge.API,//钱包充豆
            // WEB2_wallet_withdraw.API,//钱包 提豆
            // WEB2_wallet_flow.API,//钱包 转豆
            // WEB2_room_dz_create.API,
            // WEB2_room_omaha_create.API,
            // WEB2_room_bp_create.API,
            // WEB2_room_sng_create.API,
            // WEB2_user_vip_setting.API,
            // WEB2_sms_sendCode.API,//验证码
            // WEB2_user_exist_phone.API,//是否存在了
            // WEB2_backpack_consignee_info.API,//背包地址
        };

        public static bool IsNeedShowJuhua(string API)
        {
            if (ShowJuhuaSet.Contains(API))
            {
                return true;
            }
            return false;
        }
    }
}