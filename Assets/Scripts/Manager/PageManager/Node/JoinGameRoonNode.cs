using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using net_protocol;
public class JoinGameRoonNode : Node
{
    [SerializeField]
    private List<Text> numberText = new List<Text>();

    [SerializeField]
    private List<GameObject> numberBtn = new List<GameObject>();

    [SerializeField]
    private Button reInputBtn;

    [SerializeField]
    private Button deleteBtn;

    [SerializeField]
    private RectTransform errorObj;

    [SerializeField]
    private GameObject errorMask;

    [SerializeField]
    private GameObject openBtn;

    private List<int> inputNumber = new List<int>();

    private int inputIndex = 0;

    public GameObject downBtn;
    public GameObject downObj;

    public GameObject logBtn;

    public Image downCode;
    public override void Init()
    {
        base.Init();
        ClearInput();

        UGUIEventListener.Get(reInputBtn.gameObject).onClick = reInputBtnClickHandler;
        UGUIEventListener.Get(deleteBtn.gameObject).onClick = deleteBtnClickHandler;
        UGUIEventListener.Get(openBtn).onClick = delegate { OpenRoom(); };
        UGUIEventListener.Get(downBtn).onClick = delegate { OpenDown(); };
        UGUIEventListener.Get(logBtn).onClick = delegate { OpenLog(); };
        //UGUIEventListener.Get(errorMask.gameObject).onClick = errorMaskClickHandler;
        for (int i = 0; i < numberBtn.Count; i++)
        {
            UGUIEventListener.Get(numberBtn[i].gameObject).onClick = numberBtnClickHandler;
        }
        downCode.sprite = MiscUtils.TextureToSprite(QRCode.GetQRTexture(QRCode.GetQRString(3, UserInfoModel.userInfo.downUrl)));
    }

    /// <summary>
    /// 重新输入
    /// </summary>
    /// <param name="obj"></param>
    private void reInputBtnClickHandler(GameObject obj)
    {
        ClearInput();
    }

    /// <summary>
    /// 删除按钮
    /// </summary>
    /// <param name="obj"></param>
    private void deleteBtnClickHandler(GameObject obj)
    {
        if (inputIndex <= 6 && inputIndex > 0)
        {
            inputNumber.RemoveAt(inputIndex-1);
            numberText[inputIndex-1].text = string.Empty;
            inputIndex--;
        }
    }

    /// <summary>
    /// 数字按钮
    /// </summary>
    /// <param name="obj"></param>
    private void numberBtnClickHandler(GameObject obj)
    {
        errorObj.gameObject.SetActive(false);
        if (inputIndex >= numberText.Count)
            return;
        else 
        {
            numberText[inputIndex].text = obj.name;
            inputNumber.Add(int.Parse(obj.name));
            inputIndex++;
        }
        if (inputIndex == numberText.Count)
        {
            string stringInput = string.Empty;
            for (int i = 0; i < inputNumber.Count; i++)
            {
                stringInput += inputNumber[i];
            }
            UserJoinTable joinInfo = new UserJoinTable();
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage
            {
                msgid = MessageId.C2G_QueryTableInfo,
                queryTableInfo = new QueryTableInfo()
                {
                    tableId = stringInput
                }
            });
        }
    }

    /// <summary>
    /// 清除所有输入
    /// </summary>
    private void ClearInput()
    {
        inputIndex = 0;
        inputNumber.Clear();
        for (int i = 0; i < numberText.Count; i++)
        {
            numberText[i].text = string.Empty;
        }
    }
    /// <summary>
    /// 关闭错误输入提示
    /// </summary>
    /// <param name="obj"></param>
    private void errorMaskClickHandler(GameObject obj)
    {
        ClearInput();
        errorObj.gameObject.SetActive(false);
    }

    /// <summary>
    ///  房间号正确
    /// </summary>
    public static void G2C_EnterSuccess(QueryTableInfoResp resp)
    {
        NodeManager.OpenNode<TipsEnterNode>().Inits(resp);
    }

    /// <summary>
    ///  房间号错误
    /// </summary>
    public static void G2C_RoomNumberError()
    {
        JoinGameRoonNode node = NodeManager.GetNode<JoinGameRoonNode>();
        if (node)
        {
            node.ClearInput();
            node.errorObj.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 开房
    /// </summary>
    void OpenRoom()
    {
        NodeManager.OpenNode<CreateGameNode>();
    }

    /// <summary>
    /// 下载按钮
    /// </summary>
    void OpenDown()
    {
        downObj.SetActive(!downObj.activeSelf);
    }

    /// <summary>
    /// 游戏记录按钮
    /// </summary>
    void OpenLog()
    {
        NodeManager.OpenNode<YuepaiLogNode>();
    }


    public static void JoinGameOnFocus()
    {
        if (UserInfoModel.userInfo.userId == 0)
            return;
        string clipValue = SDKManager.Instance.GetFromClipboard();
        if (clipValue.Contains("约牌房间中和好友在一起约牌，你也赶快加入我们吧"))
        {
            int subStartindex = clipValue.IndexOf("#") + 1;
            string userId = clipValue.Substring(clipValue.IndexOf("(") + 1, clipValue.IndexOf(")") - (clipValue.IndexOf("(") + 1));
            if (UserInfoModel.userInfo.userId != int.Parse(userId))
            {
                string tableId = clipValue.Substring(subStartindex, clipValue.Length - subStartindex);
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage
                        {
                            msgid = MessageId.C2G_QueryTableInfo,
                            queryTableInfo = new QueryTableInfo()
                            {
                                tableId = tableId
                            }
                        });
                SDKManager.Instance.CopyToClipboard("");
            }
        }
    }
}


