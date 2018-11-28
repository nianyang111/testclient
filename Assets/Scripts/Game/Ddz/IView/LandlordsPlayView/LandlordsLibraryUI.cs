using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 牌库显示层
/// </summary>
public class LandlordsLibraryUI : MonoBehaviour 
{
    /// <summary>
    /// 我的牌
    /// </summary>
    private List<CardUI> myCards = new List<CardUI>();
    /// <summary>
    /// 玩家1(左)的牌
    /// </summary>
    private CardUI player1Cards;
    /// <summary>
    /// 玩家2(右)的牌
    /// </summary>
    private CardUI player2Cards;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        InitMyCards();
        InitOtherCards();
    }


    /// <summary>
    /// 初始化我的牌
    /// </summary>
    private void InitMyCards()
    {

    }

    /// <summary>
    /// 初始化其他玩家的牌
    /// </summary>
    private void InitOtherCards()
    {

    }
}
