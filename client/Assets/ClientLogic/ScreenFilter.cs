using UnityEngine;
using System.Collections;

// https://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/
public class ScreenFilter : MonoBehaviour {
    public float intensity;
    private Material material;

    void Awake ()
    {
        material = new Material(Shader.Find("Hidden/Test"));
    }
    
    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit (source, destination);
            return;
        }
        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit (source, destination, material);
    }
}