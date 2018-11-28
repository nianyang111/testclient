using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class FriendRankPanel : MonoBehaviour
{

    public InputField findInput;
    public GameObject input_findBtn;

    public UIWarpContent warp;    
    public GameObject friendItem;

    public Transform findParent;

    public GameObject noFriendObj;

    public Dropdown dropDown;
    public Transform arrow;
    //当前排序模式
    public SortState curSortState = SortState.Line; 


    List<FriendInfo> friendRankInfos = new List<FriendInfo>();

    void Start()
    {
        UGUIEventListener.Get(input_findBtn).onClick = delegate { OnInputValueChanged(); };
        findInput.onValueChanged.AddListener((str) =>
            {
                if(string.IsNullOrEmpty(str))
                {
                    noFriendObj.SetActive(friendRankInfos.Count == 0);
                    warp.gameObject.SetActive(true);
                    UIUtils.DestroyChildren(findParent);
                }
            });
        warp.onInitializeItem = InitItem;
        SetDropDown();
        Inits();
    }

    public static void Inits()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid=MessageId.C2G_QueryFriendReq
            });           
    }

    /// <summary>
    /// 服务器发来好友排行
    /// </summary>
    public void G2C_FriendRank(List<FriendInfo> rankInfos)
    {
        this.friendRankInfos = rankInfos;
        SortByState();
        warp.Init(rankInfos.Count);
        noFriendObj.SetActive(rankInfos.Count == 0);
    }
    
    void SetDropDown()
    {
        dropDown.ClearOptions();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        list.Add(new Dropdown.OptionData("在线"));
        list.Add(new Dropdown.OptionData("银币"));
        list.Add(new Dropdown.OptionData("等级"));
        list.Add(new Dropdown.OptionData("大师分"));
        dropDown.AddOptions(list);
        dropDown.onValueChanged.AddListener(DropDown);                    
        dropDown.value = (int)curSortState;
        UGUIEventListener.Get(dropDown.gameObject).onClick = delegate { DropDownClick(); };
    }

    /// <summary>
    /// 下拉菜单点击回调
    /// </summary>
    void DropDownClick()
    {
        print("点击");
        //arrow.localEulerAngles = arrow.localEulerAngles.z == 180 ? new Vector3(0, 0, 0) : new Vector3(0, 0, 180);
    }

    /// <summary>
    /// 下拉菜单ValueChanged回调
    /// </summary>
    void DropDown(int index)
    {
        curSortState = (SortState)System.Enum.Parse(typeof(SortState), index.ToString());
        SortByState();
        TipManager.Instance.OpenTip(TipType.SimpleTip, "操作成功！");
    }
       

    void OnInputValueChanged()
    {        
        if (string.IsNullOrEmpty(findInput.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请输入好友昵称或ID");            
        }
        else
        {
            UIUtils.DestroyChildren(findParent);
            FriendInfo info = FindFriendByNameOrId(findInput.text);
            warp.gameObject.SetActive(false);
            noFriendObj.SetActive(info == null);
            if (info != null)
            {
                FriendRankItem item = Instantiate(friendItem, findParent).GetComponent<FriendRankItem>();
                item.Init(info, 1);
            }
        }
    }

    /// <summary>
    /// 根据昵称或者id查找好友
    /// </summary>
    FriendInfo FindFriendByNameOrId(string value)
    {
        FriendInfo info = null;
        if (friendRankInfos.Count == 0)
            return info;
        info = friendRankInfos.Find(p => p.nickname == value);
        if (info == null && GameTool.iNum(value))        
            info = friendRankInfos.Find(p => p.userId == int.Parse(value));
        return info;
    }

    void InitItem(GameObject go, int index)
    {
        FriendRankItem item = go.GetComponent<FriendRankItem>();
        if (item == null)
            item = go.AddComponent<FriendRankItem>();
        item.Init(friendRankInfos[index], index + 1);
    }

    void SortByState()
    {
        switch (curSortState)
        {
            case SortState.Line:
                IdSort();
                break;
            case SortState.Yinbi:
                YinbiSort();
                break;
            case SortState.Lv:
                LvSort();
                break;
            case SortState.Dashifen:
                DashifenSort();
                break;
            default:
                break;
        }
        warp.Init(friendRankInfos.Count);
    }

    /// <summary>
    /// id(在线)排序
    /// </summary>
    void IdSort()
    {
        if (friendRankInfos == null)
            return;
        friendRankInfos.Sort((a, b) =>
        {
            int num = b.isOnline.CompareTo(a.isOnline);
            if (num == 0)
                num = a.userId.CompareTo(b.userId);
            return num;
        });
    }

    /// <summary>
    /// 等级排序
    /// </summary>
    void LvSort()
    {
        if (friendRankInfos == null)
            return;
        friendRankInfos.Sort((a, b) =>
        {
            int num = b.level.CompareTo(a.level);
            if (num == 0)
            {
                num = b.exp.CompareTo(a.exp);
            }
            return num;
        });        
    }

    /// <summary>
    /// 银币排序
    /// </summary>
    void YinbiSort()
    {
        if (friendRankInfos == null)
            return;
        friendRankInfos.Sort((a, b) =>
        {

            return b.sliver.CompareTo(a.sliver);
        });
    }

    /// <summary>
    /// 大师分排序
    /// </summary>
    void DashifenSort()
    {
        if (friendRankInfos == null)
            return;
        friendRankInfos.Sort((a, b) =>
        {
            return b.masterScore.CompareTo(a.masterScore);
        });
    }


    public enum SortState
    {
        /// <summary>
        /// 在线
        /// </summary>
        Line=0,
        /// <summary>
        /// 银币
        /// </summary>
        Yinbi=1,
        /// <summary>
        /// 等级
        /// </summary>
        Lv=2,
        /// <summary>
        /// 大师分
        /// </summary>
        Dashifen=3,
    }
}


