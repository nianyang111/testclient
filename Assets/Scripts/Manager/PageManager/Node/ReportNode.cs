using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReportNode : Node {

    /// <summary>
    /// 当前举报的人
    /// </summary>
    public int curReportUid;

    public GameObject reportBtn;


    public Toggle gameTg, nickTg, headTg;

    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(reportBtn).onClick = delegate { Report(); };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reportUid">要举报的人</param>
    /// <param name="isGameZuobi">是否包含游戏作弊</param>
	public void Inits(int reportUid,bool isGameZuobi)
    {
        curReportUid = reportUid;
        gameTg.gameObject.SetActive(isGameZuobi);
    }

    /// <summary>
    /// 举报
    /// </summary>
    void Report()
    {//0游戏作弊1不良昵称2不良头像
        List<int> types = new List<int>();
        if (gameTg.isOn)
        {
            types.Add(0);
        }
        if (nickTg.isOn)
        {
            types.Add(1);
        }
        if (headTg.isOn)
        {
            types.Add(2);
        }
        Report report = new Report();
        report.userId = curReportUid;
        for (int i = 0; i < types.Count; i++)
        {
            report.reason.Add(types[i]);
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                report = report,
                msgid = MessageId.C2G_Report
            });
        TipManager.Instance.OpenTip(TipType.SimpleTip, "举报已发送");
        Close();
    }
}
