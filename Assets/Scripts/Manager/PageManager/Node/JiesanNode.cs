using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JiesanNode : Node
{

    public Transform parent;
    public GameObject prefab;
    public Text desLb;
    public Timer timer;
    public GameObject sureBtn;
    public GameObject noBtn;
    List<GameObject> items = new List<GameObject>();
    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(sureBtn).onClick = delegate { SetSelect(true); };
        UGUIEventListener.Get(noBtn).onClick = delegate { SetSelect(false); };
        timer.allLength = 60;
        timer.endAction = delegate { SetSelect(false); };
        timer.StartTime();
        Init();
    }

    void SetSelect(bool isAgree)
    {
        if (PageManager.Instance.CurrentPage is LandlordsPage)
            LandlordsNet.C2G_Vote(isAgree);
        else if (PageManager.Instance.CurrentPage is MaJangPage)
            MaJangPage.Instance.SelectDisband(isAgree);
        Close();
    }

    void Init()
    {
        List<JiesanPlayerInfo> playerInfos = new List<JiesanPlayerInfo>();
        if (PageManager.Instance.CurrentPage is LandlordsPage)
        {
            for (int i = 0; i < LandlordsModel.Instance.RoomPlayerHands.Count; i++)
            {
                JiesanPlayerInfo info = new JiesanPlayerInfo();
                info.userId = int.Parse(LandlordsModel.Instance.RoomPlayerHands[i].playerInfo.uid);
                info.headIconUrl = LandlordsModel.Instance.RoomPlayerHands[i].playerInfo.icon;
                playerInfos.Add(info);
            }
        }
        else if (PageManager.Instance.CurrentPage is MaJangPage)
        {
            foreach (MaJangPlayer mjp in MaJangPage.Instance.playerList)
            {
                if (mjp.userId != 0)
                {
                    JiesanPlayerInfo info = new JiesanPlayerInfo();
                    info.userId = mjp.userId;
                    info.headIcon = mjp.headIcon.sprite;
                    playerInfos.Add(info);
                }
            }
        }
        LoadHeadIcon(playerInfos);
    }

    public void Inits(int jiesanId)
    {

        if (PageManager.Instance.CurrentPage is LandlordsPage)
        {
            LandkirdsHandCardModel player = LandlordsModel.Instance.RoomPlayerHands.Find(p => p.playerInfo.uid == jiesanId.ToString());
            if (player != null)
                desLb.text = "用户" + player.playerInfo.userNickname + "申请解散该房间";
            SetPlayerState(jiesanId, true);
        }
        else if (PageManager.Instance.CurrentPage is MaJangPage)
        {
            MaJangPlayer player = MaJangPage.Instance.GetPlayerFromSeatNo(jiesanId);
            if (player != null)
                desLb.text = "用户" + player.nickName.text + "申请解散该房间";
            SetPlayerState(player.userId, true);
        }
    }

    void LoadHeadIcon(List<JiesanPlayerInfo> playerInfos)
    {
        //加载
        for (int i = 0; i < playerInfos.Count; i++)
        {
            GameObject go = Instantiate(prefab, parent);
            go.name = playerInfos[i].userId.ToString();
            if (!playerInfos[i].headIcon)
                StartCoroutine(MiscUtils.DownloadImage(playerInfos[i].headIconUrl, spr =>
                    {
                        go.transform.Find("HeadIcon").GetComponent<Image>().sprite = spr;
                    }));
            else
                go.transform.Find("HeadIcon").GetComponent<Image>().sprite = playerInfos[i].headIcon;
            SetIsGrey(go.transform.Find("HeadIcon").GetComponent<Image>(), true);
            items.Add(go);
        }
    }

    /// <summary>
    /// 设置玩家投票状态
    /// </summary>
    public void SetPlayerState(int userId, bool isAgree)
    {
        GameObject item = items.Find(p => p.name == userId.ToString());
        if (item)
        {
            SetIsGrey(item.transform.Find("HeadIcon").GetComponent<Image>(), !isAgree);
        }
    }

    void SetIsGrey(Image headIcon, bool isGrey)
    {
        if (isGrey)
            headIcon.color = new Color(111 / 255f, 111 / 255f, 111 / 255f);
        else
            headIcon.color = Color.white;
    }

    /// <summary>
    /// 托送有人投票
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="isAgree"></param>
    public static void G2C_Vote(int userId, bool isAgree)
    {
        JiesanNode node = NodeManager.GetNode<JiesanNode>();
        if (node)
        {
            node.SetPlayerState(userId, isAgree);
        }
    }
}

public class JiesanPlayerInfo
{
    public int userId;
    public string headIconUrl;
    public Sprite headIcon;
}
