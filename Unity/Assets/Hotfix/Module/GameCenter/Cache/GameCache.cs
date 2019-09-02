/*
* ==============================================================================
*
* Filename: GameCache.cs
* Description: 游戏缓存数据
* Created: 2019/03/16
* Author: xxx
*
* ==============================================================================
*/
namespace ETHotfix
{
    enum UserGender
    {
        Male,
        Female
    }

    public class GameCache
    {
        private static GameCache _instance;

        public static GameCache Instance
        {
            get
            {
                return _instance ?? (_instance = new GameCache());
            }
        }
        /// <summary>
        /// 用户登录手机
        /// </summary>
        public string strPhone;

        /// <summary>
        /// 用户登录手机前缀(例"86")
        /// </summary>
        public string strPhoneFirst;

        /// <summary>
        /// 用户登录密码
        /// </summary>
        public string strPwd;
        /// <summary>
        /// 用户user id
        /// </summary>
        public int nUserId;
        /// <summary>
        /// 用户登录 token
        /// </summary>
        public string strToken;
        /// <summary>
        /// 是否首次登录0、1
        /// </summary>
        public sbyte isfirstLogin;
        /// <summary>
        /// 登录res服务器的IP或域名
        /// </summary>
        public string resIP;
        /// <summary>
        /// 登录res服务器的端口
        /// </summary>
        public int resport;
        /// <summary>
        /// 登录game服务器的IP或域名
        /// </summary>
        public string roomIP;
        /// <summary>
        /// 登录game服务器的端口
        /// </summary>
        public int roomPort;
        /// <summary>
        /// 0男,1女
        /// </summary>
        public sbyte sex;
        /// <summary>
        /// 消息个数
        /// </summary>
        public sbyte msg_count;
        /// <summary>
        /// 头像
        /// </summary>
        public string headPic;
        /// <summary>
        /// 昵称
        /// </summary>
        public string nick;
        /// <summary>
        /// 修改昵称次数
        /// </summary>
        public int modifyNickNum;
        /// <summary>
        /// 金豆余额
        /// </summary>
        public int gold;
        /// <summary>
        /// 钻石余额
        /// </summary>
        public int idou;
        /// <summary>
        /// 会员等级,0普通玩家 1伯爵 2侯爵 3王公
        /// </summary>
        public sbyte vipLevel;
        /// <summary>
        /// 会员到期日
        /// </summary>
        public string vipEndDate;
        /// <summary>
        /// 经度
        /// </summary>
        public string longitude = "23.1020856856";
        /// <summary>
        /// 纬度
        /// </summary>
        public string latitude = "113.3845281601";
        /// <summary>
        /// 定位到的地址名称
        /// </summary>
        public string locationName = "China";
        /// <summary>
        /// 当前客户端的ip地址，在牌局内坐下带入时要用到
        /// </summary>
        public string client_ip;
        /// <summary>
        /// 房间名称
        /// </summary>
        public string roomName;
        /// <summary>
        /// 普通局:61 MTT:71 SNG:81 奥马哈:91 大菠萝:51
        /// </summary>
        public int room_path;
        /// <summary>
        /// 房间号，如961672
        /// </summary>
        public int room_id;
        /// <summary>
        /// 最小带入记分牌，如200
        /// </summary>
        public int carry_small;
        /// <summary>
        /// 是否开启Straddle 0 1
        /// </summary>
        public int straddle;
        /// <summary>
        /// 是否开启保险，0 1
        /// </summary>
        public int insurance;
        /// <summary>
        /// 0 关闭 1 开启
        /// </summary>
        public int muck_switch;
        /// <summary>
        /// 最短上桌时间（分钟）
        /// </summary>
        public int shortest_time;
        /// <summary>
        /// 牌局类型,不需此条件，传值：-1 61 = 德州 91 = 奥马哈 51 = 大菠萝 41 = 必下场 31 = AOF 81 = SNG 71 = MTT
        /// </summary>
        public int rtype;
        /// <summary>
        /// jackPot基金，如60824
        /// </summary>
        public int jackPot_fund;
        /// <summary>
        /// 是否开启JackPot，0 1
        /// </summary>
        public int jackPot_on;
        /// <summary>
        /// JackPotID
        /// </summary>
        public int jackPot_id;
        /// <summary>
        /// 当前游戏对象
        /// </summary>
        // public TexasGame CurGame;
        /// <summary>
        /// 组局界面的缓存信息
        /// </summary>
        // public RcConfig[] reconfigs;
        /// <summary>
        /// 牌局内 个人信息 默认页  德州=1,奥马哈=2
        /// </summary>
        public int CurInfoRoomPath;
        /// <summary>
        /// 0 = 无资格 1 = 调用首次登陆API 2 = 已有首充资格 3 = 待领取
        /// </summary>
        public int isActivity;
        /// <summary>
        /// 只显示一次活动
        /// </summary>
        public bool isFirstShowActivity = true;
        public int kDouNum;

        GameCache()
        {
            nUserId = 0;
            strToken = "0";
        }
    }
}
