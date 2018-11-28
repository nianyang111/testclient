using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BqItem : MonoBehaviour {

    public string bqId;
    public Button btn;
    public Image icon;
    public void Init(CallBack<string> onClick)
    {
        bqId = icon.sprite.name.Substring(0, icon.sprite.name.IndexOf('_'));
        btn.onClick.AddListener(() => onClick(bqId));
    }
}
