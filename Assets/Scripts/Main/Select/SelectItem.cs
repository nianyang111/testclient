using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour
{
    public Image itemIcon, itemName;
    public GameObject itemBg, preventCheat;
    public Text anteText, peopleText;
    SelectData _data;
    public bool isPlay = false;
    public void Init(SelectData data)
    {
        FlushData(data);
        UGUIEventListener.Get(itemBg).onClick = (g) => { EnterGameRoom(); };
    }
    public SelectData GetData()
    {
        return _data;
    }
    public void FlushData(SelectData data)
    {
        _data = data;
        if (_data == null) return;
        itemIcon.sprite = BundleManager.Instance.GetSprite("Select/" + _data.gameIcon);
        itemName.sprite = BundleManager.Instance.GetSprite("Select/" + _data.wordIcon);
        preventCheat.SetActive(_data.preventCheat != "0");
        anteText.text = string.Format(_data.ante + " 底注");
        if (_data.roomData != null)
        {
            peopleText.text = _data.roomData.onlines.ToString();
        }
        if (_data.roomTypeId == 1)//1银币 2金条
            isPlay = _data.joinLimit <= UserInfoModel.userInfo.walletAgNum;
        else if (_data.roomTypeId == 2)
            isPlay = _data.joinLimit <= UserInfoModel.userInfo.walletGoldBarNum;
    }
    public void EnterGameRoom()
    {
        if (_data == null||_data.roomData==null)
        {
            TipManager.Instance.OpenTip(TipType.ChooseTip, "正在请求数据，请稍后再试");
            return;
        }
        if (_data.roomTypeId == 1)//1银币 2金条
        {
            if (_data.joinLimit > UserInfoModel.userInfo.walletAgNum)
            {
                TipManager.Instance.OpenTip(TipType.ChooseTip, "银币不足不能进入该房间");
                return;
            }
        }
        else if (_data.roomTypeId == 2)
        {
            if (_data.joinLimit > UserInfoModel.userInfo.walletGoldBarNum)
            {
                TipManager.Instance.OpenTip(TipType.ChooseTip, "金条不足不能进入该房间");
                return;
            }
        }
        if (_data.gameTypeId == 1)//1 斗地主 2麻将
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_EnterDdzRoomReq,
                enterDdzRoomReq = new EnterDdzRoomReq()
                {
                    roomId = _data.roomData.roomId,
                }
            },true);
        }
        else if (_data.gameTypeId == 2)
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_EnterMjRoomReq,
                enterMjRoomReq = new EnterMjRoomReq()
                {
                    roomId=_data.roomData.roomId,
                }
            },true);
        }
    }
}
public class SelectData
{
    /// <summary> 底注</summary>
    public int ante;
    /// <summary> 等级图标</summary>
    public string gameIcon;
    /// <summary> 游戏类型</summary>
    public string gameType;
    /// <summary> 游戏类型Id</summary>
    public int gameTypeId;
    /// <summary> id</summary>
    public int id;
    /// <summary> 开始游戏最低金额</summary>
    public int joinLimit;
    /// <summary> 防作弊</summary>
    public string preventCheat;
    /// <summary> 房间等级</summary>
    public string roomGradeType;
    /// <summary> 房间等级Id</summary>
    public int roomGradeTypeId;
    /// <summary> 麻将人数</summary>
    public int roomPeople;
    /// <summary> 游戏币场</summary>
    public string roomType;
    /// <summary> 1银币 2金条</summary>
    public int roomTypeId;
    /// <summary> </summary>
    public int tax;
    /// <summary> 字体图标</summary>
    public string wordIcon;

    public net_protocol.RoomResult roomData;
}

