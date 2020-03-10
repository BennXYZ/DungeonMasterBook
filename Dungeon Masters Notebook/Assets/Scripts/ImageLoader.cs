using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    [HideInInspector]
    public string path;

    public Image image;

    public UnityEvent onImageChanged;

    public TMP_InputField urlInput;

    public FDE_Source fde;

    public void ClearImage()
    {
        image.sprite = null;
        path = string.Empty;
        onImageChanged.Invoke();
    }

    public void LoadImageChecked()
    {
        if(path.StartsWith("file:///"))
        {
            LoadImage();
        }
        else
        {
            LoadViaUrl();
        }
    }

    public void OpenExplorer()
    {
        if (path == null)
            path = "";
        //string newPath = EditorUtility.OpenFilePanel("Open image file", path, "png,jpg,jpeg");
        //if(path.Length <= 0 || newPath.Length > 0)
        //{
        //    path = "file:///" + newPath;
        //}
        //LoadImage();
        //onImageChanged.Invoke();
    }

    public void OnExplorerClose()
    {
        if (path == null)
            path = "";
        //string newPath = fde.MainPath;
        if (path.Length <= 0 || path.Length > 0)
        {
            path = "file:///" + path;
        }
        LoadImage();
        onImageChanged.Invoke();
    }

    public void LoadViaUrl()
    {
        if(urlInput.text.Length > 0)
        {
            path = urlInput.text;
            FDE_Source lel;
            StartCoroutine("WaitForImage");
            urlInput.text = "";
        }
    }

    IEnumerator WaitForImage()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
        UnityWebRequestAsyncOperation imageRequest = www.SendWebRequest();

        int timeOutCounter = 0;

        while(!imageRequest.isDone && timeOutCounter < 10)
        {
            timeOutCounter++;
            yield return new WaitForSeconds(2);
        }

        if (!imageRequest.isDone || www.isNetworkError || www.isHttpError)
        {
            image.sprite = null;
        }
        else
        {
            //return valid results:
            Texture2D result = DownloadHandlerTexture.GetContent(www);
            image.sprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.one * 0.5f);
        }
        onImageChanged.Invoke();
    }

    public void LoadImage()
    {
        if (!string.IsNullOrEmpty(path))
        {
            WWW www = new WWW(path);
            image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f);
        }
        else
        {
            image.sprite = null;
        }
    }
}
