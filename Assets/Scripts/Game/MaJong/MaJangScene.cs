using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MaJangScene : MonoBehaviour
{
    public static Vector3 mjSize = new Vector3(0.825f, 1.2f, 0.475f);
    public static MaJangScene Instance;
    public static int surplusCount;

    public Transform diceParent, moJongStackParent, orientationParent;
    public GameObject maJongPf;
    public Camera sceneCamera, selfCamera;
    public int bankerIndex;
    public int hostPos;//服務器給的位置
    public List<Transform> stackMaJong = new List<Transform>();
    public Sprite[] fengPics;

    List<Transform> allMaJong = new List<Transform>();
    MaJangModel lastSelectMj;

    List<List<Vector3>> dices = new List<List<Vector3>>();
    public void Awake()
    {
        Instance = this;
        dices.Add(new List<Vector3>() { diceParent.GetChild(0).localPosition, diceParent.GetChild(0).localEulerAngles });
        dices.Add(new List<Vector3>() { diceParent.GetChild(1).localPosition, diceParent.GetChild(1).localEulerAngles });
    }


    public void Init(List<int> diceAngle, int bankerIndex,int hostPos, int playerCount,UnityAction act)
    {
        transform.position = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        this.bankerIndex = bankerIndex;
        this.hostPos = hostPos;
        if (allMaJong.Count > 0)
        {
            foreach (MaJangPlayer mjp in MaJangPage.Instance.playerList)
                mjp.ClearData();
            foreach (var item in allMaJong)
                Destroy(item.gameObject);
            stackMaJong.Clear();
            allMaJong.Clear();
        }
        CreateMaJangStack();
        StartCoroutine(ThrowDice(diceAngle, playerCount, true, null, act));
    }

    int curHeight;
    void SetSelfCameraSize()
    {
        if (curHeight != Screen.height)
        {
            selfCamera.orthographicSize = 0.00420f * Screen.height;
            curHeight = Screen.height;
        }
    }
    void Update()
    {
        SetSelfCameraSize();
        if (InputUtils.OnReleased())
        {
            Ray ray = selfCamera.ScreenPointToRay(InputUtils.GetTouchPosition());
            RaycastHit rh;
            if (Physics.Raycast(ray, out rh))
            {
                MaJangModel mjmTemp = rh.collider.GetComponent<MaJangModel>();
                if (mjmTemp && mjmTemp.gameObject.layer == ConstantUtils.mjLayer)
                {
                    if (lastSelectMj && lastSelectMj.gameObject.layer == ConstantUtils.mjLayer && !mjmTemp.Equals(lastSelectMj))
                        lastSelectMj.Select(false);
                    mjmTemp.Select(true);
                    lastSelectMj = mjmTemp;
                }
                else
                {
                    if (lastSelectMj)
                    {
                        lastSelectMj.Select(false);
                        lastSelectMj = null;
                    }
                }
            }
        }
    }

    //投掷骰子
    public IEnumerator ThrowDice(List<int> diceAngle, int playerCount, bool isAutoDeal = true, UnityAction ua = null,UnityAction act = null)
    {
        #region ...骰子
        //List<Vector3> lastPos = new List<Vector3>();
        //Rigidbody[] rbs = diceParent.GetComponentsInChildren<Rigidbody>(true);
        //rbs[0].transform.localEulerAngles = new Vector3(35, 115, 85);
        //rbs[1].transform.localEulerAngles = new Vector3(15, -45, 20);
        //for (int i = 0; i < rbs.Length; i++)
        //{
        //    Rigidbody rb = rbs[i];
        //    rb.gameObject.SetActive(true);
        //    rb.transform.localEulerAngles *= diceAngle[i];
        //    rb.transform.localPosition = Vector3.right * i;
        //    lastPos.Add(rb.transform.localPosition);
        //    rb.AddForce(Vector3.left * 150);
        //}
        //bool isWhile = true;
        //int diffCount;
        #endregion
        int diceNum = diceAngle[0] + diceAngle[1];
        diceParent.gameObject.SetActive(true);
        if (SetNode.off == 0 && SetNode.read == 1)
            AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.mjaudiotouzi, PageManager.Instance.CurrentPage.name);
        yield return new WaitForSecondsRealtime(1.5f);
        diceParent.gameObject.SetActive(false);
        UIUtils.SetAllChildrenLayer(diceParent, ConstantUtils.modelLayer);
        Transform diceParent1 = Instantiate(diceParent, diceParent.transform.parent);
        diceParent1.GetComponent<Animator>().enabled = false;
        //yield return new WaitForSecondsRealtime(0.5f);
        for (int i = 0; i < diceParent1.childCount; i++)
        {
            Transform tf = diceParent1.GetChild(i);
            if (diceAngle[i] == 1)
                tf.localEulerAngles = new Vector3(90, 0, 0);
            else if (diceAngle[i] == 2)
                tf.localEulerAngles = new Vector3(0, 0, 90);
            else if (diceAngle[i] == 3)
                tf.localEulerAngles = new Vector3(180, 0, 0);
            else if (diceAngle[i] == 4)
                tf.localEulerAngles = new Vector3(0, 0, 0);
            else if (diceAngle[i] == 5)
                tf.localEulerAngles = new Vector3(0, 0, -90);
            else if (diceAngle[i] == 6)
                tf.localEulerAngles = new Vector3(-90, 0, 0);
        }
        diceParent1.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        Destroy(diceParent1.gameObject);
        #region ...计算骰子的值
        //while (isWhile)
        //{
        //    diffCount = 0;
        //    yield return new WaitForEndOfFrame();
        //    for (int i = 0; i < rbs.Length; i++)
        //    {
        //        if (!rbs[i].transform.localPosition.Equals(lastPos[i]))
        //            diffCount++;
        //        lastPos[i] = rbs[i].transform.localPosition;
        //    }
        //    isWhile = diffCount != 0;
        //}
        //Debug.Log(lastPos[0]+"---"+lastPos[1]);
        //Vector3 angle;
        //float angleX;
        //
        //float diffRange = 3;
        //for (int i = 0; i < rbs.Length; i++)
        //{
        //    angle = rbs[i].transform.localEulerAngles;
        //    angleX = angle.x;
        //    if (angleX < 0) angleX += 360;
        //    if (Mathf.Abs(angleX) < diffRange || Mathf.Abs(angleX - 360) < diffRange)
        //    {
        //        if (Mathf.Abs(angle.z) < diffRange)
        //            diceNum += 4;
        //        else if (Mathf.Abs(angle.z - 90) < diffRange)
        //            diceNum += 2;
        //        else if (Mathf.Abs(angle.z - 180) < diffRange)
        //            diceNum += 3;
        //        else if (Mathf.Abs(angle.z - 270) < diffRange)
        //            diceNum += 5;
        //    }
        //    else if (Mathf.Abs(angleX - 90) < diffRange)
        //        diceNum += 1;
        //    else if (Mathf.Abs(angleX - 180) < diffRange)
        //    {
        //        if (Mathf.Abs(angle.z) < 1)
        //            diceNum += 3;
        //        else if (Mathf.Abs(angle.z - 90) < diffRange)
        //            diceNum += 5;
        //        else if (Mathf.Abs(angle.z - 180) < diffRange)
        //            diceNum += 4;
        //        else if (Mathf.Abs(angle.z - 270) < diffRange)
        //            diceNum += 2;
        //    }
        //    else if (Mathf.Abs(angleX - 270) < diffRange)
        //        diceNum += 6;
        //    else
        //        diceNum += 6;
        //}
        //
        //for (int i = 0; i < diceParent.childCount; i++)
        //{
        //    Transform tf = diceParent.GetChild(i);
        //    tf.localPosition = dices[i][0];
        //    tf.localEulerAngles = dices[i][1];
        //}
        //foreach (Transform tf in diceParent) tf.gameObject.SetActive(false);
        #endregion
        Debug.Log("骰子值:" + diceNum);
        if (act != null) act();
        yield return new WaitForSecondsRealtime(1.5f);
        RefreshMjStackStruct(diceNum);
        if (isAutoDeal) StartCoroutine(AutoDeal(playerCount));
        if (ua != null) ua();
    }

    //创建牌堆
    public void CreateMaJangStack(bool isNew = true)
    {
        surplusCount = MaJangPage.allMjNum;
        int oneGroupNum = (int)(MaJangPage.allMjNum * 0.25f);
        int group, relativeIndex;
        Transform tf;
        Vector3 origin = Vector3.right * (oneGroupNum * 0.5f - 1) * 0.5f * mjSize.x;
        for (int i = 0; i < MaJangPage.allMjNum; i++)
        {
            group = i / oneGroupNum;
            relativeIndex = i - group * oneGroupNum;
            tf = moJongStackParent.GetChild(3 - group);
            GameObject go;
            //if (isNew)
            //{
                go = Instantiate(maJongPf, tf);
                allMaJong.Add(go.transform);
            //}
            //else
            //{
            //    go = allMaJong[i].gameObject;
            //    go.transform.SetParent(tf);
            //    UIUtils.SetAllChildrenLayer(go.transform, ConstantUtils.modelLayer);
            //}
            go.name = i.ToString();
            go.transform.localEulerAngles = Vector3.left * ConstantUtils.const90;
            if (relativeIndex % 2 == 0)
                go.transform.localPosition = origin - new Vector3(relativeIndex * MaJangPage.mj_interval * mjSize.x, -mjSize.z, 0);
            else
                go.transform.localPosition = origin - Vector3.right * (relativeIndex - 1) * MaJangPage.mj_interval * mjSize.x;
            go.transform.localScale = Vector3.one * MaJangPage.mj_size;
        }
        stackMaJong.AddRange(allMaJong);
    }

    /// <summary>
    /// 根据当前庄家和骰子数计算切牌位置索引
    /// </summary>
    /// <param name="diceNum"></param>
    /// <returns></returns>
    int GetCutGroupIndex(int diceNum)
    {
        for (int i = 0; i < MaJangPage.Instance.playerList.Length; i++)
            moJongStackParent.GetChild(i).name = MaJangPage.Instance.playerList[i].seatNo.ToString();
        int target = 0;
        if ((diceNum - 1) % 4 == 0)
            target = bankerIndex + 2;
        else
            target = bankerIndex + (diceNum - 1) % 4;
        if (target > 3)
            target -= 4;
        Debug.Log("要切牌的位置：" + target);
        return int.Parse(moJongStackParent.Find(target.ToString()).GetChild(diceNum * 2).name);
    }

    //根据切牌位置重置牌堆顺序
    void RefreshMjStackStruct(int diceNum)
    {
        int currentMaJongIndex = GetCutGroupIndex(diceNum);
        List<Transform> temp = stackMaJong.GetRange(0, currentMaJongIndex);
        stackMaJong.RemoveRange(0, currentMaJongIndex);
        stackMaJong.AddRange(temp);
    }

    /// <summary>
    /// 自动发牌
    /// </summary>
    /// <param name="diceNum"></param>
    /// <returns></returns>
    IEnumerator AutoDeal(int playerCount)
    {
        int touchNum = 4, touchMoundCount = 3, touchCount = playerCount * (touchMoundCount + 1) + 1, currentTouchUserIndex = hostPos;
        MaJangPlayer mjp;
        Mj mj = null;
        bool isCurrentPlayer;
        for (int i = 0; i < touchCount; i++)
        {
            mjp = MaJangPage.Instance.GetPlayerFromSeatNo(currentTouchUserIndex);
            isCurrentPlayer = mjp == MaJangPage.Instance.currentPlayer;
            int group = i / playerCount;
            if (i < playerCount * touchMoundCount)
            {
                for (int j = 0; j < touchNum; j++)
                {
                    if (isCurrentPlayer)
                        mj = mjp.mjList[group * touchNum + j];
                    else
                        mj = null;
                    mjp.GetMj(mj);
                }
                mjp.isTurn = false;
            }
            else
            {
                if (isCurrentPlayer)
                    mj = mjp.mjList[touchNum * touchMoundCount + group - touchMoundCount];
                else
                    mj = null;
                if (i == touchCount - 1)
                {
                    if (isCurrentPlayer)
                        mjp.SortMj();
                    mjp.GetMj(mj, true);
                    SocketClient.Instance.AddSendMessageQueue(new C2GMessage() { msgid = MessageId.C2G_FaiPaiAllOk });
                    mjp.isTurn = true;
                }
                else
                {
                    mjp.GetMj(mj);
                    mjp.isTurn = false;
                }
            }
            currentTouchUserIndex++;
            if (currentTouchUserIndex > playerCount - 1)
                currentTouchUserIndex -= playerCount;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        //如果操作菜单中有按钮处于显示状态，则说明可听、杠、胡
        //foreach (Transform tf in MaJangPage.Instance.gameMenu.transform)
        //    if (tf.gameObject.activeSelf)
        //    {
        //        MaJangPage.Instance.gameMenu.SetActive(true);
        //        break;
        //    }
    }

    int lastOperator = -1;
    /// <summary>
    /// 设置当前操作者
    /// </summary>
    /// <param name="index">操作者座位号</param>
    public void SetOperator(int index)
    {
        if (index != lastOperator)
        {
            orientationParent.GetChild(index).GetComponent<MeshRenderer>().sharedMaterial.mainTexture
                = BundleManager.Instance.GetSprite(fengPics[index].name, MaJangPage.Instance.GetSpriteAB()).texture;
            orientationParent.GetChild(index).GetChild(0).gameObject.SetActive(true);
            if (lastOperator >= 0)
            {
                orientationParent.GetChild(lastOperator).GetComponent<MeshRenderer>().sharedMaterial.mainTexture
                    = BundleManager.Instance.GetSprite(fengPics[fengPics.Length - 1].name, MaJangPage.Instance.GetSpriteAB()).texture;
                orientationParent.GetChild(lastOperator).GetChild(0).gameObject.SetActive(false);
            }
            lastOperator = index;
            HandheldManager.Instance.Close();
        }
        if (MaJangPage.Instance.playerCount == 2)
            index /= 2;
        MaJangPage.Instance.currentOperator = MaJangPage.Instance.GetPlayerFromSeatNo(index);
        MaJangPage.Instance.currentOperator.isFinishAction = false;
        MaJangPage.Instance.RefreshTimer();
    }
}
