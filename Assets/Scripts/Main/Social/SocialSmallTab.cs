using System.Collections;
using System.Collections.Generic;
using UiEffect;
using UnityEngine;
using UnityEngine.UI;
public class SocialSmallTab : MonoBehaviour
{    
    private CallBack<int> onClickItemCall;

    public List<GameObject> tabs;

    GameObject selectTab;
    /// <summary>
    /// 好友排行/认识的人/附近的人页签
    /// </summary>
    GameObject SelectTab
    {
        set
        {
            if (selectTab != null)
            {
                selectTab.transform.Find("choose").gameObject.SetActive(false);
                selectTab.transform.Find("Text").GetComponent<Outline>().enabled = false;
                GradientColor gcolor = selectTab.transform.Find("Text").GetComponent<GradientColor>();
                gcolor.colorTop = new Color(209, 212, 217);
                gcolor.colorBottom = new Color(164, 190, 213);
            }
            selectTab = value;

            selectTab.transform.Find("choose").gameObject.SetActive(true);
            selectTab.transform.Find("Text").GetComponent<Outline>().enabled = true;
            GradientColor gcolors = selectTab.transform.Find("Text").GetComponent<GradientColor>();
            gcolors.colorTop = new Color(209, 212, 217);
            gcolors.colorBottom = new Color(164, 190, 213);
            onClickItemCall(tabs.IndexOf(selectTab));
        }
        get
        {
            return selectTab;
        }
    }


    /// <summary>
    /// 上方页签初始化
    /// </summary>
    /// <param name="titleInfoList"></param>
    /// <param name="_onClickItemCall"></param>
    public void InitData(CallBack<int> _onClickItemCall, int chooseIndex = 0)
    {
        this.onClickItemCall = _onClickItemCall;

        for (int i = 0; i < tabs.Count; i++)
        {
            UGUIEventListener.Get(tabs[i]).onClick = p=>
            {
                SelectTab = p;
            };
        }
        SelectTab = tabs[chooseIndex];
    }


}

