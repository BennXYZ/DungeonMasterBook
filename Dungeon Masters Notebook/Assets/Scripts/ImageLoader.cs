using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    [HideInInspector]
    public string path;

    public Image image;

    public UnityEvent onImageChanged;

    public void ClearImage()
    {
        image.sprite = null;
        path = string.Empty;
        onImageChanged.Invoke();
    }

    public void OpenExplorer()
    {
        if (path == null)
            path = "";
        string newPath = EditorUtility.OpenFilePanel("Open image file", path, "png,jpg,jpeg");
        if(path.Length <= 0 || newPath.Length > 0)
        {
            path = newPath;
        }
        LoadImage();
        onImageChanged.Invoke();
    }

    public void LoadImage()
    {
        if (!string.IsNullOrEmpty(path))
        {
            WWW www = new WWW("file:///" + path);
            image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f);
        }
        else
        {
            image.sprite = null;
        }
    }
}
