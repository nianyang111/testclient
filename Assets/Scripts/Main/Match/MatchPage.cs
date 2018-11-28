using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MatchPage : Page
{
    public Button closeBtn;
    public Tab allBtn, ddzBtn, mjBtn;
    public MatchItem prefab1;
    public MatchItem prefab2;
    public MatchBottomPanel bottomPanel;
    public Transform content1,content2;
    List<MatchItem> itemList = new List<MatchItem>();
    private int curType;
    public int CurType
    {
        get { return curType; }
        set { curType = value; }
    }
    public override void Init()
    {
        base.Init();
        bottomPanel.Init(this);
        UGUIEventListener.Get(allBtn.gameObject).onClick = (g) => { SelectType(0); };
        UGUIEventListener.Get(ddzBtn.gameObject).onClick = (g) => { SelectType(1); };
        UGUIEventListener.Get(mjBtn.gameObject).onClick = (g) => { SelectType(2); };
        UGUIEventListener.Get(closeBtn.gameObject).onClick = (g) => { PageManager.Instance.OpenPage<MainPage>(); };
    }
    /// <summary>
    /// 查看
    /// </summary>
    public void OnClickRegis()
    {
        TipManager.Instance.OpenTip(TipType.SimpleTip, "老板，您还没有任何比赛");
    }
    public override void Open()
    {
        base.Open();
        CreateItem();
        SelectType();
        FinishData();
        SetNode.ExpireSet();
        SetNode.FloatBall();
    }
    /// <summary> 1是斗地主，2是麻将 </summary>
    void SelectType(int type = 0)
    {
        CurType = type;
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_LoadMatcher,
            loadMatcher = new net_protocol.LoadMatcher() { type = type }
        },true);
    }
    /// <summary>
    /// 创建
    /// </summary>
    public void CreateItem()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();
        List<net_protocol.MatcherInfo> matcherInfoList = MatchModel.Instance.matcherInfoList;
        for (int i = 0; i < matcherInfoList.Count; i++)
        {
            Transform trans=i%2==0?content1:content2;
            MatchItem item = Instantiate(prefab1, trans);
            item.gameObject.SetActive(true);
            item._page = this;
            item.Init(matcherInfoList[i]);
            itemList.Add(item);
        }
    }
    /// <summary>
    /// 刷新
    /// </summary>
    public void FinishData()
    {
        bottomPanel.FinishData();
    }

    public static string GetTimerText(float time, int timeFormat)
    {
        StringBuilder sb = new StringBuilder();
        int seconds = (int)time % 60;
        int hours = time >= 3600 ? (int)time / 3600 : 0;
        int minutes = (int)(time - hours * 3600 - seconds) / 60;
        switch (timeFormat)
        {
            case 1:
                sb.Append(GetTimeText(hours) + ":" + GetTimeText(minutes) + ":" + GetTimeText(seconds));
                break;
            case 2:
                sb.Append(GetTimeText(minutes) + ":" + GetTimeText(seconds));
                break;
            case 3:
                if(minutes<=0)
                    sb.Append("<color=#F21717FF>" + seconds + "秒</color>后开赛");
                else
                    sb.Append("<color=#F21717FF>" + minutes + "分</color>后开赛");
                break;
        }
        return sb.ToString();
    }
    static string GetTimeText(int time)
    {
        if (time < 10)
            return "0" + time;
        else
            return time.ToString();
    }
    
}
