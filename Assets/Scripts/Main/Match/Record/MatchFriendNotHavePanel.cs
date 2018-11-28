using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFriendNotHavePanel : MonoBehaviour {
    public UnityEngine.UI.Text passwdText;
    public GameObject inviteBtn;

    public void Init(string  pas)
    {
        passwdText.text = pas;
        UGUIEventListener.Get(inviteBtn).onClick = delegate { OnInvite(); };
    }

    private void OnInvite()
    {
        SDKManager.Instance.ShareText(SDKManager.WechatShareScene.WXSceneSession, passwdText.text + "   " + UserInfoModel.userInfo.downUrl);
    }
}
