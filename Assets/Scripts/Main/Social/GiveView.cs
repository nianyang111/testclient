using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiveView : MonoBehaviour
{

    public GameObject closeBtn;

    public Text receiverLb;

    public InputField sendCountInput;

    public Text myYinbiLb;

    public GameObject restBtn;

    public GameObject sureBtn;

    FriendInfo sendInfo;

    public void Init(FriendInfo _sendInfo)
    {
        sendInfo = _sendInfo;
        UGUIEventListener.Get(closeBtn).onClick = delegate { SetVisibel(false); };
        UGUIEventListener.Get(restBtn).onClick = delegate { Rest(); };
        UGUIEventListener.Get(sureBtn).onClick = delegate { Sure(); };
        receiverLb.text = sendInfo.nickname;
        sendCountInput.text = "0";
        myYinbiLb.text = UserInfoModel.userInfo.walletAgNum.ToString();
    }

    /// <summary>
    /// 重置按钮回调
    /// </summary>
    void Rest()
    {
        sendCountInput.text = "0";
    }

    /// <summary>
    /// 确认按钮回调
    /// </summary>
    void Sure()
    {
        long sendCount = long.Parse(sendCountInput.text);
        //if (sendCount < 50000)
        //{
        //    TipManager.Instance.OpenTip(TipType.SimpleTip, "每次最少赠送50000哦!");
        //    return;
        //}
        //if (sendCount > 1000000)
        //{
        //    TipManager.Instance.OpenTip(TipType.SimpleTip, "每次最多赠送1000000哦!");
        //    return;
        //}
        if (UserInfoModel.userInfo.walletAgNum < sendCount)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "您没有那么多银币啦!");
            return;
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                giveSilver = new GiveSilver()
                {
                    userId = sendInfo.userId,
                    amount = (int)sendCount
                },
                msgid = MessageId.C2G_GiveSilver
            });
        SetVisibel(false);
    }

    public void SetVisibel(bool isShow)
    {
        gameObject.SetActive(isShow);
    }


}
