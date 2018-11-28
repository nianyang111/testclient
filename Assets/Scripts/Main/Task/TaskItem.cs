using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TaskItem : MonoBehaviour {

    public DailyTaskResp.DailyTask info;

    public Image headIcon;
    public Text nameLb;
    public Image rewardIcon;
    public Text rewardCountLb;
    public Text progressLb;
    public GameObject goTaskBtn;
    public Button getrewardBtn;
    public Text getrewardBtnText;
    public void Inits(DailyTaskResp.DailyTask task)
    {
        info = task;
        headIcon.sprite = BundleManager.Instance.GetSprite("task/meirirenwu_pic_" + info.task_icon);
        nameLb.text = info.task_title;
        rewardIcon.sprite = BundleManager.Instance.GetSprite("common/normal_log_1");
        rewardCountLb.text = info.task_award.ToString();
        progressLb.text = info.actule_times + "/" + info.daliy_times;
        UGUIEventListener.Get(goTaskBtn).onClick = delegate { ReceiveTask(); };
        UGUIEventListener.Get(getrewardBtn.gameObject, AudioManager.AudioSoundType.BtnReward).onClick = delegate { Reward(); };
        UpdateProgree(null);
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    public void UpdateProgree(TaskEventArgs args)
    {
        if (info.actule_times >= info.daliy_times)
        {//完成            
            progressLb.text = "已完成";

            goTaskBtn.SetActive(false);
            getrewardBtn.gameObject.SetActive(true);
            if (info.has_cashing_prize == 1)//0未领取 1已领取 
            {
                getrewardBtnText.text = "已领取";
                getrewardBtn.interactable = false;
            }
            else if (info.has_cashing_prize == 0)
            {
                getrewardBtn.interactable = true;
                getrewardBtnText.text = "领奖励";
            }
        }
        else
        {//未完成
            progressLb.text = info.actule_times + "/" + info.daliy_times;
            goTaskBtn.SetActive(true);
            getrewardBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 领奖
    /// </summary>
    void Reward()
    {
        if (getrewardBtn.interactable)
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                {
                    CashingPrize = new CashingPrize()
                    {
                        taskType = info.daliy_type,
                        totalPrize = 0
                    },
                    msgid = MessageId.C2G_CashingPrize
                });
        }
    }

    /// <summary>
    /// 做任务
    /// </summary>
    void ReceiveTask()
    {
        if (info.daliy_type == 1)
        {//斗地主
            PageManager.Instance.OpenPage<SelectRoomPage>(() => PageManager.Instance.GetPage<SelectRoomPage>().OpenPanel(1));
        }
        else if (info.daliy_type == 2)
        {//麻将
            PageManager.Instance.OpenPage<SelectRoomPage>(() => PageManager.Instance.GetPage<SelectRoomPage>().OpenPanel(1));
        }
        else if (info.daliy_type == 3)//朋友圈
            OnShare(SDKManager.WechatShareScene.WXSceneTimeline);
        else if (info.daliy_type == 4)
        {//开房一次
            NodeManager.OpenNode<JoinGameRoonNode>();
        }
    }

    /// <summary>
    /// 分享
    /// </summary>
    /// <param name="type"></param>
    void OnShare(SDKManager.WechatShareScene type)
    {
        Sprite icon = BundleManager.Instance.GetSprite("task/meirirenwu_pic_1");

        SDKManager.Instance.ShareWebPage(type, UserInfoModel.userInfo.downUrl, "雪瑶明水棋牌", string.Format("我正在雪瑶明水棋牌，快来跟我一起玩吧"), MiscUtils.SizeTextureBilinear(icon.texture, Vector2.one * 150).EncodeToJPG());
        NodeManager.CloseTargetNode<TaskNode>();
    }

}
