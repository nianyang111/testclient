using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 排行榜每个玩家数据显示
/// </summary>
public class RankItem : MonoBehaviour
{
    [HideInInspector]
    public Image rankNumIcon,playIcon;
    [HideInInspector]
    public Text rankNumText,playNameOrId,getNum;
    [HideInInspector]
    public GameObject vip;
    [HideInInspector]
    public RankNode _node;
    private RankData _data;
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(RankData data)
    {
        _data = data;
        SetValue();
        vip.SetActive(_data.vip>0);
        getNum.text = RankNode.KeepGoldNum(_data.getNum);
        playNameOrId.text = string.Format(_data.nickName + " ID:" + _data.userId);
        UGUIEventListener.Get(playIcon.gameObject).onClick = (g) => { _node.playinfoPanel.OpenPanel(_data); };
        StartCoroutine(MiscUtils.DownloadImage(_data.icon, spr => { if(spr!=null) playIcon.sprite = _data.iconSprite = spr; }));
    }
    /// <summary>
    /// 设置排名
    /// </summary>
    private void SetValue()
    {
        if (_data.index > 0 && _data.index < 4)
        {
            rankNumText.gameObject.SetActive(false);
            rankNumIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_jiangbei_" + _data.index);
            rankNumIcon.SetNativeSize();
        }
        else
        {
            rankNumText.gameObject.SetActive(true);
            rankNumText.text = _data.index.ToString();
            rankNumIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_icon_mingci");
        }
    }
    //IEnumerator DownLoadHeadIcon()
    //{
    //    WWW www = new WWW(UserInfoModel.userInfo.headIcon);
    //    yield return www;
    //    if (string.IsNullOrEmpty(www.error))
    //    {
    //        _data.iconSprite = MiscUtils.TextureToSprite(www.texture);
    //        playIcon.sprite = _data.iconSprite;
    //    }
    //    else
    //        UIUtils.Log(www.error);
    //}
    /// <summary>
    /// 销毁
    /// </summary>
    public void DestroyItem()
    {
        Destroy(gameObject);
    }

}
/// <summary>
/// 排行榜玩家数据
/// </summary>
public class RankData
{
    public Sprite iconSprite;
    public int userId;
    public string openId;
    public string nickName;
    public string icon;
    public int gender;
    public long ag;
    public long gold;
    public int vip;
    public int level;
    public long getNum;
    public int index;
    public long curExp;
}