using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RechargeVIPNode : Node {

    public GameObject xufeiBtn;
    public Toggle zhoukaTg;
    public Text zhoukaRMBLb;
    public Toggle yuekaTg;
    public Text yuekaRMBLb;

    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(xufeiBtn).onClick = delegate { Xufei(); };
    }

    /// <summary>
    /// 续费回调
    /// </summary>
    void Xufei()
    {
        int vipType = 0;//0充周卡1月卡
        int rmb = 0;
        if (zhoukaTg.isOn)
        {
            vipType = 0;
            rmb = int.Parse(zhoukaRMBLb.text);
        }
        else if (yuekaTg.isOn)
        {
            vipType = 1;
            rmb = int.Parse(yuekaRMBLb.text);
        }

        //打开充值界面
    }
}
