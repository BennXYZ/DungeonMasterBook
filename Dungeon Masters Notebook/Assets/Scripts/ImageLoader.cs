using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using SFB;
using System.Windows.Forms;
using System.IO;
using Ookii.Dialogs;
using System.Linq.Expressions;
using System;

public class ImageLoader : MonoBehaviour
{
    [HideInInspector]
    public string path;

    public Sprite defaultSprite;

    public Sprite currentlyLoadedSprite;

    public Image[] images;

    public UnityEvent startLoadingImages;
    public UnityEvent onImageChanged;

    public TMP_InputField urlInput;

    List<UnityWebRequest> runningCoroutines = new List<UnityWebRequest>();

    public void ClearImage()
    {
        currentlyLoadedSprite = defaultSprite;
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
        if (path.StartsWith("file:///"))
        {
            LoadViaUrl(path);
        }
        else
        {
            LoadViaUrl(urlInput.text);
        }
    }

    public void LoadWithURL()
    {
        LoadViaUrl(urlInput.text);
    }

    public void OpenExplorer()
    {
        if (path == null)
            path = "";

        ExtensionFilter[] extenstions = new ExtensionFilter[1];
        extenstions[0] = new ExtensionFilter("Images", "png", "jpg");

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image", "", extenstions, false);
        if (paths.Length > 0)
        {
            path = "file:///" + paths[0];
        }
        LoadViaUrl(path);
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
        LoadViaUrl(path);
        onImageChanged.Invoke();
    }

    private void OnEnable()
    {
        if (urlInput != null)
        {
            urlInput.interactable = true;
        }
    }

    public void Test()
    {
        ExtensionFilter[] extenstions = new ExtensionFilter[1];
        extenstions[0] = new ExtensionFilter("Images", "png", "jpg");

        path = StandaloneFileBrowser.OpenFilePanel("Open Image", "", extenstions, false)[0];
    }

    public void LoadViaUrl(string path, Action<Sprite> callback = null, bool updateMainPage = true)
    {
        StartCoroutine(WaitForImage(path, callback, updateMainPage));
    }

    IEnumerator WaitForImage(string path, Action<Sprite> callback = null, bool updateMainPage = true)
    {
        if (runningCoroutines.Count <= 0)
        {
            startLoadingImages.Invoke();
        }
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
        UnityWebRequestAsyncOperation imageRequest = www.SendWebRequest();
        runningCoroutines.Add(www);

        int timeOutCounter = 0;
        currentlyLoadedSprite = defaultSprite;

        while (!imageRequest.isDone && timeOutCounter < 20)
        {
            timeOutCounter++;
            yield return new WaitForSeconds(1);
        }

        if (!imageRequest.isDone || www.isNetworkError || www.isHttpError)
        {
            currentlyLoadedSprite = defaultSprite;
            if(updateMainPage)
                SetImagesToCurrentSprite();
        }
        else
        {
            //return valid results:
            Texture2D result = DownloadHandlerTexture.GetContent(www);
            currentlyLoadedSprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.one * 0.5f);
            if (callback != null)
                callback.Invoke(currentlyLoadedSprite);
            if(updateMainPage)
                SetImagesToCurrentSprite();
        }
        runningCoroutines.Remove(www);
        if (runningCoroutines.Count <= 0)
        {
            //No more Images to load
            onImageChanged.Invoke();
        }
    }

    public void LoadImage()
    {
        LoadViaUrl(path);
        return;
        if (!string.IsNullOrEmpty(path))
        {
            WWW www = new WWW(path);
            while (!www.isDone) ;
            currentlyLoadedSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f);
            SetImagesToCurrentSprite();
        }
        else
        {
            currentlyLoadedSprite = defaultSprite;
            SetImagesToCurrentSprite();
        }
    }
}