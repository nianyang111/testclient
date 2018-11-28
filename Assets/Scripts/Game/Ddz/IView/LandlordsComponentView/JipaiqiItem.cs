using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JipaiqiItem : MonoBehaviour {

    public Weight weight;

    public Text valueLb;

    public int curNum;

    public void SetValue(int num)
    {
        curNum = Mathf.Max(0, num);
        valueLb.text = curNum.ToString();
    }
}
