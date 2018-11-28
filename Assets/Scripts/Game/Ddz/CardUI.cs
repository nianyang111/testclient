using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// 卡牌显示层
/// </summary>
public class CardUI : MonoBehaviour
{
    public static bool isDrag = false;
    public static List<CardUI> dragSelectUI = new List<CardUI>();

    public Card card;
    public Image image;
    public GameObject dzObj;
    private bool isSelected = false;

    /// <summary>
    /// sprite所装载的card
    /// </summary>
    public Card Card
    {
        set
        {
            card = value;
            if (card != null)
                card.IsSprite = true;
            SetSprite();
        }
        get { return card; }
    }

    /// <summary>
    /// 是否被点击中
    /// </summary>
    public bool Select
    {
        set { isSelected = value; }
        get { return isSelected; }
    }


    private void Awake()
    {
        if (image)
            image = transform.Find("icon").GetComponent<Image>();
        UGUIEventListener listener = UGUIEventListener.Get(image.gameObject);
        listener.onClick = (g) => OnClick();
        listener.onDragStart = delegate { OnDragStart(); };
        listener.onDragEnd = delegate { OnDragEnd(); };
        listener.onEnter = delegate { OnPointerEnter(); };
        //listener.onDown = (g, eventData) => OnPointerDown(eventData);
        //listener.onDrag = (g, eventData) => OnDrag(eventData);
    }


    /// <summary>
    /// 设置Image的显示
    /// </summary>
    void SetSprite(bool isShowBack)
    {
        string sprname = "";
        if (isShowBack || Card == null)
        {
            sprname = "card_background";
        }
        else
        {
            sprname = card.GetCardName;
        }
        image.sprite = BundleManager.Instance.GetSprite(sprname, LandlordsPage.Instance.cardAssetBunle);

        dzObj.SetActive(card.isDzCard);
    }

    /// <summary>
    /// 设置Image的显示
    /// </summary>
    private void SetSprite()
    {
        SetSprite(card == null);
        if (card == null || card.Attribution != UserInfoModel.userInfo.userId.ToString())
            GetComponentInChildren<Button>().enabled = false;
    }

    /// <summary>
    /// 卡牌点击
    /// </summary>
    public void OnClick()
    {
        if (card == null)
            return;
        if (card.Attribution == UserInfoModel.userInfo.userId.ToString())
        {
            transform.localPosition += isSelected ? Vector3.up * -30 : Vector3.up * 30;
            isSelected = !isSelected;
        }
    }

    void TestClick()
    {
        if (isSelected)
        {
            transform.localPosition -= Vector3.up * 30;
            isSelected = false;
        }
        else
        {
            transform.localPosition += Vector3.up * 30;
            isSelected = true;
        }
    }
    /// <summary>
    /// 设置父物体索引
    /// </summary>
    /// <param name="index"></param>
    public void SetIndex(int index)
    {
        transform.SetSiblingIndex(index);
    }

    /// <summary>
    /// 设置卡牌的位置
    /// </summary>
    /// <param name="parent">父物体</param>
    /// <param name="index">索引</param>
    public void SetPosition(int index)
    {
        transform.SetSiblingIndex(index);
        //右方向
        if (card.Attribution == UserInfoModel.userInfo.userId.ToString())
        {
            transform.localPosition = Vector3.right * 25 * index;
        }
    }

    public void Destroy()
    {
        if (card != null)
            card.IsSprite = false;
        Destroy(gameObject);
    }

    #region 触摸事件    
    Vector2 beginPointer;//开始按下的位置
    bool isDown = false;//是否按下
    public void OnPointerDown(PointerEventData eventData)
    {
        if (OrderController.Instance.TypeUid == UserInfoModel.userInfo.userId.ToString())
        {
            beginPointer = eventData.position;
            isDown = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OrderController.Instance.TypeUid == UserInfoModel.userInfo.userId.ToString())
        {
            if (isDown == false) return;

            //触摸偏移量
            Vector2 touchOffset = eventData.position - beginPointer;//x  y 
            float x = Mathf.Abs(touchOffset.x);
            float y = Mathf.Abs(touchOffset.y);

            if (x > y && x > 70)//水平移动
            {
            }
            else if (x < y && y > 40)//垂直移动
            {
                if (OrderController.Instance.TypeUid == UserInfoModel.userInfo.userId.ToString() && OrderController.Instance.CurInterationType == InterationType.PopCard)
                    Interaction.Instance.PlayCard();
            }
            isDown = false;
        }
    }

    public void OnPointerEnter()
    {
        if (isDrag)
        {
            CardUI.AddToList(this);
        }
    }

    public void OnDragStart()
    {
        dragSelectUI.Clear();
        CardUI.AddToList(this);
        CardUI.isDrag = true;
    }

    public void OnDragEnd()
    {
        print("end");
        CardUI.isDrag = false;
        for (int i = 0; i < dragSelectUI.Count; i++)
        {
            CardUI.dragSelectUI[i].OnClick();
            CardUI.dragSelectUI[i].image.color = Color.white;
        }
        dragSelectUI.Clear();
    }

    static void AddToList(CardUI cardUI)
    {
        if (CardUI.dragSelectUI.Contains(cardUI))
            return;
        CardUI.dragSelectUI.Add(cardUI);
        cardUI.image.color = Color.gray;
    }

    #endregion

    public void SetCardSize(Vector2 vec)
    {
        (transform as RectTransform).sizeDelta = vec;
        SetDzIconSize();
    }

    public void SetDzIconSize()
    {
        RectTransform trans = (RectTransform)transform;
        ((RectTransform)dzObj.transform).sizeDelta = new Vector2(trans.sizeDelta.x / 1.595f, trans.sizeDelta.y / 2.145f);
    }

}
