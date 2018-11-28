using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/**
 * 滚动列表优化 
*/
[DisallowMultipleComponent]
public class UIWarpContent : MonoBehaviour
{

    public CallBack<GameObject,int> onInitializeItem;

    public enum Arrangement
    {
        Horizontal,
        Vertical,
    }

    /// <summary>
    /// Type of arrangement -- vertical or horizontal.
    /// </summary>

    public Arrangement arrangement = Arrangement.Horizontal;

    /// <summary>
    /// Maximum children per line.
    /// If the arrangement is horizontal, this denotes the number of rows.
    /// If the arrangement is vertical, this stands for the number of columns.
    /// </summary>
    [Range(1, 50)]
    public int maxPerLine = 1;

    /// <summary>
    /// 显示区域显示的行数或列数
    /// </summary>
    [Range(1, 50)]
    public int showLine = 1;

    /// <summary>
    /// The width of each of the cells.
    /// </summary>

    public float cellWidth = 200f;

    /// <summary>
    /// The height of each of the cells.
    /// </summary>

    public float cellHeight = 200f;

    /// <summary>
    /// The Width Space of each of the cells.
    /// </summary>
    [Range(0, 50)]
    public float cellWidthSpace = 0f;

    /// <summary>
    /// The Height Space of each of the cells.
    /// </summary>
    [Range(0, 50)]
    public float cellHeightSpace = 0f;


    [Range(0, 30)]
    public int viewCount = 5;

    public ScrollRect scrollRect;

    public RectTransform content;

    public GameObject prefab;

    private int dataCount;

    private int curScrollPerLineIndex = -1;

    public List<UIWarpContentItem> listItem;

    private Queue<UIWarpContentItem> unUseItem;

    private CallBack initCall;

    void Awake()
    {
        listItem = new List<UIWarpContentItem>();
        unUseItem = new Queue<UIWarpContentItem>();
        scrollRect.onValueChanged.RemoveAllListeners();
        scrollRect.onValueChanged.AddListener(onValueChanged);
    }

    /// <summary>
    /// 显示初始化
    /// </summary>
    /// <param name="dataCount"></param>
    public void Init(int dataCount,CallBack call=null)
    {
        if (scrollRect == null || content == null || prefab == null)
        {
            Debug.LogError("异常:请检测<" + gameObject.name + ">对象上UIWarpContent对应ScrollRect、Content、prefab 是否存在值...." + scrollRect + " _" + content + "_" + prefab);
            return;
        }
        if (dataCount <= 0)
        {
            return;
        }
        initCall = call;
        unUseItem.Clear();
        listItem.Clear();
        UIUtils.DestroyChildren(content);
        SetDataCount(dataCount);
        UpdateDataByIndex(0);
        if (initCall != null)
        {
            initCall();
        }
        //StartCoroutine(Inits());
    }

    IEnumerator Inits()
    {
        yield return new WaitForSecondsRealtime(0.03f);
        if (initCall != null)
        {
            initCall();
        }
    }

    /// <summary>
    /// 设置显示数据长度
    /// </summary>
    /// <param name="count"></param>
    private void SetDataCount(int count)
    {
        if (dataCount == count)
        {
            return;
        }
        dataCount = count;
        SetUpdateContentSize();
    }

    private void onValueChanged(Vector2 vt2)
    {
        switch (arrangement)
        {
            case Arrangement.Vertical:
                float y = vt2.y;
                if (y >= 1.0f || y <= 0.0f)
                {
                    return;
                }
                break;
            case Arrangement.Horizontal:
                float x = vt2.x;
                if (x <= 0.0f || x >= 1.0f)
                {
                    return;
                }
                break;
        }
        int _curScrollPerLineIndex = getCurScrollPerLineIndex();
        if (_curScrollPerLineIndex == curScrollPerLineIndex)
        {
            return;
        }
        UpdateDataByIndex(_curScrollPerLineIndex);
    }

    /// <summary>
    /// If the arrangement is horizontal, this denotes the number of rows.
    /// If the arrangement is vertical, this stands for the number of columns.
    /// </summary>
    /// <param name="index"></param>
    public void ShowIndex(int index)
    {
        Vector2 position = content.anchoredPosition;
        int offseLine;
        switch (arrangement)
        {
            case Arrangement.Horizontal: //水平方向
                 offseLine = index / maxPerLine - (showLine-1);
                 if (offseLine > 0)
                     position.x = -1*((cellWidth + cellWidthSpace) * (offseLine) + cellHeightSpace);
                
                 break;
            case Arrangement.Vertical://垂着方向
                 offseLine = index / maxPerLine - (showLine-1);
                 if (offseLine > 0)
                     position.y = (cellHeight + cellHeightSpace) * (offseLine) + cellHeightSpace;
                break;
        }
        content.anchoredPosition = position;
        int _curScrollPerLineIndex = getCurScrollPerLineIndex();
        if (dataCount - 1 - index < maxPerLine && _curScrollPerLineIndex>0)
        {
            _curScrollPerLineIndex -= 1;
        }
        if (_curScrollPerLineIndex == curScrollPerLineIndex)
        {
            return;
        }
        UpdateDataByIndex(_curScrollPerLineIndex);
    }

    /**
	 * @des:设置更新区域内item
	 * 功能:
	 * 1.隐藏区域之外对象
	 * 2.更新区域内数据
	 */
    private void UpdateDataByIndex(int scrollPerLineIndex)
    {
        if (scrollPerLineIndex < 0)
        {
            return;
        }
        curScrollPerLineIndex = scrollPerLineIndex;
        int startDataIndex = curScrollPerLineIndex * maxPerLine;
        int endDataIndex = (curScrollPerLineIndex + viewCount) * maxPerLine;
        //移除
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            UIWarpContentItem item = listItem[i];
            int index = item.Index;
            if (index < startDataIndex || index >= endDataIndex)
            {
                item.Index = -1;
                listItem.Remove(item);
                unUseItem.Enqueue(item);
            }
        }
        //显示
        for (int dataIndex = startDataIndex; dataIndex < endDataIndex; dataIndex++)
        {
            if (dataIndex >= dataCount)
            {
                continue;
            }
            if (isExistDataByDataIndex(dataIndex))
            {
                continue;
            }
            createItem(dataIndex);
        }
    }



    /**
	 * @des:添加当前数据索引数据
	 */
    public void AddItem(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex > dataCount)
        {
            return;
        }
        //检测是否需添加gameObject
        bool isNeedAdd = false;
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            UIWarpContentItem item = listItem[i];
            if (item.Index >= (dataCount - 1))
            {
                isNeedAdd = true;
                break;
            }
        }
        SetDataCount(dataCount + 1);

        if (isNeedAdd)
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                UIWarpContentItem item = listItem[i];
                int oldIndex = item.Index;
                if (oldIndex >= dataIndex)
                {
                    item.Index = oldIndex + 1;
                }
                item = null;
            }
            UpdateDataByIndex(getCurScrollPerLineIndex());
        }
        else
        {
            //重新刷新数据
            for (int i = 0; i < listItem.Count; i++)
            {
                UIWarpContentItem item = listItem[i];
                int oldIndex = item.Index;
                if (oldIndex >= dataIndex)
                {
                    item.Index = oldIndex;
                }
                item = null;
            }
        }

    }

    /**
	 * @des:删除当前数据索引下数据
	 */
    public void DelItem(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= dataCount)
        {
            return;
        }
        //删除item逻辑三种情况
        //1.只更新数据，不销毁gameObject,也不移除gameobject
        //2.更新数据，且移除gameObject,不销毁gameObject
        //3.更新数据，销毁gameObject

        bool isNeedDestroyGameObject = (listItem.Count >= dataCount);
        SetDataCount(dataCount - 1);

        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            UIWarpContentItem item = listItem[i];
            int oldIndex = item.Index;
            if (oldIndex == dataIndex)
            {
                listItem.Remove(item);
                if (isNeedDestroyGameObject)
                {
                    GameObject.Destroy(item.gameObject);
                }
                else
                {
                    item.Index = -1;
                    unUseItem.Enqueue(item);
                }
            }
            if (oldIndex > dataIndex)
            {
                item.Index = oldIndex - 1;
            }
        }
        UpdateDataByIndex(getCurScrollPerLineIndex());
    }


    /**
	 * @des:获取当前index下对应Content下的本地坐标
	 * @param:index
	 * @内部使用
	*/
    public Vector3 getLocalPositionByIndex(int index)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        switch (arrangement)
        {
            case Arrangement.Horizontal: //水平方向
                x = (index / maxPerLine) * (cellWidth + cellWidthSpace) + (cellWidth / 2);
                y = -(index % maxPerLine) * (cellHeight + cellHeightSpace) - (cellHeight / 2);
                break;
            case Arrangement.Vertical://垂着方向
                x = (index % maxPerLine) * (cellWidth + cellHeightSpace) + (cellWidth / 2 );
                y = -(index / maxPerLine) * (cellHeight + cellHeightSpace) - (cellHeight / 2);
                break;
        }
        return new Vector3(x, y, z);
    }

    /**
	 * @des:创建元素
	 * @param:dataIndex
	 */
    private void createItem(int dataIndex)
    {
        UIWarpContentItem item;
        if (unUseItem.Count > 0)
        {
            item = unUseItem.Dequeue();
        }
        else
        {
            item = addChild(prefab, content).AddComponent<UIWarpContentItem>();
        }
        item.WarpContent = this;
        item.Index = dataIndex;
        listItem.Add(item);
    }

    /**
	 * @des:当前数据是否存在List中
	 */
    private bool isExistDataByDataIndex(int dataIndex)
    {
        if (listItem == null || listItem.Count <= 0)
        {
            return false;
        }
        for (int i = 0; i < listItem.Count; i++)
        {
            if (listItem[i].Index == dataIndex)
            {
                return true;
            }
        }
        return false;
    }


    /**
	 * @des:根据Content偏移,计算当前开始显示所在数据列表中的行或列
	 */
    private int getCurScrollPerLineIndex()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal: //水平方向
                return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.x) / (cellWidth + cellWidthSpace));
            case Arrangement.Vertical://垂着方向
                return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.y) / (cellHeight + cellHeightSpace));
        }
        return 0;
    }

    /**
	 * @des:更新Content SizeDelta
	 */
    private void SetUpdateContentSize()
    {
        int lineCount = Mathf.CeilToInt((float)dataCount / maxPerLine);
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                content.sizeDelta = new Vector2(cellWidth * lineCount + cellWidthSpace * (lineCount - 1), content.sizeDelta.y);
                break;
            case Arrangement.Vertical:
                content.sizeDelta = new Vector2(content.sizeDelta.x, cellHeight * lineCount + cellHeightSpace * (lineCount - 1));
                break;
        }


    }

    /**
	 * @des:实例化预设对象 、添加实例化对象到指定的子对象下
	 */
    private GameObject addChild(GameObject goPrefab, Transform parent)
    {
        if (goPrefab == null || parent == null)
        {
            Debug.LogError("异常。UIWarpContent.cs addChild(goPrefab = null  || parent = null)");
            return null;
        }
        GameObject goChild = GameObject.Instantiate(goPrefab) as GameObject;
        goChild.layer = parent.gameObject.layer;
        goChild.transform.SetParent(parent, false);
        goChild.gameObject.SetActive(true);
        return goChild;
    }

    void OnDestroy()
    {
        scrollRect = null;
        content = null;
        prefab = null;
        onInitializeItem = null;
        listItem.Clear();
        unUseItem.Clear();
        listItem = null;
        unUseItem = null;
    }
}
