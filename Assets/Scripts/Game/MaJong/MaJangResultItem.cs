using net_protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MaJangResultItem : MonoBehaviour
{
    public Image headIcon, typeIcon, bankerIcon;
    public Text nickName, result;
    public Transform infoPanel;
    GameObject item;

    void Awake()
    {
        if (infoPanel)
            item = infoPanel.GetChild(0).gameObject;
    }

    public void Init()
    {
        bankerIcon.gameObject.SetActive(false);
        if (infoPanel && infoPanel.childCount > 1)
            Destroy(infoPanel.GetChild(1).gameObject);
    }

    public void RefreshData(MaJangPlayer mjPlayer, List<MjResultRate> mrrs = null)
    {
        return;
        bool hasPlayer = mjPlayer.userId != 0;
        gameObject.SetActive(hasPlayer);
        if (hasPlayer)
        {
            Init();
            headIcon.sprite = mjPlayer.headIcon.sprite;
            typeIcon.sprite = mjPlayer.currencyIcon.sprite;
            nickName.text = mjPlayer.nickName.text;
            if (infoPanel && mrrs != null)
            {
                MjResultRate mrr1 = mrrs[0];
                {
                    //StringBuilder sb = new StringBuilder();
                    //char comma = ',';
                    //if (mrr1.type == 1)
                    //    sb.Append("自摸");
                    //else if (mrr1.type == 2)
                    //    sb.Append("点炮");
                    //sb.Append("(");
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.天胡, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.地胡, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.风宝, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.小鸡下蛋, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.平胡, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.大世界, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.七小对, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.清一色, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.摸宝, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.大风, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.豪, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.红中满天飞, sb)) sb.Append(comma);
                    //if (AddTargetType(mrr1.rateMask, MaJangRateMask.直播, sb)) sb.Append(comma);
                    //sb.Remove(sb.Length - 1, 1);
                    //sb.Append(")");
                    //item.transform.Find("TypeName").GetComponent<Text>().text = sb.ToString();
                    //item.transform.Find("Multiple").GetComponent<Text>().text = mrr1.rate + "倍";
                    //item.transform.Find("Type").GetComponent<Image>().sprite = typeIcon.sprite;
                    
                    //item.transform.Find("Score").GetComponent<Text>().text = mrr1.win > 0 ? '+' + mrr1.win.ToString() : mrr1.win.ToString();
                    //item.transform.Find("Target").GetComponent<Text>().text = GetOrigin(mjPlayer, mrr1.pos);
                }
                if (mrrs.Count > 1)
                {
                    //MjResultRate mrr2 = mrrs[1];
                    //GameObject item2 = Instantiate(item, item.transform.parent);
                    //StringBuilder sb = new StringBuilder();
                    //AddTargetType(mrr2.rateMask, MaJangRateMask.小鸡下蛋, sb);
                    //item2.transform.Find("TypeName").GetComponent<Text>().text = sb.ToString();
                    //item2.transform.Find("Multiple").GetComponent<Text>().text = mrr2.rate + "倍";
                    //item2.transform.Find("Type").GetComponent<Image>().sprite = typeIcon.sprite;
                    //item2.transform.Find("Score").GetComponent<Text>().text = mrr2.win > 0 ? '+' + mrr2.win.ToString() : mrr2.win.ToString();
                    //if (MaJangPage.Instance.playerCount == 2)
                    //    mrr2.pos *= 2;
                    //item2.transform.Find("Target").GetComponent<Text>().text = GetOrigin(mjPlayer, mrr2.pos);
                }
            }
        }
    }

    //public bool AddTargetType(int targetNum, MaJangRateMask mjrm, StringBuilder sb)
    //{
    //    if ((targetNum & (int)mjrm) > 0)
    //    {
    //        sb.Append(mjrm.ToString());
    //        return true;
    //    }
    //    return false;
    //}

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
}


public enum MaJangRateMask
{
    平胡 = 0x001,//1
    天胡 = 0x002,//2
    地胡 = 0x004,//4
    风宝 = 0x008,//8
    小鸡下蛋 = 0x010,//16
    大世界 = 0x020,
    七小对 = 0x040,
    清一色 = 0x080,
    摸宝 = 0x100,
    大风 = 0x200,
    豪 = 0x400,
    红中满天飞 = 0x800,
    直播 = 0x1000,
}