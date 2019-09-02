using System;
using System.Collections.Generic;
using System.IO;
using ETModel;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public class GlobalProtoEditor: EditorWindow
    {
        const string pathOffline = @".\Assets\Res\Config\GlobalProto_Offline.txt";
        const string pathTest = @".\Assets\Res\Config\GlobalProto_Test.txt";
        const string path = @".\Assets\Res\Config\GlobalProto.txt";
        const string pathAppStore = @".\Assets\Res\Config\GlobalProto_AppStore.txt";

        const string installPacketPath = @".\Assets\Res\Config\InstallPacket.txt";

        // private GlobalProto globalProto;

        // 注意顺序
        private List<string> paths = new List<string>() { pathOffline, pathTest, path, pathAppStore };
        private List<GlobalProto> globalProtos;
        private int selectIndex = 0;

        private InstallPacketConfig installPacketConfig;
        private string cacheApkSize;

        [MenuItem("Tools/全局配置")]
        public static void ShowWindow()
        {
            GetWindow<GlobalProtoEditor>();
        }

        public void Awake()
        {
            if (null == globalProtos)
                globalProtos = new List<GlobalProto>();
            else
                globalProtos.Clear();
            for (int i = 0, n = paths.Count; i < n; i++)
            {
                if (File.Exists(paths[i]))
                {
                    globalProtos.Add(JsonHelper.FromJson<GlobalProto>(File.ReadAllText(paths[i])));
                }
                else
                {
                    GlobalProto mGlobalProto = new GlobalProto();
                    mGlobalProto.AssetBundleServerUrl = "http://127.0.0.1:8080/";
                    mGlobalProto.AssetBundleServerUrl = "127.0.0.1:10002";
                    globalProtos.Add(mGlobalProto);
                }
            }

            if (File.Exists(installPacketPath))
            {
                this.installPacketConfig = JsonHelper.FromJson<InstallPacketConfig>(File.ReadAllText(installPacketPath));
                cacheApkSize = this.installPacketConfig.ApkSize.ToString();
            }
            else
            {
                this.installPacketConfig = new InstallPacketConfig();
                this.installPacketConfig.Android = UnityEngine.Application.version;
                this.installPacketConfig.AndroidUrl = "http://127.0.0.1:8080/crazypoker.apk";
                this.installPacketConfig.IOS = UnityEngine.Application.version;
                this.installPacketConfig.IOSUrl = "http://www.baidu.com";
                this.installPacketConfig.Msg = "";
                this.installPacketConfig.ApkName = "crazypoker.apk";
                this.installPacketConfig.ApkSize = 0;
                cacheApkSize = this.installPacketConfig.ApkSize.ToString();
            }
        }

        public void OnGUI()
        {
            selectIndex = EditorGUILayout.Popup(selectIndex, paths.ToArray());
            globalProtos[selectIndex].NetLineSwitchUrl = EditorGUILayout.TextField("Ftell路径:", globalProtos[selectIndex].NetLineSwitchUrl);
            globalProtos[selectIndex].AssetBundleServerUrl = EditorGUILayout.TextField("资源路径:", globalProtos[selectIndex].AssetBundleServerUrl);
            // globalProtos[selectIndex].Address = EditorGUILayout.TextField("服务器地址(无用):", globalProtos[selectIndex].Address);

            if (GUILayout.Button("保存"))
            {
                File.WriteAllText(paths[selectIndex], JsonHelper.ToJson(globalProtos[selectIndex]));
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log($"保存成功 path:{paths[selectIndex]}");
            }

            // if (GUILayout.Button("保存"))
            // {
            //     File.WriteAllText(path, JsonHelper.ToJson(this.globalProto));
            //     AssetDatabase.Refresh();
            //     UnityEngine.Debug.Log($"保存GlobalProto.txt成功 path:{path}");
            // }

            GUILayout.Space(50);
            this.installPacketConfig.Android = EditorGUILayout.TextField("Android:", this.installPacketConfig.Android);
            this.installPacketConfig.AndroidUrl = EditorGUILayout.TextField("AndroidUrl:", this.installPacketConfig.AndroidUrl);
            this.installPacketConfig.IOS = EditorGUILayout.TextField("IOS:", this.installPacketConfig.IOS);
            this.installPacketConfig.IOSUrl = EditorGUILayout.TextField("IOSUrl:", this.installPacketConfig.IOSUrl);
            this.installPacketConfig.Msg = EditorGUILayout.TextField("Msg:", this.installPacketConfig.Msg);
            this.installPacketConfig.ApkName = EditorGUILayout.TextField("ApkName:", this.installPacketConfig.ApkName);
            EditorGUILayout.BeginHorizontal();
            this.cacheApkSize = EditorGUILayout.TextField("ApkSize:", this.cacheApkSize);
            if (GUILayout.Button("选择Apk", GUILayout.Width(100)))
            {
                string path = EditorUtility.OpenFilePanel("选择Apk获取包大小", "", "");
                byte[] bytes = File.ReadAllBytes(path);
                this.cacheApkSize = bytes.Length.ToString();
            }
            EditorGUILayout.EndHorizontal();

            this.installPacketConfig.IOSAppStore = EditorGUILayout.TextField("IOSAppStore:", this.installPacketConfig.IOSAppStore);
            this.installPacketConfig.IsAppStore = EditorGUILayout.Toggle("IsAppStore", this.installPacketConfig.IsAppStore);
            this.installPacketConfig.CheckRes = EditorGUILayout.Toggle("CheckRes", this.installPacketConfig.CheckRes);

            if (GUILayout.Button("保存"))
            {
                this.installPacketConfig.ApkSize = Convert.ToInt32(this.cacheApkSize);
                File.WriteAllText(installPacketPath, JsonHelper.ToJson(this.installPacketConfig));
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log($"保存InstallPacket.txt成功 path:{installPacketPath}");
            }

            if (GUILayout.Button("Copy到Release"))
            {
                string currentDir = System.Environment.CurrentDirectory;
                string desPath = Path.Combine(currentDir, @"..\Release\InstallPacket.txt");
                string srcPath = Path.Combine(currentDir, @"Assets\Res\Config\InstallPacket.txt");
                // FileUtil.CopyFileOrDirectory(installPacketPath, desPath);
                File.Copy(srcPath, desPath, true);
                UnityEngine.Debug.Log($"Copy InstallPacket.txt成功 path:{desPath}");
            }
        }
    }
}
