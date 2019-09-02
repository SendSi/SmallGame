using UnityEngine;
#if LOG_ZSX
using UnityEditor;
#endif

namespace ETModel
{
	public class CanvasConfig: MonoBehaviour
	{
		public string CanvasName;

#if LOG_ZSX
        [SerializeField]
        private TextAsset goRootScript;
        private void Start()
        {
            try
            {
                var tName = this.gameObject.name.Replace("(Clone)", "").Trim() + "Component";
                var tScriptPath = AssetDatabase.FindAssets("t:Script " + tName, new string[] { "Assets" });
                var tObj = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(tScriptPath[0])) as TextAsset;
                goRootScript = tObj;
            }
            catch (System.Exception)
            {
                Debug.Log("淡定...用宏LOG_ZSX");
                throw;
            }

        }
#endif
    }
}
