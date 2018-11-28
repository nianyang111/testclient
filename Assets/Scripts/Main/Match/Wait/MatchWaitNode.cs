using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchWaitNode : Node
{
    public GameObject closeBtn;
    public MatchWaitTopPanel topPanel;
    public MatchWaitBottomPanel bottomPanel;

    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(closeBtn).onClick = delegate { OnQuitMatch(); };
        topPanel.Init();
        bottomPanel.Init();
    }

    public static void OpenMatchWait(net_protocol.WaitMatcherRiseResp resp)
    {
        MatchWaitNode mwn = NodeManager.OpenNode<MatchWaitNode>("match",null,false,false);
        mwn.topPanel.FlushData(resp);
        mwn.bottomPanel.FlushData(resp);
    }

    private void OnQuitMatch()
    {
        TipManager.Instance.OpenTip(TipType.ChooseTip, "现在离开将会放弃当前比赛，是否要退赛？", 0, () =>
        {
            SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
            {
                msgid = net_protocol.MessageId.C2G_QuitMatcher,
                quitMatcher = new net_protocol.QuitMatcher()
                {
                    matcherId = MatchModel.Instance.CurData.matchId
                }
            });
        });
    }

    public void QuitMatcher()
    {
        PageManager.Instance.OpenPage<MatchPage>();
    }

    public void MatcherRise(net_protocol.WaitMatcherRiseResp resp)
    {
        topPanel.waitIcon.sprite = BundleManager.Instance.GetSprite("Match/match_word_2");
        this.Close(false);
    }

}
