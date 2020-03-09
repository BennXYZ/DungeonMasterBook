using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public CanvasScaler canvas;

    private void Awake()
    {
        //OnSizeChanged(0.75f);
    }

    public void OnSizeChanged(float value)
    {
        canvas.referenceResolution = (Vector2.up * 2) * value * 1500 + Vector2.right * value * 1500;
    }
}
