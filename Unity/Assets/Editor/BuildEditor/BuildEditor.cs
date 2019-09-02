using System.Collections.Generic;
using System.IO;
using System.Linq;
using ETModel;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ETEditor
{
    public class BundleInfo
    {
        public List<string> ParentPaths = new List<string>();
    }

    public enum PlatformType
    {
        None,
        Android,
        IOS,
        PC,
        MacOS,
    }

    public enum BuildType
    {
        Development,
        Release,
    }

    public class BuildEditor : EditorWindow
    {
        private readonly Dictionary<string, BundleInfo> dictionary = new Dictionary<string, BundleInfo>();

        private PlatformType platformType = PlatformType.Android;
        private bool isBuildExe;
        private bool isContainAB;
        private BuildType buildType;
        private BuildOptions buildOptions = BuildOptions.AllowDebugging | BuildOptions.Development;
        private BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;

        [MenuItem("Tools/打包工具 #&%B")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BuildEditor));
        }

        void OnEnable()
        {
#if UNITY_IOS
            this.platformType = PlatformType.IOS;
#elif UNITY_ANDROID
            this.platformType = PlatformType.Android;
#elif UNITY_STANDALONE_WIN
            this.platformType = PlatformType.PC;
#elif UNITY_STANDALONE_OSX
            this.platformType = PlatformType.MacOS;
#endif
        }

        private void OnGUI()
        {
            this.platformType = (PlatformType)EditorGUILayout.EnumPopup(platformType);
            this.isBuildExe = EditorGUILayout.Toggle("是否打包EXE: ", this.isBuildExe);
            this.isContainAB = EditorGUILayout.Toggle("是否同将资源打进EXE: ", this.isContainAB);
            this.buildType = (BuildType)EditorGUILayout.EnumPopup("BuildType: ", this.buildType);

            switch (buildType)
            {
                case BuildType.Development:
                    this.buildOptions = BuildOptions.Development | BuildOptions.AutoRunPlayer | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
                    break;
                case BuildType.Release:
                    this.buildOptions = BuildOptions.None;
                    break;
            }

            this.buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("BuildAssetBundleOptions(可多选): ", this.buildAssetBundleOptions);

            if (GUILayout.Button("开始打包", GUILayout.Height(30)))
            {
                if (this.platformType == PlatformType.None)
                {
                    Log.Error("请选择打包平台!");
                    return;
                }

                var filepath = Application.dataPath.Replace("Unity/Assets", "") + "Release/" + this.platformType.ToString() + "/StreamingAssets";
                if (Directory.Exists(filepath))
                {
                    Directory.Delete(filepath, true);
                    Log.Debug("一口气删除原文件夹,全部重新生成,时间会久一点,但未改动的文件的md5会不变的,不用担心热更包会很大---Version.txt");
                }

                BuildHelper.Build(this.platformType, this.buildAssetBundleOptions, this.buildOptions, this.isBuildExe, this.isContainAB);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("一键标记AssetBundle"))
            {
                if (EditorUtility.DisplayDialog("温馨提示", "是否一键标记AssetBundle", "确认", "取消"))
                {
                    SetPackingTagAndAssetBundle();
                }
            }

            if (GUILayout.Button("一键清除AssetBundle标记"))
            {
                if (EditorUtility.DisplayDialog("温馨提示", "是否一键清除AssetBundle标记", "确认", "取消"))
                {
                    ClearPackingTagForBundlesFolder();
                    ClearPackingTagForResFolder();
                }
            }


            GUILayout.Space(20);
            GUILayout.Label("看宏:Async=True&&ILRuntime=true,才走AB包模式:PS运行时,看下web服务器开了没,GlobalProto***.txt路径对不");
            EditorGUILayout.BeginVertical("Box");
            {
                mIsSecurity = EditorGUILayout.Toggle("AB包加密", mIsSecurity);
                if (GUILayout.Button("生成Version.txt的json的md5(ps:若加密别重复点击)", GUILayout.Height(30)))
                {
                    if (mIsSecurity)
                        WillABSecurity();
                    Log.Debug(mIsSecurity ? "使用-加密模式" : "未使用-加密模式");
                    BuildHelper.GenerateVersionInfo(string.Format("../Release/{0}/StreamingAssets/", this.platformType), mIsSecurity);
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            //GUILayout.Label("android\n1.查看ResourcesComponent.cs的LoadOneBundle的方法调用的LoadFromFile方法加128字节.  两处改  \n2.查看AssetsBundleLoaderAsync.cs的LoadAsync的方法.LoadFromFileAsync要加上128字节.  一处改",GUILayout.Height(48));  
            GUILayout.Label("原理:生成的ab包后给其加上128个字节,这样有多余的字节AssetStudio工具就无法解析啦(加密)\n读取时,拿到的资源就往后移128个字节,这样就能解析出来(解密文件)");
            if (GUILayout.Button("想过用byte还原回去,但还原不了,只能打包AB时删除原目录或手动解压压缩回去"))
            {
                return;
                var filepath = Application.dataPath.Replace("Unity/Assets", "") + "Release/" + this.platformType.ToString() + "/StreamingAssets";
                var files = Directory.GetFiles(filepath, "*.*");
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains("StreamingAssets.manifest") || files[i].Contains("Version.txt"))
                    {
                    }
                    else
                    {//因为打包要使用,还原回去
                        byte[] oldData = File.ReadAllBytes(files[i]);
                        if (oldData[0] == 0 && oldData[1] == 0)
                        {
                            Log.Debug(files[i]);
                            int newOldLen = oldData.Length - 128;
                            var newData = new byte[newOldLen];
                            int n = 0;
                            for (int y = 128; y < oldData.Length; y++)
                            {
                                newData[n] = oldData[y];
                                n++;
                            }
                            FileStream fs = File.OpenWrite(files[i]);
                            fs.Write(newData, 0, newOldLen);
                            fs.Close();
                        }
                    }
                }
            }
        }
        bool mIsSecurity = true;
        void WillABSecurity()
        {
            var filepath = Application.dataPath.Replace("Unity/Assets", "") + "Release/" + this.platformType.ToString() + "/StreamingAssets";
            var files = Directory.GetFiles(filepath, "*.*");
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains("StreamingAssets.manifest") || files[i].Contains("Version.txt"))
                {
                }
                else
                {
                    byte[] oldData = File.ReadAllBytes(files[i]);
                    int newOldLen = 128 + oldData.Length;
                    var newData = new byte[newOldLen];
                    for (int tb = 0; tb < oldData.Length; tb++)
                    {
                        newData[128 + tb] = oldData[tb];
                    }
                    FileStream fs = File.OpenWrite(files[i]);
                    fs.Write(newData, 0, newOldLen);
                    fs.Close();
                    //查看LoadOneBundle的方法调用的LoadFromFile方法加128字节.  两处改
                    //查看LoadAsync的方法.LoadFromFileAsync要加上128字节.  一处改
                }
            }
        }



        private void SetPackingTagAndAssetBundle()
        {
            ClearPackingTagAndAssetBundle();

            SetIndependentBundleAndAtlas("Assets/Bundles/Independent");

            SetBundleAndAtlasWithoutShare("Assets/Bundles/UI");

            SetRootBundleOnly("Assets/Bundles/Unit");

            SetRootBundleOnlyForFolder("Assets/Bundles/Games");

            SetPackingTagForCommonAtlas();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        }

        private void SetPackingTagForCommonAtlas()
        {
            List<string> paths = EditorResHelper.GetAllResourcePath("Assets/Res/Atlas/Common", true);
            foreach (string pt in paths)
            {
                string tmp = pt.Replace('\\', '/');
                tmp = tmp.Replace("Assets/Res/Atlas/", "");
                tmp = tmp.Substring(0, tmp.IndexOf('/'));
                SetBundleAndAtlas(pt, $"atlas{tmp.ToLower()}", true);
            }
        }

        private static void SetNoAtlas(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);

            foreach (string path in paths)
            {
                List<string> pathes = CollectDependencies(path);

                foreach (string pt in pathes)
                {
                    if (pt == path)
                    {
                        continue;
                    }

                    SetAtlas(pt, "", true);
                }
            }
        }

        // 会将目录下的每个prefab引用的资源强制打成一个包，不分析共享资源
        private static void SetBundles(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                SetBundle(path1, go.name);
            }
        }

        // 会将目录下的每个prefab引用的资源打成一个包,只给顶层prefab打包
        private static void SetRootBundleOnly(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                SetBundle(path1, go.name);
            }
        }

        private static void SetRootBundleOnlyForFolder(string dir)
        {
            List<string> dirs = new List<string>();
            FileHelper.GetAllDirectories(dirs, dir);
            foreach (string s in dirs)
            {
                List<string> paths = EditorResHelper.GetPrefabsAndScenes(s);
                foreach (string path in paths)
                {
                    string path1 = path.Replace('\\', '/');
                    Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);
                    string name = path1.Replace(dir, "");
                    int startIndex = 0;
                    int length = name.LastIndexOf('.');
                    if (name[0].Equals('/'))
                    {
                        startIndex = 1;
                        length -= 1;
                    }
                    name = name.Substring(startIndex, length);
                    SetBundle(path1, name.ToLower());
                }
            }
        }

        // 会将目录下的每个prefab引用的资源强制打成一个包，不分析共享资源
        private static void SetIndependentBundleAndAtlas(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                AssetImporter importer = AssetImporter.GetAtPath(path1);
                if (importer == null || go == null)
                {
                    Log.Error("error: " + path1);
                    continue;
                }
                importer.assetBundleName = $"{go.name}.unity3d";

                List<string> pathes = CollectDependencies(path1);

                foreach (string pt in pathes)
                {
                    if (pt == path1)
                    {
                        continue;
                    }

                    SetBundleAndAtlas(pt, go.name, true);
                }
            }
        }

        private static void SetBundleAndAtlasWithoutShare(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                SetBundle(path1, go.name);

                //List<string> pathes = CollectDependencies(path1);
                //foreach (string pt in pathes)
                //{
                //	if (pt == path1)
                //	{
                //		continue;
                //	}
                //
                //	SetBundleAndAtlas(pt, go.name);
                //}
            }
        }

        private static List<string> CollectDependencies(string o)
        {
            string[] paths = AssetDatabase.GetDependencies(o);

            //Log.Debug($"{o} dependecies: " + paths.ToList().ListToString());
            return paths.ToList();
        }

        // 分析共享资源
        private void SetShareBundleAndAtlas(string dir)
        {
            this.dictionary.Clear();
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);

            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                SetBundle(path1, go.name);

                List<string> pathes = CollectDependencies(path1);
                foreach (string pt in pathes)
                {
                    if (pt == path1)
                    {
                        continue;
                    }

                    // 不存在则记录下来
                    if (!this.dictionary.ContainsKey(pt))
                    {
                        // 如果已经设置了包
                        if (GetBundleName(pt) != "")
                        {
                            continue;
                        }
                        Log.Info($"{path1}----{pt}");
                        BundleInfo bundleInfo = new BundleInfo();
                        bundleInfo.ParentPaths.Add(path1);
                        this.dictionary.Add(pt, bundleInfo);

                        SetAtlas(pt, go.name);

                        continue;
                    }

                    // 依赖的父亲不一样
                    BundleInfo info = this.dictionary[pt];
                    if (info.ParentPaths.Contains(path1))
                    {
                        continue;
                    }
                    info.ParentPaths.Add(path1);

                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    string dirName = dirInfo.Name;

                    SetBundleAndAtlas(pt, $"{dirName}-share", true);
                }
            }
        }

        private static void ClearPackingTagAndAssetBundle()
        {
            //List<string> bundlePaths = EditorResHelper.GetAllResourcePath("Assets/Bundles/", true);
            //foreach (string bundlePath in bundlePaths)
            //{
            //	SetBundle(bundlePath, "", true);
            //}

            List<string> paths = EditorResHelper.GetAllResourcePath("Assets/Res", true);
            foreach (string pt in paths)
            {
                SetBundleAndAtlas(pt, "", true);
            }
        }

        private static void ClearPackingTagForBundlesFolder()
        {
            List<string> paths = EditorResHelper.GetAllResourcePath("Assets/Bundles", true);
            foreach (string pt in paths)
            {
                SetBundleAndAtlas(pt, "", true);
            }
        }

        private static void ClearPackingTagForResFolder()
        {
            List<string> paths = EditorResHelper.GetAllResourcePath("Assets/Res", true);
            foreach (string pt in paths)
            {
                SetBundleAndAtlas(pt, "", true);
            }
        }

        private static void ClearPackingTagForResAtlasFolder()
        {
            List<string> paths = EditorResHelper.GetAllResourcePath("Assets/Res/Atlas", true);
            foreach (string pt in paths)
            {
                SetBundleAndAtlas(pt, "", true);
            }
        }

        private static string GetBundleName(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js")
            {
                return "";
            }
            if (path.Contains("Resources"))
            {
                return "";
            }

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                return "";
            }

            return importer.assetBundleName;
        }

        private static void SetBundle(string path, string name, bool overwrite = false)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js")
            {
                return;
            }
            if (path.Contains("Resources"))
            {
                return;
            }

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                return;
            }

            if (importer.assetBundleName != "" && overwrite == false)
            {
                return;
            }

            //Log.Info(path);
            string bundleName = "";
            if (name != "")
            {
                bundleName = $"{name}.unity3d";
            }

            importer.assetBundleName = bundleName;
        }

        private static void SetAtlas(string path, string name, bool overwrite = false)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js")
            {
                return;
            }
            if (path.Contains("Resources"))
            {
                return;
            }

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter == null)
            {
                return;
            }

            if (textureImporter.spritePackingTag != "" && overwrite == false)
            {
                return;
            }

            textureImporter.spritePackingTag = name;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        }

        private static void SetBundleAndAtlas(string path, string name, bool overwrite = false)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js" || extension == ".mat")
            {
                return;
            }
            if (path.Contains("Resources"))
            {
                return;
            }

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                return;
            }

            if (importer.assetBundleName == "" || overwrite)
            {
                string bundleName = "";
                if (name != "")
                {
                    bundleName = $"{name}.unity3d";
                }

                importer.assetBundleName = bundleName;
            }

            TextureImporter textureImporter = importer as TextureImporter;
            if (textureImporter == null)
            {
                return;
            }

            if (textureImporter.spritePackingTag == "" || overwrite)
            {
                textureImporter.spritePackingTag = name;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
