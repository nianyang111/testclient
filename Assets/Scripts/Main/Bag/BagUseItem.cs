using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUseItem : MonoBehaviour {
    public Text itemName,useText;
    public Button addBtn, cutBtn;
    public int useNum = 0;
    private int useMax;
    public BagGoodsData _data;
    public void Init(BagGoodsData data)
    {
        _data = data;
        useMax = _data.counts;
        itemName.text = _data.name;
        UGUIEventListener.Get(addBtn.gameObject).onClick = delegate { useNum++; ChangeUse(); };
        UGUIEventListener.Get(cutBtn.gameObject).onClick = delegate { useNum--; ChangeUse(); };
    }
    private void ChangeUse()
    {
        if (useNum > useMax)
            useNum = useMax;
        if (useNum < 0)
            useNum = 0;
        useText.text = useNum.ToString();
    }
}
