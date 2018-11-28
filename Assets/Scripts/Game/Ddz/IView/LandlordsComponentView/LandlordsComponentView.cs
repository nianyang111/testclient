using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandlordsComponentView : MonoBehaviour
{


    public GameObject exitBtn;    

    public GameObject cancleTuoguanBtn;
    public Text desLb;

    /// <summary>
    /// 非比赛场的东西
    /// </summary>
    public GameObject normal;
    public Text roomNumberLb;
    public Text max_ratioLb;//最大倍数
    public GameObject chatBtn;
    public GameObject playLogBtn;//玩牌记录
    public GameObject jipaiqiBtn;//记牌器
    public JipaiqiPanel jipaiqiPanel;

    /// <summary>
    /// 比赛场的东西
    /// </summary>
    public GameObject match;
    public Text scoreLb;
    public Text scoreDesLb;
    public GameObject rankBtn;
    public Text rankLb;
    public GameObject jinjiBtn;
    public Text jinjiDesLb;
    public GameObject waitObj;
    public CommonAnimation bisaiLbShow;

    public Text ratioLb;//倍率
    public Text dizhuLb;
    public MenuPanel menuePanel;

    public FangkaResultPanel fangkaResultPanel; 

    //电量时间
    public Image dianliangIcon;
    public Text timerLb;

    public GameObject btnVoice;
    public GameObject voiceTipsObj;


    private void Start()
    {
        InvokeRepeating("PhoneInfoShow", 0, 60);
        InitButton();
        bisaiLbShow.sizeEndAction = delegate { bisaiLbShow.gameObject.SetActive(false); };
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
        {
            btnVoice.SetActive(true);
            UGUIEventListener.Get(btnVoice.gameObject).onDown = delegate { DownYuyin(); };
            UGUIEventListener.Get(btnVoice.gameObject).onUp = delegate { OnUp(); };
        }
        else
        {
            btnVoice.SetActive(false);
        }
        
    }

    void InitButton()
    {
        UGUIEventListener.Get(exitBtn).onClick = (o) =>
        {
            menuePanel.gameObject.SetActive(!menuePanel.gameObject.activeInHierarchy);
            CallBack tuoguanCall = delegate
            {
                if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
                {
                    TipManager.Instance.OpenTip(TipType.SimpleTip, "比赛场不能托管!");
                    //return;
                }
                if (!LandlordsModel.Instance.IsInFight || LandlordsModel.Instance.IsTuoGuan)
                {
                    TipManager.Instance.OpenTip(TipType.SimpleTip, "当前不能托管!");
                    return;
                }
                LandlordsNet.C2G_Tuoguan(1);
            };
            CallBack exitCall = () =>
            {
                if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
                {
                    if (LandlordsModel.Instance.RoomPlayerHands != null && LandlordsModel.Instance.RoomPlayerHands.Count == 3)
                    {
                        TipManager.Instance.OpenTip(TipType.AlertTip, "比赛场不能退出!");
                        return;
                    }
                    else
                        PageManager.Instance.OpenPage<MainPage>();
                }
                else
                {
                    if (LandlordsModel.Instance.IsInFight)
                    {
                        TipManager.Instance.OpenTip(TipType.AlertTip, "本局未结束!");
                        return;
                    }
                    else if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
                    {
                        LandlordsNet.C2G_IsCanLeaveRep();
                    }
                    else
                    {
                        LandlordsNet.QuiteReq();
                    }
                }
            };
            menuePanel.isCanOpenStore = () =>
            {
                if (LandlordsModel.Instance.IsInFight)
                {
                    TipManager.Instance.OpenTip(TipType.SimpleTip, "打牌中不能进行取款操作哦");
                    return false;
                }
                return true;
            };
            menuePanel.Init(exitCall, tuoguanCall);
        };

        UGUIEventListener.Get(chatBtn).onClick = (o) =>
        {
            NodeManager.OpenNode<ChatNode>(null, null, false);
        };

        UGUIEventListener.Get(cancleTuoguanBtn).onClick = (o) =>
        {
            LandlordsNet.C2G_Tuoguan(0);
        };

        UGUIEventListener.Get(jipaiqiBtn).onClick = delegate
        {
            if (UserInfoModel.userInfo.vipCard == 0)
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "VIP才能使用记牌器哦");
                return;
            }
            if (!LandlordsModel.Instance.IsInFight)
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "游戏还未开始呢!");
                return;
            }
            if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard || LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.Match)
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "只有游戏币场才有记牌器哦!");
                return;
            }
            bool isShow = !jipaiqiPanel.gameObject.activeInHierarchy;
            jipaiqiPanel.gameObject.SetActive(isShow);
            if (isShow)
                jipaiqiPanel.ReqJipaiqi();
        };

        UGUIEventListener.Get(playLogBtn).onClick = delegate
        {
            NodeManager.OpenNode<GameLogNode>(null, null, false).Inits(LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomID);
        };
    }
    /// <summary>
    /// 组件界面初始化
    /// </summary>
    public void Init()
    {
        LandlordsPage.Instance.Multiples = 1;
        LandlordsPage.Instance.Dizhu = LandlordsModel.Instance.RoomModel.CurRoomInfo.LeastStore;

        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
        {
            UGUIEventListener.Get(rankBtn).onClick = delegate { NodeManager.OpenNode<MatchRankRewardNode>("match").Inits(LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomID); };
            UGUIEventListener.Get(jinjiBtn).onClick = delegate
            {
                MatchRulesNode node = NodeManager.GetNode<MatchRulesNode>();
                if (!node)
                    node = NodeManager.OpenNode<MatchRulesNode>();
                node.gameObject.SetActive(!node.gameObject.activeInHierarchy);
            };
            normal.SetActive(false);
            match.SetActive(true);
            waitObj.SetActive(true);

            scoreLb.text = LandlordsModel.Instance.MyInfo.MatchScore.ToString();
        }
        else
        {
            normal.SetActive(true);
            match.SetActive(false);
            if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
            {
                roomNumberLb.transform.parent.parent.gameObject.SetActive(true);
                roomNumberLb.text = int.Parse(LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomID).ToString("D6");
                max_ratioLb.text = LandlordsModel.Instance.RoomModel.CurRoomInfo.Beishu.ToString();
                jipaiqiBtn.SetActive(false);
                playLogBtn.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                roomNumberLb.transform.parent.parent.gameObject.SetActive(false);
                jipaiqiBtn.SetActive(true);
                playLogBtn.transform.parent.gameObject.SetActive(false);
            }
        }
        InvokeRepeating("PhoneInfoShow", 0, 60);
    }

    public void GameStart()
    {
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
        {
            waitObj.SetActive(false);
            bisaiLbShow.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 手机电量 时间
    /// </summary>
    void PhoneInfoShow()
    {
        timerLb.text = DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2");
        StartCoroutine(Battery());
    }
    IEnumerator Battery()
    {
        yield return new WaitForSecondsRealtime(1);
        int sdkBattery = SDKManager.Instance.GetBattery();
        if (sdkBattery == -1)
            StartCoroutine(Battery());
        else
            dianliangIcon.fillAmount = sdkBattery / 100f;
    }

    public void FreshMatchInfo(CurrentMatcherInfoResp resp)
    {
        bisaiLbShow.GetComponent<Text>().text = resp.stage;
        //matchInfoPanel.FreshData(resp);
    }

    public void FreshMatchRankInfo(MyMatcherRankingResp resp)
    {
        rankLb.text = resp.myRank + "/" + resp.totalNum;
        scoreDesLb.text = "(低于" + resp.dieScore + "会被淘汰)";
    }

    /// <summary>
    /// 更新玩家信息显示
    /// </summary>
    public void UpdateUserInfoShow()
    {
        ratioLb.text = LandlordsPage.Instance.Multiples.ToString();
        dizhuLb.text = LandlordsPage.Instance.Dizhu.ToString();
    }

    /// <summary>
    /// 开启/关闭托管界面
    /// </summary>
    public void OpenTuoGuanView(bool isShow)
    {
        LandlordsModel.Instance.IsTuoGuan = isShow;
        cancleTuoguanBtn.transform.parent.gameObject.SetActive(isShow);
    }

    Vector3 startClickPos;
    Vector3 endClickPos;

    /// <summary>
    /// 按下语音
    /// </summary>
    void DownYuyin()
    {
        startClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        voiceTipsObj.SetActive(true);
        GVoice.Instance.Click_GetRecFileParam();
        GVoice.Instance.Click_btnReqAuthKey();
        GVoice.Instance.Click_btnStartRecord();
    }
    /// <summary>
    /// 抬起语音
    /// </summary>
    void OnUp()
    {
        endClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (endClickPos.y > startClickPos.y && endClickPos.y - startClickPos.y > 100)
        {//取消发送
            TipManager.Instance.OpenTip(TipType.SimpleTip, "取消发送");
            voiceTipsObj.SetActive(false);
            GVoice.Instance.Click_btnStopRecord();
        }
        else
        {
            voiceTipsObj.SetActive(false);
            GVoice.Instance.Click_btnStopRecord();
            GVoice.Instance.Click_btnUploadFile(filed =>
            {
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                {
                    ddzChatReq = new DdzChatReq()
                    {
                        type = 1,
                        ddzChatContent = new DdzChatContent()
                        {
                            text = filed,
                        }
                    },
                    msgid = MessageId.C2G_DdzChatReq
                });
                if (SetNode.chat == 0)
                    TipManager.Instance.OpenTip(TipType.SimpleTip, "您已关闭聊天功能,如要查看聊天信息请在设置里打开聊天功能");
            });
        }
    }

    /// <summary>
    /// 游戏结束全局事件
    /// </summary>
    /// <param name="args"></param>
    public void GameOver()
    {
        jipaiqiPanel.gameObject.SetActive(false);
        UpdateUserInfoShow();
        OpenTuoGuanView(false);
        NodeManager.CloseTargetNode<MatchRulesNode>();        
    }

    public void ClearUI()
    {
        jipaiqiPanel.Clear();
    }

    void OnDestroy()
    {
        CancelInvoke("PhoneInfoShow");
    }


}
