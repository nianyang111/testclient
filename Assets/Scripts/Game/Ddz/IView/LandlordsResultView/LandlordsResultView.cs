using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LandlordsResultView : MonoBehaviour {

    public FangkaResultPanel fangkaResult;
    public YouxibiResultPanel youxibiResult;
    public Image resultIcon;

    public void Init()
    {
        fangkaResult.gameObject.SetActive(false);
        youxibiResult.gameObject.SetActive(false);
        resultIcon.gameObject.SetActive(false);
    }

    public void OpenUI(RoomType type)
    {
        if (!LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
        {
            switch (type)
            {
                case RoomType.RoomCard:
                    fangkaResult.Init(false);
                    break;
                case RoomType.GoldBar:
                    youxibiResult.Init();
                    break;
                case RoomType.SilverCoin:
                    youxibiResult.Init();
                    break;
                default:
                    break;
            }
        }
    }

    public void GameOver()
    {
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
        {
            MatchResult();        
            return;
        }

        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
        {
            if (LandlordsModel.Instance.ResultModel.curJs != LandlordsModel.Instance.ResultModel.allJs)
            {
                OpenUI(RoomType.RoomCard);
            }
            else
            {
                NodeManager.OpenNode<CardResultShowNode>().Inits(null);
                Interaction.Instance.HideAllBtn();                
            }
        }
        else
        {
            resultIcon.gameObject.SetActive(true);

            LandkirdsHandCardModel myInfo = LandlordsModel.Instance.MyInfo;
            bool isWin = LandlordsModel.Instance.CurWinerIds.Contains(UserInfoModel.userInfo.userId);
            string iconRes = string.Format("{0}_{1}", myInfo.AccessIdentity, isWin ? "win" : "lose");
            resultIcon.sprite = BundleManager.Instance.GetSprite(iconRes, LandlordsPage.Instance.GetSpriteAB());
            resultIcon.SetNativeSize();

            GameObject winEffet = resultIcon.transform.Find("DouDiZhu_Win").gameObject;
            GameObject loseEffet = resultIcon.transform.Find("DouDiZhu_Fail").gameObject;
            if (isWin)
            {
                winEffet.SetActive(true);
                loseEffet.SetActive(false);
            }
            else
            {
                winEffet.SetActive(false);
                loseEffet.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 比赛的结算处理
    /// </summary>
    void MatchResult()
    {
        for (int i = 0; i < LandlordsModel.Instance.RoomPlayerHands.Count; i++)
        {//此处注意，是否移除还是设为null
            if (LandlordsModel.Instance.RoomPlayerHands[i].playerInfo.uid == UserInfoModel.userInfo.userId.ToString())
                continue;
            LandlordsNet.G2C_LeaveRoomResp(LandlordsModel.Instance.RoomPlayerHands[i].playerInfo.uid, 0);            
        }
        Debug.LogWarning("结算移除玩家");
    }

    public void ClearUI()
    {
        fangkaResult.gameObject.SetActive(false);
        youxibiResult.gameObject.SetActive(false);
        resultIcon.gameObject.SetActive(false);
    }
}
