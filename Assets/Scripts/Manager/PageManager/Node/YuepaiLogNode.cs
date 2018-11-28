using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuepaiLogNode : Node {

    public Transform parent;

    public GameObject prefab;

    //void OnGUI()
    //{
    //    if (GUILayout.Button("加载战绩"))
    //    {
    //        List<LogInfo> logs = new List<LogInfo>();
    //        for (int i = 0; i < 40; i++)
    //        {
    //            LogInfo info = new LogInfo();
    //            info.gameType = Random.Range(0, 2);
    //            info.alljushu = i;
    //            info.jushuResult = new List<ResultInfo>();
    //            for (int j = 0; j < 2; j++)
    //            {
    //                info.jushuResult.Add(new ResultInfo() { id = i + i, name = "名字" + i + j, allResult = i + j, oneAndFour = i - j, twoAndFour = j - j });
    //            }
    //            info.jushuResult.Add(new ResultInfo() { id = UserInfoModel.userInfo.userId, name = "这是我", allResult = 10, oneAndFour = 1, twoAndFour = 5 });
    //            info.time = "2015年15月99日";
    //            logs.Add(info);
    //        }
    //        G2C_ReceiveLog(logs);
    //    }

    //}

    public override void Open()
    {
        base.Open();
        //请求约牌记录
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_YuePaiTable 
            });
    }

    void LoadItem(YuePaiTable info)
    {
        Instantiate(prefab, parent).GetComponent<YuepaiLogItem>().Inits(info, ChakanCall);
    }

    /// <summary>
    /// 查看详情回调
    /// </summary>
    /// <param name="info"></param>
    void ChakanCall(YuePaiTable info)
    {
        NodeManager.OpenNode<CardResultShowNode>(null,null,false).Inits(info);
    }

    public static void G2C_ReceiveLog(YuePaiTableResp resp)
    {
        YuepaiLogNode node = NodeManager.GetNode<YuepaiLogNode>();
        if (node)
        {
            UIUtils.DestroyChildren(node.parent);
            for (int i = 0; i < resp.yuePaiTable.Count; i++)
            {
                node.LoadItem(resp.yuePaiTable[i]);
            }


        }
    }
}
public class LogInfo
{
    public string time;
    public int gameType;
    public int alljushu;
    public List<ResultInfo> jushuResult;
}

/// <summary>
/// 每局战绩
/// </summary>
public class ResultInfo
{
    public int id;
    public string name;
    public string headIcon;
    public int allResult;//总成绩

    public int curJushu;
    public List<playerResultInfo> playersResInfo = new List<playerResultInfo>();//每个人的战绩
}

/// <summary>
/// 每个人战绩信息
/// </summary>
public class playerResultInfo
{
    public int userId;
    public string headIcon;
    public string nickname;
    public int income;
}
