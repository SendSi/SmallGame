using UnityEngine;
using UnityEngine.UI;
namespace ETModel
{

    public class LanguageText : MonoBehaviour
    {
        public string mKey;
        private Text textLbl;
        /// <summary>除登录页=false               其他都=true         </summary>
        public bool mUseAwake = true;

        private void Awake()
        {
            if (mUseAwake)
            {
                LoadContent();
            }
        }

        private void Start()
        {
            if (mUseAwake == false)
                LoadContent();
        }

        void LoadContent()
        {
            if (string.IsNullOrEmpty(mKey)) return;
            textLbl = this.GetComponent<Text>();
            if (textLbl == null) { Debug.LogError("没Text 组件呀~~"); return; }
            textLbl.text = LanguageMgr.mInstance.GetLanguageForKey(mKey);
            LanguageMgr.mInstance.EActTextChange += EventActionChangeTxt;//加上监听
        }


        private void EventActionChangeTxt()
        {
            if (textLbl != null)
                textLbl.text = LanguageMgr.mInstance.GetLanguageForKey(mKey);
        }

        public void OnDestroy()
        {
            LanguageMgr.mInstance.EActTextChange -= EventActionChangeTxt;
        }

    }

}
