using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchBottomPanel : MonoBehaviour {
    public GameObject vip,goldBtn, agBtn, regisBtn, recordBtn, bagBtn;
    public Text nickName, goldText, agText;
    public Image playIcon;
    MatchPage _page;
    public void Init(MatchPage page)
    {
        _page = page;
        UGUIEventListener.Get(goldBtn).onClick = delegate { NodeManager.OpenNode<StoreNode>().goldBtn.isOn = true; };
        UGUIEventListener.Get(agBtn).onClick = delegate { NodeManager.OpenNode<StoreNode>().agBtn.isOn = true; };
        UGUIEventListener.Get(recordBtn).onClick = delegate { NodeManager.OpenNode<MatchRecordNode>("match"); };
        UGUIEventListener.Get(bagBtn).onClick = delegate { NodeManager.OpenNode<BagNode>(); };
        UGUIEventListener.Get(regisBtn.gameObject).onClick = delegate { OnClickRegis(); };
        UGUIEventListener.Get(playIcon.gameObject).onClick = delegate { NodeManager.OpenNode<UserInfoNode>(); };
    }

    private void OnClickRegis()
    {
        _page.OnClickRegis();
    }
    public void FinishData()
    {
        goldText.text = UserInfoModel.userInfo.walletGoldBarNum.ToString();
        agText.text = UserInfoModel.userInfo.walletAgNum.ToString();
        nickName.text = UserInfoModel.userInfo.nickName;
        vip.SetActive(UserInfoModel.userInfo.vipCard > 0);
        playIcon.sprite = UserInfoModel.userInfo.headIconSprite;
    }
}
