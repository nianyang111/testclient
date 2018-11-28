using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 玩家出牌显示管理器
/// </summary>
public class DzPopDesk : MonoBehaviour
{

    public CommonAnimation ani;
    public Transform normalDesk;
    public Transform angleDesk;

    Transform curParentTrans;
    public void Inits(List<Card> cards)
    {
        CardsType cardsType = CardsType.None;
        CardRules.PopEnable(cards.ToArray(), out cardsType);
        Layout(cardsType, cards);
    }

    public void ClearPopesk()
    {
        CardUI[] cardsUI = normalDesk.GetComponentsInChildren<CardUI>();
        for (int i = 0; i < cardsUI.Length; i++)
        {
            cardsUI[i].Destroy();
        }
        cardsUI = angleDesk.GetComponentsInChildren<CardUI>();
        for (int i = 0; i < cardsUI.Length; i++)
        {
            cardsUI[i].Destroy();
        }
    }

    //创建牌
    void CreateCards(Transform parentTrans, List<Card> cards,bool isSort)
    {
        List<Card> temp = cards;
        if (isSort)
        {
            temp.Sort((a, b) =>
                {
                    int a_count = cards.FindAll(p => p.GetCardWeight == a.GetCardWeight).Count;
                    int b_count = cards.FindAll(p => p.GetCardWeight == b.GetCardWeight).Count;
                    if (a_count > b_count)
                        return -1;
                    else if (a_count < b_count)
                        return 1;
                    else
                        return 0;
                });
        }
        for (int i = 0; i < temp.Count; i++)
        {
            CardUI a = LandlordsPage.MakeSprite(temp[i], false, parentTrans);
            a.SetCardSize(new Vector2(145, 190));
        }
        ani.Play();
    }

    //布局
    void Layout(CardsType layoutType, List<Card> cards)
    {
        switch (layoutType)
        {
            case CardsType.OnlyThree:
            case CardsType.ThreeAndOne:
            case CardsType.ThreeAndTwo:
                curParentTrans = angleDesk;
                CreateCards(curParentTrans, cards,true);
                AngleLayout(3);
                break;
            case CardsType.FourAndTwo:
                curParentTrans = angleDesk;
                CreateCards(curParentTrans, cards, true);
                AngleLayout(4);
                break;
            default:
                curParentTrans = normalDesk;
                CreateCards(curParentTrans, cards, false);
                NormalLayout();
                break;
        }
    }


    #region 各种布局方式

    //正常布局
    void NormalLayout()
    {
        //horizontalGrid.enabled = true;
    }

    // 角度布局
    void AngleLayout(int angleCardNumber)
    {
        Vector3 anglePos=Vector3.zero;
        for (int i = 0; i < angleCardNumber; i++)
        {
            Transform chiledTrans = curParentTrans.GetChild(i);
            chiledTrans.localPosition = Vector3.zero;
            int index = Index(i + 1, angleCardNumber);
            chiledTrans.localPosition += Vector3.left * 200 + Vector3.up * i * 7;
            CommonAnimation chiledAni = chiledTrans.gameObject.AddComponent<CommonAnimation>();
            chiledAni.angleList.Add(chiledTrans.localEulerAngles);
            chiledAni.angleList.Add(Vector3.forward * 20 * index);
            chiledAni.angleDelayTime = 0.2f;
            chiledAni.time = 0.1f;
            chiledAni.Play();
            anglePos=chiledTrans.localPosition;
        }

        

        int posIndex = 1;
        for (int i = angleCardNumber; i < curParentTrans.childCount; i++)
        {
            Transform chiledTrans = curParentTrans.GetChild(i);                        
           chiledTrans.localPosition = new Vector3(anglePos.x, 0, 0) + Vector3.right * 200 + (Vector3.right * posIndex++ * (chiledTrans as RectTransform).sizeDelta.x / 2.5f);
        }
    }

    #endregion


    /// <summary>
    /// 得到相对于中间位置的索引
    /// </summary>
    int Index(int indexInParent, int allCount) 
    {
        float midelle = allCount / 2f;
        if (midelle % 2 == 0)
        {//偶数
            return Index(indexInParent, allCount + 1);
        }
        else
        {//基数
            midelle = Mathf.Round(midelle);
            return (int)midelle - indexInParent;
        }
    }
}
