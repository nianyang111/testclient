using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 任务管理器
/// </summary>
public class TaskManager : MonoBehaviour
{
    /// <summary>
    /// this instance
    /// </summary>
    static TaskManager instance;
    public static TaskManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(TaskManager).ToString(), typeof(TaskManager)).GetComponent<TaskManager>();
            }
            return instance;
        }
    }

    public Dictionary<string, TaskInfo> dictionary = new Dictionary<string, TaskInfo>();//id,task  


    public event EventHandler<TaskEventArgs> getEvent;//接受任务时,更新任务到任务面板等操作
    public event EventHandler<TaskEventArgs> refreshEvent;//更新任务进度
    public event EventHandler<TaskEventArgs> finishEvent;//完成任务时,提示完成任务等操作  
    public event EventHandler<TaskEventArgs> rewardEvent;//得到奖励时,显示获取的物品等操作  
    public event EventHandler<TaskEventArgs> cancelEvent;//取消任务时,显示提示信息等操作  


    public void GetTask(string taskID, int cur_complete)
    {
        if (!dictionary.ContainsKey(taskID))
        {
            TaskInfo t = new TaskInfo(taskID, cur_complete);
            dictionary.Add(taskID, t);

            TaskEventArgs e = new TaskEventArgs();
            e.taskID = taskID;
            if (getEvent != null)
                getEvent(this, e);
        }
    }

    public void CheckTask(System.Object sender, TaskEventArgs e)
    {
        foreach (KeyValuePair<string, TaskInfo> kv in dictionary)
        {
            kv.Value.Check(e);
        }
    }

    public void RereshTask(TaskEventArgs e)
    {
        if (refreshEvent != null)
            refreshEvent(this, e);
    }

    public void FinishTask(TaskEventArgs e)
    {
        if (finishEvent != null)
            finishEvent(this, e);
    }

    public void GetReward(TaskEventArgs e)
    {
        if (dictionary.ContainsKey(e.taskID))
        {
            TaskInfo t = dictionary[e.taskID];
            if (!t.taskReward.isGet)
            {
                t.taskReward.isGet = true;
                TaskEventArgs a = new TaskEventArgs();
                a.id = t.taskReward.id;
                a.amount = t.taskReward.amount;
                a.taskID = e.taskID;
                if (refreshEvent != null)
                    refreshEvent(this, e);
                if (rewardEvent != null)
                    rewardEvent(this, a);                
                //dictionary.Remove(e.taskID);
            }
            else
            {
                Debug.Log("已领过！");
            }
        }
    }

    public void CancelTask(TaskEventArgs e)
    {
        if (dictionary.ContainsKey(e.taskID))
        {
            if (cancelEvent != null)
                cancelEvent(this, e);
            dictionary.Remove(e.taskID);
        }
    }

    public void RestTask(TaskEventArgs e)
    {
        TaskInfo info = null;
        if (dictionary.TryGetValue(e.taskID, out info))
        {
            info.taskCondition.nowAmount = 0;
            info.taskCondition.isFinish = false;
            info.taskReward.isGet = false;
            if (refreshEvent != null)
                refreshEvent(this, e);
        }
    }

    public void RestAllTask()
    {
        foreach (var item in dictionary)
        {
            TaskEventArgs e = new TaskEventArgs();
            e.taskID = item.Value.taskID;
            e.id = item.Value.taskCondition.id;
            RestTask(e);
        }
    }
}

/// <summary>
/// 任务对象参数
/// </summary>
public class TaskEventArgs : EventArgs
{
    public string taskID;//当前任务的ID
    public string id;//发生事件的对象的ID(例如敌人,商品)
    public int amount;//数量
}


/// <summary>
///  任务条件
/// </summary>
public class TaskCondition
{
    public string id;//条件id
    public int nowAmount;//条件id的当前进度
    public int targetAmount;//条件id的目标进度
    public bool isFinish = false;//记录是否满足条件

    public TaskCondition(string id, int nowAmount, int targetAmount)
    {
        this.id = id;
        this.nowAmount = nowAmount;
        this.targetAmount = targetAmount;
    }
}


/// <summary>
/// 任务奖励
/// </summary>
public class TaskReward
{
    public string id;//奖励id
    public int amount = 0;//奖励数量
    public bool isGet = false;//是否已领取
    public TaskReward(string id, int amount,bool isGet=false)
    {
        this.id = id;
        this.amount = amount;
        this.isGet = isGet;
    }
}
