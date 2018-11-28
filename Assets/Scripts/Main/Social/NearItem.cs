using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NearItem : MonoBehaviour
{
    public Image headIcon;
    public Text nameLb;
    public Text distanceLb;
    public Button addFriendBtn;
    public Text btnText;
    public GameObject btnAdd;
    public void Init(FriendInfo info, CallBack<FriendInfo> addFriendCall)
    {
        StartCoroutine(MiscUtils.DownloadImage(info.photo, spr =>
            {
                headIcon.sprite = spr;
            }));
        
        nameLb.text = info.nickname;
        distanceLb.text = info.distance + "米以内";
        SetBtnState(info.relation);
        UGUIEventListener.Get(addFriendBtn.gameObject).onClick = delegate
        {
            if (!addFriendBtn.interactable)
                return;
            SetBtnState(1);
            addFriendCall(info);
            info.relation = 1;
        };
    }

    //设置按钮状态
    void SetBtnState(int relation)
    {
        btnText.horizontalOverflow = HorizontalWrapMode.Overflow;
        FriendApplyState state = SocialModel.Instance.getFriendState(relation);
        switch (state)
        {
            case FriendApplyState.Normal:
                addFriendBtn.interactable = true;
                btnText.text = "好友";
                btnText.transform.localPosition = new Vector3(28, btnText.transform.localPosition.y);
                btnAdd.SetActive(true);
                break;
            case FriendApplyState.MeAppling:
                addFriendBtn.interactable = false;
                btnText.text = "已申请";
                btnAdd.SetActive(false);
                btnText.transform.localPosition = new Vector3(0, btnText.transform.localPosition.y);
                break;
            case FriendApplyState.HisAppling:
                addFriendBtn.interactable = false;
                btnText.text = "对方已申请";
                btnAdd.SetActive(false);
                btnText.transform.localPosition = new Vector3(0, btnText.transform.localPosition.y);
                break;
            case FriendApplyState.Friending:
                addFriendBtn.interactable = false;
                btnText.text = "已是好友";
                btnAdd.SetActive(false);
                btnText.transform.localPosition = new Vector3(0, btnText.transform.localPosition.y);
                break;
            default:
                break;
        }
    }
}
