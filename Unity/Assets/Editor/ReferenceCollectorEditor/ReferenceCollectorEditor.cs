using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ETModel;
using NPOI.SS.Formula.PTG;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ReferenceCollector))]
[CanEditMultipleObjects]
public class ReferenceCollectorEditor : Editor
{
    private Dictionary<string, string> uiComponentType = new Dictionary<string, string>()
    {
            {"Text", "Text" },
            {"Image", "Image" },
            {"RawImage", "RawImage" },
            {"Button", "Button" },
            {"Toggle", "Toggle" },
            {"Slider", "Slider" },
            {"Scrollbar", "Scrollbar" },
            {"Dropdown", "Dropdown" },
            {"InputField", "InputField" },
            {"Canvas", "Canvas" },
            {"Panel", "Transform" },
            {"ScrollView", "ScrollRect" },
            {"RC", "ReferenceCollector" },
            {"Armature", "UnityArmatureComponent" }
    };

    private string searchKey
    {
        get
        {
            return _searchKey;
        }
        set
        {
            if (_searchKey != value)
            {
                _searchKey = value;
                heroPrefab = referenceCollector.Get<Object>(searchKey);
            }
        }
    }

    private ReferenceCollector referenceCollector;

    private Object heroPrefab;

    private string _searchKey = "";

    private void DelNullReference()
    {
        var dataProperty = serializedObject.FindProperty("data");
        for (int i = dataProperty.arraySize - 1; i >= 0; i--)
        {
            var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
            if (gameObjectProperty.objectReferenceValue == null)
            {
                dataProperty.DeleteArrayElementAtIndex(i);
            }
        }
    }

    private void OnEnable()
    {
        referenceCollector = (ReferenceCollector)target;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(referenceCollector, "Changed Settings");
        var dataProperty = serializedObject.FindProperty("data");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("添加引用"))
        {
            AddReference(dataProperty, Guid.NewGuid().GetHashCode().ToString(), null);
        }
        if (GUILayout.Button("全部删除"))
        {
            dataProperty.ClearArray();
        }
        if (GUILayout.Button("删除空引用"))
        {
            DelNullReference();
        }
        if (GUILayout.Button("排序"))
        {
            referenceCollector.Sort();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("快速命名"))
        {
            foreach (var item in referenceCollector.data)
            {
                item.key = item.gameObject.name;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("生成变量声明"))
        {
            int mSuccess = 0;
            int mError = 0;
            int mErrorName = 0;
            StringBuilder build = new StringBuilder();

            build.AppendLine($"private ReferenceCollector rc;");

            string[] mCache = null;
            foreach (var item in referenceCollector.data)
            {
                if (item.gameObject == null)
                    continue;
                mCache = item.gameObject.name.Split('_');
                if (mCache.Length == 2)
                {
                    mCache[0] = mCache[0].Replace(" ", "");
                    string mType = null;
                    foreach (var keyval in this.uiComponentType)
                    {
                        if (mCache[0] == keyval.Key)
                        {
                            mType = keyval.Value;
                            break;
                        }
                    }

                    if (mType == null)
                    {
                        mError++;
                        build.AppendLine($"private Transform trans{mCache[1]};");
                        Debug.Log($"注意：{item.gameObject.name} 生成UI变量声明失败");
                        // break;
                        continue;
                    }

                    mSuccess++;
                    build.AppendLine($"private {mType} {mCache[0].ToLower()}{mCache[1]};");
                }
                else if (mCache.Length == 1)
                {
                    build.AppendLine($"private Transform trans{mCache[0]};");
                }
                else
                {
                    mErrorName++;
                    Debug.Log($"{item.gameObject.name} 命名不规范。请按照如下命名：Type_Name，Button_Login");
                }
            }
            ClipboardHelper.ClipBoard = build.ToString();
            Debug.Log($"{referenceCollector.gameObject.name} 生成变量声明 -> 成功：{mSuccess}，失败：{mError}，命名错误：{mErrorName}    |    已复制正确部分的代码!");
        }

        if (GUILayout.Button("生成获取代码"))
        {
            StringBuilder build = new StringBuilder();

            build.AppendLine($"rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();");

            string[] mCache = null;
            foreach (var item in referenceCollector.data)
            {
                mCache = item.gameObject.name.Split('_');
                if (mCache.Length == 2)
                {
                    mCache[0] = mCache[0].Replace(" ", "");
                    string mType = null;
                    foreach (var keyval in this.uiComponentType)
                    {
                        if (mCache[0] == keyval.Key)
                        {
                            mType = keyval.Value;
                            break;
                        }
                    }

                    if (mType == null)
                    {
                        // Debug.Log($"注意：{item.gameObject.name} 生成获取代码失败");
                        build.AppendLine($"trans{mCache[1]} = rc.Get<GameObject>(\"{item.key}\").transform;");
                        // break;
                        continue;
                    }

                    build.AppendLine($"{mCache[0].ToLower()}{mCache[1]} = rc.Get<GameObject>(\"{item.key}\").GetComponent<{mType}>();");
                }
                else if (mCache.Length == 1)
                {
                    build.AppendLine($"trans{mCache[0]} = rc.Get<GameObject>(\"{item.key}\").transform;");
                }
                else
                {
                    Debug.Log($"{item.gameObject.name} 命名不规范。请按照如下命名：Type_Name，Button_Login");
                }
            }

            ClipboardHelper.ClipBoard = build.ToString();

            Debug.Log("复制获取代码成功!");
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        searchKey = EditorGUILayout.TextField(searchKey);
        EditorGUILayout.ObjectField(heroPrefab, typeof(Object), false);
        if (GUILayout.Button("删除"))
        {
            referenceCollector.Remove(searchKey);
            heroPrefab = null;
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        var delList = new List<int>();
        // SerializedProperty property;
        for (int i = referenceCollector.data.Count - 1; i >= 0; i--)
        {
            GUILayout.BeginHorizontal();
            referenceCollector.data[i].key = EditorGUILayout.TextField(referenceCollector.data[i].key, GUILayout.Width(150));
            referenceCollector.data[i].gameObject = EditorGUILayout.ObjectField(referenceCollector.data[i].gameObject, typeof(Object), true);

            if (GUILayout.Button("B"))
            {
                var tStrKey = referenceCollector.data[i].key;
                string tCopy = string.Format("UIEventListener.Get(rc.Get<GameObject>(\"{0}\")).onClick = Click{0};", tStrKey);
                ClipboardHelper.ClipBoard = tCopy;
            }
            if (GUILayout.Button("C"))//添加简单粗暴单项的copy.....防止手写字符串写错
            {
                var tStrKey = referenceCollector.data[i].key;
                if (tStrKey.Contains("text_") || tStrKey.Contains("Text_"))
                {
                    var tName = tStrKey.Split('_')[1];
                    string tCopy = string.Format("private Text text{0};\ntext{0} = rc.Get<GameObject>(\"{1}\").GetComponent<Text>();", tName, tStrKey);
                    ClipboardHelper.ClipBoard = tCopy;
                }
                else if (tStrKey.ToLower().Contains("inputfield_"))
                {
                    var tName = tStrKey.Split('_')[1];
                    string tCopy = string.Format("private InputField inputfield{0};\ninputfield{0} = rc.Get<GameObject>(\"{1}\").GetComponent<InputField>();", tName, tStrKey);
                    ClipboardHelper.ClipBoard = tCopy;
                }
                else
                {
                    string tGoName = string.Format("private GameObject {0};\n{0} = rc.Get<GameObject>(\"{0}\");", tStrKey);
                    ClipboardHelper.ClipBoard = tGoName;
                }
            }

            if (GUILayout.Button("X"))
            {
                delList.Add(i);
            }
            GUILayout.EndHorizontal();
        }
        var eventType = Event.current.type;
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            // Show a copy icon on the drag
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var o in DragAndDrop.objectReferences)
                {
                    AddReference(dataProperty, o.name, o);
                }
            }

            Event.current.Use();
        }
        foreach (var i in delList)
        {
            dataProperty.DeleteArrayElementAtIndex(i);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    private void AddReference(SerializedProperty dataProperty, string key, Object obj)
    {
        int index = dataProperty.arraySize;
        dataProperty.InsertArrayElementAtIndex(index);
        var element = dataProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("key").stringValue = key;
        element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
    }
}