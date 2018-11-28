using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRecordNode : Node {
    public Tab myRecord, history, friend;
    public MatchMyRecordPanel myRecordPanel;
    public MatchHistoryRecordPanel historyRecordPaenl;
    public MatchFriendRankPanel friendRankPanel;
    public override void Init()
    {
        base.Init();
        myRecordPanel.Init();
        historyRecordPaenl.Init();
    }
    public override void Open()
    {
        base.Open();
        myRecordPanel.Open();
        historyRecordPaenl.Open();
        friendRankPanel.Open();
    }
    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        myRecordPanel.Close();
        historyRecordPaenl.Close();
        friendRankPanel.Close();
    }
}
