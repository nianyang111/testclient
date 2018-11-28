using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
public class TaskInfo  {

    public string taskID;//任务ID
    public string taskName;//任务名字
    public string icon;
    public TaskCondition taskCondition;
    public TaskReward taskReward;

    //根据taskId读取json,实现初始化
    public TaskInfo(string taskID, int curComplete = 0, bool isGetReward = false)
    {
        this.taskID = taskID;

        JsonData json = GetTaskJson(taskID);
        taskName = json.TryGetString("taskName");
        icon = json.TryGetString("taskIcon");
        string conditionId = json.TryGetString("conditionId");
        string rewardId = json.TryGetString("rewardId");
        taskCondition = new TaskCondition(conditionId, curComplete, int.Parse(json.TryGetString("conditionCount")));
        taskReward = new TaskReward(rewardId, int.Parse(json.TryGetString("rewardCount")), isGetReward);
    }

    //判断条件是否满足
    public void Check(TaskEventArgs e)
    {
        if (taskCondition.isFinish)
            return;
        if (taskCondition.id == e.id)
        {
            taskCondition.nowAmount += e.amount;
            TaskManager.Instance.RereshTask(e);
            if (taskCondition.nowAmount < 0) taskCondition.nowAmount = 0;
            if (taskCondition.nowAmount >= taskCondition.targetAmount)
            {
                taskCondition.isFinish = true;
                e.taskID = taskID;
                TaskManager.Instance.FinishTask(e);
            }
            else
                taskCondition.isFinish = false;
        }
    }

    //获取奖励
    public void Reward()
    {
        TaskEventArgs e = new TaskEventArgs();
        e.taskID = taskID;
        TaskManager.Instance.GetReward(e);
    }

    //取消任务
    public void Cancel()
    {
        TaskEventArgs e = new TaskEventArgs();
        e.taskID = taskID;
        TaskManager.Instance.CancelTask(e);
    }

    /// <summary>
    /// 根据id得到任务json
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    static JsonData GetTaskJson(string id)
    {
        JsonData json = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.taskConfigPath));
        for (int i = 0; i < json.Count; i++)
        {
            if (json[i].TryGetString("id") == id)
                return json[i];
        }
        return null;
    }
}
