using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreRechargeResultPanel : MonoBehaviour {
    public Button closeBtn, againBtn, trueBtn;
    public Text  content, trueText;
    private StoreNode _node;
    private int _type;
    public void Init(StoreNode node)
    {
        _node = node;
        UGUIEventListener.Get(closeBtn.gameObject).onClick = (g) => { gameObject.SetActive(false); };
        UGUIEventListener.Get(againBtn.gameObject).onClick = (g) => { gameObject.SetActive(false); };
        UGUIEventListener.Get(trueBtn.gameObject).onClick = (g) => { OnTrueBtn(); };
        gameObject.SetActive(false);
    }
    public void OnTrueBtn()
    {
        switch (_type)
        {
            case 0:
                _node.goldPanel.CloseRechargePanel();
                break;
            case 1:
                _node.agPanel.CloseRechargePanel();
                break;
            case 2:
                _node.cardPanel.CloseRechargePanel();
                break;
            case 3:
                _node.vipPanel.CloseRechargePanel();
                break;
            default:
                break;
        }
    }

    public void Open(net_protocol.ChargeNotice resp)
    {
        _type = resp.type;
        gameObject.SetActive(true);
        if (resp.result != 1) {
            againBtn.gameObject.SetActive(false);
            trueText.text = "重新支付";
            content.text = "支付未完成";
        }
        else
        {
            trueText.text = "暂不充值";
            content.text = "支付成功";
        }
    }
}
