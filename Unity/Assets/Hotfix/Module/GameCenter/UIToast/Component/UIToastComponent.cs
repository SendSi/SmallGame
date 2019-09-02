using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiToastComponentSystem : AwakeSystem<UIToastComponent>
	{
		public override void Awake(UIToastComponent self)
		{
			self.Awake();
		}
	}

	public class UIToastComponent : UIBaseComponent
	{
		private sealed class ToastData
		{
			public GameObject gameObject;
			public Text textContent;
			public Image imageContent;
			public int state; //0未使用，1使用中
		}

		private ReferenceCollector rc;
        private Transform transToast;

        public void Awake()
		{
			rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			transToast = rc.Get<GameObject>("Toast").transform;

		}

		public override void OnShow(object param)
		{
			// Log.Debug($"Toast OnShow");
			if (null != param)
			{
				string mContent = param as string;
				if (null != mContent)
				{
                    if (mContent == LanguageMgr.mInstance.GetLanguageForKey("error105"))
                        return;
                    GameObject mGameObject = GameObject.Instantiate(transToast.gameObject);
                    mGameObject.transform.SetParent(rc.transform);
                    mGameObject.transform.localPosition = Vector3.zero;
                    mGameObject.transform.localRotation = Quaternion.identity;
                    mGameObject.transform.localScale = Vector3.one;
                    ToastData mToastData = new ToastData();
                    mToastData.gameObject = mGameObject;
                    mToastData.textContent = mGameObject.transform.Find("Text_Toast").GetComponent<Text>();
                    mToastData.imageContent = mGameObject.transform.Find("Image_Toast").GetComponent<Image>();
                    mToastData.state = 1;
                    mToastData.textContent.text = mContent;
                    RectTransform mRectTransform = mToastData.imageContent.transform as RectTransform;
                    RectTransform mTextRectTransform = mToastData.textContent.transform as RectTransform;
                    if (null != mRectTransform)
                    {
                        // float mWidth = getFontlen(mToastData.textContent);
                        float mWidth = mToastData.textContent.preferredWidth;
                        if (mWidth < 1000)
                        {
                            mRectTransform.sizeDelta = new Vector2(mWidth + 80, 100);
                            mTextRectTransform.sizeDelta = new Vector2(mWidth, 100);
                        }
                        else
                        {
                            mRectTransform.sizeDelta = new Vector2(1080, 150);
                            mTextRectTransform.sizeDelta = new Vector2(1000, 150);
                        }

                    }


                    float deltaY = (rc.transform.childCount - 2) * 150f;

                    mToastData.gameObject.transform.localPosition = new Vector3(0, -deltaY, 0);

                    mToastData.gameObject.SetActive(true);
                    mToastData.imageContent.color = new Color(1, 1, 1, 1);
                    mToastData.textContent.color = new Color(1, 1, 1, 1);
                    Sequence mSequence = DOTween.Sequence();
                    mSequence.Append(mToastData.gameObject.transform.DOLocalMoveY(250f-deltaY, 1f).SetEase(Ease.OutQuart));
                    mSequence.AppendInterval(1f);
                    mSequence.Append(mToastData.imageContent.DOFade(0, 0.3f));
                    mSequence.Join(mToastData.textContent.DOFade(0, 0.3f));
                    // mSequence.AppendCallback(() =>
                    // {
                    //     mToastData.state = 0;
                    //     mToastData.gameObject.SetActive(false);
                    //     GameObject.Destroy(mToastData.gameObject);
                    //     mToastData = null;
                    //     CheckPos();
                    // });
                    mSequence.OnComplete(() =>
                    {
                        mToastData.state = 0;
                        mToastData.gameObject.SetActive(false);
                        GameObject.Destroy(mToastData.gameObject);
                        mToastData = null;
                        CheckPos();
                    });
                }
			}
		}

        private void CheckPos()
        {
            for (int i = 0; i < rc.transform.childCount; i++)
            {
                GameObject temp = rc.transform.GetChild(i).gameObject;
                if (temp.activeInHierarchy)
                {
                    temp.transform.DOLocalMoveY(250f - 150f * (i-2), 0.5f);
                }
            }
        }


        public override void OnHide()
		{
			// Log.Debug($"Toast OnHide");
		}

		private int getAllTextWidth(Text text)
		{
			string mContent = text.text;
			text.font.RequestCharactersInTexture(mContent, text.fontSize, text.fontStyle);
			int mAllTextWidth = 0;
			char[] arr = mContent.ToCharArray();
			CharacterInfo info = new CharacterInfo();
			foreach (char c in arr)
			{
				text.font.GetCharacterInfo(c, out info, text.font.fontSize);
				mAllTextWidth += info.advance;
			}

			return mAllTextWidth;
		}

		/// <summary>
		/// 获取文本的绘制长度，不同于text的rectTransform.sizeDelta
		/// </summary>
		/// <param name="str">文本</param>
		/// <returns></returns>
		private int getFontlen(Text text)
		{
			string mContent = text.text;
			float mWidth = 0;
			Font font = text.font;
			font.RequestCharactersInTexture(mContent, text.fontSize, text.fontStyle);
			byte[] arr = Encoding.ASCII.GetBytes(mContent);
			CharacterInfo mCharacterInfo;
			for (int i = 0; i < mContent.Length; i++)
			{
				font.GetCharacterInfo(mContent[i], out mCharacterInfo, text.fontSize);
				mWidth += mCharacterInfo.advance;
				if (arr[i] == 63)
					mWidth += mCharacterInfo.advance / 2f;
			}

			return Mathf.FloorToInt(mWidth);
		}
	}
}
