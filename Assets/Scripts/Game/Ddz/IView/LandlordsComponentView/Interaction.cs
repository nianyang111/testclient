using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 交互界面
/// </summary>
public class Interaction : MonoBehaviour
{
    public static Interaction Instance;

    /// <summary>X分</summary> 
    public Button onefenBtn, twofenBtn, threefenBtn;
    /// <summary>不叫</summary> 
    public Button noCallBtn;
    /// <summary>不出</summary> 
    public Button noPlayBtn;
    /// <summary>出牌</summary>
    public Button playBtn;
    /// <summary>提示</summary>
    public GameObject tipsBtn;
    /// <summary>重选</summary>
    public Button chongxuanBtn;
    /// <summary>叫地主/不叫</summary>
    public GameObject jiaodizhuBtn, bujiaodizhuBtn;
    /// <summary>抢地主/不抢</summary>
    public GameObject qiangBtn, buqiangBtn;
    /// <summary>卡牌遮罩</summary>
    public GameObject cardMask;
    /// <summary>
    /// 结算
    /// </summary>
    public GameObject changeBtn, zhunbeiBtn, resultXiangqingBtn;
    public Text zhunbeiBtnText;

    private void Awake()
    {
        Instance = this;
        OrderController.Instance.enterCall += ActiveCardButton;
        OrderController.Instance.exitCall += NoActiveCardButton;
    }

    private void Start()
    {
        UGUIEventListener.Get(onefenBtn.gameObject).onClick = delegate { if (!onefenBtn.interactable)return; CallFen(1); };
        UGUIEventListener.Get(twofenBtn.gameObject).onClick = delegate { if (!twofenBtn.interactable)return; CallFen(2); };
        UGUIEventListener.Get(threefenBtn.gameObject).onClick = delegate { if (!threefenBtn.interactable)return; CallFen(3); };
        UGUIEventListener.Get(noCallBtn.gameObject).onClick = delegate { CallFen(0); };
        
        UGUIEventListener.Get(noPlayBtn.gameObject).onClick = delegate { NoPlayCard(0); };
        UGUIEventListener.Get(playBtn.gameObject).onClick = delegate { PlayCard(); };
        UGUIEventListener.Get(chongxuanBtn.gameObject).onClick = delegate { Chongxuan(); };
        UGUIEventListener.Get(tipsBtn).onClick = delegate { Tips(); };

        UGUIEventListener.Get(jiaodizhuBtn).onClick = delegate { PlayerQiangDzReq(true); };
        UGUIEventListener.Get(bujiaodizhuBtn).onClick = delegate { PlayerQiangDzReq(false); };

        UGUIEventListener.Get(qiangBtn).onClick = delegate { PlayerQiangDzReq(true); };
        UGUIEventListener.Get(buqiangBtn).onClick = delegate { PlayerQiangDzReq(false); };

        UGUIEventListener.Get(changeBtn).onClick = delegate { Change(); };
        UGUIEventListener.Get(zhunbeiBtn).onClick = delegate 
        {
            if (zhunbeiBtnText.text == "准备")
                Zhunbei();
            else if (zhunbeiBtnText.text == "开局")
                StartGame();
        };
        UGUIEventListener.Get(resultXiangqingBtn).onClick = delegate { ResultInfo(); }; 
    }

    public void InitBtn()
    {        
        onefenBtn.gameObject.SetActive(false);
        twofenBtn.gameObject.SetActive(false);
        threefenBtn.gameObject.SetActive(false);
        noCallBtn.gameObject.SetActive(false);

        noPlayBtn.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(false);
        
        tipsBtn.SetActive(false);
        chongxuanBtn.gameObject.SetActive(false);

        jiaodizhuBtn.SetActive(false);
        bujiaodizhuBtn.SetActive(false);

        qiangBtn.SetActive(false);
        buqiangBtn.SetActive(false);

        SetChangeAndZhunbei(true);
        resultXiangqingBtn.SetActive(false);
    }


    public void GameStart()
    {
       cardMask.SetActive(true);

       onefenBtn.gameObject.SetActive(false);
       twofenBtn.gameObject.SetActive(false);
       threefenBtn.gameObject.SetActive(false);
       noCallBtn.gameObject.SetActive(false);

       noPlayBtn.gameObject.SetActive(false);
       playBtn.gameObject.SetActive(false);
       tipsBtn.SetActive(false);
       chongxuanBtn.gameObject.SetActive(false);

       jiaodizhuBtn.SetActive(false);
       bujiaodizhuBtn.SetActive(false);

       qiangBtn.SetActive(false);
       buqiangBtn.SetActive(false);

       SetChangeAndZhunbei(false);

       resultXiangqingBtn.SetActive(false);
    }

    /// <summary>
    /// 设置玩家卡牌遮罩，防点击
    /// </summary>
    /// <param name="args"></param>
    public void SetCardMaskShow()
    {
        cardMask.SetActive(false);
    }

    /// <summary>
    /// 设置准备和换桌按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void SetChangeAndZhunbei(bool isShow)
    {
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            return;
        zhunbeiBtn.SetActive(isShow);
        changeBtn.SetActive(isShow);
        if (!isShow)
            return;
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
        {
            changeBtn.SetActive(false);
            zhunbeiBtn.transform.localPosition = new Vector3(0, zhunbeiBtn.transform.localPosition.y);
            if (LandlordsModel.Instance.MyInfo.IsRoomer)
                zhunbeiBtnText.text = "开局";
        }
        else
        {
            zhunbeiBtnText.text = "准备";
            zhunbeiBtn.transform.localPosition = new Vector3(302.6f, zhunbeiBtn.transform.localPosition.y);
        }
    }

    /// <summary>
    /// 激活叫分按钮
    /// </summary>
    void ActiviteCallFenButton()
    {
        onefenBtn.gameObject.SetActive(true);
        twofenBtn.gameObject.SetActive(true);
        threefenBtn.gameObject.SetActive(true);
        noCallBtn.gameObject.SetActive(true);

        //检测是否有炸弹和王炸  
        List<Card> cards = new List<Card>();
        LandkirdsHandCardModel my = LandlordsModel.Instance.MyInfo;
        for (int i = 0; i < my.CardsCount; i++)
        {
            cards.Add(my[i]);
        }
        cards = CardRules.FindBoom(cards, 0, true);
        if (cards == null || cards.Count == 0)
        {
            //设置分值按钮置灰
            List<int> list = LandlordsModel.Instance.GetCanCallLandlordNum();
            onefenBtn.interactable = list.Contains(1);
            twofenBtn.interactable = list.Contains(2);
            threefenBtn.interactable = list.Contains(3);
        }
        else
        {
            //只能叫3分或者不叫           
            onefenBtn.interactable = false;
            twofenBtn.interactable = false;
            threefenBtn.interactable = true;
            noCallBtn.interactable = true;
        }
    }

    /// <summary>
    /// 激活叫地主按钮
    /// </summary>
    void ActicveJdzBtn()
    {
        jiaodizhuBtn.SetActive(true);
        bujiaodizhuBtn.SetActive(true);
    }

    /// <summary>
    /// 激活抢地主按钮
    /// </summary>
    void ActicveQdzBtn()
    {
        qiangBtn.SetActive(true);
        buqiangBtn.SetActive(true);
    }

    /// <summary>
    /// 激活出牌按钮
    /// </summary>
    void ActivePopButton(bool canReject)
    {
        if (LandlordsModel.Instance.IsTuoGuan || LandlordsMainPlayer.CheckIsLastCanAutoPop())
            return;
        List<Card> cards = CardRules.DelayDiscardCard(UserInfoModel.userInfo.userId.ToString());
        chongxuanBtn.gameObject.SetActive(true);
        chongxuanBtn.interactable = cards.Count > 0;
        tipsBtn.SetActive(true);
        playBtn.gameObject.SetActive(true);
        noPlayBtn.gameObject.SetActive(true);
        noPlayBtn.interactable = canReject;
        playBtn.interactable = cards.Count > 0;
    }

    void ActiveQiangButton()
    {
        if (LandlordsModel.Instance.IsTuoGuan)
            return;
        qiangBtn.SetActive(true);
        buqiangBtn.SetActive(true);
    }


    /// <summary>
    /// 激活操作按钮
    /// </summary>
    /// <param name="canReject">是否可以不出</param>
    void ActiveCardButton(bool canReject)
    {
        if (OrderController.Instance.TypeUid == UserInfoModel.userInfo.userId.ToString())
        {
            switch (OrderController.Instance.CurInterationType)
            {
                case InterationType.CallLandlords:
                    ActicveJdzBtn();
                    break;
                case InterationType.QiangLandlords:
                    ActicveQdzBtn();
                    break;
                case InterationType.CallFen:
                    ActiviteCallFenButton();
                    break;
                case InterationType.PopCard:
                    ActivePopButton(canReject);
                    break;
                default:
                    break;
            }
        }
    }



    /// <summary>
    /// 关闭按钮
    /// </summary>
    public void NoActiveCardButton()
    {
        if (OrderController.Instance.TypeUid == UserInfoModel.userInfo.userId.ToString())
        {
            tipsBtn.SetActive(false); 
            playBtn.gameObject.SetActive(false);
            noPlayBtn.gameObject.SetActive(false);
            chongxuanBtn.gameObject.SetActive(false);
           
            onefenBtn.gameObject.SetActive(false);
            twofenBtn.gameObject.SetActive(false);
            threefenBtn.gameObject.SetActive(false);
            noCallBtn.gameObject.SetActive(false);
            jiaodizhuBtn.SetActive(false);
            bujiaodizhuBtn.SetActive(false);
            qiangBtn.SetActive(false);
            buqiangBtn.SetActive(false);
        }
    }

    public void ClearUI()
    {
        onefenBtn.gameObject.SetActive(false);
        twofenBtn.gameObject.SetActive(false);
        threefenBtn.gameObject.SetActive(false);
        noCallBtn.gameObject.SetActive(false);

        noPlayBtn.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(false);

        tipsBtn.SetActive(false);
        chongxuanBtn.gameObject.SetActive(false);

        jiaodizhuBtn.SetActive(false);
        bujiaodizhuBtn.SetActive(false);

        qiangBtn.SetActive(false);
        buqiangBtn.SetActive(false);

        SetChangeAndZhunbei(true);
        resultXiangqingBtn.SetActive(false);
    }

    /// <summary>
    /// 重选回调
    /// </summary>
    public void Chongxuan()
    {
        LandlordsMainPlayer playCard = LandlordsPage.Instance.playView.GetPlayer(UserInfoModel.userInfo.userId.ToString()) as LandlordsMainPlayer;
        playCard.Chongxuan();
    }

    /// <summary>
    /// 叫分回调
    /// </summary>
    void CallFen(int score)
    {
        LandlordsNet.C2G_PlayerCallReq(score);
    }

    /// <summary>
    /// 叫地主回调
    /// </summary>
    void PlayerQiangDzReq(bool qiang)
    {
        LandlordsNet.C2G_PlayerQiangDzReq(qiang ? 1 : 0);
    }

    /// <summary>
    /// 出牌回调
    /// </summary>
    public void PlayCard()
    {
        if (!playBtn.interactable)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "无法大过上家");
            return;
        }
        LandlordsMainPlayer player = LandlordsPage.Instance.playView.GetPlayer(UserInfoModel.userInfo.userId.ToString()) as LandlordsMainPlayer;
        bool isPlaySuccess = player.MainRolePopReq(() =>
            {
            });
        if (!isPlaySuccess)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请选择正确的牌!");
        }
    }

    /// <summary>
    /// 不出回调 -1要不起 0不出
    /// </summary>
    public void NoPlayCard(int type)
    {
        if (!noPlayBtn.interactable)
            return;
        LandlordsNet.C2G_PopReq(type, null, null, () =>
        {
            //cannotPlayBtn.gameObject.SetActive(false);
            //playBtn.gameObject.SetActive(false);
            //noPlayBtn.gameObject.SetActive(false);
            //tipsBtn.SetActive(false);
        });
    }



    /// <summary>
    /// 提示回调
    /// </summary>
    void Tips()
    {
        bool isHaveTips = false;
        List<Card> cards = LandlordsModel.Instance.TipsModel.Tips(out isHaveTips);
        LandlordsMainPlayer playCard = LandlordsPage.Instance.playView.GetPlayer(UserInfoModel.userInfo.userId.ToString()) as LandlordsMainPlayer;
        playCard.Tips(cards);
    }

    /// <summary>
    /// 换桌回调
    /// </summary>
    void Change()
    {
        LandlordsNet.C2G_ChangeTabelReq();        
        SetChangeAndZhunbei(false);
        resultXiangqingBtn.SetActive(false);
    }

    /// <summary>
    /// 开局回调
    /// </summary>
    void StartGame()
    {
        LandlordsNet.StartReq();
    }

    /// <summary>
    /// 准备回调
    /// </summary>
    public void Zhunbei()
    {
        LandlordsNet.ZhunbeiReq();        
        LandlordsPage.Instance.InitRoom();           
    }

    /// <summary>
    /// 结算详情回调
    /// </summary>
    void ResultInfo()
    {
        LandlordsPage.Instance.resultView.OpenUI(LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType);
    }


    public void GameOver()
    {
        onefenBtn.gameObject.SetActive(false);
        twofenBtn.gameObject.SetActive(false);
        threefenBtn.gameObject.SetActive(false);
        noCallBtn.gameObject.SetActive(false);

        noPlayBtn.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(false);
        tipsBtn.SetActive(false);
        chongxuanBtn.gameObject.SetActive(false);

        qiangBtn.SetActive(false);
        buqiangBtn.SetActive(false);

        jiaodizhuBtn.SetActive(false);
        bujiaodizhuBtn.SetActive(false);
        SetChangeAndZhunbei(true);
        if (!LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            resultXiangqingBtn.SetActive(true);
    }

    public void HideAllBtn()
    {
        onefenBtn.gameObject.SetActive(false);
        twofenBtn.gameObject.SetActive(false);
        threefenBtn.gameObject.SetActive(false);
        noCallBtn.gameObject.SetActive(false);

        noPlayBtn.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(false);
        tipsBtn.SetActive(false);
        chongxuanBtn.gameObject.SetActive(false);

        qiangBtn.SetActive(false);
        buqiangBtn.SetActive(false);

        jiaodizhuBtn.SetActive(false);
        bujiaodizhuBtn.SetActive(false);
        SetChangeAndZhunbei(false);
        resultXiangqingBtn.SetActive(false);
    }

    void OnDestroy()
    {
        OrderController.Instance.enterCall -= ActiveCardButton;
        OrderController.Instance.exitCall -= NoActiveCardButton;
    }
}
