namespace ETModel
{
    public class InstallPacketConfig : Object
    {
        /// <summary>
        /// 安卓版本
        /// </summary>
        public string Android;
        /// <summary>
        /// 安卓Apk下载地址
        /// </summary>
        public string AndroidUrl;
        /// <summary>
        /// 安卓Apk文件名
        /// </summary>
        public string ApkName;
        /// <summary>
        /// 安卓Apk文件大小
        /// </summary>
        public int ApkSize;
        /// <summary>
        /// IOS版本(非AppStore)
        /// </summary>
        public string IOS;
        /// <summary>
        /// IOS下载连接(非AppStore)
        /// </summary>
        public string IOSUrl;
        /// <summary>
        /// 安卓Apk更新内容
        /// </summary>
        public string Msg;
        /// <summary>
        /// IOS版本(AppStore)
        /// </summary>
        public string IOSAppStore;
        /// <summary>
        /// 是否AppStore
        /// </summary>
        public bool IsAppStore;
        /// <summary>
        /// AppStore版本是否更新资源
        /// </summary>
        public bool CheckRes;
    }
}