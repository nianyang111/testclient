using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 消息数据
/// </summary>
public class MessageModel
{

    private static MessageModel instance;
    public static MessageModel Instance
    {
        get
        {
            if (instance == null)
                instance = new MessageModel();
            return instance;
        }
    }
    public UnityAction<bool> msgAct;

    public UserMsg msgData = new UserMsg();

    public void QueryUserMsgFinish(UserMsg datas)
    {
        msgData = datas;
        PlayAction();
    }
    /// <summary>
    /// 执行回调
    /// </summary>
    public void PlayAction()
    {
        if (msgData.msg.Count > 0)
        {
            var notRead = msgData.msg.Find(p => p.msgIsread == "0");
            if (msgAct != null)
                msgAct(notRead != null);
        }
    }

}
