using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShareNode : Node 
{
    public Text gameTypeLb;
    public Text timeLb;
    public Text allJsLb;
    public Transform parent;
    public GameObject prefab;

    public void Inits(ShareInfo info)
    {
        gameTypeLb.text = info.gameType;
        allJsLb.text = "共对局：" + info.allJs.ToString();
        timeLb.text = string.Format("{0}月{1}日 {2}：{3}", info.gameTime.Month.ToString("D2"), info.gameTime.Day.ToString("D2"), info.gameTime.Hour.ToString("D2"), info.gameTime.Minute.ToString("D2"));
        UIUtils.DestroyChildren(parent);
        for (int i = 0; i < info.playerInfos.Count; i++)
        {
            Instantiate(prefab, parent).GetComponent<ShareItem>().Inits(info.playerInfos[i]);
        }
    }
}

public class ShareInfo
{
    public string gameType;
    public DateTime gameTime;
    public int allJs;
    public List<ShareItemInfo> playerInfos;
}

public class ShareItemInfo
{
    public int rank;
    public Sprite headIcon;
    public string name;
    public int winCount;
    public int score;
}
