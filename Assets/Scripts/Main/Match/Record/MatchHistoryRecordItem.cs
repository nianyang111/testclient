using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatchHistoryRecordItem : MonoBehaviour {
    public Text dateText;
    public Text typeText;
    public Text rankText;
    public Transform arrow;
    public GameObject desPanel,closeBtn;
    public Text desText;
    public Button itemBtn, circlesBtn, weChatBtn;
    public MatchHistoryRecordPanel panel;
    MatchHistoryRecordData _data;
    public void Init(MatchHistoryRecordData data)
    {
        _data = data;
        //dateText.text = _data.date;
        typeText.text = _data.type;
        rankText.text = string.Format("第" + _data.rank + "名");
        desText.text=string .Format("实力超强，您共计淘汰"+_data.eliminate+"位选手");
        UGUIEventListener.Get(closeBtn).onClick = delegate { panel.CurItem = this; };
        closeBtn.transform.localScale = new Vector2(Screen.width, Screen.height);
        circlesBtn.onClick.AddListener(delegate {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            OnShare(SDKManager.WechatShareScene.WXSceneTimeline); 
        });
        weChatBtn.onClick.AddListener(delegate {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            OnShare(SDKManager.WechatShareScene.WXSceneSession); 
        });
        itemBtn.onClick.AddListener(delegate
        {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            ItemOnClick();
        });
        desPanel.SetActive(false);
    }
    /// <summary>
    /// 分享
    /// </summary>
    /// <param name="type"></param>
    void OnShare(SDKManager.WechatShareScene type)
    {
        Sprite icon = BundleManager.Instance.GetSprite("task/meirirenwu_pic_1");
        SDKManager.Instance.ShareWebPage(type, 
            UserInfoModel.userInfo.downUrl, 
            "雪瑶明水棋牌", 
            string.Format("在雪瑶明水棋牌的" + _data.type + "中我共计淘汰" + _data.eliminate + "位选手，快来跟我一起玩吧"), 
            MiscUtils.SizeTextureBilinear(icon.texture, Vector2.one * 150).EncodeToJPG());
    }
    private void ItemOnClick()
    {
        if (!AudioManager.Instance.IsSoundPlaying)
            AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
        panel.CurItem = this;
    }
    public void Select()
    {
        desPanel.SetActive(true);
        desPanel.transform.SetParent(panel.show);
        arrow.localEulerAngles = new Vector3(0, 0, -90);
    }
    public void OnSelect()
    {
        desPanel.SetActive(false);
        desPanel.transform.SetParent(this.transform);
        arrow.localEulerAngles = new Vector3(0, 0, 0);
    }
}
public class MatchHistoryRecordData
{
    public long date;
    public string type;
    public int rank;
    public int eliminate;
}
