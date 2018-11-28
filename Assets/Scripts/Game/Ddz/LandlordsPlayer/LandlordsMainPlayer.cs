using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandlordsMainPlayer : LandlordsBasePlayer {

    protected override void RestToZhunbei(bool isZhunbei)
    {
        DestroyCardUI();
        base.RestToZhunbei(isZhunbei);
    }

    public override void Zhunbei(bool isZhunbei)
    {
        if (isZhunbei)
            Interaction.Instance.SetChangeAndZhunbei(false);
        base.Zhunbei(isZhunbei);
    }

    public override void GameStart()
    {
        DestroyCardUI();
        base.GameStart();
    }

    public void Tips(List<Card> cards)
    {
        if (cards.Count > 0)
        {
            //1.全部卡牌设为不选中
            CardUI[] sprites = GetSpriteUIs();
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].Select)
                    sprites[i].OnClick();
            }
            //2.找出提示的卡牌,设为选中
            List<CardUI> cardUIs = GetSprite(cards);
            for (int i = 0; i < cardUIs.Count; i++)
                cardUIs[i].OnClick();
        }
        else
            Interaction.Instance.NoPlayCard(0);
    }

    public void Chongxuan()
    {
        //全部卡牌设为不选中
        CardUI[] sprites = GetSpriteUIs();
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].Select)
                sprites[i].OnClick();
        }
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="isDelay"></param>
    public override void DealCard(bool isDelay)
    {
        if (isDelay)
            StartCoroutine(DelayDealCard());
        else
            QuickDealCard();
        base.DealCard(isDelay);
    }


    // 协程发牌
    IEnumerator DelayDealCard()
    {
        AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.dealCard, PageManager.Instance.CurrentPage.name);
        for (int i = 0; i < _handCard.CardsCount; i++)
        {
            CardUI ui = LandlordsPage.MakeSprite(_handCard[i], false, handCard);
            if (ui)
                ui.SetCardSize(new Vector2(239, 325));
            yield return new WaitForSecondsRealtime(0.1f);
        }
        StartCoroutine(SpriteSort());
    }

    // 快速发牌
    void QuickDealCard()
    {
        for (int i = 0; i < _handCard.CardsCount; i++)
        {
            CardUI ui = LandlordsPage.MakeSprite(_handCard[i], false, handCard);
            if (ui)
                ui.SetCardSize(new Vector2(239, 325));
        }
        StartCoroutine(SpriteSort());
    }

    /// <summary>
    /// 卡牌精灵排序
    /// </summary>
    /// <param name="cType"></param>
    IEnumerator SpriteSort()
    {
        CardUI[] sprites = GetSpriteUIs();
        LandkirdsHandCardModel handCard = LandlordsModel.Instance.GetHandCardMode(UserInfoModel.userInfo.userId.ToString());
        handCard.Sort();
        for (int i = 0; i < handCard.CardsCount; i++)
        {
            for (int j = 0; j < sprites.Length; j++)
            {
                if (sprites[j].Card.Poker.hs == handCard[i].Poker.hs && sprites[j].Card.Poker.ds == handCard[i].Poker.ds)
                {
                    sprites[j].SetIndex(i);
                }
            }
        }
        yield return null;
        SetCardUISelect(sprites);
    }

    /// <summary>
    /// 主角请求出牌
    /// </summary>
    public bool MainRolePopReq(CallBack call)
    {
        CardUI[] sprites = GetSpriteUIs();

        //找出所有选中的牌
        List<Card> selectedCardsLists = new List<Card>();
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].Select)
            {
                selectedCardsLists.Add(sprites[i].Card);
            }
        }
        //排好序            

        CardRules.SortCards(selectedCardsLists, true);
        bool isCanPop = PlayCard.IsCanPop(selectedCardsLists);
        if (isCanPop)
        {
            LandlordsNet.C2G_PopReq(1, selectedCardsLists, call, null);
        }
        else
        {
            List<Card> exclude = new List<Card>();
            for (int i = 0; i < sprites.Length; i++)
            {
                if (!sprites[i].Select)
                {
                    exclude.Add(sprites[i].Card);
                }
            }
            List<Card> popCard = LandlordsModel.Instance.TipsModel.Tips2(exclude);
            isCanPop = popCard.Count > 0;
            if(isCanPop)
            {
                LandlordsNet.C2G_PopReq(1, popCard, call, null);
            }
        }
        return isCanPop;
    }

    public override void PopCard(List<Card> cardsList)
    {
        StartCoroutine(Pop(cardsList));
        base.PopCard(cardsList);
    }

    IEnumerator Pop(List<Card> cardsList)
    {
        List<CardUI> uis = GetSprite(cardsList);
        for (int i = 0; i < uis.Count; i++)
        {
            uis[i].Destroy();
        }
        Chongxuan();
        yield return new WaitForSecondsRealtime(Time.deltaTime * 2);
        SetCardUISelect(GetSpriteUIs());
    }

    // 自动出牌
    void AutoPop()
    {
        List<Card> cards = CardRules.DelayDiscardCard(UserInfoModel.userInfo.userId.ToString());
        bool isCanPop = cards.Count > 0;
        if (isCanPop)
        {
            List<Card> select = new List<Card>();
            List<CardUI> selectUis = new List<CardUI>();
            for (int i = 0; i < GetSpriteUIs().Length; i++)
            {
                if (GetSpriteUIs()[i].Select)
                {
                    select.Add(GetSpriteUIs()[i].Card);
                    selectUis.Add(GetSpriteUIs()[i]);
                }
            }
            //先检测已选择的牌可以出不
            if (PlayCard.IsCanPop(select))
            {
                LandlordsNet.C2G_PopReq(1, select, null, null);
            }
            else
            {
                CardRules.SortCards(cards, true);
                LandlordsNet.C2G_PopReq(1, cards, null, null);
            }
        }
        else
        {
            Interaction.Instance.NoPlayCard(0);
        }
        LandlordsModel.Instance.TimeOutAudioPopCount++;
    }


    // 自动不叫地主    
    void AutoCallLandlord()
    {
        LandlordsNet.C2G_PlayerCallReq(0);
    }

    // 检查是否是最后一手牌,是-->自动出
    public static bool CheckIsLastCanAutoPop()
    {
        List<Card> cards = CardRules.DelayDiscardCard(UserInfoModel.userInfo.userId.ToString());
        return LandlordsModel.Instance.MyInfo.CardsCount - cards.Count == 0;
    }

    public override void RoundEnter(bool isCanNoPlay)
    {
        base.RoundEnter(isCanNoPlay);
        switch (OrderController.Instance.CurInterationType)
        {
            case InterationType.CallLandlords:
                clock.Init(LandlordsPage.wait_CallLandlordsTime, 10, 5, null, true);
                break;
            case InterationType.QiangLandlords:
                clock.Init(LandlordsPage.wait_QiangTime, 10, 5, null, true);
                break;
            case InterationType.CallFen:
                clock.Init(LandlordsPage.wait_CallFenTime, 10, 5, () => AutoCallLandlord(), true);
                break;
            case InterationType.PopCard:
                if (LandlordsModel.Instance.IsTuoGuan)
                    //如果是托管状态
                    return;
                List<Card> cards = CardRules.DelayDiscardCard(UserInfoModel.userInfo.userId.ToString());
                if (CheckIsLastCanAutoPop())
                {
                    SetTimeout.add(1.5f, () =>
                        {
                            AutoPop();
                        });
                    return;
                }
                if (cards.Count == 0)
                    clock.Init(LandlordsPage.wait_canNotPopTime, 4, 5, () => AutoPop(), true);
                else
                    clock.Init(LandlordsPage.wait_PopTime, 10, 5, () => AutoPop(), true);
                break;
            default:
                break;
        }
    }

    #region 辅助工具
    // 设置已选择卡牌的位置
    void SetCardUISelect(CardUI[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].Select)
            {
                sprites[i].transform.localPosition += Vector3.up * 30;
            }
        }
    }    

    // 获得card对应的精灵
    List<CardUI> GetSprite(List<Card> cards)
    {
        CardUI[] sprites = GetSpriteUIs();

        List<CardUI> selectedSpriteList = new List<CardUI>();

        for (int i = 0; i < sprites.Length; i++)
        {
            for (int j = 0; j < cards.Count; j++)
            {
                if (cards[j].Poker.hs == sprites[i].Card.Poker.hs && cards[j].Poker.ds == sprites[i].Card.Poker.ds)
                {
                    selectedSpriteList.Add(sprites[i]);
                    break;
                }
            }
        }

        return selectedSpriteList;
    }

    /// <summary>得到手牌显示层</summary>
    public CardUI[] GetSpriteUIs()
    {
        return handCard.GetComponentsInChildren<CardUI>();
    }

    /// <summary>销毁手牌所有卡牌精灵</summary>
    public void DestroyCardUI()
    {
        CardUI[] sprs = GetSpriteUIs();
        if (sprs != null)
        {
            for (int i = 0; i < sprs.Length; i++)
            {
                sprs[i].Destroy();
            }
        }
    }

    #endregion

}
