using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using net_protocol;
public class TaskNode : Node
{

    public GameObject prefab;
    public Transform parent;
    public GameObject invateBtn;
    public TaskRewardPanel rewardPanel;
    List<TaskItem> items = new List<TaskItem>();

    public override void Open()
    {
        base.Open();
        //TaskManager.Instance.refreshEvent += TaskInfoUpdate;
        //TaskManager.Instance.rewardEvent += TaskInfoUpdate;
        //TaskManager.Instance.rewardEvent += Reward;
        UGUIEventListener.Get(invateBtn).onClick = delegate { Close(false); };
        ReqTasks(); 
    }

    public static void ReqTasks()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_DaliyTask
        });  
    }

    public static void TaskRefresh(List<DailyTaskResp.DailyTask> taskList)
    {
        TaskNode node = NodeManager.GetNode<TaskNode>();
        if (node)
        {
            node.LoadItems(taskList);
        }
        MainPage mainPage = PageManager.Instance.GetPage<MainPage>();
        if (mainPage)
        {
            mainPage.SetTaskRed(taskList.Find(p => p.actule_times >= p.daliy_times && p.has_cashing_prize == 0) != null);
        }
    }

    /// <summary>
    /// 领奖
    /// </summary>
    public static void Reward(int rewardCount)
    {
        TaskNode.ReqTasks();
        TaskNode node = NodeManager.GetNode<TaskNode>();
        if (node)
        {            
            node.RewardShow(rewardCount);
        }
        ReqTasks();
    }

    void LoadItems(List<DailyTaskResp.DailyTask> taskList)
    {
        UIUtils.DestroyChildren(parent);
        for (int i = 0; i < taskList.Count; i++)
        {
            LoadItem(taskList[i]);
        }
    }

    void LoadItem(DailyTaskResp.DailyTask task)
    {
        TaskItem item = Instantiate(prefab, parent).GetComponent<TaskItem>();
        item.Inits(task);
        items.Add(item);
    }

    ///// <summary>
    ///// 任务信息更新
    ///// </summary>
    //void TaskInfoUpdate(object sender, TaskEventArgs args)
    //{
    //    List<TaskItem> tasks = items.FindAll(p => p.info.taskCondition.id == args.id);
    //    if (tasks.Count == 0)
    //        tasks = items.FindAll(p => p.info.taskID == args.taskID);
    //    for (int i = 0; i < tasks.Count; i++)
    //    {
    //        tasks[i].UpdateProgree(args);
    //    }        
    //}

    /// <summary>
    /// 领奖
    /// </summary>
    /// <param name="args"></param>
    void RewardShow(int rewardCount)
    {
        rewardPanel.SetVisible(true);
        rewardPanel.Inits(rewardCount);
    }
}

