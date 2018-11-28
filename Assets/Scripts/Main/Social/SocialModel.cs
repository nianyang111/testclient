using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SocialModel {

    static SocialModel instance = new SocialModel();

    public static SocialModel Instance
    {
        get { return instance; }
    }

    public ChatInfo curChatInfo;//当前我和谁在聊天

    public bool isHaveNewMessage = false;
    /// <summary>
    /// 添加好友
    /// </summary>
    /// <param name="userId"></param>
    public void AddFriend(int userId)
    {
        if (userId == UserInfoModel.userInfo.userId)
        {
            TipManager.Instance.OpenTip(TipType.AlertTip, "不能加自己为好友!");
            return;
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                applyFriendReq = new ApplyFriendReq()
                {
                    userId = userId
                },
                msgid = MessageId.C2G_ApplyFriendReq
            });
    }

    /// <summary>
    /// 删除好友
    /// </summary>
    /// <param name="userId"></param>
    public void DeleteFriend(int userId)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            removeFriendReq = new RemoveFriendReq()
            {
                userId = userId
            },
            msgid = MessageId.C2G_RemoveFriendReq
        });
        ClearChatLog(userId.ToString());
    }

    /// <summary>
    /// 得到与好友的状态
    /// </summary>
    /// <param name="relation"></param>
    /// <returns></returns>
    public FriendApplyState getFriendState(int relation)
    {
        switch (relation)
        {
            case 1:
                return FriendApplyState.MeAppling;
            case 2:
                return FriendApplyState.Friending;
            default:
                return FriendApplyState.Normal;
        }
    }

    public CallBack<ChatInfo> onReceiveMessage;

    /// <summary>
    /// 发送好友聊天消息  0文字 1语音
    /// </summary>
    /// <param name="ToUserId"></param>
    /// <param name="message"></param>
    /// <param name="Type"></param>
    public void SendMessage(int ToUserId, string message, int Type)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                friendChatReq = new FriendChatReq()
                {
                    userId = ToUserId,
                    chatContent = new ChatContent()
                    {
                        type = Type,
                        content = message
                    }
                },
                msgid = MessageId.C2G_FriendChatReq
            });
    }

    /// <summary>
    /// 接受好友聊天消息
    /// </summary>
    /// <param name="chatResp"></param>
    public void ReceiveMessage(FriendChatResp chatResp)
    {
        ChatInfo info = new ChatInfo()
        {
            senderId = chatResp.friendInfo.userId,
            chatWithId = chatResp.friendInfo.userId == UserInfoModel.userInfo.userId ? curChatInfo.chatWithId : chatResp.friendInfo.userId,
            chatWithName = chatResp.friendInfo.nickname == UserInfoModel.userInfo.nickName ? curChatInfo.chatWithName : chatResp.friendInfo.nickname,

        };
        info.playerBaseInfo.userNickname = chatResp.friendInfo.nickname;
        info.playerBaseInfo.icon = chatResp.friendInfo.photo;
        info.playerBaseInfo.six = chatResp.friendInfo.gender == 0 ? Six.boy : Six.girl;
        info.playerBaseInfo.exp = chatResp.friendInfo.exp;
        info.playerBaseInfo.lv = chatResp.friendInfo.level;
        info.playerBaseInfo.uid = chatResp.friendInfo.userId.ToString();
        info.playerBaseInfo.money = chatResp.friendInfo.sliver;
        //info.playerBaseInfo.vip = chatResp.friendInfo.;
        info.type = chatResp.content.type;
        info.text = chatResp.content.content;
        info.chatTime = chatResp.content.date;
        if (onReceiveMessage != null)
            onReceiveMessage(info);
        ChatLog(info);
    }

    /// <summary>
    /// 播放语音
    /// </summary>
    /// <param name="str"></param>
    public void PlayRecord(string recordRes)
    {
        GVoice.Instance.Click_btnDownloadFile(recordRes, () =>
            {
                GVoice.Instance.Click_btnPlayReocrdFile();
            });
    }

    /// <summary>
    /// 清除聊天记录
    /// </summary>
    /// <param name="chatWithId"></param>
    public void ClearChatLog(string chatWithId)
    {
        if (File.Exists(ConstantUtils.chatConfigPath))
        {
            string text = File.ReadAllText(ConstantUtils.chatConfigPath);
            JsonData json = JsonMapper.ToObject(text);
            string userId = json.TryGetString(chatWithId);
            if (!string.IsNullOrEmpty(userId))
            {
                string deleteStr1 = string.Format("\"{0}\":", chatWithId) + json[chatWithId].ToJson() + ",";//清一次后面加逗号的
                text = text.Replace(deleteStr1, "");
                string deleteStr2 = "," + string.Format("\"{0}\":", chatWithId) + json[chatWithId].ToJson();//清一次前面加逗号的
                text = text.Replace(deleteStr2, "");
                string deleteStr3 = string.Format("\"{0}\":", chatWithId) + json[chatWithId].ToJson();//清一次不加逗号的
                text = text.Replace(deleteStr3, "");
                MiscUtils.CreateTextFile(ConstantUtils.chatConfigPath, text);
            }
        }
    }

    void ChatLog(ChatInfo info)
    {
        string contents;
        if (File.Exists(ConstantUtils.chatConfigPath))
        {
            string text = File.ReadAllText(ConstantUtils.chatConfigPath);
            JsonData json = JsonMapper.ToObject(text);
            string userId = json.TryGetString(info.chatWithId.ToString());
            if (string.IsNullOrEmpty(userId))
           {
                json[info.chatWithId.ToString()] = new JsonData();
                json[info.chatWithId.ToString()]["name"] = info.chatWithName;
                json[info.chatWithId.ToString()]["headIcon"] = info.playerBaseInfo.icon;
                json[info.chatWithId.ToString()]["six"] = info.playerBaseInfo.six == Six.boy ? "0" : "1";

                json[info.chatWithId.ToString()]["history"] = new JsonData();
                json[info.chatWithId.ToString()]["history"].Add(GetChatJson(info));
            }
            else
            {
                json[info.chatWithId.ToString()]["history"].Add(GetChatJson(info));
            }
            contents = json.ToJson();
        }
        else
        {
            JsonData json = new JsonData();

            json[info.chatWithId.ToString()] = new JsonData();
            json[info.chatWithId.ToString()]["name"] = info.chatWithName;
            json[info.chatWithId.ToString()]["headIcon"] = info.playerBaseInfo.icon;
            json[info.chatWithId.ToString()]["six"] = info.playerBaseInfo.six == Six.boy ? "0" : "1";
            json[info.chatWithId.ToString()]["history"] = new JsonData();

            json[info.chatWithId.ToString()]["history"].Add(GetChatJson(info));
            contents = json.ToJson();
        }


        MiscUtils.CreateTextFile(ConstantUtils.chatConfigPath, contents);        
    }

    JsonData GetChatJson(ChatInfo info)
    {
        JsonData json = new JsonData();
        
        json["text"] = info.text;
        json["timer"] = info.chatTime;
        json["senderId"] = info.senderId;
        json["type"] = info.type;

        json["playerBaseInfo"] = new JsonData();
        json["playerBaseInfo"]["userNickname"] = info.chatWithName;
        json["playerBaseInfo"]["icon"] = info.playerBaseInfo.icon;

        return json;
    }

    

}

/// <summary>
/// 好友申请状态
/// </summary>
public enum FriendApplyState
{
    /// <summary>
    /// 正常
    /// </summary>
    Normal,
    /// <summary>
    /// 我已申请
    /// </summary>
    MeAppling,
    /// <summary>
    /// 对方已申请
    /// </summary>
    HisAppling,
    /// <summary>
    /// 已是好友
    /// </summary>
    Friending
}

public class ChatInfo
{
    public int senderId;
    public int chatWithId;//我和谁的聊天
    public string chatWithName;

    public BasePlayerInfo playerBaseInfo = new BasePlayerInfo();

    public long chatTime;
    public int type;//0打字1语音2表情
    public string text;//打字内容
    public int voice_timer;//语音时间
}