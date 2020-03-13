using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using SFB;

public class ImageLoader : MonoBehaviour
{
    [HideInInspector]
    public string path;

    public Sprite currentlyLoadedSprite;

    public Image[] images;

    public UnityEvent onImageChanged;

    public TMP_InputField urlInput;

    public void ClearImage()
    {
        currentlyLoadedSprite = null;
        SetImagesToCurrentSprite();

        path = string.Empty;
        onImageChanged.Invoke();
    }

    public void SetImagesToCurrentSprite()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = currentlyLoadedSprite;
        }
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

        ExtensionFilter[] extenstions = new ExtensionFilter[1];
        extenstions[0] = new ExtensionFilter("Images", "png", "jpg");

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image", "", extenstions, false);
        if(paths.Length > 0)
        {
            path = "file:///" + paths[0];
        }
        LoadImage();
        onImageChanged.Invoke();
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

    public void Test()
    {
        ExtensionFilter[] extenstions = new ExtensionFilter[1];
        extenstions[0] = new ExtensionFilter("Images", "png", "jpg");

        path = StandaloneFileBrowser.OpenFilePanel("Open Image","", extenstions, false )[0];
    }

    public void LoadViaUrl()
    {
        if(urlInput.text.Length > 0 && gameObject.activeInHierarchy)
        {
            path = urlInput.text;
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
            currentlyLoadedSprite = null;
            SetImagesToCurrentSprite();
        }
        else
        {
            //return valid results:
            Texture2D result = DownloadHandlerTexture.GetContent(www);
            currentlyLoadedSprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.one * 0.5f);
            SetImagesToCurrentSprite();
        }
        onImageChanged.Invoke();
    }

    public void LoadImage()
    {
        if (!string.IsNullOrEmpty(path))
        {
            WWW www = new WWW(path);
            currentlyLoadedSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f);
            SetImagesToCurrentSprite();
        }
        else
        {
            currentlyLoadedSprite = null;
            SetImagesToCurrentSprite();
        }
    }
}
