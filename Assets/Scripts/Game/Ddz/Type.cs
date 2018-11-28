using UnityEngine;
using System.Collections;

/// <summary>
/// 角色类型
/// </summary>
public enum CharacterType
{
    Library = 0,
    /// <summary>
    /// 自己
    /// </summary>
    Player0,
    /// <summary>
    /// 玩家1
    /// </summary>
    Player1,
    /// <summary>
    /// 玩家2
    /// </summary>
    Player2,
    Desk
}


/// <summary>
/// 花色
/// </summary>
public enum Suits
{
    /// <summary>
    /// 梅花
    /// </summary>
    Club = 0,
    /// <summary>
    /// 方块
    /// </summary>
    Diamond,
    /// <summary>
    /// 红桃
    /// </summary>
    Heart,
    /// <summary>
    /// 黑桃
    /// </summary>
    Spade,
    None
}

/// <summary>
/// 卡牌权值
/// </summary>
public enum Weight
{
    Three = 0,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    One,
    Two,
    SJoker,
    LJoker,
}

/// <summary>
/// 身份
/// </summary>
public enum Identity
{
    Farmer,
    Landlord  
}

/// <summary>
/// 性别
/// </summary>
public enum Six
{
    boy,
    girl
}

/// <summary>
/// 出牌类型
/// </summary>
public enum CardsType
{
    //未知类型
    None,
    //王炸
    JokerBoom,
    //炸弹
    Boom,
    //三个不带
    OnlyThree,
    //三个带一
    ThreeAndOne,
    //三个带二
    ThreeAndTwo,
    //四个带二
    FourAndTwo,
    //顺子 五张或更多的连续单牌
    Straight,
    //双顺 三对或更多的连续对牌
    DoubleStraight,
    //三顺 二个或更多的连续三张牌
    TripleStraight,
    //飞机带
    TripleStraightDaiOne,
    TripleStraightDaiTwo,
    //对子
    Double,
    //单个
    Single
}

/// <summary>
/// 斗地主人物动画类型
/// </summary>
public enum LandlordsRoleAniType
{
    /// <summary>
    /// 伤心(失败)
    /// </summary>
    Angry,
    /// <summary>
    /// 开心(赢)
    /// </summary>
    Happy,
    /// <summary>
    /// 正常
    /// </summary>
    Normal,
    /// <summary>
    /// 机器人
    /// </summary>
    Robot,
}

/// <summary>
/// 斗地主场景事件动画
/// </summary>
public enum LandlordsEventAniType
{
    /// <summary>
    /// 王炸
    /// </summary>
    JokerBoom,
    /// <summary>
    /// 炸弹
    /// </summary>
    Boom,
    /// <summary>
    /// 顺子 五张或更多的连续单牌
    /// </summary>  
    Straight,
    /// <summary>
    /// 双顺 三对或更多的连续对牌
    /// </summary>
    DoubleStraight,
    /// <summary>
    /// 飞机
    /// </summary>
    Fly,
    /// <summary>
    /// 胜利
    /// </summary>
    Win,
    /// <summary>
    /// 失败
    /// </summary>
    Lose,
    /// <summary>
    /// 春天
    /// </summary>
    ChunTian,
    /// <summary>
    /// 反春天
    /// </summary>
    FanChunTian,
}


/// <summary>
/// 操作类型
/// </summary>
public enum InterationType
{
    /// <summary>
    /// 叫地主
    /// </summary>
    CallLandlords = 0,
    /// <summary>
    /// 抢地主
    /// </summary>
    QiangLandlords = 1,
    /// <summary>
    /// 叫分
    /// </summary>
    CallFen = 2,
    /// <summary>
    /// 出牌
    /// </summary>
    PopCard = 3
}


