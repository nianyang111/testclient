using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour 
{
    public GameObject exitBtn;
    public GameObject setBtn;
    public GameObject tuoGuanBtn;
    public GameObject shangchengBtn;
    public GameObject qukuanBtn;
    public GameObject mask;

    public BoolCallBack isCanOpenStore;
    void Start()
    {        
        UGUIEventListener.Get(setBtn).onClick = delegate
        {
            if (PageManager.Instance.CurrentPage is LandlordsPage)
            {
                //if (LandlordsModel.Instance.IsInFight)
                //{
                //    TipManager.Instance.OpenTip(TipType.SimpleTip, "游戏中不能进行这项操作");
                //    gameObject.SetActive(false);
                //    return;
                //}
                NodeManager.OpenNode<SetGameNode>();
                gameObject.SetActive(false);
            }            
        };        
        UGUIEventListener.Get(shangchengBtn).onClick = delegate
        {
            if (PageManager.Instance.CurrentPage is LandlordsPage)
            {
                if(LandlordsModel.Instance.IsInFight)
                {
                    TipManager.Instance.OpenTip(TipType.SimpleTip, "游戏中不能进行这项操作");
                    gameObject.SetActive(false);
                    return;
                }
                NodeManager.OpenNode<StoreNode>();
                gameObject.SetActive(false);
            }
        };        
        UGUIEventListener.Get(mask).onClick = delegate
        {            
            gameObject.SetActive(false);
        };
    }

    public void Init(CallBack isCanExitCall, CallBack tuoguanCall)
    {
        UGUIEventListener.Get(qukuanBtn).onClick = delegate
        {
            if (isCanOpenStore != null && isCanOpenStore())
            {
                NodeManager.OpenNode<SafeBoxNode>();
                gameObject.SetActive(false);
            }
        };
        UGUIEventListener.Get(exitBtn).onClick = delegate
        {
            if (isCanExitCall != null)
            {
                isCanExitCall();
                //PageManager.Instance.OpenPage<MainPage>();
            }
            gameObject.SetActive(false);
        };
        UGUIEventListener.Get(tuoGuanBtn).onClick = delegate
        {
            gameObject.SetActive(false);
            if (tuoguanCall != null)
                tuoguanCall();
        };

    }

}
