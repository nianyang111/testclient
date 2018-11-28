using UnityEngine;
using System.Collections;

public class Page : MonoBehaviour
{
    protected AssetBundle spriteAB;

    void Start()
    {
        Init();
    }

    public void InitData(AssetBundle ab)
    {
        if (ab != null)
            spriteAB = ab;
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        gameObject.SetActive(false);
    }

    public virtual void Init() { }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public AssetBundle GetSpriteAB()
    {
        return spriteAB;
    }
}
