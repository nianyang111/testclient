using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatView : MonoBehaviour {

    public int ratio = 300;
    public int limit = 50;
    public GameObject canTalkObj;
    public GameObject canNotObj;

    public Transform parent;

    /// <summary>
    /// 打字相关
    /// </summary>
    public GameObject daziObj;
    public InputField input;
    public GameObject sendBtn;
    public GameObject to_voiceBtn;

    /// <summary>
    /// 语音相关
    /// </summary>
    public GameObject yuyinObj;
    public GameObject recordBtn;
    public GameObject to_inputBtn;
    public GameObject yuyinTipsObj;
    public Text yuyinTipsText;

    public GameObject prefab;

    void Start()
    {
        UGUIEventListener.Get(sendBtn).onClick = (g) => SendBtn();
        UGUIEventListener.Get(to_voiceBtn).onClick = (g) => OpenYuyin();
        UGUIEventListener recordListener = UGUIEventListener.Get(recordBtn.gameObject);
        recordListener.onDown = delegate { DownYuyin(); };
        recordListener.onUp = delegate { OnUp(); };
        UGUIEventListener.Get(to_inputBtn).onClick = (g) => OpenDazi();

        bool isRoomCardRoom = false;
        if (PageManager.Instance.CurrentPage is LandlordsPage)
            isRoomCardRoom = LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard;
        else if (PageManager.Instance.CurrentPage is MaJangPage)
            isRoomCardRoom = MaJangPage.Instance.roomType == RoomType.RoomCard;
        canTalkObj.SetActive(isRoomCardRoom);
        canNotObj.SetActive(!isRoomCardRoom);

        OpenDazi();

    }


    public void LoadChatItem(ChatInfo info)
    {
        GameObject go = null;
        if (parent.childCount < 50)
            go = Instantiate(prefab, parent);
        else
            go = parent.GetChild(0).gameObject;
        go.GetComponent<GameChatItem>().Inits(info);        
    }
    

    void SendChatMessage(string text,int type)
    {
        ChatInfo info = new ChatInfo();
        info.text = text;
        info.type = type;
        NodeManager.GetNode<ChatNode>().SendMessage(info, false);
    }

    /// <summary>
    /// 打开语音界面
    /// </summary>
    void OpenYuyin()
    {
        yuyinObj.SetActive(true);
        daziObj.SetActive(false);
    }

    /// <summary>
    /// 打开打字界面
    /// </summary>
    void OpenDazi()
    {
        yuyinObj.SetActive(false);
        daziObj.SetActive(true);
    }

    #region 打字
    /// <summary>
    /// 发送消息
    /// </summary>
    void SendBtn()
    {
        string text = input.text;

        if (string.IsNullOrEmpty(text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请输入消息内容");
            return;
        }
        if (System.Text.Encoding.UTF8.GetBytes(text.ToCharArray()).Length > limit * 3)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "消息长度不能超过" + limit + "个字符!");
            return;
        }
        if (BlockWordModel.CheckIsBlock(text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "消息中含有敏感词汇!");
            return;
        }
        SendChatMessage(text, 0);
        input.text = string.Empty;
    }
    #endregion


    #region 语音

    Vector3 startClickPos;
    Vector3 endClickPos;

    /// <summary>
    /// 按下语音
    /// </summary>
    void DownYuyin()
    {
        startClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        yuyinTipsObj.SetActive(true);
        yuyinTipsText.text = "手指上滑,取消发送";
        yuyinTipsText.color = Color.white;
        GVoice.Instance.Click_GetRecFileParam();
        GVoice.Instance.Click_btnReqAuthKey();
        GVoice.Instance.Click_btnStartRecord();        
    }
    /// <summary>
    /// 抬起语音
    /// </summary>
    void OnUp()
    {
        endClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (endClickPos.y > startClickPos.y && endClickPos.y - startClickPos.y > 100)
        {//取消发送
            TipManager.Instance.OpenTip(TipType.SimpleTip, "取消发送");
            yuyinTipsObj.SetActive(false);
            GVoice.Instance.Click_btnStopRecord();
        }
        else
        {
            yuyinTipsObj.SetActive(false);
            GVoice.Instance.Click_btnStopRecord();
            GVoice.Instance.Click_btnUploadFile(filed =>
                {
                    SendChatMessage(filed, 1);
                });
        }
    }



    #endregion   
}






