using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    public class OneKeyReplaceWindow : EditorWindow
    {

        [MenuItem("Tools/小工具/打开 - Release目录")]
        public static void OpenReleaseDirectory()
        {
            System.Diagnostics.Process.Start("explorer.exe", Path.Combine(System.Environment.CurrentDirectory, @"..\Release"));

        }
        [MenuItem("Tools/小工具/清空 - Release目录")]
        public static void ClearReleaseFile()
        {
#if UNITY_IOS
            string path = System.Environment.CurrentDirectory.Replace("Unity", "Release/IOS");
#elif UNITY_ANDROID
          // string path = System.Environment.CurrentDirectory.Replace("Unity", "Release/Android");
          string path = $"{System.Environment.CurrentDirectory}/../Release/Android";
#else
            string path = $"{System.Environment.CurrentDirectory}/../Release/PC";
#endif
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Debug.Log("已经删除release文件夹");
            }
        }
        [MenuItem("Tools/小工具/打开 - 本地热磁盘目录")]
        public static void OpenSDHotfixDirectory()
        {
            if (Directory.Exists(Application.persistentDataPath))
            {
              
                System.Diagnostics.Process.Start("explorer.exe",$@"C:\Users\{System.Environment.UserName}\AppData\LocalLow\{Application.companyName}\{Application.productName}");
            }
            
        }
        [MenuItem("Tools/小工具/清空 - 本地热磁盘目录")]
        public static void DeleteSDHotfixDirectory()
        {
            string path = Application.persistentDataPath;
            if (Directory.Exists(path))
            {
                PlayerPrefs.DeleteAll();
                Directory.Delete(path, true);
                Debug.Log("已经删除热更沙盒路径");
            }
        }
       
        [MenuItem("Tools/小工具/清空StreamingAssets清单文件")]
        public static void ClearManifist()
        {
            string[] files = Directory.GetFiles(Application.streamingAssetsPath, "*.manifest");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Debug.Log("已经处理完毕清单文件");
        }

        [MenuItem("Tools/小工具/清空Release目录清单文件")]
        public static void ClearReleaseManifist()
        {
#if UNITY_IOS
            string path = System.Environment.CurrentDirectory.Replace("Unity", "Release/IOS");
#elif UNITY_ANDROID
          // string path = System.Environment.CurrentDirectory.Replace("Unity", "Release/Android");
          string path = $"{System.Environment.CurrentDirectory}/../Release/Android";
#else
            string path = $"{System.Environment.CurrentDirectory}/../Release/PC";
#endif
            path = $"{path}/StreamingAssets";
            
            if (!Directory.Exists(path))
            {
                Debug.Log($"目录不存在:{path}");
                return;
            }
            
            string[] files = Directory.GetFiles(path, "*.manifest");
            foreach (string file in files)
            {
                File.Delete(file);
            }

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                string[] files1 = Directory.GetFiles(dir, "*.manifest");
                foreach (string file in files1)
                {
                    File.Delete(file);
                }
            }
            
            Debug.Log("已经处理完毕清单文件");
        }

        //[MenuItem("Tools/小工具/删除 Missing Scripts")]
        //public static void RemoveMissingScript()
        //{


        //}
    }
}