using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangyongPanel : MonoBehaviour {

    public GameObject ddzObj;
    public GameObject mjObj;

    public List<ChangyongItem> items = new List<ChangyongItem>();

    void Start()
    {
        ddzObj.SetActive(PageManager.Instance.CurrentPage is LandlordsPage);
        mjObj.SetActive(PageManager.Instance.CurrentPage is MaJangPage);
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Init(SendChatMessage);
        }
    }

    void SendChatMessage(string text)
    {
        ChatInfo info = new ChatInfo();
        info.text = text;
        info.type = 0;
        NodeManager.GetNode<ChatNode>().SendMessage(info, true);        
    }

}
