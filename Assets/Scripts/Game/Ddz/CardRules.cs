using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 出牌规则
/// </summary>
public class CardRules
{
    /// <summary>
    /// 卡牌数组排序
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static void SortCards(List<Card> cards, bool ascending)
    {
        cards.Sort(
            (Card a, Card b) =>
            {
                if (!ascending)
                {
                    //先按照权重降序，再按花色升序
                    return -a.GetCardWeight.CompareTo(b.GetCardWeight) * 2 +
                        a.GetCardSuit.CompareTo(b.GetCardSuit);
                }
                else
                    //按照权重升序
                    return a.GetCardWeight.CompareTo(b.GetCardWeight);
            }
        );
    }

    /// <summary>
    /// 是否是单
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsSingle(Card[] cards)
    {
        if (cards.Length == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 是否是对子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsDouble(Card[] cards)
    {
        if (cards.Length == 2)
        {
            if (cards[0].GetCardWeight == cards[1].GetCardWeight)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否四带二
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsFourAndTwo(Card[] cards)
    {
        if (cards.Length != 6)
            return false;

        if (cards[0].GetCardWeight == cards[1].GetCardWeight &&
            cards[1].GetCardWeight == cards[2].GetCardWeight && cards[2].GetCardWeight == cards[3].GetCardWeight)
        {
            return true;
        }
        else if (cards[1].GetCardWeight == cards[2].GetCardWeight &&
            cards[2].GetCardWeight == cards[3].GetCardWeight && cards[3].GetCardWeight == cards[4].GetCardWeight)
        {
            return true;
        }
        else if (cards[2].GetCardWeight == cards[3].GetCardWeight &&
            cards[3].GetCardWeight == cards[4].GetCardWeight && cards[4].GetCardWeight == cards[5].GetCardWeight)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 是否是顺子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsStraight(Card[] cards)
    {
        if (cards.Length < 5 || cards.Length > 12)
            return false;
        for (int i = 0; i < cards.Length - 1; i++)
        {
            Weight w = cards[i].GetCardWeight;
            if (cards[i + 1].GetCardWeight - w != 1)
                return false;

            //不能超过A
            if (w > Weight.One || cards[i + 1].GetCardWeight > Weight.One)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 是否是双顺子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsDoubleStraight(Card[] cards)
    {
        if (cards.Length < 6 || cards.Length % 2 != 0)
            return false;

        for (int i = 0; i < cards.Length; i += 2)
        {
            if (cards[i + 1].GetCardWeight != cards[i].GetCardWeight)
                return false;

            if (i < cards.Length - 2)
            {
                if (cards[i + 2].GetCardWeight - cards[i].GetCardWeight != 1)
                    return false;

                //不能超过A
                if (cards[i].GetCardWeight > Weight.One || cards[i + 2].GetCardWeight > Weight.One)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 飞机不带
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsTripleStraight(Card[] cards)
    {
        if (cards.Length < 6 || cards.Length % 3 != 0)
            return false;

        for (int i = 0; i < cards.Length - 3; i += 3)
        {
            if (cards[i].GetCardWeight != cards[i + 1].GetCardWeight)
                return false;
            if (cards[i + 2].GetCardWeight != cards[i + 1].GetCardWeight)
                return false;
            if (cards[i].GetCardWeight != cards[i + 2].GetCardWeight)
                return false;

            if (cards[i + 3].GetCardWeight - cards[i].GetCardWeight != 1)
                return false;
            //不能超过2
            if (cards[i].GetCardWeight > Weight.Two || cards[i + 3].GetCardWeight > Weight.Two)
                return false;
        }

        return true;
    }



    /// <summary>
    /// 飞机带1
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsTripleStraightDaiOne(Card[] cards)
    {
        if (cards.Length <= 6)
            return false;

        List<Card> cardsList = new List<Card>();
        cardsList.AddRange(cards);
        //1.先排序  111 222 333 j q k
        SortFly(cardsList);

        //2.比较
        int threeCount = 0;
        for (int i = 0; i < cardsList.Count; i++)
        {
            if (i < cardsList.Count - 1 && cardsList[i].GetCardWeight != cardsList[i + 1].GetCardWeight && cardsList.FindAll(p => cardsList[i].GetCardWeight == p.GetCardWeight).Count == 3 && cardsList[i].GetCardWeight != Weight.Two)
                threeCount++;
        }
        if (threeCount < 2)
            return false;

        //比较前面格式
        List<Card> qianmian = new List<Card>();
        qianmian.AddRange(cardsList.FindAll(p => cardsList.IndexOf(p) < threeCount * 3));
        for (int i = 0; i < qianmian.Count - 3; i += 3)
        {
            if (qianmian[i + 3].GetCardWeight - qianmian[i].GetCardWeight != 1)
                return false;
        }

        //比较后面格式
        if (cardsList.Count == threeCount * 3 + threeCount * 1)
        {//333 444 12
            for (int i = 0; i < threeCount; i++)
            {
                Card card = cardsList[threeCount * 3 + i];
                if (cardsList.FindAll(p => p.GetCardWeight == card.GetCardWeight).Count>1)
                    return false;
            }
            return true;
        }       
        return false;

    }

    public static bool IsTripleStraightDaiTwo(Card[] cards)
    {
        if (cards.Length <= 6)
            return false;

        List<Card> cardsList = new List<Card>();
        cardsList.AddRange(cards);
        //1.先排序  111 222 333 j q k
        SortFly(cardsList);

        //2.比较
        int threeCount = 0;
        for (int i = 0; i < cardsList.Count; i++)
        {
            if (i < cardsList.Count - 1 && cardsList[i].GetCardWeight != cardsList[i + 1].GetCardWeight && cardsList.FindAll(p => cardsList[i].GetCardWeight == p.GetCardWeight).Count == 3)
                threeCount++;
        }
        if (threeCount < 2)
            return false;

        //比较前面格式
        List<Card> qianmian = new List<Card>();
        qianmian.AddRange(cardsList.FindAll(p => cardsList.IndexOf(p) < threeCount * 3));
        for (int i = 0; i < qianmian.Count - 3; i += 3)
        {
            if (qianmian[i + 3].GetCardWeight - qianmian[i].GetCardWeight != 1)
                return false;
        }

        if (cardsList.Count == threeCount * 3 + threeCount * 2)
        {//333 444 2255
            List<Card> houmian = new List<Card>();
            houmian.AddRange(cardsList.FindAll(p => cardsList.IndexOf(p) >= threeCount * 3));
            for (int i = 0; i <= houmian.Count - 2; i += 2)
            {
                if (houmian[i].GetCardWeight != houmian[i + 1].GetCardWeight || (i != 0 && houmian[i].GetCardWeight == houmian[i - 1].GetCardWeight))
                    return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 三不带
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsOnlyThree(Card[] cards)
    {
        if (cards.Length % 3 != 0)
            return false;
        if (cards[0].GetCardWeight != cards[1].GetCardWeight)
            return false;
        if (cards[1].GetCardWeight != cards[2].GetCardWeight)
            return false;
        if (cards[0].GetCardWeight != cards[2].GetCardWeight)
            return false;

        return true;
    }


    /// <summary>
    /// 三带一
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsThreeAndOne(Card[] cards)
    {
        if (cards.Length != 4)
            return false;

        if (cards[0].GetCardWeight == cards[1].GetCardWeight &&
            cards[1].GetCardWeight == cards[2].GetCardWeight)
            return true;
        else if (cards[1].GetCardWeight == cards[2].GetCardWeight &&
            cards[2].GetCardWeight == cards[3].GetCardWeight)
            return true;
        return false;
    }

    /// <summary>
    /// 三代二
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsThreeAndTwo(Card[] cards)
    {
        if (cards.Length != 5)
            return false;

        if (cards[0].GetCardWeight == cards[1].GetCardWeight &&
            cards[1].GetCardWeight == cards[2].GetCardWeight)
        {
            if (cards[3].GetCardWeight == cards[4].GetCardWeight)
                return true;
        }

        else if (cards[2].GetCardWeight == cards[3].GetCardWeight &&
            cards[3].GetCardWeight == cards[4].GetCardWeight)
        {
            if (cards[0].GetCardWeight == cards[1].GetCardWeight)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 炸弹
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsBoom(Card[] cards)
    {
        if (cards.Length != 4)
            return false;

        if (cards[0].GetCardWeight != cards[1].GetCardWeight)
            return false;
        if (cards[1].GetCardWeight != cards[2].GetCardWeight)
            return false;
        if (cards[2].GetCardWeight != cards[3].GetCardWeight)
            return false;

        return true;
    }


    /// <summary>
    /// 王炸
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsJokerBoom(Card[] cards)
    {
        if (cards.Length != 2)
            return false;
        if (cards[0].GetCardWeight == Weight.SJoker)
        {
            if (cards[1].GetCardWeight == Weight.LJoker)
                return true;
            return false;
        }
        else if (cards[0].GetCardWeight == Weight.LJoker)
        {
            if (cards[1].GetCardWeight == Weight.SJoker)
                return true;
            return false;
        }

        return false;
    }

    /// <summary>
    /// 判断是否符合出牌规则
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool PopEnable(Card[] cards, out CardsType type)
    {
        {
            type = CardsType.None;
            bool can = false;

            switch (cards.Length)
            {
                case 1:
                    if (IsSingle(cards))
                    {
                        type = CardsType.Single;
                        can = true;
                    }
                    break;
                case 2:
                    if (IsDouble(cards))
                    {
                        type = CardsType.Double;
                        can = true;
                    }
                    else if (IsJokerBoom(cards))
                    {
                        type = CardsType.JokerBoom;
                        can = true;
                    }
                    break;
                case 3:
                    if (IsOnlyThree(cards))
                    {
                        type = CardsType.OnlyThree;
                        can = true;
                    }
                    break;
                case 4:
                    if (IsBoom(cards))
                    {
                        type = CardsType.Boom;
                        can = true;
                    }
                    else if (IsThreeAndOne(cards))
                    {
                        type = CardsType.ThreeAndOne;
                        can = true;
                    }
                    break;
                case 5:
                    if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }
                    else if (IsThreeAndTwo(cards))
                    {
                        type = CardsType.ThreeAndTwo;
                        can = true;
                    }
                    break;
                case 6:
                    if (IsFourAndTwo(cards))
                    {
                        type = CardsType.FourAndTwo;
                        can = true;
                    }
                    else if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }
                    else if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }
                    else if (IsTripleStraight(cards))
                    {
                        type = CardsType.TripleStraight;
                        can = true;
                    }
                    break;
                case 7:
                    if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }
                    break;
                case 8:
                    if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }
                    else if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }
                    else if(IsTripleStraightDaiOne(cards))
                    {
                        type = CardsType.TripleStraightDaiOne;
                        can = true;
                    }
                    break;
                case 9:
                    if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }//777 888 999 
                    else if (IsTripleStraight(cards))
                    {
                        type = CardsType.TripleStraight;
                        can = true;
                    }
                    break;
                case 10:
                    if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }
                    else if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }
                    else if (IsTripleStraightDaiOne(cards))
                    {
                        type = CardsType.TripleStraightDaiOne;
                        can = true;
                    }
                    else if (IsTripleStraightDaiTwo(cards))
                    {
                        type = CardsType.TripleStraightDaiTwo;
                        can = true;
                    }
                    break;
                case 11:
                    if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }
                    break;
                case 12:
                    if (IsStraight(cards))
                    {
                        type = CardsType.Straight;
                        can = true;
                    }
                    else if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }// 444 555 666 777
                    else if (IsTripleStraight(cards))
                    {
                        type = CardsType.TripleStraight;
                        can = true;
                    }
                    else if (IsTripleStraightDaiOne(cards))
                    {
                        type = CardsType.TripleStraightDaiOne;
                        can = true;
                    }
                    break;
                case 13:
                    break;
                case 14:
                    if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }
                    break;
                case 15:
                    if (IsTripleStraight(cards))
                    {
                        type = CardsType.TripleStraight;
                        can = true;
                    }
                    else if (IsTripleStraightDaiOne(cards))
                    {
                        type = CardsType.TripleStraightDaiOne;
                        can = true;
                    }
                    else if (IsTripleStraightDaiTwo(cards))
                    {
                        type = CardsType.TripleStraightDaiTwo;
                        can = true;
                    }
                    break;
                case 16:
                    if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }
                    else if (IsTripleStraightDaiOne(cards))
                    {
                        type = CardsType.TripleStraightDaiOne;
                        can = true;
                    }
                    break;
                case 17:
                    break;
                case 18:
                    if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }// 444 555 666 777 888 999 
                    else if (IsTripleStraight(cards))
                    {
                        type = CardsType.TripleStraight;
                        can = true;
                    }
                    break;
                case 19:
                    break;
                case 20:
                    if (IsDoubleStraight(cards))
                    {
                        type = CardsType.DoubleStraight;
                        can = true;
                    }
                    else if (IsTripleStraightDaiOne(cards))
                    {
                        type = CardsType.TripleStraightDaiOne;
                        can = true;
                    }
                    else if (IsTripleStraightDaiTwo(cards))
                    {
                        type = CardsType.TripleStraightDaiTwo;
                        can = true;
                    }
                    break;
                default:
                    break;
            }
            if (!can)
                UIUtils.Log("不符合出牌规则");
            return can;


        }
    }


    /// <summary>
    /// 一手牌
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="isUpascending">是否从大牌开始出</param>
    /// <returns></returns>
    static List<Card> FirstCard(string uid, bool isUpascending = false)
    {
        List<Card> ret = new List<Card>();
        if (isUpascending)
        {
            for (int i = 12; i >= 5; i--)
            {
                ret = FindStraight(GetAllCards(uid), (int)Weight.Three, i, true);
                if (ret.Count != 0)
                    break;
            }
            if (ret.Count == 0)
            {
                for (int i = 0; i < 36; i += 3)
                {
                    ret = FindThreeAndTwo(uid, GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }
            if (ret.Count == 0)
            {
                for (int i = 0; i < 36; i += 3)
                {
                    ret = FindThreeAndOne(uid, GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }

            if (ret.Count == 0)
            {
                for (int i = 0; i < 36; i += 3)
                {
                    ret = FindOnlyThree(GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }

            if (ret.Count == 0)
            {
                for (int i = 0; i < 24; i += 2)
                {
                    ret = FindDouble(GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }

            if (ret.Count == 0)
            {
                ret = FindSingle(GetAllCards(uid), (int)Weight.Three, true);
            }

            return ret;
        }
        else
        {
            if (ret.Count == 0)
            {
                ret = FindSingle(GetAllCards(uid), (int)Weight.Three, true);
            }
            if (ret.Count == 0)
            {
                for (int i = 0; i < 24; i += 2)
                {
                    ret = FindDouble(GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }
            if (ret.Count == 0)
            {
                for (int i = 0; i < 36; i += 3)
                {
                    ret = FindOnlyThree(GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }
            if (ret.Count == 0)
            {
                for (int i = 0; i < 36; i += 3)
                {
                    ret = FindThreeAndOne(uid, GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }
            if (ret.Count == 0)
            {
                for (int i = 0; i < 36; i += 3)
                {
                    ret = FindThreeAndTwo(uid, GetAllCards(uid), i, true);
                    if (ret.Count != 0)
                        break;
                }
            }
            if (ret.Count == 0)
            {
                for (int i = 12; i >= 5; i--)
                {
                    ret = FindStraight(GetAllCards(uid), (int)Weight.Three, i, true);
                    if (ret.Count != 0)
                        break;
                }
            }

            return ret;
        }
    }

    /// <summary>
    /// 检测当前可以出什么出牌
    /// </summary>
    /// <returns></returns>
    public static List<Card> DelayDiscardCard(string uid, List<Card> exclude = null)
    {
        List<Card> canPopList = new List<Card>();
        CardsType rule = DeskCardsCache.Instance.Rule;
        int deskWeight = DeskCardsCache.Instance.TotalWeight;
        //根据桌面牌的类型和权值大小出牌
        switch (rule)
        {
            case CardsType.None:
                canPopList = FirstCard(uid, true);
                break;
            case CardsType.JokerBoom:
                break;
            case CardsType.Boom:
                canPopList = FindBoom(GetAllCards(uid, exclude), deskWeight, false);
                break;
            case CardsType.Double:
                canPopList = FindDouble(GetAllCards(uid, exclude), deskWeight, false);
                break;
            case CardsType.Single:
                canPopList = FindSingle(GetAllCards(uid, exclude), deskWeight, false);
                break;
            case CardsType.OnlyThree:
                canPopList = FindOnlyThree(GetAllCards(uid, exclude), deskWeight, false);
                break;
            case CardsType.ThreeAndOne:
                canPopList = FindThreeAndOne(uid, GetAllCards(uid, exclude), deskWeight, false);
                break;
            case CardsType.ThreeAndTwo:
                canPopList = FindThreeAndTwo(uid, GetAllCards(uid, exclude), deskWeight, false);
                break;
            case CardsType.FourAndTwo:
                canPopList = FindFourAndTwo(uid, GetAllCards(uid, exclude), deskWeight, false);
                break;
            case CardsType.Straight:
                canPopList = FindStraight(GetAllCards(uid, exclude), DeskCardsCache.Instance.MinWeight, DeskCardsCache.Instance.CardsCount, false);
                break;
            case CardsType.DoubleStraight:
                canPopList = FindDoubleStraight(GetAllCards(uid, exclude), DeskCardsCache.Instance.MinWeight, DeskCardsCache.Instance.CardsCount);
                break;
            case CardsType.TripleStraight:
                canPopList = FindBoom(GetAllCards(uid, exclude), 0, true);
                break;
            case CardsType.TripleStraightDaiOne:
                canPopList = FindBoom(GetAllCards(uid, exclude), 0, true);
                break;
            case CardsType.TripleStraightDaiTwo:
                canPopList = FindBoom(GetAllCards(uid, exclude), 0, true);
                break;
            default:
                break;
        }
        if (rule != CardsType.JokerBoom && rule != CardsType.Boom && canPopList.Count == 0)
        {
            canPopList = FindBoom(GetAllCards(uid, exclude), 0, true);
        }
        return canPopList;
    }



    /// <summary>
    /// 获取所有手牌
    /// </summary>
    /// <returns></returns>
    static List<Card> GetAllCards(string uid, List<Card> exclude = null)
    {
        List<Card> cards = new List<Card>();
        LandkirdsHandCardModel allCards = LandlordsModel.Instance.GetHandCardMode(uid);

        bool isContinue = false;
        for (int i = 0; i < allCards.CardsCount; i++)
        {
            isContinue = false;
            if (exclude != null)
            {
                for (int j = 0; j < exclude.Count; j++)
                {
                    if (allCards[i] == exclude[j])
                    {
                        isContinue = true;
                        break;
                    }

                }
            }
            if (!isContinue)
                cards.Add(allCards[i]);
        }
        //从小到大排序
        CardRules.SortCards(cards, true);
        return cards;      
    }

    

    /// <summary>
    /// 找到手牌中符合要求的炸弹
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static List<Card> FindBoom(List<Card> allCards, int weight, bool equal)
    {
        List<Card> ret = new List<Card>();       

        SortCards(allCards, false);
        if (allCards.Count >= 4)
        {
            for (int i = 0; i < allCards.Count; i++)
            {
                if (i <= allCards.Count - 4)
                {
                    //先找普通炸弹
                    if (allCards[i].GetCardWeight == allCards[i + 1].GetCardWeight &&
                        allCards[i].GetCardWeight == allCards[i + 2].GetCardWeight &&
                        allCards[i].GetCardWeight == allCards[i + 3].GetCardWeight)
                    {
                        int totalWeight = (int)allCards[i].GetCardWeight + (int)allCards[i + 1].GetCardWeight + (int)allCards[i + 2].GetCardWeight
                            + (int)allCards[i + 3].GetCardWeight;
                        if (equal)
                        {
                            if (totalWeight >= weight)
                            {
                                ret.Add(allCards[i]);
                                ret.Add(allCards[i + 1]);
                                ret.Add(allCards[i + 2]);
                                ret.Add(allCards[i + 3]);
                                break;
                            }
                        }
                        else
                        {
                            if (totalWeight > weight)
                            {
                                ret.Add(allCards[i]);
                                ret.Add(allCards[i + 1]);
                                ret.Add(allCards[i + 2]);
                                ret.Add(allCards[i + 3]);
                                break;
                            }
                        }

                    }
                }
            }
        }
        //找王炸
        if (ret.Count == 0)
        {
            for (int j = 0; j < allCards.Count; j++)
            {
                if (j < allCards.Count - 1)
                {
                    if (allCards[j].GetCardWeight == Weight.LJoker &&
                        allCards[j + 1].GetCardWeight == Weight.SJoker)
                    {
                        ret.Add(allCards[j]);
                        ret.Add(allCards[j + 1]);
                    }
                }

            }
        }
        //找王炸
        if (ret.Count == 0)
        {
            for (int j = 0; j < allCards.Count; j++)
            {
                if (j < allCards.Count - 1)
                {
                    if (allCards[j].GetCardWeight == Weight.SJoker &&
                        allCards[j + 1].GetCardWeight == Weight.LJoker)
                    {
                        ret.Add(allCards[j]);
                        ret.Add(allCards[j + 1]);
                    }
                }

            }
        }
        return ret;
    }

    /// <summary>
    /// 找到手牌中符合要求的是对子
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    static List<Card> FindDouble(List<Card> allCards, int weight, bool equal)
    {
        List<Card> ret = new List<Card>();
        for (int i = 0; i < allCards.Count; i++)
        {
            if (i < allCards.Count - 1)
            {
                if (allCards[i].GetCardWeight == allCards[i + 1].GetCardWeight)
                {
                    int totalWeight = (int)allCards[i].GetCardWeight + (int)allCards[i + 1].GetCardWeight;
                    if (equal)
                    {
                        if (totalWeight >= weight)
                        {
                            ret.Add(allCards[i]);
                            ret.Add(allCards[i + 1]);
                            break;
                        }
                    }
                    else
                    {
                        if (totalWeight > weight)
                        {
                            ret.Add(allCards[i]);
                            ret.Add(allCards[i + 1]);
                            break;
                        }
                    }

                }
            }
        }

        return ret;
    }

    /// <summary>
    /// 找到手牌中符合要求的是单牌
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    static List<Card> FindSingle(List<Card> allCards, int weight, bool equal)
    {
        List<Card> ret = new List<Card>();
        for (int i = 0; i < allCards.Count; i++)
        {
            if (equal)
            {
                if ((int)allCards[i].GetCardWeight >= weight)
                {
                    ret.Add(allCards[i]);
                    break;
                }
            }
            else
            {
                if ((int)allCards[i].GetCardWeight > weight)
                {
                    ret.Add(allCards[i]);
                    break;
                }
            }

        }
        return ret;
    }

    /// <summary>
    /// 找到手牌中符合要求的是3章
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    static List<Card> FindOnlyThree(List<Card> allCards, int weight, bool equal)
    {
        List<Card> ret = new List<Card>();
        for (int i = 0; i < allCards.Count; i++)
        {
            if (i <= allCards.Count - 3)
            {
                if (allCards[i].GetCardWeight == allCards[i + 1].GetCardWeight &&
                    allCards[i].GetCardWeight == allCards[i + 2].GetCardWeight)
                {
                    int totalWeight = (int)allCards[i].GetCardWeight +
                        (int)allCards[i + 1].GetCardWeight +
                        (int)allCards[i + 2].GetCardWeight;

                    if (equal)
                    {
                        if (totalWeight >= weight)
                        {
                            ret.Add(allCards[i]);
                            ret.Add(allCards[i + 1]);
                            ret.Add(allCards[i + 2]);
                            break;
                        }
                    }
                    else
                    {
                        if (totalWeight > weight)
                        {
                            ret.Add(allCards[i]);
                            ret.Add(allCards[i + 1]);
                            ret.Add(allCards[i + 2]);
                            break;
                        }
                    }

                }
            }
        }

        return ret;
    }

    /// <summary>
    /// 找到手牌中符合要求的是连子
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    static List<Card> FindStraight(List<Card> allCards, int minWeight, int length, bool equal)
    {
        List<Card> ret = new List<Card>();
        int counter = 1;
        List<int> indeies = new List<int>();
        for (int i = 0; i < allCards.Count; i++)
        {
            if (i < allCards.Count - 4)
            {
                int weight = (int)allCards[i].GetCardWeight;
                if (equal)
                {
                    if (weight >= minWeight)
                    {
                        counter = 1;
                        indeies.Clear();

                        for (int j = i + 1; j < allCards.Count; j++)
                        {
                            if (allCards[j].GetCardWeight > Weight.One)
                                break;

                            if ((int)allCards[j].GetCardWeight - weight == counter)
                            {
                                counter++;
                                indeies.Add(j);
                            }

                            if (counter == length)
                                break;
                        }
                    }
                }
                else
                {
                    if (weight > minWeight)
                    {
                        counter = 1;
                        indeies.Clear();

                        for (int j = i + 1; j < allCards.Count; j++)
                        {
                            if (allCards[j].GetCardWeight > Weight.One)
                                break;
                            if ((int)allCards[j].GetCardWeight - weight == counter)
                            {
                                counter++;
                                indeies.Add(j);
                            }

                            if (counter == length)
                                break;
                        }
                    }
                }

            }
            if (counter == length)
            {
                indeies.Insert(0, i);
                break;
            }

        }

        if (counter == length)
        {
            for (int i = 0; i < indeies.Count; i++)
            {
                ret.Add(allCards[indeies[i]]);
            }
        }

        return ret;
    }

    /// <summary>
    /// 找到手牌中符合要求的是双连子
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    static List<Card> FindDoubleStraight(List<Card> allCards, int minWeight, int length)
    {
        List<Card> ret = new List<Card>();
        int counter = 0;
        List<int> indeies = new List<int>();
        for (int i = 0; i < allCards.Count; i++)
        {
            if (i < allCards.Count - 4)
            {
                int weight = (int)allCards[i].GetCardWeight;
                if (weight > minWeight)
                {
                    counter = 0;
                    indeies.Clear();

                    int circle = 0;
                    for (int j = i + 1; j < allCards.Count; j++)
                    {
                        if (allCards[j].GetCardWeight > Weight.One)
                            break;

                        if ((int)allCards[j].GetCardWeight - weight == counter)
                        {
                            circle++;
                            if (circle % 2 == 1)
                            {
                                counter++;
                            }
                            indeies.Add(j);
                        }

                        if (counter == length / 2)
                            break;
                    }
                }
            }
            if (counter == length / 2)
            {
                indeies.Insert(0, i);
                break;
            }

        }

        if (counter == length / 2)
        {
            for (int i = 0; i < indeies.Count; i++)
            {
                ret.Add(allCards[indeies[i]]);
            }
        }

        return ret;
    }


    /// <summary>
    /// 三代二
    /// </summary>
    /// <param name="allCards"></param>
    /// <param name="weight"></param>
    /// <param name="equal"></param>
    /// <returns></returns>
    static List<Card> FindThreeAndTwo(string uid, List<Card> allCards, int weight, bool equal)
    {
        List<Card> three = FindOnlyThree(allCards, weight, equal);
        if (three.Count != 0)
        {
            List<Card> leftCards = GetAllCards(uid, three);
            List<Card> two = FindDouble(leftCards, (int)Weight.Three, true);

            three.AddRange(two);
        }
        else
            three.Clear();
        if (three.Count < 5)
            three.Clear();
        return three;

    }

    /// <summary>
    /// 三带一
    /// </summary>
    /// <param name="allCards"></param>
    /// <param name="weight"></param>
    /// <param name="equal"></param>
    /// <returns></returns>
    static List<Card> FindThreeAndOne(string uid, List<Card> allCards, int weight, bool equal)
    {
        List<Card> three = FindOnlyThree(allCards, weight, equal);
        if (three.Count != 0)
        {
            List<Card> leftCards = GetAllCards(uid, three);
            List<Card> one = FindSingle(leftCards, (int)Weight.Three, true);
            three.AddRange(one);
        }
        else
            three.Clear();

        return three;
    }

    /// <summary>
    /// 四带2
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="allCards"></param>
    /// <param name="weight"></param>
    /// <param name="equal"></param>
    /// <returns></returns>
    public static List<Card> FindFourAndTwo(string uid, List<Card> allCards, int weight, bool equal)
    {
        List<Card> four = FindBoom(allCards, weight, equal);
        if (four.Count != 0)
        {
            List<Card> leftCards = GetAllCards(uid, four);
            List<Card> one = FindDouble(leftCards, (int)Weight.Three, true);
            if (one.Count == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    List<Card> single = FindSingle(leftCards, (int)Weight.Three, true);
                    four.AddRange(single);
                    leftCards = GetAllCards(uid, four);
                    one.AddRange(single);
                }
            }
            else
                four.AddRange(one);
            if (four.Count < 6)
                four.Clear();
        }
        else
            four.Clear();

        return four;
    }

    /// <summary>
    /// 飞机排序
    /// </summary>
    /// <returns></returns>
    public static void SortFly(List<Card> cards)
    {
        cards.Sort((a, b) =>
        {
            int a_count = cards.FindAll(p => p.GetCardWeight == a.GetCardWeight).Count;
            int b_count = cards.FindAll(p => p.GetCardWeight == b.GetCardWeight).Count;
            if (a_count > b_count)
                return -1;
            else if (a_count < b_count)
                return 1;
            else if (a_count == b_count)
                if (a.GetCardWeight < b.GetCardWeight)
                    return -1;
            return 0;
        });
    }
}
