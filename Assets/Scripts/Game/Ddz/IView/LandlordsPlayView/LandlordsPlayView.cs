using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LandlordsPlayView : MonoBehaviour
{
    public Transform dipaiDesk;
    public Text roomDesLb;

    public GameObject fapaiObj;

    public List<LandlordsBasePlayer> playerViews = new List<LandlordsBasePlayer>();

    void Awake()
    {
        OrderController.Instance.enterCall += RoundEnter;
        OrderController.Instance.exitCall += RoundExit;
    }

    void OnDestroy()
    {
        OrderController.Instance.enterCall -= RoundEnter;
        OrderController.Instance.exitCall -= RoundExit;
    }

    public void Init()
    {
        InitRoomInfo();
        InitPlayerPrefab();
        ClearUI();
    }

    void InitRoomInfo()
    {
        bool ShowDes = !LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch && LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard;
        roomDesLb.gameObject.SetActive(ShowDes);
        if (ShowDes)
        {
            roomDesLb.text = "总局数：" + LandlordsModel.Instance.RoomModel.CurRoomInfo.MaxPlayCount;
        }
    }

    /// <summary>
    /// 刚进房间初始化玩家信息
    /// </summary>
    public void InitPlayerPrefab()
    {
        for (int i = 0; i < playerViews.Count; i++)
        {
            playerViews[i].RestToNoPlayer(false);
        }
        LandlordsModel.Instance.RoomPlayerSort();
        if (LandlordsModel.Instance.RoomPlayerHands == null)
            return;
        for (int i = 0; i < LandlordsModel.Instance.RoomPlayerHands.Count; i++)
        {
            if (LandlordsModel.Instance.RoomPlayerHands[i] == null)
                continue;
            LoadPlayerPrefab(LandlordsModel.Instance.RoomPlayerHands[i], i);
        }              
    }

    /// <summary>
    /// 加载某个新进入玩家信息
    /// </summary>
    public void LoadPlayerPrefab(LandkirdsHandCardModel hand, int posIndex)
    {
        LandlordsBasePlayer player = playerViews[posIndex];
        player.gameObject.SetActive(true);
        player.Init(hand,OnClickHeadIcon);
    }

    /// <summary>
    /// 某个玩家离开
    /// </summary>
    public void PlayerExit(string uid,bool isKick)
    {
        LandlordsBasePlayer playerView = GetPlayer(uid);
        playerView.RestToNoPlayer(isKick);
    }

    /// <summary>
    /// 某个玩家准备
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="isZhunbei"></param>
    public void PlayerZhunbei(string uid,bool isZhunbei)
    {
        LandlordsBasePlayer playerView = GetPlayer(uid);
        playerView.Zhunbei(isZhunbei);
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    public void GameStart()
    {
        for (int i = 0; i < playerViews.Count; i++)
        {
            playerViews[i].GameStart();
        }
    }

    /// <summary>
    /// 每个玩家显示层发牌
    /// </summary>
    public void InitPayerLibrary()
    {
        fapaiObj.SetActive(true);
        UIUtils.DelayDesOrDisObject(fapaiObj, 2f, false);
        for (int i = 0; i < LandlordsModel.Instance.RoomPlayerHands.Count; i++)
        {
            LandkirdsHandCardModel handCard = LandlordsModel.Instance.RoomPlayerHands[i];            
            if (handCard != null)
            {
                LandlordsBasePlayer player=GetPlayer(handCard.playerInfo.uid);
                player.DealCard(true);
                if (player is LandlordsOtherPlayer)
                    ((LandlordsOtherPlayer)player).CardRemainCountShow();
            }
        }
        LandlordsPage.Instance.LoadComplete();
    }

    /// <summary>
    /// 显示层发底牌
    /// </summary>
    public void DealDipai(List<Card> cards)
    {
        for (int i = 0; i < dipaiDesk.childCount; i++)
        {
            dipaiDesk.GetChild(i).GetComponent<CardUI>().Destroy();
        }
        for (int i = 0; i < cards.Count; i++)
        {
            CardUI cardUI = LandlordsPage.MakeSprite(cards[i], false, dipaiDesk);
            cardUI.SetCardSize(new Vector2(56, 76));
        }
    }

    // 回合进入
    void RoundEnter(bool isCanNoPlay)
    {
        LandlordsBasePlayer playerUI = GetPlayer(OrderController.Instance.TypeUid);
        if (playerUI != null)
            playerUI.RoundEnter(isCanNoPlay);
    }

    // 回合离开
    void RoundExit()
    {
        LandlordsBasePlayer playerUI = GetPlayer(OrderController.Instance.TypeUid);
        if (playerUI != null)
            playerUI.RoundExit();
    }

    /// <summary>
    /// 设置警报
    /// </summary>
    /// <param name="args"></param>
    public void SetWarningShow(string uid,int remainCard, bool isShow)
    {
        if (uid != null)
        {//指定玩家显隐            
            LandlordsBasePlayer player = GetPlayer(uid);
            player.SetWarring(remainCard,isShow);
        }
        else
        {//全部显隐
            for (int i = 0; i < playerViews.Count; i++)
            {
                playerViews[i].SetWarring(remainCard, isShow);
            }
        }
    }
   
    /// <summary>
    /// 清理已出的牌
    /// </summary>
    public void ClearDesk()
    {
        for (int i = 0; i < playerViews.Count; i++)
        {
            playerViews[i].ClearDesk();
        }
    }


    public void GameOver()
    {
        for (int i = 0; i < playerViews.Count; i++)
        {
            playerViews[i].GameOver();
            playerViews[i].SetCoin();
            if (playerViews[i] is LandlordsOtherPlayer)
                ((LandlordsOtherPlayer)playerViews[i]).ShowCards(true);
        }
        List<DdzJSPlayerInfo> results = LandlordsModel.Instance.ResultModel.GetResultInfos();

        for (int i = 0; i < playerViews.Count; i++)
        {
            if (playerViews[i]._handCard != null && results != null && results.Count > 0)
            {
                playerViews[i].ShowResultLb(results.Find(p => p.userId.ToString() == playerViews[i]._handCard.playerInfo.uid).income);
            }
        }
    }

    /// <summary>
    /// 游戏UI清理
    /// </summary>
    /// <param name="args"></param>
    public void ClearUI()
    {
        for (int i = 0; i < dipaiDesk.childCount; i++)
        {
            dipaiDesk.GetChild(i).GetComponent<CardUI>().Destroy();
        }
        for (int i = 0; i < playerViews.Count; i++)
        {
            if (playerViews[i] is LandlordsOtherPlayer)
                ((LandlordsOtherPlayer)playerViews[i]).ShowCards(false);
            if (playerViews[i] is LandlordsMainPlayer)
                ((LandlordsMainPlayer)playerViews[i]).DestroyCardUI();
            playerViews[i].ClearResultLb();
            playerViews[i].ClearDesText();
            playerViews[i].ClearDesk();
            playerViews[i].ClearClock();
        }
        LandlordsPage.Instance.LandlordsWarning(null, 0, false);        
    }

    public LandlordsBasePlayer GetPlayer(string uid)
    {
        LandlordsBasePlayer player = null;
        player = playerViews.Find(p => p._handCard != null && p._handCard.playerInfo.uid == uid);
        return player;
    }

    void OnClickHeadIcon(LandkirdsHandCardModel player)
    {
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
        {
            if (player.playerInfo.uid != UserInfoModel.userInfo.userId.ToString())
                return;
        }
        NodeManager.OpenNode<GameRoleInfoNode>().Inits(int.Parse(player.playerInfo.uid));
    }

}
