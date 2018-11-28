using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameChatItem : MonoBehaviour {

    public Text yuyin_nameLb;
    public GameObject yuyinObj;
    public GameObject yuyinBtn;

    public GameObject daziObj;
    public Text daziValue;

    ChatInfo curInfo;
    string recordRes;
    public void Inits(ChatInfo info)
    {
        curInfo = info;        
        if (info.type == 0)
        {//打字
            daziObj.SetActive(true);
            yuyinObj.SetActive(false);
            daziValue.text = string.Format("[{0}]:{1}", info.chatWithName, info.text);

            (transform as RectTransform).sizeDelta = new Vector2(daziValue.rectTransform.sizeDelta.x, daziValue.preferredHeight);
            daziValue.rectTransform.sizeDelta = new Vector2(daziValue.rectTransform.sizeDelta.x, daziValue.preferredHeight);
            daziValue.rectTransform.SetAsLastSibling();
        }
        else
        {//语音
            recordRes = info.text;
            yuyin_nameLb.text = info.chatWithName + ":";
            UGUIEventListener.Get(yuyinBtn).onClick = delegate { SocialModel.Instance.PlayRecord(recordRes); };//播放语音
            daziObj.SetActive(false);
            yuyinObj.SetActive(true);
        }
    }
}
