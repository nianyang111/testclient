using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 出牌管理器
/// </summary>
public class PlayCard
{
    static PlayCard instance;

    public static PlayCard Instance
    {
        get
        {
            if (instance == null)
                instance = new PlayCard();
            return instance;
        }
    }

    //出牌者
    private LandkirdsHandCardModel player;
    //出牌列表
    private List<Card> selectedCardsList;

    /// <summary>
    /// 出牌
    /// </summary>
    public void PopCard(string uid, List<Card> selectedCardsList)
    {
        Clear();
        Init(uid, selectedCardsList);
        CheckSelectCards();
    }


    void Init(string uid, List<Card> selectedCardsList)
    {
        this.player = LandlordsModel.Instance.GetHandCardMode(uid);
        this.selectedCardsList = selectedCardsList;        
    }

    void Clear()
    {
        selectedCardsList = null;
    }



    /// <summary>
    /// 遍历选中的牌
    /// </summary>
    void CheckSelectCards()
    {
        //排好序
        CardRules.SortCards(selectedCardsList, true);

        CardsType type = CardsType.None;
        //出牌
        CheckPlayCards(selectedCardsList, out type);//这里不用判断能否出牌,因为selectedCardsList是服务器已经判断过能出的牌
        PlayCards(type);
    }

    /// <summary>
    /// 检测玩家出牌
    /// </summary>
    bool CheckPlayCards(List<Card> selectedCardsLists, out CardsType type)
    {
        bool isCanPlayCard = false;
        Card[] selectedCardsArray = selectedCardsLists.ToArray();
        //检测是否符合出牌规则
        //CardsType type;
        if (CardRules.PopEnable(selectedCardsArray, out type))
        {
            CardsType rule = DeskCardsCache.Instance.Rule;
            if (OrderController.Instance.BiggestUid == OrderController.Instance.TypeUid)
            {
                isCanPlayCard = true;
            }
            else if (DeskCardsCache.Instance.Rule == CardsType.None)
            {
                isCanPlayCard = true;
            }
            else if (type == rule && selectedCardsLists.Count == DeskCardsCache.Instance.CardsCount && LandlordsModel.Instance.GetWeight(selectedCardsArray, type) > DeskCardsCache.Instance.TotalWeight)
            {
                isCanPlayCard = true;
            }
            //飞机带1
            else if (type == rule && type == CardsType.TripleStraightDaiOne)
            {
                if (selectedCardsLists.Count == DeskCardsCache.Instance.CardsCount && LandlordsModel.Instance.GetWeight(selectedCardsArray, type) > DeskCardsCache.Instance.TotalWeight)
                    isCanPlayCard = true;
            }
            //飞机带2
            else if (type == rule && type == CardsType.TripleStraightDaiTwo)
            {
                if (selectedCardsLists.Count == DeskCardsCache.Instance.CardsCount && LandlordsModel.Instance.GetWeight(selectedCardsArray, type) > DeskCardsCache.Instance.TotalWeight)
                    isCanPlayCard = true;
            }
            //炸弹
            else if (type == CardsType.Boom && rule != CardsType.Boom)
            {
                isCanPlayCard = true;
            }
            else if (type == CardsType.JokerBoom)
            {
                isCanPlayCard = true;
            }
            else if (type == CardsType.Boom && rule == CardsType.Boom &&
               LandlordsModel.Instance.GetWeight(selectedCardsArray, type) > DeskCardsCache.Instance.TotalWeight)
            {
                isCanPlayCard = true;
            }
        }

        return isCanPlayCard;
    }

    /// <summary>
    /// 玩家出牌
    /// </summary>
    /// <param name="selectedCardsList"></param>
    /// <param name="selectedSpriteList"></param>
    void PlayCards(CardsType type)
    {
        //如果符合将牌从手牌移到出牌缓存区      
        DeskCardsCache.Instance.Clear();
        DeskCardsCache.Instance.Rule = type;

        for (int i = 0; i < selectedCardsList.Count; i++)
        {
            //先进行卡牌移动
            player.PopCard(selectedCardsList[i]);            
            DeskCardsCache.Instance.AddCard(selectedCardsList[i]);
        }
        ChangeRatio(type);
        PlayAniByCarsType(type);
        //临时try
        try
        {
            PlaySound(type, (Weight)DeskCardsCache.Instance.MinWeight);
        }
        catch
        {
            
        }
        DeskCardsCache.Instance.Sort();
        if (player.CardsCount != 0)
            OrderController.Instance.BiggestUid = player.playerInfo.uid;

    }


    /// <summary>
    /// 主角是否能出牌
    /// </summary>
    public static bool IsCanPop(List<Card> cards)
    {
        //排好序
        CardRules.SortCards(cards, true);

        CardsType type = CardsType.None;

        bool isCanPlayCard = PlayCard.Instance.CheckPlayCards(cards, out type);
        if (!isCanPlayCard)
            UIUtils.Log("不能大过场上的牌或者和场上的牌型不一致");
        return isCanPlayCard;
    }


    /// <summary>
    /// 根据牌型改变倍率
    /// </summary>
    void ChangeRatio(CardsType type)
    {
        switch (type)
        {
            case CardsType.None:
                break;
            case CardsType.JokerBoom:
                LandlordsPage.Instance.Multiples *= 2;
                break;
            case CardsType.Boom:
                LandlordsPage.Instance.Multiples *= 2;
                break;
        }
    }

    /// <summary>
    /// 根据牌型播放动画
    /// </summary>
    void PlayAniByCarsType(CardsType type)
    {        
        switch (type)
        {
            case CardsType.JokerBoom:
                NodeManager.OpenNode<LandlordsEffectNode>(null, null, false).Inits(LandlordsEventAniType.JokerBoom, 0, 0, player.playerInfo.icon);
                break;
            case CardsType.Boom:
                NodeManager.OpenNode<LandlordsEffectNode>(null, null, false).Inits(LandlordsEventAniType.Boom, 1, 0);
                break;
            case CardsType.Straight:
                NodeManager.OpenNode<LandlordsEffectNode>(null, null, false).Inits(LandlordsEventAniType.Straight, 0, 0);
                break;
            case CardsType.DoubleStraight:
                NodeManager.OpenNode<LandlordsEffectNode>(null, null, false).Inits(LandlordsEventAniType.DoubleStraight, 0, 0);
                break;
            case CardsType.TripleStraight:
            case CardsType.TripleStraightDaiOne:
            case CardsType.TripleStraightDaiTwo:
                NodeManager.OpenNode<LandlordsEffectNode>(null, null, false).Inits(LandlordsEventAniType.Fly, 0, 0, player.playerInfo.icon);
                break;
        }

    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="type"></param>
    /// <param name="?"></param>
    /// <param name="weight"></param>
    void PlaySound(CardsType type, Weight weight)
    {
        LandlordsModel.Instance.PlaySound(AudioManager.AudioSoundType.popCard);
        AudioManager.AudioSoundType soundType = AudioManager.AudioSoundType.None;
        if (type == CardsType.Single || type == CardsType.Double)
        {
            soundType = LandlordsSoundModel.PlayPlayerPopSound(player.playerInfo.uid, type, weight);
        }
        else
        {
            soundType = LandlordsSoundModel.PlayPlayerPopSound(player.playerInfo.uid, type);
        }
        LandlordsModel.Instance.PlaySound(soundType);

        //就剩2张牌
        if (player.CardsCount == 2)
        {
            AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.warring, PageManager.Instance.CurrentPage.name);
            LandlordsModel.Instance.PlaySound(player.Six == Six.boy ? AudioManager.AudioSoundType.boyTwocard : AudioManager.AudioSoundType.girlTwocard);
        }
        else if (player.CardsCount == 1)
        {
            LandlordsModel.Instance.PlaySound(player.Six == Six.boy ? AudioManager.AudioSoundType.boyOnecard : AudioManager.AudioSoundType.girlOnecard);
        }
        if (player.CardsCount <= 2 && player.CardsCount > 0)
            LandlordsPage.Instance.LandlordsWarning(player.playerInfo.uid, player.CardsCount, true);
    }


}
