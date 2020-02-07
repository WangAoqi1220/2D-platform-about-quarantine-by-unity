using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFader : MonoBehaviour
{
    public Camera[] Cameras;

    public Color[] CamCols = null;

    public float FadeTime = 2.0f;

    public Material Mat = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Camera c in Cameras)
            c.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);//摄像机的目标渲染纹理
    }

    private void OnPostRender()//相机完成渲染后调用该方法
    {
        Rect ScreenRct = new Rect(0, 0, Screen.width, Screen.height);

        Rect SourceRect = new Rect(0, 1, 1, -1);

        for(int i = 0; i < Cameras.Length; i++)
        {
            Cameras[i].Render();
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            Graphics.DrawTexture(ScreenRct, Cameras[i].targetTexture, SourceRect, 0, 0, 0, 0, CamCols[i]);
            GL.PopMatrix();
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)//相机完成图片渲染后调用该方法
    {
        Graphics.Blit(src, dst, Mat);//将源纹理src复制到目标dst中，以使用着色器渲染纹理。这主要用于实现mat后处理效果。

    }

    public IEnumerator Fade(Color From, Color To, float TotalTime)
    {
        float ElapsedTime = 0f;
        while(ElapsedTime <= TotalTime)
        {
            CamCols[1] = Color.Lerp(From, To, ElapsedTime/TotalTime);

            yield return null;

            ElapsedTime += Time.deltaTime;

            CamCols[1] = Color.Lerp(From, To, 1f);

        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();//停止所有协程

            if (CamCols[1].a <= 0f)
                StartCoroutine(Fade(CamCols[1], new Color(0.5f, 0.5f, 0.5f, 1f), FadeTime));
            else
                StartCoroutine(Fade(CamCols[1], new Color(0.5f, 0.5f, 0.5f, 0f), FadeTime));

        }
    }
}
