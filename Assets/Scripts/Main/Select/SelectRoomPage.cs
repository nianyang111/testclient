using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRoomPage : Page
{
    public static SelectRoomPage Instance;
    public GameObject topTitleBtn, topDown, closeBtn;
    public SelectMJPanel mjPanel;
    public SelectDdzPanel ddzPanel;
    public SelectBottomPanel bottomPanel;
    public Text titleText, downText;
    private string[] names = new string[2] { "斗地主", "明水麻将" };
    private string selectName = "";
    private static int selectInt = 1;
    private string SelectName
    {
        set
        {
            selectName = value;
            downText.text = titleText.text;
            titleText.text = selectName;
            selectInt = selectName == names[0] ? 1 : 2;
            ddzPanel.gameObject.SetActive(SelectName == names[0]);
            mjPanel.gameObject.SetActive(SelectName == names[1]);
            topDown.SetActive(false);
        }
        get { return selectName; }
    }
    public override void Init()
    {
        Instance = this;
        base.Init();
        mjPanel.Init();
        ddzPanel.Init();
        bottomPanel.Init(this);
        topDown.SetActive(false);
        UGUIEventListener.Get(closeBtn).onClick = delegate { PageManager.Instance.OpenPage<MainPage>(); };
        //UGUIEventListener.Get(moreBtn).onClick = delegate { };
        UGUIEventListener.Get(topTitleBtn).onClick = delegate { topDown.SetActive(!topDown.activeInHierarchy); };
        UGUIEventListener.Get(downText.gameObject).onClick = delegate { SelectName = downText.text; };
        //暂时不加公告
        //NodeManager.OpenNode<NoticeNode>(null, null, false, false);
    }
    public override void Open()
    {
        base.Open();
        OpenPanel(selectInt);
        FinishData();
        SetNode.ExpireSet();
        SetNode.FloatBall();
        bottomPanel.SetFriendRedPoint();
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QueryUserMsg,
        });
    }
    /// <summary> 快速开始游戏 </summary>
    public void QuickGame()
    {
        if (selectName == names[0])
        {
            var ddzItemList = ddzPanel.itemList.FindAll(p => p.GetData().roomType == ddzPanel.ddzType);
            for (int i = ddzItemList.Count-1; i > -1; i--)
            {
                if (ddzItemList[i].isPlay)
                {
                    ddzItemList[i].EnterGameRoom();
                    return;
                }
            }
            TipManager.Instance.OpenTip(TipType.ChooseTip, string.Format(ddzPanel.ddzType == "银币场" ? "银币不足不能开始游戏" : "金条不足不能开始游戏"));
        }
        else if (selectName == names[1])
        {
            var mjDataList = mjPanel.itemList.FindAll(p => p.GetData().roomType == mjPanel.mjType);
            for (int i = mjDataList.Count - 1; i > -1; i--)
            {
                if (mjDataList[i].isPlay)
                {
                    mjDataList[i].EnterGameRoom();
                    return;
                }
            }
            TipManager.Instance.OpenTip(TipType.ChooseTip, string.Format(mjPanel.mjType == "二人银币场" || mjPanel.mjType == "四人银币场" ? "银币不足不能开始游戏" : "金条不足不能开始游戏"));
        }
       
    }
    /// <summary> 刷新数据 </summary>
    public void FinishData()
    {
        bottomPanel.FinishData();
    }
    /// <summary> 1是斗地主 2是麻将 </summary>
    public void OpenPanel(int panelType)
    {
        if (panelType == 1)
        {
            titleText.text = names[1];
            SelectName = names[0];
        }
        if (panelType == 2)
        {
            titleText.text = names[0];
            SelectName = names[1];
        }
    }
    /// <summary> 请求房间数据 </summary>
    public void QueryGameRoomFinish(net_protocol.QueryGameRoomResp resp)
    {
        if (PageManager.Instance.CurrentPage is SelectRoomPage)
        {
            var rooms = resp.room;
            if (resp.gid == 1)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    ddzPanel.dataList.Find(p => p.roomGradeTypeId == rooms[i].level && p.roomTypeId == rooms[i].coinType).roomData = rooms[i];
                }
                ddzPanel.SelectType();
            }
            else if (resp.gid == 2)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    mjPanel.dataList.Find(p => p.roomGradeTypeId == rooms[i].level && p.roomTypeId == rooms[i].coinType && p.roomPeople == rooms[i].tableSize).roomData = rooms[i];
                }
                mjPanel.SelectType();
            }
        }
    }
    public override void Close()
    {
        base.Close();
        bottomPanel.Close();
    }
}
