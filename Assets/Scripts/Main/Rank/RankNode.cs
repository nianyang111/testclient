using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 排行榜
/// </summary>
public class RankNode : Node
{
    public enum RankType
    {
        Reap,
        Riches,
    }
    public RadioButton dayBtn;
    /// <summary>切换排行榜 </summary>
    public GameObject richesBtn, reapBtn;
    public RankItem prefab;
    public Transform content;
    /// <summary> 下端面板 </summary>
    public RankBottomPanel bottomPanel;
    /// <summary> 详细信息面板 </summary>
    public RankPlayInfoPanel playinfoPanel;
    private bool curDay = true;
    private RankType curType = RankType.Riches;
    private List<RankItem> itemList = new List<RankItem>();
    public override void Init()
    {
        base.Init();
        bottomPanel.Init();
        playinfoPanel.Init();
        dayBtn.onValueChange += SelectDay;
        UGUIEventListener.Get(richesBtn).onClick = (g) => { if (curType == RankType.Riches)return; SelectTab(RankType.Riches); };
        UGUIEventListener.Get(reapBtn).onClick = (g) => { if (curType == RankType.Reap)return; SelectTab(RankType.Reap); };
    }
    public override void Open()
    {
        base.Open();
        SelectDay();
    }
    /// <summary>
    /// 选择天数
    /// </summary>
    private void SelectDay(bool day = true)
    {
        curDay = day;
        QueryRank();
    }
    /// <summary>
    /// 选择类型
    /// </summary>
    private void SelectTab(RankType type = RankType.Riches)
    {
        curType = type;
        QueryRank();
    }
    /// <summary>
    /// 请求排行榜数据
    /// </summary>
    private void QueryRank()
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            queryRankReq = new net_protocol.QueryRankReq()
            {
                dt = curDay ? 1 : 0,
                type = curType == RankType.Riches ? 1 : 0
            },
            msgid = net_protocol.MessageId.C2G_QueryRankReq
        },true);
    }
    /// <summary>
    /// 请求排行榜数据完成
    /// </summary>
    /// <param name="resp"></param>
    public static void QueryRankFinish( net_protocol.QueryRankResp resp)
    {
        var node=NodeManager.GetNode<RankNode>();
        if (node)
        {
            node.bottomPanel.SetValue(resp.rank, resp.income);
            var rankInfoList = resp.playerRankInfo;
            
            List<RankData> dataList = new List<RankData>();
            for (int i = 0; i < rankInfoList.Count; i++)
            {
                RankData data = new RankData();
                data.userId = rankInfoList[i].userId;
                data.openId = rankInfoList[i].openId;
                data.nickName = rankInfoList[i].nickname;
                data.gender = rankInfoList[i].gender;
                data.icon = rankInfoList[i].photo;
                data.ag = rankInfoList[i].sliver;
                data.gold = rankInfoList[i].gold;
                data.vip = rankInfoList[i].vip;
                data.level = rankInfoList[i].level;
                data.getNum = rankInfoList[i].rankSliver;
                data.index = rankInfoList[i].index;
                data.curExp = rankInfoList[i].exp;
                dataList.Add(data);
            }
            for (int i = 0; i < node.itemList.Count; i++)
            {
                node.itemList[i].DestroyItem();
            }
            node.itemList.Clear();
            node.CreateItem(dataList);
        }
    }
    /// <summary>
    /// 创建Item
    /// </summary>
    private void CreateItem(List<RankData> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            RankItem item = Instantiate(prefab, content);
            item._node = this;
            item.Init(dataList[i]);
            itemList.Add(item);
        }
    }
    /// <summary>
    /// 保留位数
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string KeepGoldNum(long num)
    {
        string str = num.ToString();
        string valus;
        if (str.Length >= 9)
        {
            str = str.Substring(0, str.Length - 7);
            valus = str;
            valus = valus.Substring(valus.Length - 1, 1);
            if (valus != "0")
                str = str.Insert(str.Length - 1, ".");
            else
                str = str.Substring(0, str.Length - 1);
            return str + "亿";
        }
        if (str.Length >= 5)
        {
            str = str.Substring(0, str.Length - 3);
            valus = str;
            valus = valus.Substring(valus.Length - 1, 1);
            if (valus != "0")
                str = str.Insert(str.Length - 1, ".");
            else
                str = str.Substring(0, str.Length - 1);
            return str + "万";
        }
        return str;
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        curType = RankType.Riches;
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].DestroyItem();
        }
        itemList.Clear();
    }
}
