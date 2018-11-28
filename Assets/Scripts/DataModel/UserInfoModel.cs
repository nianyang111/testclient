using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoModel
{
    private static UserInfoModel _userInfo;
    public static UserInfoModel userInfo
    {
        set { _userInfo = value; }
        get
        {
            if (_userInfo == null)
                _userInfo = new UserInfoModel();
            return _userInfo;
        }
    }

    public int userId;//用户id 
    public string nickName;//昵称
    public string headIcon; //头像地址 
    public Sprite headIconSprite;
    public int sex = 0;//性别 0男1女
    public string province;//省市

    public long walletGoldBarNum;//背包金条数    
    public long bankGoldBarNum;//银行金条数 
    public long walletAgNum;//背包银币数
    public long bankAgNum;//银行银币数 
    public int roomCardNum;//房卡

    public int vipCard = 0;//vip卡  0没有1周卡2月卡 
    public string vipDay ;//vip剩余几天
    public int level;//等级 
    public long exp;//经验
    public bool inDzz;
    public long masterScore;//大师分
    public int masterLevel;//大师等级

    public string downUrl = "https://www.baidu.com/";
    public string phoneNum = "10086";
    public string release = "1234564482";
    public void InitUserData(UserFlushData data)
    {
        userId = data.id;
        nickName = data.nickName;
        walletGoldBarNum = data.gold;
        headIcon = data.icon;
        sex = data.gender;
        province = data.province;
        walletGoldBarNum = data.gold;
        walletAgNum = data.sliver;
        roomCardNum = data.roomCard;
        masterLevel = data.masterLevel;
        masterScore = data.masterScore;
        bankGoldBarNum = data.safebox_gold;
        bankAgNum = data.safebox_sliver;

        vipCard = data.vip;
        level = data.level;
        exp = data.exp;

        //if (!string.IsNullOrEmpty(data.tableId))
        //{
        //    SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        //    {
        //        msgid = MessageId.C2G_UserJoinTable,
        //        UserJoinTable = new UserJoinTable() { tableId = data.tableId }
        //    });
        //    MaJangPage.isReConnect = true;
        //}

        UserInfoNode.SetCity(province);
        if (NodeManager.GetNode<StoreNode>())
            NodeManager.GetNode<StoreNode>().FlushData();
        if (PageManager.Instance.GetPage<MainPage>())
            PageManager.Instance.GetPage<MainPage>().InitUserInfo();
        if (PageManager.Instance.GetPage<SelectRoomPage>())
            PageManager.Instance.GetPage<SelectRoomPage>().FinishData();
        if (PageManager.Instance.GetPage<MatchPage>())
            PageManager.Instance.GetPage<MatchPage>().FinishData();
        if (PageManager.Instance.GetPage<MaJangPage>())
            PageManager.Instance.GetPage<MaJangPage>().FinishData();
        if (PageManager.Instance.GetPage<LandlordsPage>())
            PageManager.Instance.GetPage<LandlordsPage>().FinishData();
    }

    /// <summary>
    /// 得到等级配置
    /// </summary>
    /// <param name="curLevel"></param>
    /// <returns></returns>
    public static LitJson.JsonData GetLvJsonData(int Level)
    {
        LitJson.JsonData json = LitJson.JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.levelConfigPath));
        LitJson.JsonData lvJson = json[Level.ToString()];
        return lvJson;
    }
}
[System.Serializable]
public class BasePlayerInfo
{
    public string uid;
    public Six six;
    public string icon;
    public int vip;
    public string userNickname;
    public bool isFriend;
    public long money;
    public long score;//房卡房积分
    public int lv;
    public long exp;
    public int win;
    public int lose;
    public float ratio;
    public string pos;
    public int relation;
}

