using LitJson;
using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MessagePanel : MonoBehaviour {

    public Transform parent;
    public GameObject playerPrefab;
    public GameObject systemPrefab;
    public ChatPanel chatPanel;
    //public void OnGUI()
    //{
    //    if (GUILayout.Button("加载"))
    //    {
    //        UIUtils.DestroyChildren(parent);
    //        for (int i = 0; i < 50; i++)
    //        {
    //            MessagePanelInfo info = new MessagePanelInfo();
    //            info.messageType = Random.Range(0, 2);
    //            info.name = "名字+i";
    //            info.id = i;
    //            info.six = Random.Range(0, 2);
    //            info.timer = "1427";
    //            info.text = "这里是内容哦~~~~" + i;
    //            LoadItem(info);
    //        }
    //    }

    //}

    void OnEnable()
    {
        ReqAddFriendMessage();
    }

    void Start()
    {
        LoadChatLogs();
        ReqNoOnlineMessage();        
        SocialModel.Instance.onReceiveMessage += G2C_Chat;
    }

    /// <summary>
    /// 加载本地最近聊天的人
    /// </summary>
    void LoadChatLogs()
    {
        UIUtils.DestroyChildren(parent);
        if (!File.Exists(ConstantUtils.chatConfigPath))
            return;
        string text = File.ReadAllText(ConstantUtils.chatConfigPath);
        JsonData json = JsonMapper.ToObject(text);

        foreach (var item in json.Keys)
        {
            MessagePanelInfo info = new MessagePanelInfo();
            info.messageType = 0;
            info.name = json[item]["name"].ToString();
            info.headIcon = json[item]["headIcon"].ToString();

            info.id = int.Parse(item);
            info.six = int.Parse(json[item]["six"].ToString());
            long Timer= long.Parse(json[item]["history"][json[item]["history"].Count - 1]["timer"].ToString());
            info.timer = Timer;
            info.text = json[item]["history"][json[item]["history"].Count - 1]["text"].ToString();
            info.type = int.Parse(json[item]["history"][json[item]["history"].Count - 1]["type"].ToString());
            LoadItem(info);
        }
    }

    /// <summary>
    /// 请求离线消息
    /// </summary>
    void ReqNoOnlineMessage()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_QueryFriendOfflineChat
        });
    }

    /// <summary>
    /// 请求加好友消息
    /// </summary>
    public static void ReqAddFriendMessage()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_QueryApplicantsReq
        });
    }

    void G2C_Chat(ChatInfo info)
    {
        MessagePanelItem item = getMessageItem(info);
        if (item)
        {
            item.RefreshNowMessage(info.type, info.text, info.chatTime);
        }
        else
        {
            //如果之前没有
            MessagePanelInfo infos = new MessagePanelInfo();
            infos.name = info.chatWithName;
            infos.id = info.chatWithId;
            infos.six = info.playerBaseInfo.six == Six.boy ? 0 : 1;

            infos.timer = info.chatTime;
            infos.text = info.text;
            infos.headIcon = info.playerBaseInfo.icon;
            infos.type = info.type;
            infos.messageType = 0;
            LoadItem(infos);
        }
    }    


    void LoadItem(MessagePanelInfo info)
    {
        MessagePanelItem item = null;
        if (info.messageType == 0)
        {
            item = Instantiate(playerPrefab, parent).GetComponent<MessagePanelItem>();
        }
        else
        {
            item = Instantiate(systemPrefab, parent).GetComponent<MessagePanelItem>();
        }
        item.Inits(info, OnClickCall);
        item.transform.SetAsFirstSibling();
    }

    MessagePanelItem getMessageItem(ChatInfo info)
    {
        MessagePanelItem[] items = parent.GetComponentsInChildren<MessagePanelItem>();
        //如果之前有
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].info.id == info.chatWithId)
                return items[i];
        }
        return null;
    }

    public void OnClickCall(FriendInfo info)
    {
        chatPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);

        ChatInfo chatInfo = new ChatInfo();
        chatInfo.playerBaseInfo.six = info.gender==0?Six.boy:Six.girl;
        //chatInfo.text=info.;
        //chatInfo.timer = int.Parse(info.timer);
        chatInfo.playerBaseInfo.icon= info.photo;
        //chatInfo.senderId=info.id;
        chatInfo.chatWithId = info.userId;
        chatInfo.chatWithName = info.nickname;
        chatInfo.playerBaseInfo.userNickname = info.nickname;
        chatInfo.playerBaseInfo.six = info.gender == 0 ? Six.boy : Six.girl;
        chatInfo.playerBaseInfo.exp = info.exp;
        chatInfo.playerBaseInfo.lv = info.level;
        chatInfo.playerBaseInfo.uid = info.userId.ToString();
        chatInfo.playerBaseInfo.money = info.sliver;
        //info.playerBaseInfo.vip = chatResp.friendInfo.;
        chatPanel.Inits(chatInfo);
    }

    /// <summary>
    /// 服务器推送最近联系人离线消息列表
    /// </summary>
    public void G2C_RecentContacts(List<FriendOfflineChat> infos)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            for (int j = 0; j < infos[i].chatContent.Count; j++)
            {
                FriendChatResp resp = new FriendChatResp();
                resp.friendInfo = infos[i].friendInfo;
                resp.content = infos[i].chatContent[j];
                SocialModel.Instance.ReceiveMessage(resp);    
            }            
        }
    }

    /// <summary>
    /// 服务器推送所有加好友列表
    /// </summary>
    public void G2C_AddFriends(List<FriendInfo> infos)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            MessagePanelItem item = parent.GetChild(i).GetComponent<MessagePanelItem>();
            if (item.info.messageType == 1)
                Destroy(item.gameObject);
        }
        for (int i = 0; i < infos.Count; i++)
        {
            MessagePanelInfo info = new MessagePanelInfo();
            info.messageType = 1;
            info.name =infos[i].nickname;
            info.id = infos[i].userId;
            info.six = infos[i].gender;
            //DateTime time=MiscUtils.GetDateTimeByTimeStamp(infos[i].)
            info.timer = MiscUtils.GetTimeStamp(DateTime.Now) * 1000;
            LoadItem(info);
        }
    }

    /// <summary>
    /// 添加一条最近联系
    /// </summary>
    public void AddMessage(ChatInfo info)
    {
        G2C_Chat(info);        
    }

    /// <summary>
    /// 删除一条最近联系
    /// </summary>
    public void RemoveMessage(ChatInfo info)
    {
        MessagePanelItem item = getMessageItem(info);
        if (item)
        {
            item.Remove();
        }
    }

    void OnDestroy()
    {
        SocialModel.Instance.onReceiveMessage -= G2C_Chat;
    }

    public class MessagePanelInfo
    {
        public string headIcon;
        public string name;
        public int id;
        public int six;//0男1女
        public int messageType;//0玩家1系统  
        public int type;//0打字1语音2表情
        public string text;//最新一条消息内容
        public long timer;//最新一条消息时间
    }
}
