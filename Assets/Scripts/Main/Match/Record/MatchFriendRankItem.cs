using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatchFriendRankItem : MonoBehaviour, IPointerClickHandler
{
    public Image rankIcon;
    public Text rankText, nameText, lvText, scoreText;
    public GameObject desPanel,closeBtn;
    public Text masterLv, masterScore, champion, promotion, finals ,des;
    public MatchFriendRankPanel panel;
    MatchFriendRankData _data;
    public void Init(MatchFriendRankData data)
    {
        _data=data;
        UGUIEventListener.Get(closeBtn).onClick = delegate { OpenFrendInfo(); };
        closeBtn.transform.localScale = new Vector2(Screen.width, Screen.height);
        nameText.text = _data.userName;
        lvText.text = string.Format("Lv."+_data.masterLevel);
        scoreText.text = string.Format(_data.masterScore+"大师分");
        masterScore.text = string.Format("大师分:" + _data.masterScore);
        masterLv.text = string.Format("等级：Lv." + _data.masterLevel);

        desPanel.SetActive(false);
        if (_data.rankId > 0 && _data.rankId < 4)
        {
            rankText.text = "";
            rankIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_jiangbei_" + _data.rankId);
        }
        else
        {
            rankText.text = _data.rankId.ToString();
            rankIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_icon_mingci");
        }
        if (_data.userId == UserInfoModel.userInfo.userId)
        {
            var count = MatchModel.Instance.matcherCount;
            champion.text = count.successNum.ToString();
            promotion.text = count.promotionNum.ToString();
            finals.text = count.finalistNum.ToString();
            if (count.latestMatcher == null || count.latestMatcher.Replace(" ", "") == "")
                des.text = "暂无参赛记录";
            else
                des.text = "最近常玩 " + count.latestMatcher;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!AudioManager.Instance.IsSoundPlaying)
            AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
        OpenFrendInfo();
    }
    /// <summary>
    /// 打开详细信息
    /// </summary>
    void OpenFrendInfo()
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QueryMatcherCount,
            QueryMatcherCount = new net_protocol.QueryMatcherCount()
            {
                userId = _data.userId
            }
        });
        panel.CurItem = this;
    }
    /// <summary>
    /// 选中
    /// </summary>
    public void Select()
    {
        desPanel.SetActive(true);
        desPanel.transform.SetParent(panel.show);
    }
    /// <summary>
    /// 非选中
    /// </summary>
    public void OnSelect()
    {
        desPanel.SetActive(false);
        desPanel.transform.SetParent(this.transform);
    }
    /// <summary>
    /// 更新信息
    /// </summary>
    public void FlushData()
    {
        var flushData = MatchModel.Instance.friendList.Find(p => p.userId == _data.userId).matcherCount;
        if(flushData!=null)
        {
            champion.text = flushData.successNum.ToString();
            promotion.text = flushData.promotionNum.ToString();
            finals.text = flushData.finalistNum.ToString();
            if (flushData.latestMatcher.Replace(" ", "") == "")
                des.text = "暂无参赛记录";
            else
                des.text ="最近常玩 "+ flushData.latestMatcher;
        }
    }
}
public class MatchFriendRankData
{
    public int rankId;
    public string userName;
    public long masterScore;
    public int masterLevel;
    public int userId;
    public MatcherCount matcherCount;
}