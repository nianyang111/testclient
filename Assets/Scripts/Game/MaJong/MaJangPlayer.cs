using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaJangPlayer : MonoBehaviour
{
    public int userId, seatNo = -1;
    public bool isTurn = false;//是否轮到出牌
    public int statu = 0;//0-正常出牌，1-准备听，2-听牌状态 3已经胡牌
    public bool atTrusteeship = false;
    public bool isWaitAction = false;//是否需要等待
    public bool isOutNewCard = false;//是否要出新牌
    public int sex;
    public MaJangResultItem mjri;
    public Transform handParent, outParent, lightParent;
    public Text nickName, currencyNum;
    public Image headIcon, currencyIcon;
    public GameObject btnChangeSeat, loseConnectIcon, vipIcon, readyIcon, bankerIcon, tingIcon, huIcon, zimoIcon, trusteeshipIcon, showMj;
    public SequenceAnimation xjxdAnimation;
    public CommonAnimation actionAnimation;
    public Text addScoreText, minusScoreText;
    public List<Mj> mjList = new List<Mj>();
    public List<MjTing> tingOutList = new List<MjTing>();
    public List<MaJangModel> handMjList = new List<MaJangModel>();
    public List<MaJangModel[]> lightMjList = new List<MaJangModel[]>();
    bool isCurrentPlayer = false, isGang = false;
    public TalkView talkView;
    public CommonAnimation kickedObj;
    public bool isFinishAction = true;//是否已经完成了操作行为
    public int win;
    void Start()
    {
        UGUIEventListener.Get(headIcon.gameObject).onClick = delegate
        {
            if (userId == 0)
                ChangeSeat();
            else
                NodeManager.OpenNode<GameRoleInfoNode>().Inits(userId);
        };
        if (btnChangeSeat)
            UGUIEventListener.Get(btnChangeSeat).onClick = delegate { ChangeSeat(); };
    }

    public void Init(GameUser gu)
    {
        this.userId = gu.uid;
        this.seatNo = gu.pos;
        this.sex = gu.sex;
        if (MaJangPage.Instance.playerCount < 4)
        {
            this.seatNo *= 2;
            columnMax = 15;//2人玩法每行15张牌，起始位置x=-5.8
            outParent.localPosition = new Vector3(-5.8f, 0, 5.15f);
        }
        this.nickName.text = gu.name.Length > 5 ? gu.name.Substring(0, 4) + "..." : gu.name;
        this.currencyNum.text = gu.coin.ToString();
        readyIcon.SetActive(gu.prepared);
        vipIcon.SetActive(gu.vip);
        currencyIcon.sprite = MaJangPage.Instance.currencySprites[(int)MaJangPage.Instance.roomType - 1];
        isCurrentPlayer = userId == UserInfoModel.userInfo.userId;
        gameObject.SetActive(true);
        StartCoroutine(MiscUtils.DownloadImage(gu.icon, sprite => { headIcon.sprite = sprite; }));
        if (btnChangeSeat)
            btnChangeSeat.SetActive(true);
    }

    //换座位
    void ChangeSeat()
    {
        if (!MaJangPage.isSendChangeSeatRequest)
        {
            int seat = seatNo;
            int oldSeat = MaJangPage.Instance.currentPlayer.seatNo;
            if (MaJangPage.Instance.playerCount == 2)
            {
                oldSeat /= 2;
                seat /= 2;
            }
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_MjChangePosReq,
                mjChangePosReq = new MjChangePosReq()
                {
                    type = 1,
                    oldPosUserId = MaJangPage.Instance.currentPlayer.userId,
                    oldPos = oldSeat,
                    posUserId = userId,
                    pos = seat
                }
            });
        }
        else
            TipManager.Instance.OpenTip(TipType.SimpleTip, "你已经发起了换座位请求，请不要重复发起");
    }

    //清空数据
    public void ClearData()
    {
        handMjList.Clear();
        lightMjList.Clear();
        tingOutList.Clear();
        handParent.localPosition = new Vector3(-5, 0.35f, handParent.localPosition.z);
        if (btnChangeSeat)
            btnChangeSeat.SetActive(false);
    }

    //清空ui显示
    public void ClearUI()
    {
        isGang = false;
        readyIcon.SetActive(false);
        huIcon.SetActive(false);
        zimoIcon.SetActive(false);
        tingIcon.SetActive(false);
        bankerIcon.SetActive(false);
        showMj.SetActive(false);
        actionAnimation.gameObject.SetActive(false);
    }

    //准备
    public void Ready(bool isReady)
    {
        readyIcon.SetActive(isReady);
        if (isCurrentPlayer)
        {
            huIcon.SetActive(false);
            zimoIcon.SetActive(false);
            MaJangPage.Instance.liuJuImage.SetActive(false);
        }
    }

    //离开  是否被踢
    public void Leave(bool iskicked)
    {
        if (iskicked && kickedObj)
        {
            kickedObj.pointEndAction = delegate { kickedObj.gameObject.SetActive(false); };
            kickedObj.gameObject.SetActive(true);
        }
        userId = 0;
        seatNo = -1;
        nickName.text = currencyNum.text = null;
        headIcon.sprite = null;
        vipIcon.SetActive(false);
        readyIcon.SetActive(false);
        tingIcon.SetActive(false);
        if (loseConnectIcon)
            loseConnectIcon.SetActive(false);
        if (iskicked)
            UIUtils.DelayDesOrDisObject(gameObject, 0.8f, false);
        else
            gameObject.SetActive(false);
    }

    //状态改变
    public void ConnectChange(bool isConnect)
    {
        if (loseConnectIcon)
            loseConnectIcon.SetActive(!isConnect);
    }

    //托管
    public void TrusteeshipResult(bool isTrusteeship)
    {
        if (atTrusteeship != isTrusteeship)
        {
            atTrusteeship = isTrusteeship;
            trusteeshipIcon.SetActive(isTrusteeship);
            if (isCurrentPlayer)
            {
                MaJangPage.Instance.trusteeshipPanel.SetActive(isTrusteeship);
                if (isTrusteeship)
                {
                    TrusteeshipMethods();
                }
            }
        }
    }
    /// <summary>
    /// 托管方法
    /// </summary>
    public void TrusteeshipMethods()
    {
        isOutNewCard = true;
        if (MaJangPage.Instance.gameMenu.activeSelf)
        {
            if (MaJangPage.Instance.btnTing.activeSelf)
            {
                MaJangPage.Instance.FinishAction();
                Ting();
            }
            else if (MaJangPage.Instance.btnChiTing.activeSelf)
            {
                MaJangPage.Instance.FinishAction();
                ChiTing();
            }
            else if (MaJangPage.Instance.btnDingTing.activeSelf)
            {
                MaJangPage.Instance.FinishAction();
                DingTing();
            }
            else if (MaJangPage.Instance.btnPengTing.activeSelf)
            {
                MaJangPage.Instance.FinishAction();
                PengTing();
            }
            else if (MaJangPage.Instance.btnGang.activeSelf)
            {
                MaJangPage.Instance.FinishAction();
                Gang();
                isOutNewCard = false;
            }
            else if (MaJangPage.Instance.btnPeng.activeSelf)
            {
                MaJangPage.Instance.FinishAction();
                Peng();
            }
            else if (MaJangPage.Instance.btnChi.activeSelf)
            {
                MaJangPage.Instance.FinishAction();
                Chi();
            }
            else if (MaJangPage.Instance.btnGuo.activeSelf)
                CreateAction(MjActionType.Guo);
            MaJangPage.Instance.FinishAction();
        }
        if (handMjList.Count + lightParent.childCount >= 14)
        {
            if (statu != 1)
                CreateAction(MjActionType.OutMj, handMjList[handMjList.Count - 1]);
            else
                CreateAction(MjActionType.OutMj, handMjList.Find(p => !p.mask.activeInHierarchy));
        }
    }

    #region ...通信相关
    //发起行为，出牌碰、吃、杠等
    public void CreateAction(MjActionType type, MaJangModel mjm = null, MaJangModel otherMjm = null)
    {
        UserMjAction uma = new UserMjAction();
        uma.type = (int)type;
        if (type != MjActionType.Guo)
            if (type == MjActionType.AGang)
                uma.mj = sameMj[0].mj;
            else
                uma.mj = mjm.mj;
        if (otherMjm)
            uma.mjOther = otherMjm.mj;
        if (statu == 1)
            uma.isTing = true;
        if (type == MjActionType.OutMj)
            OutMj(mjm);
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_UserMjAction,
            UserMjAction = uma
        });
        isTurn = false;
    }
    //行为回调，部分行为受到回调后才会继续执行
    public void ExecuteAction(UserActionResp uar)
    {
        MjActionType type = (MjActionType)uar.type;
        switch (type)
        {
            case MjActionType.Peng:
                if (sameMj.Count > 3)
                    sameMj.RemoveAt(0);
                LightMj(sameMj);
                isTurn = true;
                break;
            case MjActionType.DGang:
                LightMj(sameMj);
                break;
            case MjActionType.Chi:
                LightMj(chiMj);
                isTurn = true;
                break;
            case MjActionType.ChiTing:
                LightMj(chiTingMj, false, false);
                SetTingMj(uar.mj);
                Ting();
                isTurn = true;
                break;
            case MjActionType.DingTing:
            case MjActionType.PengTing:
                if (sameMj.Count > 3)
                    sameMj.RemoveAt(0);
                LightMj(sameMj, false, false);
                SetTingMj(uar.mj);
                Ting();
                isTurn = true;
                break;
        }
        ActionAnimation(type);
    }
    private AudioManager.AudioSoundType GetSoundType(string mType)
    {
        return (AudioManager.AudioSoundType)System.Enum.Parse(typeof(AudioManager.AudioSoundType), mType);
    }
    private void PlaySound(AudioManager.AudioSoundType audioType)
    {
        if (SetNode.off == 0 && SetNode.read == 1)
            AudioManager.Instance.PlayTempSound(audioType, PageManager.Instance.CurrentPage.name);
    }
    #endregion

    #region ...基本操作
    public void GetMj(Mj mj, bool isNewMj = false, bool isFenZhang = false, bool canHu = false, bool isXjxd = false)
    {
        MaJangPage.Instance.StartCoroutine(GetMjAc(mj, isNewMj, isFenZhang, canHu, isXjxd));
    }

    IEnumerator GetMjAc(Mj mj, bool isNewMj, bool isFenZhang, bool canHu, bool isXjxd)
    {
        MaJangPage.Instance.lastOutMj = null;
        MaJangPage.Instance.RefreshSurplusCount();

        List<Transform> stackMaJongList = MaJangScene.Instance.stackMaJong;
        if (stackMaJongList.Count > 0)
        {
            int targetIndex = 0;
            if (isGang)
            {
                if (MaJangPage.Instance.gangCount % 2 == 0)
                    targetIndex = stackMaJongList.Count - 2;
                else
                    targetIndex = stackMaJongList.Count - 1;
                isGang = false;
                MaJangPage.Instance.gangCount++;
            }
            Transform maJong = this.transform;
            //如果有要出的牌且没有mj就取手牌最后一个
            if (isCurrentPlayer && mj == null && isNewMj)
            {
                MaJangModel majangmodel = handMjList[handMjList.Count - 1];
                maJong = majangmodel.transform;
                mj = majangmodel.mj;
                UIUtils.SetAllChildrenLayer(maJong, ConstantUtils.mjLayer);
            }
            else
            {
                maJong = stackMaJongList[targetIndex];
                stackMaJongList.RemoveAt(targetIndex);
                MaJangModel mjm = maJong.GetComponent<MaJangModel>();
                if (isCurrentPlayer)
                {
                    mjm.Init(mj, this);
                    UIUtils.SetAllChildrenLayer(maJong, ConstantUtils.mjLayer);
                }
                handMjList.Add(mjm);
                maJong.SetParent(handParent);
                maJong.localEulerAngles = Vector3.zero;
            }
            if (isNewMj)
            {
                SortMj(null,false);
                MaJangScene.Instance.SetOperator(seatNo);
                maJong.localPosition = Vector3.right * (handParent.childCount - MaJangPage.mj_interval) * MaJangScene.mjSize.x;
                if (isXjxd)
                {
                    if (!isCurrentPlayer)
                        maJong.localEulerAngles = Vector3.right * ConstantUtils.const90;
                    yield return new WaitForSecondsRealtime(1);
                    maJong.localEulerAngles = Vector3.zero;
                }

                if (isCurrentPlayer)
                {
                    if (statu == 2)
                    {
                        if (canHu)
                            Hu(true);
                        else
                            if (!isFenZhang)
                                StartCoroutine(DelayOutMj());
                    }
                    else if (statu == 0)
                    {
                        if (!isFenZhang)
                            if (atTrusteeship)
                            {
                                StartCoroutine(DelayOutMj());
                                MaJangPage.Instance.FinishAction();
                            }
                            else
                            {
                                if (HasSame() > 3)
                                    MaJangPage.Instance.EnableActionBtn(MaJangPage.Instance.btnGang, false);
                                isTurn = true;
                            }
                    }
                }
            }
            else
                maJong.localPosition = Vector3.right * (handParent.childCount - 1) * MaJangScene.mjSize.x;
        }
    }

    IEnumerator DelayOutMj()
    {
        yield return new WaitForSecondsRealtime(1);
        CreateAction(MjActionType.OutMj, handMjList[handMjList.Count - 1]);
    }
    IEnumerator ShowMj(string mjNo)
    {
        showMj.transform.Find("ShowMjMask/Icon").GetComponent<Image>().sprite = BundleManager.Instance.GetSprite(mjNo, MaJangPage.Instance.majangBundle);
        showMj.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        showMj.SetActive(false);
    }
    int columnMax = 6;//出牌堆每行最大牌数 2人玩法每行15张牌，起始位置x=-5.8
    public void OutMj(MaJangModel mjm = null, Mj mj = null, bool playAnimation = true)
    {
        if (mjm || mj != null)
        {
            #region 打牌音效
            //point  // 点数 1- 9
            //type  // 类型 0万，1筒，2条 3中
            //0男1女
            if (mjm)
            {
                string soundString = (sex == 0 ? "boy" : "girl") + mjm.mj.type + mjm.mj.point;
                AudioManager.AudioSoundType audioType = GetSoundType(soundString);
                PlaySound(audioType);
            }
            else if (mj != null)
            {
                string soundString = (sex == 0 ? "boy" : "girl") + mj.type + mj.point;
                AudioManager.AudioSoundType audioType = GetSoundType(soundString);
                PlaySound(audioType);
            }
            #endregion
            if (!mjm)
            {
                mjm = handMjList[tingIcon.activeInHierarchy ? handMjList.Count - 1 : Random.Range(0, handMjList.Count)];
                mjm.Init(mj, this);
            }
            int index = handMjList.IndexOf(mjm);
            int row = Mathf.CeilToInt((outParent.childCount + 1f) / columnMax) - 1;
            int column = outParent.childCount % columnMax;
            mjm.transform.SetParent(outParent);
            mjm.transform.localPosition = new Vector3(column * MaJangScene.mjSize.x, 0, -row * MaJangScene.mjSize.y);
            mjm.transform.localEulerAngles = Vector3.right * ConstantUtils.const90;
            StartCoroutine(ShowMj(mjm.mjNo.ToString()));//显示打出的牌
            isOutNewCard = false;
            handMjList.Remove(mjm);
            MaJangPage.Instance.SetLastOutMj(mjm);
            PlaySound(AudioManager.AudioSoundType.mjoutcard);
            if (isCurrentPlayer)
            {
                UIUtils.SetAllChildrenLayer(mjm.transform, ConstantUtils.modelLayer);
                if (statu == 1)
                {
                    for (int i = 0; i < handMjList.Count; i++)
                        handMjList[i].SetStatu(false);
                    statu = 2;
                }
            }
            if (playAnimation && index < handMjList.Count)
                MjMoveAnimation(index);
            isFinishAction = true;
        }
    }

    public void Chi(List<Mj> mjs = null)
    {
        if (mjs == null)
        {
            if (chiMj.Count > 3)//多组吃
                EnableChiPanel(true);
            else
            {
                MaJangModel mjm1 = null, mjm2;
                mjm1 = chiMj.Find((m) => { return m != MaJangPage.Instance.lastOutMj; });
                mjm2 = chiMj.Find((m) => { return m != MaJangPage.Instance.lastOutMj && m != mjm1; });
                CreateAction(MjActionType.Chi, mjm1, mjm2);
                //0男1女
                PlaySound(sex == 0 ? AudioManager.AudioSoundType.boychi : AudioManager.AudioSoundType.girlchi);
            }
        }
        else
        {
            LightMj(GetLightMj(mjs));
            ActionAnimation(MjActionType.Chi);
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boychi : AudioManager.AudioSoundType.girlchi);
        }
    }

    List<MaJangModel> chiTingMj = new List<MaJangModel>();
    public void ChiTing(List<Mj> mjs = null)
    {
        if (mjs == null)
        {
            #region ...吃听处理
            //if (chiMj.Count > 3)
            //{
            //    List<MaJangModel> mjms = new List<MaJangModel>();
            //    for (int i = chiMj.Count - 1; i >= 0; i -= 3)
            //    {
            //        mjms.Clear();
            //        mjms.AddRange(chiMj);
            //        for (int j = i; j > i - 3; j--)
            //            if (!chiMj[j].Equals(MaJangPage.Instance.lastOutMj))
            //                mjms.RemoveAt(j);
            //        MaJangModel m = mjms.Find((mj) =>
            //         {
            //             bool has = false;
            //             foreach (Mj mTemp in tingOutList)
            //                 if (mj.mjNo == mTemp.type * 10 + mTemp.point)
            //                 {
            //                     has = true;
            //                     break;
            //                 }
            //             return has;
            //         });
            //        if (m == null)
            //        {
            //            mjms.RemoveAt(mjms.Count - 1);
            //            chiMj.Clear();
            //            chiMj.AddRange(mjms);
            //        }
            //    }
            //}
            #endregion
            if (chiTingMj.Count > 3)
            {
                statu = 1;
                EnableChiPanel(false);
            }
            else
            {
                MaJangModel mjm1 = null, mjm2;
                mjm1 = chiTingMj.Find((m) => { return m != MaJangPage.Instance.lastOutMj; });
                mjm2 = chiTingMj.Find((m) => { return m != MaJangPage.Instance.lastOutMj && m != mjm1; });
                CreateAction(MjActionType.ChiTing, mjm1, mjm2);
                //0男1女
                PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyting : AudioManager.AudioSoundType.girlting);
            }
        }
        else
        {
            LightMj(GetLightMj(mjs));
            ActionAnimation(MjActionType.ChiTing);
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyting : AudioManager.AudioSoundType.girlting);
        }
    }

    public void Peng(List<Mj> mjs = null)
    {
        if (mjs == null)
        {
            if (sameMj.Count < 1) return;
            CreateAction(MjActionType.Peng, sameMj[0]);
            //0男1女
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boypeng : AudioManager.AudioSoundType.girlpeng);
        }
        else
        {
            LightMj(GetLightMj(mjs));
            ActionAnimation(MjActionType.Peng);
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boypeng : AudioManager.AudioSoundType.girlpeng);
        }
    }

    public void PengTing(List<Mj> mjs = null)
    {
        if (mjs == null)
        {
            if (sameMj.Count < 1) return;
            statu = 1;
            CreateAction(MjActionType.PengTing, sameMj[0]);
            //0男1女
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyting : AudioManager.AudioSoundType.girlting);
        }
        else
        {
            LightMj(GetLightMj(mjs));
            ActionAnimation(MjActionType.PengTing);
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyting : AudioManager.AudioSoundType.girlting);
        }
    }

    public void Gang(List<Mj> mjs = null, MjActionType mat = MjActionType.DGang)
    {
        isGang = true;
        if (mjs == null)
        {
            if (sameMj.Count > 4)
            {
                EnableGangPanel();
            }
            else
            {
                List<MaJangModel> temp = new List<MaJangModel>();
                int targetNum = 0;
                for (int i = 0; i < handMjList.Count; i++)
                    if (handMjList[i].mjNo == sameMj[0].mjNo)
                    {
                        targetNum++;
                        temp.Add(handMjList[i]);
                    }
                if (targetNum == 4)
                {
                    LightMj(temp, true);
                    CreateAction(MjActionType.AGang, temp[0]);
                }
                else if (targetNum == 3)
                    CreateAction(MjActionType.DGang, temp[0]);
                else if (targetNum == 1)
                {
                    LightMj(temp[0]);
                    CreateAction(MjActionType.BGang, sameMj[0]);
                }
                PlaySound(sex == 0 ? AudioManager.AudioSoundType.boygang : AudioManager.AudioSoundType.girlgang);
            }
        }
        else
        {
            if (mat.Equals(MjActionType.BGang))
                LightMj(GetLightMj(mjs)[0]);
            else
                LightMj(GetLightMj(mjs), mat == MjActionType.AGang);
            ActionAnimation(MjActionType.AGang);
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boygang : AudioManager.AudioSoundType.girlgang);
        }
    }

    public void DingTing(List<Mj> mjs = null)
    {
        if (mjs == null)
        {
            statu = 1;
            for (int i = 0; i < handMjList.Count; i++)
                if (handMjList[i].mjNo == MaJangPage.Instance.lastOutMj.mjNo)
                {
                    sameMj.Clear();
                    sameMj.Add(handMjList[i]);
                    sameMj.Add(MaJangPage.Instance.lastOutMj);
                    CreateAction(MjActionType.DingTing, MaJangPage.Instance.lastOutMj);
                    PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyting : AudioManager.AudioSoundType.girlting);
                    break;
                }
        }
        else
        {
            LightMj(GetLightMj(mjs));
            ActionAnimation(MjActionType.DingTing);
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyting : AudioManager.AudioSoundType.girlting);
        }
    }

    public void Ting()
    {
        if (isCurrentPlayer)
        {
            statu = 1;
            MaJangModel mjm;
            MjTing mjTing;
            for (int i = 0; i < handMjList.Count; i++)
            {
                mjm = handMjList[i];
                mjTing = tingOutList.Find((m) => { return mjm.mjNo == m.mj.type * 10 + m.mj.point; });
                mjm.SetStatu(mjTing != null);
                mjTing = null;
            }
            isTurn = true;
            if (atTrusteeship)
                TrusteeshipMethods();
        }
        else
        {
            ActionAnimation(MjActionType.Ting);
        }
        tingIcon.SetActive(true);
        PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyting : AudioManager.AudioSoundType.girlting);
    }

    /// <summary>
    /// 加减分的处理
    /// </summary>
    public void ChanegScore(int score, bool isXjxd = true)
    {
        bool isAdd = score >= 0;
        addScoreText.gameObject.SetActive(isAdd);
        minusScoreText.gameObject.SetActive(!isAdd);
        if (isAdd)
        {
            addScoreText.text = "+" + score;
            UIUtils.DelayDesOrDisObject(addScoreText.gameObject, 3, false);
            if (isXjxd)
                xjxdAnimation.gameObject.SetActive(true);
        }
        else
        {
            minusScoreText.text = score.ToString();
            UIUtils.DelayDesOrDisObject(minusScoreText.gameObject, 3, false);
        }
        currencyNum.text = (int.Parse(currencyNum.text) + score).ToString();
    }

    public void Hu(bool isZiMo = false)
    {
        if (isZiMo)
        {
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyzimo : AudioManager.AudioSoundType.girlzimo);
            PlaySound(AudioManager.AudioSoundType.mjaudiozimo);
        }
        else
        {
            PlaySound(sex == 0 ? AudioManager.AudioSoundType.boyhu : AudioManager.AudioSoundType.girlhu);
            PlaySound(AudioManager.AudioSoundType.mjaudiohu);
            MaJangModel mjmHu = MaJangPage.Instance.lastOutMj;
            mjmHu.transform.SetParent(handParent);
            handMjList.Add(mjmHu);
            mjmHu.transform.localEulerAngles = Vector3.right * 90;
            SortMj(null, false);
            mjmHu.transform.localPosition = Vector3.right * (handParent.childCount - MaJangPage.mj_interval) * MaJangScene.mjSize.x;
        }
        statu = 3;
        MaJangPage.Instance.timer.gameObject.SetActive(false);
        MaJangPage.Instance.RequestTrusteeship(false);
        MaJangPage.Instance.FinishAction();
        Debug.Log(seatNo + "胡了");
    }

    public void Chat(string value, int type)
    {
        talkView.Chat(value, type);
    }

    public void PingHu(bool isZimo)
    {
        if (isZimo)
            zimoIcon.SetActive(true);
        else
            huIcon.SetActive(true);
    }
    //胡牌后执行的动画
    public void huAnimation(int mjrm)
    {
        //bool isActivity = true;
        actionAnimation.isDisappear = false;
        Image image = actionAnimation.GetComponent<Image>();
        Sprite sprite = BundleManager.Instance.GetSprite("majiang_pic_" + mjrm, MaJangPage.Instance.GetSpriteAB());
        image.sprite = sprite;
        PlaySound(GetSoundType("mj" + mjrm));
        if (sprite!=null)
        {
            image.SetNativeSize();
            actionAnimation.gameObject.SetActive(true);
        }
    }

    //行为动画
    void ActionAnimation(MjActionType mat)
    {
        bool isActivity = true;
        actionAnimation.isDisappear = true;
        Image image = actionAnimation.GetComponent<Image>();
        switch (mat)
        {
            case MjActionType.Peng:
            case MjActionType.Chi:
            case MjActionType.Ting:
            case MjActionType.ChiTing:
            case MjActionType.DingTing:
            case MjActionType.PengTing:
                //case MjActionType.Hu:
                //case MjActionType.ZiMo:
                image.sprite = BundleManager.Instance.GetSprite("majiang_pic_" + mat.ToString().ToLower(), MaJangPage.Instance.GetSpriteAB());
                break;
            case MjActionType.DGang:
            case MjActionType.AGang:
            case MjActionType.BGang:
                image.sprite = BundleManager.Instance.GetSprite("majiang_pic_gang", MaJangPage.Instance.GetSpriteAB());
                break;
            default:
                isActivity = false;
                break;
        }
        if (isActivity)
        {
            image.SetNativeSize();
            actionAnimation.gameObject.SetActive(true);
        }
    }
    #endregion

    #region ...工具
    public void SetBanker()
    {
        bankerIcon.SetActive(true);
    }

    List<MaJangModel> chiMj = new List<MaJangModel>();
    public int HasChi(List<Mj> mjs = null)
    {
        MaJangModel lastMjm = MaJangPage.Instance.lastOutMj;
        if (mjs == null)
        {
            chiMj.Clear();
            MaJangModel hasSmall2 = null, hasSmall1 = null, hasLarge1 = null, hasLarge2 = null;
            for (int i = 0; i < handMjList.Count; i++)
            {
                if (handMjList[i].mjNo / 10 == lastMjm.mjNo / 10)
                    switch (handMjList[i].mjNo - lastMjm.mjNo)
                    {
                        case -2:
                            hasSmall2 = handMjList[i];
                            break;
                        case -1:
                            hasSmall1 = handMjList[i];
                            break;
                        case 1:
                            hasLarge1 = handMjList[i];
                            break;
                        case 2:
                            hasLarge2 = handMjList[i];
                            break;
                    }
            }
            if (hasSmall2 && hasSmall1)
            {
                chiMj.Add(hasSmall2);
                chiMj.Add(lastMjm);
                chiMj.Add(hasSmall1);
            }
            if (hasSmall1 && hasLarge1)
            {
                chiMj.Add(hasSmall1);
                chiMj.Add(lastMjm);
                chiMj.Add(hasLarge1);
            }
            if (hasLarge1 && hasLarge2)
            {
                chiMj.Add(hasLarge1);
                chiMj.Add(lastMjm);
                chiMj.Add(hasLarge2);
            }
        }
        else
        {
            chiTingMj.Clear();
            for (int i = 0; i < mjs.Count; i++)
            {
                int mjNo = mjs[i].type * 10 + mjs[i].point;
                if (mjNo == lastMjm.mjNo)
                    chiTingMj.Add(lastMjm);
                else
                    chiTingMj.Add(handMjList.Find((m) => { return m.mjNo == mjNo; }));
            }
        }
        return chiMj.Count;
    }

    List<MaJangModel> sameMj = new List<MaJangModel>();
    public int HasSame(MaJangModel mjm = null)
    {
        sameMj.Clear();
        if (mjm)//检测是否存在点杠、碰
        {
            for (int i = 0; i < handMjList.Count; i++)
                if (handMjList[i].mjNo == mjm.mjNo)
                    sameMj.Add(handMjList[i]);

            if (sameMj.Count > 1 && mjm == MaJangPage.Instance.lastOutMj)
                sameMj.Add(mjm);
            else
                sameMj.Clear();
        }
        else
        {
            List<MaJangModel> mjmTemp = new List<MaJangModel>();
            for (int i = 0; i < handMjList.Count; i++)//检测是否存在暗杠
            {
                MaJangModel mjmi = handMjList[i];
                mjmTemp.Clear();
                if (sameMj.Contains(mjmi)) continue;
                mjmTemp.Add(mjmi);
                for (int j = 0; j < handMjList.Count; j++)
                {
                    MaJangModel mjmj = handMjList[j];
                    if (i == j || sameMj.Contains(mjmj)) continue;
                    if (mjmi.mjNo == mjmj.mjNo)
                        mjmTemp.Add(mjmj);
                }
                if (mjmTemp.Count == 4) sameMj.AddRange(mjmTemp);
            }

            for (int i = 0; i < lightMjList.Count; i++)//检测是否存在巴杠
            {
                if (lightMjList[i][0].mjNo != lightMjList[i][1].mjNo)
                    continue;
                for (int j = 0; j < handMjList.Count; j++)
                    if (lightMjList[i][0].mjNo == handMjList[j].mjNo)
                    {
                        sameMj.AddRange(lightMjList[i]);
                        sameMj.Add(handMjList[j]);
                        break;
                    }
            }
        }
        return sameMj.Count;
    }

    public void SortMj(List<MaJangModel> mjms = null, bool isSort = true)
    {
        bool isHandMj = mjms == null;
        if (isHandMj)
            mjms = handMjList;
        if (isSort)
            mjms.Sort((a, b) =>
            {
                int value = a.mj.type.CompareTo(b.mj.type);
                if (value == 0)
                    value = a.mj.point.CompareTo(b.mj.point);
                return value;
            });

        if (isHandMj)
            for (int i = 0; i < handMjList.Count; i++)
            {
                mjms[i].transform.SetSiblingIndex(i);
                if (i > 13)
                    mjms[i].transform.localPosition = Vector3.right * (i + (i - 13) * MaJangPage.mj_interval) * MaJangScene.mjSize.x;
                else
                    mjms[i].transform.localPosition = Vector3.right * i * MaJangScene.mjSize.x;
            }
    }

    //修改麻将状态，将麻将从指定位置和集合移到目标位置和集合
    //public void SetMjStatu(MaJangModel mjm, List<MaJangModel> parentList, Transform targetParent, List<MaJangModel> targetList, bool isSort = true)
    //{
    //    parentList.Remove(mjm);
    //    mjm.transform.SetParent(targetParent);
    //    targetList.Add(mjm);
    //    SortMj(targetList, true);
    //}

    //设置听后可以打的牌
    public void SetTingMj(List<MjTing> mjs)
    {
        tingOutList.Clear();
        tingOutList.AddRange(mjs);
        print("服务器停牌数据：" + tingOutList.Count);
    }

    List<MaJangModel> GetLightMj(List<Mj> mjs)
    {
        List<MaJangModel> lightMjs = new List<MaJangModel>();
        for (int i = 0; i < mjs.Count; i++)
        {
            Mj mj = mjs[i];
            if (MaJangPage.Instance.lastOutMj && !lightMjs.Contains(MaJangPage.Instance.lastOutMj) && mj.type * 10 + mj.point == MaJangPage.Instance.lastOutMj.mjNo)
                lightMjs.Add(MaJangPage.Instance.lastOutMj);
            else
            {
                handMjList[i].Init(mjs[i], this);
                lightMjs.Add(handMjList[i]);
            }
        }
        if (lightMjs.Count < 4)
        {
            var lastMj = lightMjs[lightMjs.Count - 1];
            lightMjs.RemoveAt(lightMjs.Count - 1);
            lightMjs.Insert(1, lastMj);
        }
        return lightMjs;
    }

    /// <summary>
    /// 亮牌,吃碰杠钉
    /// </summary>
    /// <param name="mjms"></param>
    /// <param name="isCover">是否为暗杠</param>
    public void LightMj(List<MaJangModel> mjms, bool isCover = false, bool isTing = true)
    {
        SortMj(mjms, false);
        MaJangScene.Instance.SetOperator(seatNo);
        float startPoint = lightMjList.Count * 3.5f * MaJangScene.mjSize.x;//3.5 = 3张牌 + 半张牌空隙
        for (int i = 0; i < mjms.Count; i++)
        {
            MaJangModel mjmTemp = mjms[i];
            mjmTemp.transform.SetParent(lightParent);
            if (mjms.Count == 4 && i == 1)
                mjmTemp.transform.localPosition = new Vector3(startPoint, MaJangScene.mjSize.z);
            else
            {
                mjmTemp.transform.localPosition = Vector3.right * startPoint;
                startPoint += MaJangScene.mjSize.x;
            }
            if (isCover)
                mjmTemp.transform.localEulerAngles = Vector3.left * ConstantUtils.const90;
            else
                mjmTemp.transform.localEulerAngles = Vector3.right * ConstantUtils.const90;
            UIUtils.SetAllChildrenLayer(mjmTemp.transform, ConstantUtils.modelLayer);
            handMjList.Remove(mjmTemp);
        }
        handParent.localPosition += Vector3.right * 2 * MaJangScene.mjSize.x;
        lightMjList.Add(mjms.ToArray());
        SortMj();
        if (isOutNewCard)
            handMjList[handMjList.Count - 1].transform.localPosition = Vector3.right * (handParent.childCount - MaJangPage.mj_interval) * MaJangScene.mjSize.x;
        if (lightMjList.Count > 1)
            LightAnGang();
        if (!isCover)
            MaJangPage.Instance.SetLastOutMj();
        if (isTing)
        {
            if (atTrusteeship && isCurrentPlayer)
                TrusteeshipMethods();
        }
    }

    //巴杠专用
    void LightMj(MaJangModel mjm)
    {
        if (isCurrentPlayer)
        {
            UIUtils.SetAllChildrenLayer(mjm.transform, ConstantUtils.modelLayer);
        }
        MaJangScene.Instance.SetOperator(seatNo);
        isFinishAction = true;
        LightAnGang();
        for (int i = 0; i < lightMjList.Count; i++)
            if (lightMjList[i][0].mjNo == lightMjList[i][1].mjNo && lightMjList[i][0].mjNo == mjm.mjNo)
            {
                mjm.transform.SetParent(lightParent);
                mjm.transform.localPosition = new Vector3(lightMjList[i][1].transform.localPosition.x, MaJangScene.mjSize.z);
                mjm.transform.localEulerAngles = Vector3.right * ConstantUtils.const90;
                handMjList.Remove(mjm);
                SortMj();
                List<MaJangModel> tempMjm = new List<MaJangModel>();
                tempMjm.AddRange(lightMjList[i]);
                tempMjm.Insert(1, mjm);
                lightMjList[i] = tempMjm.ToArray();
                break;
            }
    }

    //麻将移动动画
    void MjMoveAnimation(int vacancyIndex)
    {
        int targetIndex = 0;
        MaJangModel lastMj = handMjList[handMjList.Count - 1];
        if (isCurrentPlayer)
        {
            for (int i = 0; i < handMjList.Count - 1; i++)
                if (lastMj.mj.type < handMjList[i].mj.type || (lastMj.mj.type == handMjList[i].mj.type && lastMj.mj.point < handMjList[i].mj.point))
                {
                    targetIndex = i;
                    break;
                }
                else
                    if (i == handMjList.Count - 2)
                        targetIndex = handMjList.Count - 1;
        }
        else
            targetIndex = Random.Range(0, handMjList.Count - 1);

        bool toLeft = targetIndex >= vacancyIndex;
        lastMj.Move(Vector3.right * targetIndex * MaJangScene.mjSize.x);
        for (int i = Mathf.Min(vacancyIndex, targetIndex); i < Mathf.Max(vacancyIndex, targetIndex); i++)
            handMjList[i].Move(toLeft);
        handMjList.Remove(lastMj);
        handMjList.Insert(targetIndex, lastMj);
        if (isCurrentPlayer && targetIndex != handMjList.Count - 1)
            lastMj.transform.SetSiblingIndex(targetIndex);
    }
    #endregion

    #region ...特殊行为
    void EnableChiPanel(bool isChi)
    {
        MaJangPage.Instance.EnableActionBtn(MaJangPage.Instance.btnCancel);
        Transform chiSelectPanel = MaJangPage.Instance.chiSelectPanel.transform;
        chiSelectPanel.gameObject.SetActive(true);
        foreach (Transform tf in chiSelectPanel)
            tf.gameObject.SetActive(false);

        if (atTrusteeship)
        {
            chiTemp = isChi ? chiMj : chiTingMj;
            var lmjm = chiMj.GetRange(0, 3);
            for (int i = 0; i < chiTemp.Count; i += 3)
            {
                if (chiTemp[i].mjNo == lmjm[0].mjNo)
                {
                    chiTemp.Clear();
                    chiTemp.Add(lmjm[0]);
                    chiTemp.Add(lmjm[1]);
                    chiTemp.Add(lmjm[2]);
                    if (isChi) Chi();
                    else ChiTing();
                    break;
                }
            }
        }
        else
        {
            if (isChi)
                for (int i = 0; i < chiMj.Count; i += 3)
                    SetChiOrGangImage(chiSelectPanel, chiSelectPanel.GetChild(i / 3), chiMj.GetRange(i, 3));
            else
                for (int i = 0; i < chiTingMj.Count; i += 3)
                    SetChiOrGangImage(chiSelectPanel, chiSelectPanel.GetChild(i / 3), chiTingMj.GetRange(i, 3), false);
        }
    }

    void EnableGangPanel()
    {
        MaJangPage.Instance.EnableActionBtn(MaJangPage.Instance.btnCancel);
        Transform gangSelectPanel = MaJangPage.Instance.gangSelectPanel.transform;
        gangSelectPanel.gameObject.SetActive(true);
        foreach (Transform tf in gangSelectPanel)
            tf.gameObject.SetActive(false);

        for (int i = 0; i < sameMj.Count; i += 4)
            SetChiOrGangImage(gangSelectPanel, gangSelectPanel.GetChild(i / 4), sameMj.GetRange(i, 4));
    }

    List<MaJangModel> chiTemp;
    void SetChiOrGangImage(Transform panel, Transform item, List<MaJangModel> lmjm, bool isChi = true)
    {
        chiTemp = isChi ? chiMj : chiTingMj;
        for (int i = 0; i < item.childCount - 1; i++)
        {
            Transform mjTf = item.GetChild(i).GetChild(0);
            mjTf.GetComponent<Image>().sprite = BundleManager.Instance.GetSprite(lmjm[i].mjNo.ToString(), MaJangPage.Instance.majangBundle);
            mjTf.name = lmjm[i].mjNo.ToString();
        }
        item.gameObject.SetActive(true);

        UGUIEventListener.Get(item.gameObject).onClick = delegate(GameObject go)
        {
            if (item.GetChild(0).GetChild(0).name == item.GetChild(1).GetChild(0).name)
            {
                sameMj.Clear();
                sameMj.Add(lmjm[0]);
                sameMj.Add(lmjm[1]);
                sameMj.Add(lmjm[2]);
                sameMj.Add(lmjm[3]);
                Gang();
            }
            else
            {
                for (int i = 0; i < chiTemp.Count; i += 3)
                {
                    if (chiTemp[i].mjNo == lmjm[0].mjNo)
                    {
                        chiTemp.Clear();
                        chiTemp.Add(lmjm[0]);
                        chiTemp.Add(lmjm[1]);
                        chiTemp.Add(lmjm[2]);
                        if (isChi) Chi();
                        else ChiTing();
                        break;
                    }
                }
            }
            panel.gameObject.SetActive(false);
        };
    }

    /// <summary>
    /// 翻开所有暗杠
    /// </summary>
    public void LightAnGang()
    {
        for (int i = 0; i < lightMjList.Count; i++)
            if (lightMjList[i].Length == 4)
                lightMjList[i][1].transform.localEulerAngles = Vector3.right * ConstantUtils.const90;
    }
    #endregion
}
