using UnityEngine;
using System.Collections;
using net_protocol;
using System.Collections.Generic;

/// <summary>
/// 牌类
/// </summary>
[System.Serializable]
public class Card
{
    private string belongTo;
    private bool makedSprite = false;
    private DdzPoker poker;

    //临时
    public Weight weight;
    public Suits suit;

    public Card(DdzPoker card, string uid)
    {
        this.belongTo = uid;
        this.poker = card;
        weight = GetCardWeight;
        suit =GetCardSuit;
    }

    /// <summary>
    /// 返回牌名
    /// </summary>
    public string GetCardName
    {
        get
        {
            string name = string.Empty;
            if (GetCardWeight == Weight.SJoker || GetCardWeight == Weight.LJoker)
                name = GetCardWeight.ToString();
            else
                name = GetCardSuit + GetCardWeight.ToString();
            return name;
        }
    }

    public DdzPoker Poker
    {
        get { return poker; }
    }

    /// <summary>
    /// 返回权值
    /// </summary>
    public Weight GetCardWeight
    {
        //get { return weight; }
        get { return GetWeightByPoker(poker.ds); }
    }

    /// <summary>
    /// 返回花色
    /// </summary>
    public Suits GetCardSuit
    {
        //get { return suit;}
        get { return (Suits)(poker.hs - 1); }
    }

    /// <summary>
    /// 牌的归属
    /// </summary>
    public string Attribution
    {
        set { belongTo = value; }
        get { return belongTo; }
    }

    /// <summary>
    /// 是否地主牌
    /// </summary>
    public bool isDzCard
    {
        get
        {
            CardUI[] dipais = LandlordsPage.Instance.playView.dipaiDesk.GetComponentsInChildren<CardUI>();
            if (dipais != null && dipais.Length > 0)
            {
                for (int i = 0; i < dipais.Length; i++)
                {
                    if (dipais[i].Card.GetCardWeight == GetCardWeight && dipais[i].Card.GetCardSuit == GetCardSuit)
                        return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 是否精灵化
    /// </summary>
    public bool IsSprite
    {
        set { makedSprite = value; }
        get { return makedSprite; }
    }

    /// <summary>
    /// 根据服务器ds得到客户端卡牌权值
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public static Weight GetWeightByPoker(int ds)
    {
        if (ds == 1)
            return Weight.One;
        else if (ds == 2)
            return Weight.Two;
        else if (ds == 3)
            return Weight.Three;
        else if (ds == 4)
            return Weight.Four;
        else if (ds == 5)
            return Weight.Five;
        else if (ds == 6)
            return Weight.Six;
        else if (ds == 7)
            return Weight.Seven;
        else if (ds == 8)
            return Weight.Eight;
        else if (ds == 9)
            return Weight.Nine;
        else if (ds == 10)
            return Weight.Ten;
        else if (ds == 11)
            return Weight.Jack;
        else if (ds == 12)
            return Weight.Queen;
        else if (ds == 13)
            return Weight.King;
        else if (ds == 14)
            return Weight.SJoker;
        else if (ds == 15)
            return Weight.LJoker;
        return Weight.One;
    }
}
