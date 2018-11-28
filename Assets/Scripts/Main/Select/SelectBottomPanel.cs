using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBottomPanel : MonoBehaviour {
    public Image playIcon;
    public Text goldText, agText, nickName;
    public GameObject goldBtn, agBtn, vipIcon;
    public GameObject mailBtn, friendBtn;
    public Button safeBoxBtn, bagBtn, appointBtn, matchBtn, quickGameBtn;
    public GameObject friendRedPoint ,msgTip;
    SelectRoomPage _page;
    public void Init(SelectRoomPage page)
    {
        _page = page;

        UGUIEventListener.Get(goldBtn).onClick = delegate{ NodeManager.OpenNode<StoreNode>().goldBtn.isOn = true; };
        UGUIEventListener.Get(agBtn).onClick = delegate { NodeManager.OpenNode<StoreNode>().agBtn.isOn = true; };

        UGUIEventListener.Get(mailBtn.gameObject).onClick = delegate { NodeManager.OpenNode<MessageNode>(); };
        UGUIEventListener.Get(friendBtn.gameObject).onClick = delegate { NodeManager.OpenNode<SocialNode>(); };
        UGUIEventListener.Get(playIcon.gameObject).onClick = delegate { NodeManager.OpenNode<UserInfoNode>(); };

        UGUIEventListener.Get(safeBoxBtn.gameObject).onClick = delegate { NodeManager.OpenNode<SafeBoxNode>(); };
        UGUIEventListener.Get(bagBtn.gameObject).onClick = delegate { NodeManager.OpenNode<BagNode>(); };
        UGUIEventListener.Get(appointBtn.gameObject).onClick = delegate { NodeManager.OpenNode<JoinGameRoonNode>(); };
        UGUIEventListener.Get(matchBtn.gameObject).onClick = delegate { PageManager.Instance.OpenPage<MatchPage>(); };
        UGUIEventListener.Get(quickGameBtn.gameObject).onClick = delegate { QuickGame(); };

        MessageModel.Instance.msgAct = MsgTip;
    }
    public void MsgTip(bool isShow)
    {
        msgTip.SetActive(isShow);
    }
    public void FinishData()
    {
        goldText.text = UserInfoModel.userInfo.walletGoldBarNum.ToString();
        agText.text = UserInfoModel.userInfo.walletAgNum.ToString();
        nickName.text = UserInfoModel.userInfo.nickName;
        vipIcon.SetActive(UserInfoModel.userInfo.vipCard > 0);
        playIcon.sprite = UserInfoModel.userInfo.headIconSprite;
    }

    public void SetFriendRedPoint()
    {
        friendRedPoint.SetActive(SocialModel.Instance.isHaveNewMessage);
    }

    private void QuickGame()
    {
        _page.QuickGame();
    }

    public void Close()
    {
        MessageModel.Instance.msgAct = null;
    }
}
