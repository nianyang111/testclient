using net_protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FangkaResultPanel : MonoBehaviour
{
    public GameObject scrrenBtn;
    public GameObject nextBtn;
    public Timer timer;
    public GameObject closeBtn;
    public Text jushuLb;
    public bool isOnlyShow = false;
    public MenuPanel menuPanel;
    void Start()
    {
        UGUIEventListener.Get(closeBtn).onClick = delegate { gameObject.SetActive(false); menuPanel.gameObject.SetActive(false); };
        UGUIEventListener.Get(scrrenBtn).onClick = delegate { SDKManager.Instance.ScrrenShoot("MyScreenshot", "MyApp", true); };
        UGUIEventListener.Get(nextBtn).onClick = delegate { Close(); };
    }

    public List<ResultItem> items = new List<ResultItem>();
    public void Init(bool isOnlyShow)//是否只是查看不进行初始化
    {
        this.isOnlyShow = isOnlyShow;
        gameObject.SetActive(true);
        if (!isOnlyShow)
        {
            scrrenBtn.SetActive(true);
            nextBtn.SetActive(true);
            List<DdzJSPlayerInfo> resultInfos = LandlordsModel.Instance.ResultModel.GetResultInfos();
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Init(resultInfos[i]);
            }
            timer.allLength = 25;
            timer.StartTime();
            timer.isOnce = true;
            timer.endAction = () =>
            {
                Close();
            };
            menuPanel.gameObject.SetActive(true);
            jushuLb.text = LandlordsModel.Instance.ResultModel.curJs + "/" + LandlordsModel.Instance.ResultModel.allJs;
        }
        else
        {
            scrrenBtn.SetActive(false);
            nextBtn.SetActive(false);
        }
    }



    void Close()
    {
        if (isOnlyShow)
        {
            gameObject.SetActive(false);
            menuPanel.gameObject.SetActive(false);
        }
        else
            Next();
    }

    void Next()
    {
        gameObject.SetActive(false);
        menuPanel.gameObject.SetActive(false);
        Interaction.Instance.zhunbeiBtn.SetActive(false);
        Interaction.Instance.Zhunbei();
    }
}
