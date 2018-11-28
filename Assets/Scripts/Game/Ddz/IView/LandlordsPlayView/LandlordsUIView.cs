using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 界面显示管理器
/// </summary>
public class LandlordsUIView : MonoBehaviour
{

    public static LandlordsUIView Instance;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 获得斗地主小物件显示层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uiType"></param>
    /// <param name="cType"></param>
    /// <returns></returns>
    public T Get<T>(UIType uiType, CharacterType cType) where T : Component
    {
        string childName = uiType.ToString();
        //找到类型父物体
        Transform parent = transform.Find(childName);
        //找到该父物体下对应玩家的UI物体
        string childName2 = cType.ToString();
        Transform go = parent.Find(childName2);
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.gameObject.AddComponent<T>();
        }
        return t;
    }
}


public enum UIType
{
    /// <summary>
    /// 头像
    /// </summary>
    HeadIcon,
    /// <summary>
    /// 计时
    /// </summary>
    Clock,
    /// <summary>
    /// 警报
    /// </summary>
    Warning,
    /// <summary>
    /// 手牌
    /// </summary>
    HandCards,
    /// <summary>
    /// 出的牌
    /// </summary>
    DeskCards,
}