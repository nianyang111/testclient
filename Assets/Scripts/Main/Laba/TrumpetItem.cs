using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrumpetItem : MonoBehaviour {

    public Text belong;
    public Text value;


    public void Init(GameMessage message)
    {
        belong.text = message.type == 1 ? "[用户]" : "[系统]";
        value.text = message.sender + ":" + message.value;
    }
}
