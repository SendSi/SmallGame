using UnityEngine;
using UnityEngine.UI;

public class LanguageSprite : MonoBehaviour
{

    private Image mImg;
    /// <summary>   0简中文版  1英文版  2繁文版       </summary>
    public Sprite[] mSprite;
    /// <summary>    /// 是否原尺寸    /// </summary>
    public bool mNativeSize;


    private void Start()
    {
        mImg = this.GetComponent<Image>();
        if (mImg == null) { Debug.LogError("没有Image组件呀"); return; }
        EventActionChangeSprite();
        LanguageMgr.mInstance.EActTextChange += EventActionChangeSprite;//加上监听
    }

    private void EventActionChangeSprite()
    {
        if (mImg != null)
        {
            var pLanguage = LanguageMgr.mInstance.mCurLanguage;
            if (mSprite.Length > pLanguage)
            {
                mImg.sprite = mSprite[pLanguage];
            }

            if (mNativeSize)
                mImg.SetNativeSize();
        }
    }

    public void OnDestroy()
    {
        LanguageMgr.mInstance.EActTextChange -= EventActionChangeSprite;
    }
}
