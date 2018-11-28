using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
public class ParticleToFrame : MonoBehaviour
{
    public string folder = "PNG_Animations";

    public int frameRate = 25;// export frame rate 导出帧率，设置Time.captureFramerate会忽略真实时间，直接使用此帧率  
    public float frameCount = 100;// export frame count 导出帧的数目，100帧则相当于导出5秒钟的光效时间。由于导出每一帧的时间很长，所以导出时间会远远长于直观的光效播放时间  

    private int width, height;
    private string realFolder = "";
    private float originaltimescaleTime;
    private float currentTime = 0;
    private bool over = false;
    private int currentIndex = 0;
    private Camera exportCamera;

    public void Start()
    {
        width = Screen.width;
        height = Screen.height;

        Time.captureFramerate = frameRate;
        realFolder = Path.Combine(folder, name);

        if (!Directory.Exists(realFolder))
            Directory.CreateDirectory(realFolder);
        originaltimescaleTime = Time.timeScale;
        exportCamera = Camera.main;
        currentTime = 0;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (!over)
        {
            if (currentIndex >= frameCount)
                over = true;
            else
                // 每帧截屏  
                StartCoroutine(CaptureFrame());
        }

    }

    IEnumerator CaptureFrame()
    {
        Time.timeScale = 0;
        yield return new WaitForEndOfFrame();

        string filename = String.Format("{0}/{1:D04}.png", realFolder, ++currentIndex);
        UIUtils.Log(filename);

        RenderTexture blackCamRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture whiteCamRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

        exportCamera.targetTexture = blackCamRenderTexture;
        exportCamera.backgroundColor = Color.black;
        exportCamera.Render();
        RenderTexture.active = blackCamRenderTexture;
        Texture2D texb = GetTex2D();

        exportCamera.targetTexture = whiteCamRenderTexture;
        exportCamera.backgroundColor = Color.white;
        exportCamera.Render();
        RenderTexture.active = whiteCamRenderTexture;
        Texture2D texw = GetTex2D();

        if (texw && texb)
        {
            Texture2D outputtex = new Texture2D(width, height, TextureFormat.ARGB32, false);

            for (int y = 0; y < outputtex.height; ++y)
            {
                for (int x = 0; x < outputtex.width; ++x)
                {
                    float alpha;
                    Color b = texb.GetPixel(x, y);
                    Color w = texw.GetPixel(x, y);
                    alpha = Mathf.Max(w.r, w.g, w.b) - Mathf.Max(b.r, b.g, b.b);
                    alpha = 1 - alpha;
                    Color color;
                    if (alpha == 0)
                        color = Color.clear;
                    else
                    {
                        //if (b.r < 0.2f && Mathf.Abs(b.r - b.g) < 0.04f && Mathf.Abs(b.g - b.b) < 0.04f && Mathf.Abs(b.r - b.b) < 0.04f)
                        //    color = b + new Color(0.5f, 0.5f, 0.5f, -b.a);
                        //else
                        color = b + new Color(0.4f, 0.4f, 0.4f, 0);
                    }
                    color.a = alpha;
                    outputtex.SetPixel(x, y, color);
                }
            }

            byte[] pngShot = outputtex.EncodeToPNG();
            File.WriteAllBytes(filename, pngShot);

            pngShot = null;
            RenderTexture.active = null;
            Destroy(outputtex);
            outputtex = null;
            blackCamRenderTexture = null;
            whiteCamRenderTexture = null;
            Destroy(texb);
            texb = null;
            Destroy(texw);
            texb = null;

            GC.Collect();

            Time.timeScale = originaltimescaleTime;
        }
    }

    private Texture2D GetTex2D()
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        return tex;
    }
}