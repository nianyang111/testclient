using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaJangModel : MonoBehaviour
{
    [HideInInspector]
    public Mj mj;
    [HideInInspector]
    public MaJangPlayer mjp;

    public GameObject mask;
    public int mjNo;
    bool isSelect = false;
    bool canSelect;

    public void Init(Mj mj, MaJangPlayer mjp = null)
    {
        this.mj = mj;
        this.mjp = mjp;
        if (mj != null)
        {
            mjNo = mj.type * 10 + mj.point;
            transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture
                = BundleManager.Instance.GetSprite(mjNo.ToString(), MaJangPage.Instance.majangBundle).texture;
        }
        SetStatu(true);
        canSelect = true;
    }

    //选择一张牌
    public void Select(bool isSelect)
    {
        if (MaJangPage.Instance.currentStatu == 1 && canSelect && mjp.statu != 2)
        {
            if (isSelect)
            {
                if (SetNode.off == 0 && SetNode.read == 1)
                    AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.mjonclick, PageManager.Instance.CurrentPage.name);

                if (this.isSelect)
                {
                    //print(mjp.isTurn);
                    if (mjp.isTurn && !mjp.atTrusteeship)
                    {
                        MaJangPage.Instance.ShowSelectTingPanelFinish();
                        mjp.CreateAction(MjActionType.OutMj, this);
                        MaJangPage.Instance.FinishAction();
                        mjp.isWaitAction = false;
                        canSelect = false;
                    }
                }
                else
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, 0.3f, 0);

                    if (mjp.statu > 0 && !mjp.atTrusteeship && mjp.tingOutList.Count > 0)
                    {
                        MjTing mjTing = mjp.tingOutList.Find((m) => { return mj.type == m.mj.type && mj.point == m.mj.point; });
                        //print("对应的能胡数据：" + mjTing);
                        if (mjTing != null)
                        {
                            MaJangPage.Instance.ShowSelectTingPanel(mjTing.tingNum);
                        }
                    }
                }
            }
            else
            {
                transform.localPosition = Vector3.right * transform.localPosition.x;
                MaJangPage.Instance.ShowSelectTingPanelFinish();
            }
            this.isSelect = isSelect;
        }
    }

    float ratio = 0.015f;

    IEnumerator OnMouseDown()
    {
        if (MaJangPage.Instance.currentStatu == 1 && canSelect && mjp.statu != 2 && gameObject.layer == ConstantUtils.mjLayer)
        {
            Vector3 startPos = transform.position;

            Vector3 last2DMousePosition = Input.mousePosition;
            while (Input.GetMouseButton(0))
            {
                //求得鼠标移动方向
                Vector3 dir = (Input.mousePosition - last2DMousePosition).normalized;
                //求得鼠标位移
                float distance = Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(last2DMousePosition)) * ratio * Screen.width / 1920;
                //Debug.Log("位移距离" + distance);
                transform.position = transform.position + dir * distance;
                last2DMousePosition = Input.mousePosition;
                yield return new WaitForFixedUpdate();
            }
            OnMouseUpCall((transform.position - startPos).y, startPos);
        }
    }

    //Y方向的位移
    void OnMouseUpCall(float moveDistance_Y,Vector3 startPos)
    {
        if (gameObject.layer==ConstantUtils.mjLayer&&( !mjp.isTurn || mjp.atTrusteeship))
        {
            transform.position = startPos;
            if (SetNode.off == 0 && SetNode.read == 1)
                AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.mjonclick, PageManager.Instance.CurrentPage.name);
            return;
        }
        if (moveDistance_Y > 1f)
        {
            if (mjp.isTurn && !mjp.atTrusteeship)
            {
                MaJangPage.Instance.ShowSelectTingPanelFinish();
                mjp.CreateAction(MjActionType.OutMj, this);
                MaJangPage.Instance.FinishAction();
                canSelect = false;
            }
        }
        else if (gameObject.layer == ConstantUtils.mjLayer)
        {
            transform.position = startPos;
            if (SetNode.off == 0 && SetNode.read == 1)
                AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.mjonclick, PageManager.Instance.CurrentPage.name);
        }

    }

    public void Move(bool toLeft)
    {
        CommonAnimation ca = GetComponent<CommonAnimation>();
        ca.pointList.Clear();
        ca.pointList.Add(transform.localPosition);
        if (toLeft)
            ca.pointList.Add(transform.localPosition + MaJangScene.mjSize.x * Vector3.left);
        else
            ca.pointList.Add(transform.localPosition + MaJangScene.mjSize.x * Vector3.right);
        ca.space = Space.Self;
        ca.time = 0.25f;
        ca.Play();
    }

    public void Move(Vector3 targetPoint)
    {
        CommonAnimation ca = GetComponent<CommonAnimation>();
        ca.pointList.Clear();
        ca.pointList.Add(transform.localPosition);
        if (transform.localPosition.x - targetPoint.x > MaJangScene.mjSize.x * 2)
        {
            ca.pointList.Add(transform.localPosition + Vector3.up * MaJangScene.mjSize.y);
            ca.pointList.Add(targetPoint + Vector3.up * MaJangScene.mjSize.y);
        }
        ca.pointList.Add(targetPoint);
        ca.space = Space.Self;
        ca.time = 0.25f;
        ca.Play();
    }
    //设置牌的状态(能否点击)
    public void SetStatu(bool canSelect)
    {
        mask.SetActive(!canSelect);
        this.canSelect = canSelect;
    }
}
