using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkView : MonoBehaviour
{

    public GameObject wenziObj;
    public Text chatText;
    public RectTransform wenziBg;

    public GameObject bqObj;
    public GameObject yuyinObj;

    public SequenceAnimation bq;

    public void Chat(string value, int type)
    {
        gameObject.SetActive(true);
        if (type == 0)//文字
        {
            wenziObj.SetActive(true);
            bqObj.SetActive(false);
            yuyinObj.SetActive(false);
            chatText.text = value;
            wenziBg.sizeDelta = new Vector2(wenziBg.sizeDelta.x, chatText.preferredHeight + 50);
        }
        else if (type == 1)//语音
        {
            bqObj.gameObject.SetActive(false);
            wenziObj.SetActive(false);
            yuyinObj.SetActive(true);
        }
        else if (type == 2)//表情
        {
            bqObj.gameObject.SetActive(true);
            wenziObj.SetActive(false);
            yuyinObj.SetActive(false);
            bq.SpriteFrames.Clear();
            if (PageManager.Instance.CurrentPage is LandlordsPage)
                bq.SpriteFrames.AddRange(BundleManager.Instance.GetAnimationSprites(value, LandlordsPage.Instance.animations));
            else if (PageManager.Instance.CurrentPage is MaJangPage)
                bq.SpriteFrames.AddRange(BundleManager.Instance.GetAnimationSprites(value, MaJangPage.Instance.animations));
            bqObj.SetActive(true);
            bq.Loop = true;
            bq.Rewind();
        }
        StopCoroutine("SetTalkShow");
        StartCoroutine("SetTalkShow");
    }

    IEnumerator SetTalkShow()
    {
        yield return new WaitForSecondsRealtime(3);
        gameObject.SetActive(false);
    }
}
