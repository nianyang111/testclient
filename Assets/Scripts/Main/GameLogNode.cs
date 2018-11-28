using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩牌记录
/// </summary>
public class GameLogNode : Node {

    public Transform vertical_parent;
    public Transform horizontal_parent;

    public GameObject vertical_prefab;
    public GameObject horizontal_prefab;
    List<CardResultShowNode.YuepaiLogPlayerInfo> playerInfos = new List<CardResultShowNode.YuepaiLogPlayerInfo>();


    public void Inits(string tableid)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
               msgid=MessageId.C2G_YuePaiTable,
               yuePaiTableReq = new YuePaiTableReq() { tableId = tableid }
            });
    }

    public static void G2C_Load(YuePaiTable info)
    {
        GameLogNode node = NodeManager.GetNode<GameLogNode>();
        if (node)
            node.Load(info);
    }

    void Load(YuePaiTable info)
    {
        //加载总体
        UIUtils.DestroyChildren(vertical_parent);
        for (int i = 0; i < info.yuePaiLog[0].YuePaiOther.Count; i++)
        {
            YuePaiOther playerInfo = info.yuePaiLog[0].YuePaiOther[i];
            int allRessult = 0;
            for (int j = 0; j < info.yuePaiLog.Count; j++)
            {
                for (int k = 0; k < info.yuePaiLog[j].YuePaiOther.Count; k++)
                {
                    if (info.yuePaiLog[j].YuePaiOther[k].userId == playerInfo.userId)
                        allRessult += info.yuePaiLog[j].YuePaiOther[k].score;
                }

            }
            CardResultShowNode.YuepaiLogPlayerInfo logInfo = new CardResultShowNode.YuepaiLogPlayerInfo();
            logInfo.userId = playerInfo.userId;
            logInfo.nickname = playerInfo.userName;
            logInfo.allResult = allRessult; print(playerInfo.icon);
            logInfo.headIcon = playerInfo.icon;
            playerInfos.Add(logInfo);
        }
        CardResultShowNode.YuepaiLogPlayerInfo maxInfos = ArrayHelper.Max<CardResultShowNode.YuepaiLogPlayerInfo, int>(playerInfos.ToArray(), p => p.allResult);
        maxInfos.isMax = true;
        for (int i = 0; i < playerInfos.Count; i++)
        {
            LoadVerticalResultItem(playerInfos[i]);
        }

        info.yuePaiLog.Sort((a, b) =>
        {
            return a.curr_round.CompareTo(b.curr_round);
        });
        //加载详情
        for (int i = 0; i < info.yuePaiLog.Count; i++)
        {
            YuePaiLog result = info.yuePaiLog[i];
            LoadHorizontalResultItem(result);
        }
    }

    void LoadVerticalResultItem(CardResultShowNode.YuepaiLogPlayerInfo info)
    {
        Instantiate(vertical_prefab, vertical_parent).GetComponent<YuepaiResultItem>().Inits(info);
    }

    void LoadHorizontalResultItem(YuePaiLog resultInfo)
    {
        Instantiate(horizontal_prefab, horizontal_parent.transform).GetComponent<YuepaiHorizontalItem>().Init(resultInfo);
    }

}
