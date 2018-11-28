using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopRightPanel : MonoBehaviour {
    public UnityEngine.UI.Text timeText;
    public GameObject moerBtn,scannerBtn,closeScannerBtn;
	void Start () {
        UGUIEventListener.Get(moerBtn).onClick = delegate { closeScannerBtn.SetActive(!closeScannerBtn.activeInHierarchy); };
        UGUIEventListener.Get(scannerBtn).onClick = delegate { PageManager.Instance.OpenPage<ScannerPage>(); };
        UGUIEventListener.Get(closeScannerBtn).onClick = delegate { closeScannerBtn.SetActive(false); };
        closeScannerBtn.SetActive(false);
        InvokeRepeating("MyTime", 0f, 60f);
        MyTime();
	}
    void MyTime()
    {
        timeText.text = DateTime.Now.ToShortTimeString().ToString();
    }


}
