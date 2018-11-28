using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowPanelPanel : MonoBehaviour 
{
    public GameObject initObj;

    public GameObject showObj;
    public Transform parent;
    public GameObject itemPrefab;

    public GameObject goBtn;//去看看

    public GameObject changeBtn;//换一个

    List<FriendInfo> friends = new List<FriendInfo>();
    /// <summary>
    /// 有关item的
    /// </summary>
    public GameObject knowPrefab;
    public Image headIcon;
    public Text nameLb;
    public Text commonLb;
    public Button addFriendBtn;
    public Text btnText;
    public GameObject btnAdd;
    //当前可能认识的人
    FriendInfo curInfo;

    void OnEnable()
    {
        if (curInfo != null)
        {
            if (headIcon.sprite == null)
            {
                StartCoroutine(MiscUtils.DownloadImage(curInfo.photo, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
            else if (headIcon.sprite.name != "")
            {
                StartCoroutine(MiscUtils.DownloadImage(curInfo.photo, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
        }
    }

    void Start()
    {
        UGUIEventListener.Get(goBtn).onClick = delegate { FindFriendByPhone(); };
        UGUIEventListener.Get(changeBtn).onClick = delegate { Change(); };
        
        initObj.SetActive(true);
        showObj.SetActive(false);
    }

    /// <summary>
    /// 去看看按钮回调 
    /// </summary>
    void FindFriendByPhone()
    {
        initObj.SetActive(false);
        showObj.SetActive(true);
        ReqKnow();
    }

 

    /// <summary>
    /// 请求认识的人列表
    /// </summary>
    void ReqKnow()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {                
                msgid = MessageId.C2G_QueryRenshiReq
            });
    }

    /// <summary>
    /// 模拟服务器推送可能认识的人
    /// </summary>
    public void G2C_PossibleKnow()
    {
        if (friends.Count == 0)
        {
            knowPrefab.SetActive(false);
            return;
        }
        int index = Random.Range(0, friends.Count);
        FriendInfo info = friends[index];
        curInfo = info;
        knowPrefab.SetActive(true);
        StartCoroutine(MiscUtils.DownloadImage(info.photo, spr =>
            {
                headIcon.sprite = spr;
            }));        
        nameLb.text = info.nickname;
        commonLb.text = "你和 TA 有 " + info.gthy + " 位共同好友 ";
        SetBtnState(info.relation);
        UGUIEventListener.Get(addFriendBtn.gameObject).onClick = delegate { AddFriend(); };
    }

    /// <summary>
    /// 服务器推送认识的人
    /// </summary>
    public static void G2C_Know(List<FriendInfo> infos)
    {        
        SocialNode node = NodeManager.GetNode<SocialNode>();
        if (node)
        {            
            node.knownPanel.LoadItems(infos);
        }
    }

    public void LoadItems(List<FriendInfo> infos)
    {
        friends = infos;        
        UIUtils.DestroyChildren(parent);
        for (int i = 0; i < infos.Count; i++)
        {
            KnowItem item = Instantiate(itemPrefab, parent).GetComponent<KnowItem>();
            item.Init(infos[i]);
        }
        G2C_PossibleKnow();
    }

    /// <summary>
    /// 换一个按钮回调
    /// </summary>
    public void Change()
    {
        G2C_PossibleKnow();
    }

    /// <summary>
    /// 加好友按钮回调
    /// </summary>
    void AddFriend()
    {
        if (!addFriendBtn.interactable)
            return;
        SetBtnState(1);
        SocialModel.Instance.AddFriend(curInfo.userId);
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
                btnAdd.SetActive(true);
                btnText.transform.localPosition = new Vector3(28, btnText.transform.localPosition.y);
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
