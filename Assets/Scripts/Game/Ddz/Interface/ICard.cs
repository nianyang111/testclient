using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard  {
    /// <summary>
    /// 获取牌的数量
    /// </summary>
    int CardsCount
    {
        get;
    }
    /// <summary>
    /// 索引器
    /// </summary>
    Card this[int index]
    {
        get;
    }
    /// <summary>
    /// 发牌
    /// </summary>
    Card Deal();
    /// <summary>
    /// 添加牌
    /// </summary>
    /// <param name="card"></param>
    void AddCard(Card card);
    /// <summary>
    /// 排序
    /// </summary>
    void Sort();
}
