using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandlordsOtherPlayer : LandlordsBasePlayer {

    [HideInInspector]
    public GameObject invateBtn;
    [HideInInspector]
    public Text cardCountLb;
    [HideInInspector]
    public CommonAnimation kick;

    protected override void Awake()
    {
        base.Awake();
        UGUIEventListener.Get(invateBtn).onClick = delegate { NodeManager.OpenNode<InvateNode>().Inits(LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomID); };
        invateBtn.SetActive(false);
    }

    public override void Init(LandkirdsHandCardModel handCardModel, CallBack<LandkirdsHandCardModel> onClickHead)
    {
        base.Init(handCardModel, onClickHead);
        invateBtn.SetActive(false);
    }

    public override void RestToNoPlayer(bool isKick)
    {
        if (isKick && kick)
        {
            kick.pointEndAction = delegate { kick.gameObject.SetActive(false); };
            kick.gameObject.SetActive(true);
        }
        if (!LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch && LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
            invateBtn.SetActive(true);
        base.RestToNoPlayer(isKick);
    }

    public override void PopCard(List<Card> cardsList)
    {
        base.PopCard(cardsList);
        CardRemainCountShow();
    }

    public override void RoundEnter(bool isCanNoPlay)
    {
        base.RoundEnter(isCanNoPlay);
        switch (OrderController.Instance.CurInterationType)
        {
            case InterationType.CallLandlords:
                clock.Init(LandlordsPage.wait_CallLandlordsTime, 10, 5, null, false);
                break;
            case InterationType.QiangLandlords:
                clock.Init(LandlordsPage.wait_QiangTime, 10, 5, null, false);
                break;
            case InterationType.CallFen:
                clock.Init(LandlordsPage.wait_CallFenTime, 10, 5, null, false);
                break;
            case InterationType.PopCard:
                clock.Init(LandlordsPage.wait_PopTime, 10, 5, null, false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 展示自身的牌
    /// </summary>
    public void ShowCards(bool isShow)
    {
        if (isShow)
        {
            if (_handCard == null)
                return;
            net_protocol.DdzJSPlayerInfo result = LandlordsModel.Instance.ResultModel.GetResultInfos().Find(p => p.userId.ToString() == _handCard.playerInfo.uid);
            if (result == null)
                return;
            for (int i = 0; i < result.poker.Count; i++)
            {
                Card card = new Card(result.poker[i], _handCard.playerInfo.uid);
                CardUI ui = LandlordsPage.MakeSprite(card, false, resultCardsShow);
                ui.SetCardSize(new Vector2(145, 190));
                ui.Card.IsSprite = false;
                ui.name = (i + 1).ToString();
            }
        }
        else
        {
            for (int i = 0; i < resultCardsShow.childCount; i++)
            {
                resultCardsShow.GetChild(i).GetComponent<CardUI>().Destroy();
            }
        }
    }

    /// <summary>
    /// 手牌数量显示更新
    /// </summary>
    public void CardRemainCountShow()
    {
        cardCountLb.text = _handCard.CardsCount.ToString();
    }
}
