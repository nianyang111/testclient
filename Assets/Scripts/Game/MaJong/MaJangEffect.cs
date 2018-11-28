using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaJangEffect : MonoBehaviour
{
    public Camera ca;

    public void PlayEffect(int effectId)
    {
        gameObject.SetActive(true);
        StopCoroutine(PlayEffectAc(1));
        UIUtils.SetAllChildrenActive(transform, false);
        StartCoroutine(PlayEffectAc(effectId));
    }

    IEnumerator PlayEffectAc(int effectId)
    {
        if (transform.Find(effectId.ToString()) != null)
        {
            GameObject partical = transform.Find(effectId.ToString()).gameObject;
            partical.SetActive(true);
            yield return new WaitForSecondsRealtime(2.4f);
            gameObject.SetActive(false);
            partical.SetActive(false);
        }
    }
}
