using System;
using System.IO;
using ETModel;
using UnityEditor;

namespace ETEditor
{
	public static class BuildHelper
	{
		private const string relativeDirPrefix = "../Release";

		public static string BuildFolder = "../Release/{0}/StreamingAssets/";
		
		
		[MenuItem("Tools/web资源服务器")]
		public static void OpenFileServer()
		{
			ProcessHelper.Run("dotnet", "FileServer.dll", "../FileServer/");
		}

		public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions, BuildOptions buildOptions, bool isBuildExe, bool isContainAB)
		{
			BuildTarget buildTarget = BuildTarget.StandaloneWindows;
			string exeName = "game";
			switch (type)
			{
				case PlatformType.PC:
					buildTarget = BuildTarget.StandaloneWindows64;
					exeName += ".exe";
					break;
				case PlatformType.Android:
					buildTarget = BuildTarget.Android;
					exeName += ".apk";
					break;
				case PlatformType.IOS:
					buildTarget = BuildTarget.iOS;
					break;
				case PlatformType.MacOS:
					buildTarget = BuildTarget.StandaloneOSX;
					break;
			}

			string fold = string.Format(BuildFolder, type);
			if (!Directory.Exists(fold))
			{
				Directory.CreateDirectory(fold);
			}

            Log.Info("开始资源打包---version.txt md5未生成");
            BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget);
            Log.Info("完成资源打包---点加密 才生成");

            if (isContainAB)
			{
				FileHelper.CleanDirectory("Assets/StreamingAssets/");
				FileHelper.CopyDirectory(fold, "Assets/StreamingAssets/");
			}

			if (isBuildExe)
			{
				AssetDatabase.Refresh();
				string[] levels = {
					"Assets/Scenes/Init.unity",
				};
				Log.Info("开始EXE打包");
				BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions);
				Log.Info("完成exe打包");
			}
		}

        private static string[] gamesDirs = null;
        public static void GenerateVersionInfo(string dir, bool pIsABSec)
        {
            VersionConfig versionProto = new VersionConfig();
            GenerateVersionProto(dir, versionProto, "", pIsABSec);

            using (FileStream fileStream = new FileStream($"{dir}/Version.txt", FileMode.Create))
            {
                byte[] bytes = JsonHelper.ToJson(versionProto).ToByteArray();
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        private static void GenerateVersionProto(string dir, VersionConfig versionProto, string relativePath, bool pIsAbSec)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                string md5 = MD5Helper.FileMD5(file);
                FileInfo fi = new FileInfo(file);
                long size = fi.Length;
                string filePath = relativePath == "" ? fi.Name : $"{relativePath}/{fi.Name}";

                versionProto.FileInfoDict.Add(filePath, new FileVersionInfo
                {
                    File = filePath,
                    MD5 = md5,
                    Size = size,
                });
            }
            foreach (string directory in Directory.GetDirectories(dir))
            {
                DirectoryInfo dinfo = new DirectoryInfo(directory);
                string rel = relativePath == "" ? dinfo.Name : $"{relativePath}/{dinfo.Name}";
                GenerateVersionProto($"{dir}/{dinfo.Name}", versionProto, rel, pIsAbSec);
            }
            string tDayHour = DateTime.Now.ToString("yyMMddHHmm");
            versionProto.Version = int.Parse(tDayHour);
            versionProto.ABSecurity = pIsAbSec ? 2 : 1;//0未知,去load下, 1不使用加密   ,2加密-------若0使用不加密
            #region MyRegion
            //try
            //{//加密时,把version.txt删除了,无法记录第几次,只能使用时间记录了
            //    var oldConfig = JsonHelper.FromJson<VersionConfig>(File.ReadAllText($"{dir}/Version.txt"));
            //    var oldver = oldConfig.Version.ToString().Substring(6, 4);
            //    versionProto.Version = int.Parse(tDay + oldver) + 1;
            //}
            //catch (Exception)
            //{
            //    versionProto.Version = int.Parse(tDay + "1000");
            //} 
            #endregion
        }

        private static void GenerateVersionProtoForGame(string dir, string subGame, VersionConfig versionProto, string relativePath)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                // 剔除.manifest
                if (file.EndsWith(".manifest"))
                    continue;

                // 剔除子游戏
                bool pass = false;
                string tmp = file.Replace('\\', '/');
                foreach (string s in gamesDirs)
                {
                    if (tmp.Contains(s + "/"))
                    {
                        pass = true;
                        break;
                    }
                }
                if (!pass)
                    continue;

                string md5 = MD5Helper.FileMD5(file);
                FileInfo fi = new FileInfo(file);
                long size = fi.Length;
                string filePath = relativePath == "" ? $"{subGame}/{fi.Name}" : $"{relativePath}/{fi.Name}";

                versionProto.FileInfoDict.Add(filePath, new FileVersionInfo
                {
                    File = filePath,
                    MD5 = md5,
                    Size = size,
                });
            }

            foreach (string directory in Directory.GetDirectories(dir))
            {
                DirectoryInfo dinfo = new DirectoryInfo(directory);
                string rel = relativePath == "" ? $"{subGame}/{dinfo.Name}" : $"{relativePath}/{dinfo.Name}";
                GenerateVersionProtoForGame($"{dir}/{dinfo.Name}", subGame, versionProto, rel);
            }

            string tDay = DateTime.Now.ToString("yyMMdd");
            try
            {
                var oldConfig = JsonHelper.FromJson<VersionConfig>(File.ReadAllText($"{dir}/Version.txt"));
                var oldver = oldConfig.Version.ToString().Substring(6, 4);
                versionProto.Version = int.Parse(tDay + oldver) + 1;
            }
            catch (Exception)
            {
                versionProto.Version = int.Parse(tDay + "1000");
            }
        }

        [MenuItem("Tools/ilrt 复制Hotfix.dll, Hotfix.pdb到Res\\Code")]
        public static void CopyHotfixDll()
        {
            string CodeDir = "Assets/Res/Code/";
            string HotfixDll = "Unity.Hotfix.dll";
            string HotfixPdb = "Unity.Hotfix.pdb";

#if ILRuntime
            // Copy最新的pdb文件
            string[] dirs =
            {
                    "./Temp/UnityVS_bin/Debug",
                    "./Temp/UnityVS_bin/Release",
                    "./Temp/bin/Debug",
                    "./Temp/bin/Release"
            };

            DateTime dateTime = DateTime.MinValue;
            string newestDir = "";
            foreach (string dir in dirs)
            {
                string dllPath = Path.Combine(dir, HotfixDll);
                if (!File.Exists(dllPath))
                {
                    continue;
                }
                FileInfo fi = new FileInfo(dllPath);
                DateTime lastWriteTimeUtc = fi.LastWriteTimeUtc;
                if (lastWriteTimeUtc > dateTime)
                {
                    newestDir = dir;
                    dateTime = lastWriteTimeUtc;
                }
            }

            if (newestDir != "")
            {
                File.Copy(Path.Combine(newestDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
                File.Copy(Path.Combine(newestDir, HotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
                Log.Info($"ilrt 复制Hotfix.dll, Hotfix.pdb到Res/Code完成");
            }
#endif
        }

        public static void ChangeToAndroidForBat()
        {
            BuildTarget buildTarget = BuildTarget.Android;
            // 切换到 Android平台
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, buildTarget);
        }

        public static void BuildBundleAndroid()
        {
            BuildTarget buildTarget = BuildTarget.Android;
            // 切换到 Android平台
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, buildTarget);
            BuildHelper.Build(PlatformType.Android, BuildAssetBundleOptions.None, BuildOptions.None, false, false);
        }

        public static void BuildBundleIOS()
        {
            BuildTarget buildTarget = BuildTarget.iOS;
            // 切换到 Android平台
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, buildTarget);
            BuildHelper.Build(PlatformType.IOS, BuildAssetBundleOptions.None, BuildOptions.None, false, false);
        }
    }
}
