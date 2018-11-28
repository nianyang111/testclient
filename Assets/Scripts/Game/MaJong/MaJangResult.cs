using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MaJangResult : MonoBehaviour
{
    public Toggle myResultBtn, gameResultBtn, othersResultBtn;
    public GameObject myResultPanel, gameResultPanel, othersResultPanel;
    public GameObject isZimo;
    public Image titleWord, TitleIcon, type;
    public Text score;

    #region my
    public MaJangResultOthersItem myResultItem;
    public Text gamePoints, multipleNum;
    public GameObject noWin;
    public MaJangResultMj baopai;
    #endregion
    #region game
    public MaJangResultOthersItem gameResultItem;
    public Text resultInfo, resultScore;
    public MaJangResultMj gameResultMj;
    public Text gameResultMultip;
    #endregion
    #region other
    public Transform othersComtent;
    public MaJangResultOthersItem otherItemPrefab;
    #endregion

    List<MaJangResultOthersItem> itemList = new List<MaJangResultOthersItem>();
    public void Init()
    {
        myResultBtn.onValueChanged.AddListener(isOn => ChangePanel());
        gameResultBtn.onValueChanged.AddListener(isOn => ChangePanel());
        othersResultBtn.onValueChanged.AddListener(isOn => ChangePanel());
        ChangePanel();
    }
    public void OpenPanel()
    {
        gameObject.SetActive(true);
        othersResultBtn.isOn = true;
        ChangePanel();
    }

    private void ChangePanel()
    {
        myResultPanel.gameObject.SetActive(myResultBtn.isOn);
        gameResultPanel.gameObject.SetActive(gameResultBtn.isOn);
        othersResultPanel.gameObject.SetActive(othersResultBtn.isOn);
    }
    public void AddResultInfo(MaJangPlayer player, net_protocol.Mj xiaoji)
    {
        var otherItem = Instantiate(otherItemPrefab, othersComtent);
        otherItem.xiaoji = xiaoji;
        otherItem.Init(player);
        itemList.Add(otherItem);
        if (player.Equals(MaJangPage.Instance.currentPlayer))
        {
            myResultItem.Init(player);
            bool isWin = player.win > 0;
            titleWord.sprite = BundleManager.Instance.GetSprite("common/" + (isWin ? "normal_word_win" : "normal_word_lose"));
            TitleIcon.sprite = BundleManager.Instance.GetSprite("common/" + (isWin ? "normal_icon_win" : "normal_icon_lose"));
            type.sprite = player.currencyIcon.sprite;
            score.text = player.win.ToString();
        }
    }

    public void winOrLose(List<net_protocol.MjResultRate> rate)
    {
        var mrr1 = rate[0];
        isZimo.SetActive(false);
        #region 胡牌数据
        var currentPlayer = MaJangPage.Instance.GetPlayerFromSeatNo(mrr1.winPos);
        if (mrr1.type == 2)
        {
            var losePlayer = MaJangPage.Instance.GetPlayerFromSeatNo(mrr1.pos);
            losePlayer.huAnimation(10 + mrr1.heiPao);//1标识黑炮 其他0
            MaJangPage.Instance.mje.PlayEffect(10 + mrr1.heiPao);
        }
        var bankPlayer = itemList.Find(p => p.mPlayer == currentPlayer);
        print("胡牌类型：" + mrr1.type + "----" + mrr1.winPos + "麻将结算：服务器：" + currentPlayer.userId + "--玩家自己:" + MaJangPage.Instance.currentPlayer.userId + "__是否是玩家自己：" + currentPlayer.Equals(MaJangPage.Instance.currentPlayer));

        StringBuilder sb = new StringBuilder();
        char comma = ',';
        if (mrr1.type == 1)
            sb.Append("自摸");
        else if (mrr1.type == 2)
        {
            if (mrr1.heiPao == 1)
                sb.Append("放黑");
            else
                sb.Append("放炮");
        }
        sb.Append("(");
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.平胡, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.天胡, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.地胡, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.风宝, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.小鸡下蛋, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.大世界, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.七小对, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.清一色, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.摸宝, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.大风, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.豪, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.红中满天飞, sb)) sb.Append(comma);
        if (AddTargetType(mrr1.rateMask, MaJangRateMask.直播, sb)) sb.Append(comma);
        sb.Remove(sb.Length - 1, 1);
        sb.Append(")");
        bankPlayer.SetRateAndMultip(sb.ToString(), mrr1.rate.ToString());

        if (currentPlayer.Equals(MaJangPage.Instance.currentPlayer))
        {
            isZimo.SetActive(mrr1.type == 1);
            gamePoints.gameObject.SetActive(true);
            noWin.gameObject.SetActive(false);
            gamePoints.text = sb.ToString();
            multipleNum.text = mrr1.rate.ToString();
        }
        else
        {
            gamePoints.gameObject.SetActive(false);
            noWin.gameObject.SetActive(true);
        }
        resultScore.gameObject.SetActive(false);
        resultInfo.gameObject.SetActive(false);
        gameResultMj.transform.parent.gameObject.SetActive(false);
        gameResultMultip.gameObject.SetActive(false);
        #endregion
        if (rate.Count > 1)
        {
            var mrr2 = rate[1];
            var xiaojiItem = itemList.Find(p => p.xiaoji != null);
            if (xiaojiItem)
            {
                gameResultItem.Init(xiaojiItem.mPlayer);
                resultScore.gameObject.SetActive(true);
                resultInfo.gameObject.SetActive(true);
                gameResultMj.transform.parent.gameObject.SetActive(true);
                gameResultMultip.gameObject.SetActive(true);
                resultScore.text = mrr2.win > 0 ? '+' + mrr2.win.ToString() : mrr2.win.ToString();
                if (MaJangPage.Instance.playerCount == 2)
                    mrr2.pos *= 2;
                resultInfo.text = GetOrigin(MaJangPage.Instance.currentPlayer, mrr2.pos) + "小鸡下蛋";
                gameResultMj.Init((xiaojiItem.xiaoji.type * 10 + xiaojiItem.xiaoji.point).ToString());
                gameResultMultip.text = mrr2.rate + "倍";
            }
        }
    }
    public string GetOrigin(MaJangPlayer mjPlayer, int targetTableNo)
    {
        int selfNo = mjPlayer.seatNo;
        switch (targetTableNo - selfNo)
        {
            case -1:
            case 3:
                return "上家";
            case 0:
                return "本家";
            case 1:
            case -3:
                return "下家";
            case -2:
            case 2:
                return "对家";
        }
        return null;
    }
    public bool AddTargetType(int targetNum, MaJangRateMask mjrm, StringBuilder sb)
    {
        if ((targetNum & (int)mjrm) > 0)
        {
            sb.Append(mjrm.ToString());
            return true;
        }
        return false;
    }


    public void DelResultInfo()
    {
        baopai.transform.parent.gameObject.SetActive(false);
        myResultItem.DelInfo();
        gameResultItem.DelInfo();
        UIUtils.DestroyChildren(othersComtent);
        itemList.Clear();
    }
}
