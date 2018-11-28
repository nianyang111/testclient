using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// 公告Node
/// </summary>
public class NoticeNode : Node
{
    /// <summary>
    /// 公告尺寸布局
    /// </summary>
    public enum NoticeSize
    {
        Long,
        Short,
    }

    #region 私有域

    /***公告消息列表***/
    List<string> noticeList = new List<string>();

    /***当前系统循环索引***/
    int curSystemNoticeIndex = 0;

    /*** 每条消息播放次数***/
    int playCount = 1;

    /***播放次数***/
    int curPlayCount = 0;

    /***是否正在播放***/
    bool isPlaying = false;

    /***Text当前控件***/
    Text curText;

    #endregion

    #region 公有域
    /// <summary>循环中的系统消息列表</summary>
    public List<string> SystemNotices = new List<string>();

    /// <summary>公告背景</summary>
    public RectTransform noticeBg;

    /// <summary>公告scrollView</summary>
    public RectTransform noticeScrollView;

    /// <summary> 公告显示文本 </summary>
    public GameObject prefabLb;

    public RectTransform content;

    /// <summary>喇叭按钮</summary>
    public GameObject labaBtn;

    /// <summary>位置父物体</summary>
    public Transform posTrans;

    #endregion

    #region 测试
    /// <summary>
    /// 测试内容
    /// </summary>
    public string mNotice;

    void Awake()
    {
        //InvokeRepeating("Test", 1f, 5f);
    }

    //测试用
    void Test()
    {
        AddNotice(mNotice);
    }

    #endregion

    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(labaBtn).onClick = delegate { NodeManager.OpenNode<TrumpetNode>(); };
        if (noticeList.Count == 0)
            SetVisibel(false);
        InvokeRepeating("SystemAddNotice", 1f, 20f);
    }

    /// <summary>
    /// 初始化布局，不调用则以预制件为准
    /// </summary>
    /// <param name="noticePos"></param>
    /// <param name="noticeSize"></param>
    public void Inits(Vector3 noticePos, NoticeSize noticeSize = NoticeSize.Long)
    {
        posTrans.localPosition = noticePos;
        switch (noticeSize)
        {
            case NoticeSize.Long:
                labaBtn.transform.parent.localPosition = Vector2.left * 700;
                content.sizeDelta = new Vector2(1261, 80);
                noticeBg.sizeDelta = new Vector2(1261, 80);
                noticeScrollView.sizeDelta = new Vector2(1261, 51);
                break;
            case NoticeSize.Short:
                labaBtn.transform.parent.localPosition = Vector2.left * 510;
                content.sizeDelta = new Vector2(920, 80);
                noticeBg.sizeDelta = new Vector2(920, 80);
                noticeScrollView.sizeDelta = new Vector2(920, 51);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 循环系统消息
    /// </summary>
    void SystemAddNotice()
    {
        if (SystemNotices.Count == 0)
            return;
        if (curSystemNoticeIndex >= SystemNotices.Count)
            curSystemNoticeIndex = 0;
        AddNotice(SystemNotices[curSystemNoticeIndex]);
        curSystemNoticeIndex++;
    }    

    /// <summary>
    /// 添加一条公告
    /// </summary>
    void AddNotice(string text)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
            return;
        noticeList.Add(text);
        if (!isPlaying)
            NoticeItem(noticeList[0]);
    }

    /// <summary>
    /// 创建Item
    /// </summary>
    /// <param name="not"></param>
    void NoticeItem(string not)
    {
        SetVisibel(true);
        isPlaying = true;
        if (curText == null)
        {
            GameObject noticeItem = GameObject.Instantiate(prefabLb);
            noticeItem.SetActive(true);
            noticeItem.transform.SetParent(prefabLb.transform.parent);
            noticeItem.transform.localScale = Vector3.one;
            curText = noticeItem.GetComponent<Text>();
        }
        //curText.transform.localPosition = new Vector3(380, 0);        
        curText.text = not;
        SetMovePos();
    }

    /// <summary>
    /// 设置滚动
    /// </summary>
    void SetMovePos()
    {
        CommonAnimation anim = curText.gameObject.GetComponent<CommonAnimation>();
        anim.pointList.Clear();
        anim.pointDelayTime = 1f;
        anim.pointList.Add(new Vector2(851, 0));
        anim.pointList.Add(new Vector2(851 - curText.preferredWidth - (content.transform as RectTransform).sizeDelta.x - 30-200, 0));
        anim.time = curText.preferredWidth / (curText.preferredWidth) * 5;
        SetVisibel(true);
        curText.gameObject.SetActive(true);
        anim.Play();
        float a = curText.preferredWidth;
        anim.pointEndAction = () =>
        {
            OnMoveEnd();
        };
    }

    /// <summary>
    /// 移动结束事件
    /// </summary>
    void OnMoveEnd()
    {
        curText.transform.position = new Vector2(851, 0);
        curPlayCount++;
        if (curPlayCount == playCount)
        {
            noticeList.Remove(noticeList[0]);
            curPlayCount = 0;
        }
        if (noticeList.Count > 0)
            NoticeItem(noticeList[0]);
        else
        {
            SetVisibel(false);
            isPlaying = false;
        }
    }

    void SetVisibel(bool isShow)
    {
        content.gameObject.SetActive(isShow);
    }


    /// <summary>
    /// 添加喇叭
    /// </summary>
    public static void Add(string text)
    {
        NoticeNode node = NodeManager.GetNode<NoticeNode>();
        if (node != null)
            node.AddNotice(text);
    }
}


