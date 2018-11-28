using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class ChatPanel : MonoBehaviour {

    public UGUIEventListener exitBtn;
    public Text nameLb;
    public Dropdown dropDown;
    public RectTransform parent;
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public MessagePanel messagePanel;

    public GameObject daziObj;
    public UGUIEventListener yuyinBtn;//打字切换语音按钮
    public InputField input;
    public UGUIEventListener daziSendBtn;

    public GameObject yuyinObj;
    public UGUIEventListener daziBtn;//语音切换打字按钮
    public UGUIEventListener put_yuyinBtn;//按住发语音按钮
    public GameObject yuyinTipsObj;
    public Text yuyinTipsText;
    public Text yuyinDesText;
    public RoleInfoView roleInfoView;
    ChatInfo curInfo;

    void Start()
    {
        SocialModel.Instance.onReceiveMessage += G2C_Chat;
        dropDown.ClearOptions();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        list.Add(new Dropdown.OptionData("查看好友详情"));
        list.Add(new Dropdown.OptionData("清除聊天记录"));
        list.Add(new Dropdown.OptionData(""));
        dropDown.AddOptions(list);
        dropDown.onValueChanged.AddListener(OnValueChange);
        dropDown.value = 2;        
    }

    public void Inits(ChatInfo info)
    {
        SocialModel.Instance.curChatInfo = info;
        if (exitBtn.onClick == null)
            exitBtn.onClick = delegate { Exit(); };

        if (yuyinBtn.onClick == null)
            yuyinBtn.onClick = delegate { OpenYuyin(); };
        if (daziSendBtn.onClick == null)
            daziSendBtn.onClick = delegate { SendDazi(); };

        if (daziBtn.onClick == null)
            daziBtn.onClick = delegate { OpenDazi(); };

        if (put_yuyinBtn.onDown == null)
            put_yuyinBtn.onDown = delegate { DownYuyin(); };
        if (put_yuyinBtn.onUp == null)
            put_yuyinBtn.onUp = delegate { OnUp(); };

        this.curInfo = info;
        nameLb.text = info.playerBaseInfo.userNickname;

        OpenDazi();
        LoadChatLog();        
    }

    /// <summary>
    /// 在messagePanel里增加一条聊天记录
    /// </summary>
    void AddLogMessagePanel()
    {
        messagePanel.AddMessage(curInfo);
    }

    /// <summary>
    /// 在messagePanel里去掉一条聊天记录
    /// </summary>
    void RemoveLogMessagePanel()
    {
        messagePanel.RemoveMessage(curInfo);
    }

    #region 主聊天界面

    void OnValueChange(int index)
    {
        if (index == 0)
        {
            roleInfoView.SetVisibel(true);
            roleInfoView.Init(curInfo.playerBaseInfo, true);
        }
        else if (index == 1)
        {
            UIUtils.DestroyChildren(parent);
            RemoveLogMessagePanel();
            Exit();
            SocialModel.Instance.ClearChatLog(curInfo.chatWithId.ToString());
        }
        dropDown.value = 2;
    }

    void LoadChatLog()
    {
        UIUtils.DestroyChildren(parent);
        if (File.Exists(ConstantUtils.chatConfigPath))
        {
            JsonData jd = JsonMapper.ToObject(File.ReadAllText(ConstantUtils.chatConfigPath));
            string logStr = jd.TryGetString(curInfo.chatWithId.ToString());            
            if (!string.IsNullOrEmpty(logStr))
            {
                logStr = jd[curInfo.chatWithId.ToString()].ToJson();
                JsonData chatData = JsonMapper.ToObject(logStr);
                JsonData historyJson = chatData["history"];
                for (int i = 0; i < historyJson.Count; i++)
                {
                    ChatInfo info = JsonMapper.ToObject<ChatInfo>(JsonMapper.ToJson(historyJson[i]));
                    //JsonData chatInfoJson = historyJson[i]; print(int.Parse(historyJson[i]["sender"].ToJson().ToString()));
                    //ChatInfo info = new ChatInfo();
                    //info.senderId = (int)(float.Parse(historyJson[i]["sender"].ToJson()));
                    //info.text = historyJson[i]["value"].ToJson();
                    //info.timer = (int)(float.Parse(historyJson[i]["time"].ToJson()));
                    //info.type = (int)(float.Parse(historyJson[i]["type"].ToJson()));
                    LoadChatItem(info);
                }
            }
        }

    }

    /// <summary>
    /// 退出聊天
    /// </summary>
    void Exit()
    {
        SetVisible(false);
        messagePanel.gameObject.SetActive(true);
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
    public int ratio = 300;
    void LoadChatItem(ChatInfo info)
    {        
        ChatInfoItem item = null;
        if (info.senderId == UserInfoModel.userInfo.userId)
        {//左
            item = Instantiate(rightPrefab, parent).GetComponent<ChatInfoItem>();
        }
        else
        {//右
            item = Instantiate(leftPrefab, parent).GetComponent<ChatInfoItem>();
        }
        item.Inits(info);
        if (parent.sizeDelta.y > 480)
            parent.localPosition = new Vector3(parent.localPosition.x, ((RectTransform)parent).rect.height - ratio + (item.transform as RectTransform).sizeDelta.y, parent.localPosition.z);
    }

    public void SetVisible(bool isShow)
    {
        SocialModel.Instance.curChatInfo = null;
        gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 服务器推送玩家发送聊天消息
    /// </summary>
    void G2C_Chat(ChatInfo info)
    {
        if (info.chatWithId == curInfo.chatWithId)
            LoadChatItem(info);
    }

    

    #endregion

    #region 打字
    /// <summary>
    /// 发送打字
    /// </summary>
    void SendDazi()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请输入聊天内容");
            return;
        }
        if (Encoding.UTF8.GetByteCount(input.text) > 150)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "聊天内容过长");
            return;
        }
        if (BlockWordModel.CheckIsBlock(input.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "文字违规!");
            return;
        }
        SocialModel.Instance.SendMessage(curInfo.chatWithId, input.text, 0);

        ////测试
        //UserInfoModel.userInfo.userId = 0;
        //ChatInfo info = new ChatInfo();
        //info.senderId = Random.Range(0, 2);
        //info.chatWithId = curInfo.chatWithId;
        //info.type = 0;
        //info.text = input.text;
        //SocialModel.Instance.ReceiveMessage(info);

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
        yuyinTipsText.text = "手指上滑\n取消发送";
        yuyinTipsText.color = Color.white;
        yuyinDesText.text = "松开 结束";
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
            yuyinTipsObj.SetActive(false);
            yuyinDesText.text = "按下 说话";
            TipManager.Instance.OpenTip(TipType.SimpleTip, "取消发送");
            GVoice.Instance.Click_btnStopRecord();
        }
        else
        {//直接发送
            yuyinTipsObj.SetActive(false);
            yuyinDesText.text = "按下 说话";
            GVoice.Instance.Click_btnStopRecord();
            GVoice.Instance.Click_btnUploadFile(filed =>
                {
                    SocialModel.Instance.SendMessage(curInfo.chatWithId, filed, 1);
                });
        }
    }


    #endregion   


    void OnDestroy()
    {
        SocialModel.Instance.onReceiveMessage -= G2C_Chat;
    }
}
