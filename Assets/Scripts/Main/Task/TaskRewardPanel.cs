using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TaskRewardPanel : MonoBehaviour
{

    public Image rewardIcon;

    public Text rewardTypeText;
    public Text rewardCountText;

    public GameObject closeBtn;//知道了

    public void Inits(int rewardCount)
    {
        rewardIcon.sprite = BundleManager.Instance.GetSprite("common/normal_log_1");
        rewardTypeText.text = "银币";
        rewardCountText.text = "x" + rewardCount;
        UGUIEventListener.Get(closeBtn).onClick = delegate { SetVisible(false); };
    }

    public void SetVisible(bool isShow)
    {
        gameObject.SetActive(isShow);
        if (isShow)
        {
            PageManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            PageManager.Instance.canvas.worldCamera = Camera.main;
        }
        else
            PageManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }
}
