using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaJangResultOthersItem : MonoBehaviour
{
    public Text itemName, score, mjRateMask, multipleNum;
    public Transform horizontal, handParent;
    public GameObject altPrefab;
    public MaJangResultMj mjPrefab;
    [HideInInspector]
    public MaJangPlayer mPlayer;

    public net_protocol.Mj xiaoji;
    public void Init(MaJangPlayer player)
    {
        mPlayer = player;
        if (itemName)
            itemName.text = player.nickName.text;
        if (score)
            score.text = player.win.ToString();
        #region 吃 碰 杠
        List<MaJangModel[]> altList = player.lightMjList;
        if (altList.Count > 0)
        {
            for (int i = 0; i < altList.Count; i++)
            {
                MaJangModel[] mjModel = altList[i];
                GameObject mjAlt = Instantiate(altPrefab, horizontal);
                mjAlt.SetActive(true);
                for (int j = 0; j < mjAlt.transform.childCount; j++)
                {
                    if (mjModel.Length > j)
                    {
                        mjAlt.transform.GetChild(j).GetComponent<MaJangResultMj>().Init(mjModel[j].mjNo.ToString());
                    }
                    else
                    {
                        mjAlt.transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
                mjAlt.transform.SetParent(horizontal);
            }
        }
        #endregion
        #region 手牌
        List<MaJangModel> handList = player.handMjList;
        int handCount = player.win > 0 ? handList.Count - 1 : handList.Count;
        for (int i = 0; i < handCount; i++)
        {
            MaJangResultMj mj = Instantiate(mjPrefab, handParent);
            mj.gameObject.SetActive(true);
            mj.Init(handList[i].mjNo.ToString());
            mj.transform.SetParent(handParent);
        }
        handParent.gameObject.SetActive(false);
        handParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        handParent.GetComponent<HorizontalLayoutGroup>().enabled = true;
        handParent.SetParent(horizontal);
        if (player.win > 0)
        {
            MaJangModel last = player.handMjList[handList.Count - 1];
            mjPrefab.Init(last.mjNo.ToString());
            mjPrefab.transform.SetParent(horizontal);
        }
        else
        {
            mjPrefab.gameObject.SetActive(false);
        }
        #endregion
        gameObject.SetActive(true);
        horizontal.GetComponent<HorizontalLayoutGroup>().enabled = false;
        horizontal.GetComponent<HorizontalLayoutGroup>().enabled = true;
        SetTimeout.add(0.1f, () =>
        {
            handParent.gameObject.SetActive(false);
            horizontal.gameObject.SetActive(false);
            handParent.gameObject.SetActive(true);
            horizontal.gameObject.SetActive(true);
        });
    }
    public void SetRateAndMultip(string rateMask,string num)
    {
        mjRateMask.gameObject.SetActive(true);
        mjRateMask.text = rateMask;
        multipleNum.text = num;
    }
    public void DelInfo()
    {
        handParent.SetParent(this.transform);
        mjPrefab.transform.SetParent(this.transform);
        UIUtils.DestroyChildren(handParent);
        UIUtils.DestroyChildren(horizontal);
    }

    //public void Create()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        MaJangResultMj mj = Instantiate(mjPrefab, handParent);
    //        mj.gameObject.SetActive(true);
    //        //mj.Init(handList[i].mjNo.ToString());
    //        mj.transform.SetParent(handParent);
    //    }
    //    gameObject.SetActive(true);

    //    handParent.SetParent(horizontal);
    //    //SetTimeout.add(0.2f, () =>
    //    //{
    //    //    handParent.gameObject.SetActive(false);
    //    //    horizontal.gameObject.SetActive(false);
    //    //    handParent.gameObject.SetActive(true);
    //    //    horizontal.gameObject.SetActive(true);
    //    //});
    //}

    void OnEnable()
    {
        SetTimeout.add(0.1f, () =>
        {
            handParent.gameObject.SetActive(false);
            horizontal.gameObject.SetActive(false);
            handParent.gameObject.SetActive(true);
            horizontal.gameObject.SetActive(true);
        });
    }
}
