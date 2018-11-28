using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 出牌顺序权限管理
/// </summary>
public class OrderController
{
    public CallBack<bool> enterCall;
    public CallBack exitCall;

    private string firstUid;//首发者
    private string biggestUid;//最大出牌者
    private string currentAuthorityUid;//当前出牌者
    private InterationType curInterationType;//当前操作类型
    private static OrderController instance;

    public static OrderController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new OrderController();
            }
            return instance;
        }
    }
    /// <summary>
    /// 首发者
    /// </summary>
    public string FirstUid
    {
        set { firstUid = value; }
        get { return firstUid; }
    }

    /// <summary>
    /// 当前出牌者
    /// </summary>
    public string TypeUid
    {
        get { return currentAuthorityUid; }
    }

    /// <summary>
    /// 最大出牌者
    /// </summary>
    public string BiggestUid
    {
        set { biggestUid = value; }
        get { return biggestUid; }
    }

    /// <summary>
    /// 当前操作类型
    /// </summary>
    public InterationType CurInterationType
    {
        set { curInterationType = value; }
        get { return curInterationType; }
    }

    private OrderController()
    {
        currentAuthorityUid = "";
    }

    /// <summary>
    /// 初始化  0叫地主1抢地主2叫分3出牌
    /// </summary>
    /// <param name="type"></param>
    public void Init(string uid,int type)
    {
        if (exitCall != null)
            exitCall();
        CurInterationType = (InterationType)type;
        currentAuthorityUid = uid;
        BiggestUid = uid;
        if (enterCall != null)
            enterCall(false);
    }

    /// <summary>
    /// 出牌轮转  0叫地主1抢地主2叫分3出牌
    /// </summary>
    public void Turn(string uid,int type)
    {
        if (exitCall != null)
            exitCall();        
        currentAuthorityUid = uid;
        if (biggestUid == currentAuthorityUid)
        {//如果当前出牌者是最大的,那么清空桌面出牌
            DeskCardsCache.Instance.Clear();
        }
        CurInterationType = (InterationType)type;
        if (enterCall != null)
            enterCall(biggestUid != currentAuthorityUid);
    }

    public void Clear()
    {
        currentAuthorityUid = "";
        firstUid = string.Empty;
    } 
}


