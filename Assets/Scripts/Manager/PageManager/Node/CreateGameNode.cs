using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 番数-底注  局数-房卡消耗
/// </summary>
public class CreateGameNode : Node
{
    public GameType curGameType;

    public CreateGameBasePanel ddzPanel;
    public CreateGameBasePanel mjPanel;

    public Text curFangkaLb;
    public GameObject openBtn;


    public GameObject topTitleBtn, topDown;
    public Text downText;
    public Text titleText;

    public string[] names = new string[2] { "斗地主", "明水麻将" };
    private string selectName;
    private string SelectName
    {
        set
        {
            selectName = value;
            downText.text = titleText.text;
            titleText.text = selectName; //print(topDown);
            topDown.SetActive(false);
            SetGameType(selectName == "斗地主" ? GameType.Ddz : GameType.Mj);
        }
        get { return selectName; }
    }


    public override void Open()
    {
        base.Open();
        curFangkaLb.text = "x" + UserInfoModel.userInfo.roomCardNum;
        UGUIEventListener.Get(openBtn).onClick = delegate { OpenGame(); };
        UGUIEventListener.Get(topTitleBtn).onClick = (g) => { topDown.SetActive(!topDown.activeInHierarchy); };
        UGUIEventListener.Get(downText.gameObject).onClick = (g) => { SelectName = downText.text; };
        SelectName = curGameType == GameType.Ddz ? names[0] : names[1];
    }

    void SetGameType(GameType gameType)
    {
        curGameType = gameType;
        ShowPanelByGameType();
    }

    void ShowPanelByGameType()
    {
        switch (curGameType)
        {
            case GameType.Ddz:                
                ddzPanel.SetVisible(true);
                mjPanel.SetVisible(false);
                break;
            case GameType.Mj:
                ddzPanel.SetVisible(false);
                mjPanel.SetVisible(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 开房按钮
    /// </summary>
    void OpenGame()
    {
        CreateGameBasePanel panel = null;
        switch (curGameType)
        {
            case GameType.Ddz:
                panel = ddzPanel;
                break;
            case GameType.Mj:
                panel = mjPanel;
                break;
        }
        int fanshu = panel.GetCurFanshu();
        int jushu = panel.GetCurJushu();
        int costFangka = panel.GetCurCostFangka();
        int renshu = panel.GetCurRenshu();
        int dizhu = panel.GetCurDizhu();

        print(string.Format("当前游戏：{0},番数：{1}，局数：{2}，人数：{3}，房卡：{4}，底注：{5}", curGameType, fanshu, jushu, renshu, costFangka, dizhu));

        if (UserInfoModel.userInfo.roomCardNum < costFangka)
        {
            TipManager.Instance.OpenTip(TipType.AlertTip, "房卡不足");
            return;
        }
        if (curGameType == GameType.Ddz)
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_DdzBuildCRTable,
                ddzBuildCRTable = new DdzBuildCRTable()
                {
                    fsb = fanshu,//番数
                    jsb = jushu,//局数
                    fkc = costFangka,//房卡数
                }
            }, true);
        }
        else if (curGameType == GameType.Mj)
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_CreateTable,
                RoomDetail = new RoomDetail()
                {
                    type = 7,
                    round = jushu,
                    rate = fanshu,
                    num = renshu,
                    baseScore = dizhu,
                    roomType = 3
                }
            }, true);
        }
    }

    /// <summary>
    /// 服务器响应进入游戏房间（临时写这里，应该写在socket里，这个界面被销毁会出问题）
    /// </summary>
    public void G2C_EnterRoom()
    {
        switch (curGameType)
        {
            case GameType.Ddz:
                //PageManager.Instance.OpenPage<DdzPage>();
                break;
            case GameType.Mj:
                PageManager.Instance.OpenPage<MaJangPage>();
                break;
            default:
                break;
        }
    }
}

public enum GameType
{
    /// <summary>
    /// 斗地主
    /// </summary>
    Ddz,
    /// <summary>
    /// 麻将
    /// </summary>
    Mj
}
