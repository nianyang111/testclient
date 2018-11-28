using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LandlordsBasePlayer : MonoBehaviour {

    public LandkirdsHandCardModel _handCard = null;
    [HideInInspector]
    public DzPopDesk popDesk;
    [HideInInspector]
    public LandlordsClock clock;
    [HideInInspector]
    public TalkView talkView;

    [HideInInspector]
    public GameObject dapaiObj, userObj, dizhuObj, vipObj, zhunbeiObj, dawangEffect;
    [HideInInspector]
    public Text desLb, ratioLb, winResultLb, loseResultLb, warning, coinCountLb, nameLb;
    [HideInInspector]
    public Transform handCard, resultCardsShow;
    [HideInInspector]
    public Image headIcon, identyIcon, coinIcon;

    protected virtual void Awake()
    {

    }

    #region 准备阶段

    /// <summary>
    /// 玩家进入
    /// </summary>
    /// <param name="handCardModel"></param>
    public virtual void Init(LandkirdsHandCardModel handCardModel, CallBack<LandkirdsHandCardModel> onClickHead)
    {
        this._handCard = handCardModel;
        userObj.SetActive(true);
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            MatchChangeState(false);
        else
            NoMatchChangeState(false);
        RestToZhunbei(_handCard.IsZhunbei);
        SetName();
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            coinIcon.gameObject.SetActive(false);
        else
        {
            coinIcon.gameObject.SetActive(true);
            if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.GoldBar)
                coinIcon.sprite = BundleManager.Instance.GetSprite("common/normal_log_2");
            else if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.SilverCoin)
                coinIcon.sprite = BundleManager.Instance.GetSprite("common/normal_log_1");
            else if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
                coinIcon.sprite = BundleManager.Instance.GetSprite("common/yuepaikaifang_huizhang_1");
            coinIcon.SetNativeSize();

        }
        SetCoin();

        vipObj.SetActive(_handCard.playerInfo.vip > 0);
        UGUIEventListener.Get(headIcon.gameObject).onClick = delegate { onClickHead(_handCard); };
    }

    /// <summary>
    /// 恢复到没人状态
    /// </summary>
    public virtual void RestToNoPlayer(bool isKick)
    {
        ClearDesk();
        ClearDesText();
        ClearResultLb();
        _handCard = null;        
        zhunbeiObj.SetActive(false);
        userObj.SetActive(false);
        dapaiObj.SetActive(false);
    }

    /// <summary>
    /// 恢复到准备状态
    /// </summary>
    protected virtual void RestToZhunbei(bool isZhunbei)
    {        
        ClearDesk();
        ClearDesText();
        ClearResultLb();
        Zhunbei(isZhunbei);
    }

    /// <summary>
    /// 准备/取消准备
    /// </summary>
    /// <param name="isZhunbei"></param>
    public virtual void Zhunbei(bool isZhunbei)
    {        
        zhunbeiObj.SetActive(isZhunbei);
    }    

    /// <summary>
    /// 设置货币
    /// </summary>
    /// <param name="num"></param>
    public void SetCoin()
    {
        if (_handCard == null)
            return;
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            coinCountLb.text = _handCard.MatchScore.ToString();
        else if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
            coinCountLb.text = _handCard.playerInfo.score.ToString();
        else
            coinCountLb.text = _handCard.playerInfo.money.ToString();
    }

    public void SetName()
    {
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch && _handCard.playerInfo.uid != UserInfoModel.userInfo.userId.ToString())
            nameLb.text = "";
        else
        {
            if (_handCard.playerInfo.uid == UserInfoModel.userInfo.userId.ToString())
                nameLb.text = _handCard.playerInfo.userNickname;
            else
                nameLb.text = Encoding.ASCII.GetByteCount(_handCard.playerInfo.userNickname) > 5 ? _handCard.playerInfo.userNickname.Substring(0, 4) + ".." : _handCard.playerInfo.userNickname;
        }
    }

    #endregion

    #region 打牌中

    // 回合进入回调
    public virtual void RoundEnter(bool isCanNoPlay)
    {
        desLb.gameObject.SetActive(false);
        ClearDesk();
    }

    // 回合离开回调
    public virtual void RoundExit()
    {
        clock.Close();
    }

    /// <summary>
    /// 游戏开始！
    /// </summary>
    public virtual void GameStart()
    {
        ClearDesk();
        ClearDesText();
        ClearResultLb();        
        dapaiObj.SetActive(true);
        zhunbeiObj.SetActive(false);
        if (_handCard != null)
            dizhuObj.SetActive(_handCard.AccessIdentity == Identity.Landlord);
        ChangeIdentity(false);
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="isDelay"></param>
    public virtual void DealCard(bool isDelay)
    {
        if (_handCard.CardsCount <= 2 && _handCard.CardsCount > 0)
            LandlordsPage.Instance.LandlordsWarning(_handCard.playerInfo.uid, _handCard.CardsCount, true);
    }        

    /// <summary>
    /// 该玩家出了牌
    /// </summary>
    public virtual void PopCard(List<Card> cardsList)
    {       
        ShowPopCard(cardsList);
    }

    /// <summary>
    /// 该玩家不出牌 -1要不起0不出
    /// </summary>
    public void NoPopCard(int type)
    {
        desLb.gameObject.SetActive(true);
        LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boyBuyao : AudioManager.AudioSoundType.girlBuyao);
        desLb.text = "不出";
    }

    /// <summary>
    /// 该玩家抢地主
    /// </summary>
    /// <param name="isQiang"></param>
    public void Qiangdizhu(bool isQiang)
    {
        if (isQiang)
        {
            LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boyQiangDizhu : AudioManager.AudioSoundType.girlQiangdizhu);
            desLb.text = "抢地主";
            StartCoroutine(SetRatioLb(2));
        }
        else
        {
            LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boyBuQiang : AudioManager.AudioSoundType.girlBuQiang);
            desLb.text = "不抢";
        }
        desLb.gameObject.SetActive(true);
    }

    /// <summary>
    /// 该玩家叫地主
    /// </summary>
    /// <param name="isQiang"></param>
    public void Jiaodizhu(bool isJiao)
    {
        if (isJiao)
        {
            LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boyJiaoDizhu : AudioManager.AudioSoundType.girlJiaoDizhu);
            desLb.text = "叫地主";
            StartCoroutine(SetRatioLb(3));
        }
        else
        {
            LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boyBuJiao : AudioManager.AudioSoundType.girlBuJiao);
            desLb.text = "不叫";
        }
        desLb.gameObject.SetActive(true);
    }

    /// <summary>
    /// 该玩家叫分
    /// </summary>
    /// <param name="score"></param>
    public void CallLandlord(int score)
    {
        if (score == 0)
        {
            LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boyBuJiao : AudioManager.AudioSoundType.girlBuJiao);
            desLb.text = "不叫";
        }
        else
        {
            switch (score)
            {
                case 1:
                    LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boy1fen : AudioManager.AudioSoundType.girl1fen);
                    break;
                case 2:
                    LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boy2fen : AudioManager.AudioSoundType.girl2fen);
                    break;
                case 3:
                    LandlordsModel.Instance.PlaySound(_handCard.Six == Six.boy ? AudioManager.AudioSoundType.boy3fen : AudioManager.AudioSoundType.girl3fen);
                    break;
            }
            desLb.text = score + "分";
        }
        desLb.gameObject.SetActive(true);

    }

    /// <summary>
    /// 设置倍数显示
    /// </summary>
    /// <param name="ratio"></param>
    IEnumerator SetRatioLb(int ratio)
    {
        ratioLb.gameObject.SetActive(true);
        ratioLb.text = "x" + ratio;
        yield return new WaitForSecondsRealtime(1.5f);
        CommonAnimation commonAni = ratioLb.GetComponent<CommonAnimation>();
        commonAni.Play();
        commonAni.pointEndAction = () => ratioLb.gameObject.SetActive(false);
    }

    /// <summary>
    /// 改变身份（地主/农民)
    /// </summary>
    public void ChangeIdentity(bool isRest)//是否还原
    {
        if (isRest)
        {
            dizhuObj.SetActive(false);
            identyIcon.gameObject.SetActive(false);
            return;
        }
        identyIcon.gameObject.SetActive(true);
        if (_handCard.AccessIdentity == Identity.Landlord)
        {
            identyIcon.sprite = BundleManager.Instance.GetSprite("doudizhu_icon_dizhumao", LandlordsPage.Instance.GetSpriteAB());
            dizhuObj.SetActive(true);
        }
        else
            identyIcon.sprite = BundleManager.Instance.GetSprite("doudizhu_icon_nongminmao", LandlordsPage.Instance.GetSpriteAB());
        AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.ToLandlords, PageManager.Instance.CurrentPage.name);
        identyIcon.SetNativeSize();
    }

    /// <summary>
    /// 非比赛场改变状态（机器人/正常)
    /// </summary>
    public void NoMatchChangeState(bool isRest)//是否还原
    {
        if (isRest || !_handCard.isTuoguan || !LandlordsModel.Instance.IsInFight)
        {
            StartCoroutine(MiscUtils.DownloadImage(_handCard.playerInfo.icon, spr =>
            {
                if (!_handCard.isTuoguan)
                    headIcon.sprite = spr;
            }));
        }
        else
        {
            if (LandlordsModel.Instance.IsInFight)
                headIcon.sprite = BundleManager.Instance.GetSprite("nomal_icon_jiqiren", PageManager.Instance.gamecommonBundle);
        }
    }

    /// <summary>
    /// 比赛场改变状态（机器人/正常系统头像)
    /// </summary>
    public void MatchChangeState(bool isRest)//是否还原
    {
        if (isRest || !_handCard.isTuoguan || !LandlordsModel.Instance.IsInFight)
        {
            switch (_handCard.playerInfo.six)
            {
                case Six.boy:
                    headIcon.sprite = BundleManager.Instance.GetSprite("normal_icon_nan", PageManager.Instance.gamecommonBundle);
                    break;
                case Six.girl:
                    headIcon.sprite = BundleManager.Instance.GetSprite("normal_icon_nv", PageManager.Instance.gamecommonBundle);
                    break;
            }
        }
        else
        {
            if (LandlordsModel.Instance.IsInFight)
                headIcon.sprite = BundleManager.Instance.GetSprite("nomal_icon_jiqiren", PageManager.Instance.gamecommonBundle);
        }
    }

    /// <summary>
    /// 设置警报显隐
    /// </summary>
    public void SetWarring(int remainCard, bool isShow)
    {
        warning.text = remainCard.ToString();
        warning.transform.parent.gameObject.SetActive(isShow);
    }

    #endregion

    public void Chat(string value, int type)
    {
        talkView.Chat(value, type);
    }

    #region 结算阶段
    /// <summary>
    /// 展示出的牌
    /// </summary>
    public void ShowPopCard(List<Card> cardsList)
    {
        popDesk.Inits(cardsList);
        if (cardsList.Count == 1 && cardsList[0].GetCardWeight == Weight.LJoker)//大王效果
        {
            dawangEffect.SetActive(true);
            UIUtils.DelayDesOrDisObject(dawangEffect, 2, false);
        }
    }

    /// <summary>
    /// 显示结算文本
    /// </summary>
    /// <param name="args"></param>
    public void ShowResultLb(int gold)
    {
        if (gold > 0)
        {
            winResultLb.gameObject.SetActive(true);
            winResultLb.text = "+" + gold.ToString();
        }
        else
        {
            loseResultLb.gameObject.SetActive(true);
            loseResultLb.text = gold.ToString();
        }
    }

    /// <summary>
    /// 游戏结束回调
    /// </summary>
    /// <param name="args"></param>
    public void GameOver()
    {
        //停止计时
        clock.Close();
        //清除描述文字
        ClearDesText();
    }

    #endregion

    #region 清理UI

    /// <summary>清理自己出的牌</summary>
    public void ClearDesk()
    {
        popDesk.ClearPopesk();
    }

    /// <summary>清理描述文字</summary>
    public void ClearDesText()
    {
        desLb.text = "";
        desLb.gameObject.SetActive(false);
    }

    /// <summary>清除结算文本</summary>
    public void ClearResultLb()
    {
        winResultLb.gameObject.SetActive(false);
        loseResultLb.gameObject.SetActive(false);
    }

    /// <summary>清理闹钟</summary>
    public void ClearClock()
    {
        clock.Close();
    }
    #endregion
}
