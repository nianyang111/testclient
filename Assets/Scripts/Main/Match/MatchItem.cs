using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchItem : MonoBehaviour
{
    public Text title, applyNum, applyFree;
    public Image logIcon, applyIcon, awardIcon;
    public Text hintText, joinNumText, beginText;
    public Button matchBtn;
    public MatchPage _page;
    MatcherInfo _data;
    public void Init(MatcherInfo data)
    {
        _data = data;
        title.text = _data.name;

        logIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_icon_" + _data.type);
        applyIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_log_" + _data.costType);
        applyIcon.SetNativeSize();

        hintText.text = _data.timeType == 1 ? "今晚" + _data.beginTime + "开赛" : "每" + _data.beginTime + "分一场";
        beginText.text = MatchPage.GetTimerText(_data.distance, 3);
        joinNumText.text = _data.joinUser.ToString();

        applyNum.text = _data.cost.ToString();
        applyNum.gameObject.SetActive(_data.cost > 0);
        applyFree.gameObject.SetActive(_data.cost == -1);

        if (_data.distance > 0)
            StartCoroutine(UpMyTime());
        if (_data.icon != "")
            StartCoroutine(MiscUtils.DownloadImage(_data.icon, spr => { if (spr != null)awardIcon.sprite = spr; }));
        else
            Debug.Log("图片地址为空：" + _data.name);
        matchBtn.onClick.AddListener(() =>
        {
            MatchModel.Instance.CurData = _data;
            if (awardIcon.sprite != null)
                MatchModel.Instance.rewardIcon = awardIcon.sprite;
            NodeManager.OpenNode<MatchApplyNode>("match");
            AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
        });
    }
    IEnumerator UpMyTime()
    {
        while (_data.distance > 0)
        {

            beginText.text = MatchPage.GetTimerText(_data.distance, 3);
            _data.distance--;
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        beginText.text = "";
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_LoadMatcher,
            loadMatcher = new net_protocol.LoadMatcher() { type = _page.CurType }
        });
    }
    void OnDestroy()
    {

    }
}
