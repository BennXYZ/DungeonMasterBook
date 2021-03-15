using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class CampaignSelection : MonoBehaviour
{
    const string SettingsPostFix = "/Settings.DMNBSettings";

    public Transform loadingItemsParent;

    public GameObject loadingItemPrefab;

    public TMP_InputField inputField;

    List<CampaignLoadingItem> items;

    NotebookSettings currentSettings;

    private void Awake()
    {
        items = new List<CampaignLoadingItem>();
        if (File.Exists(Application.persistentDataPath + SettingsPostFix))
        {
            string json = File.ReadAllText(Application.persistentDataPath + SettingsPostFix);
            currentSettings = JsonConvert.DeserializeObject<NotebookSettings>(json);
            if (currentSettings.names == null)
                currentSettings.names = new List<string>();
            if (currentSettings.paths == null)
                currentSettings.paths = new List<string>();
        }
        else
        {
            currentSettings = new NotebookSettings();
            string json = JsonConvert.SerializeObject(currentSettings);
            File.WriteAllText(Application.persistentDataPath + SettingsPostFix, json);
        }
    }

    private void SaveCurrentSettings()
    {
        string json = JsonConvert.SerializeObject(currentSettings);
        File.WriteAllText(Application.persistentDataPath + SettingsPostFix, json);
    }

    private void OnEnable()
    {
        Clear();
        CreateLoadingItems();
    }

    private void Clear()
    {
        foreach(Transform child in loadingItemsParent)
        {
            Destroy(child.gameObject);
        }
        items.Clear();
    }

    public void CreateLoadingItems()
    {
        for (int i = 0; i < currentSettings.names.Count; i++)
        {
            CampaignLoadingItem newItem = Instantiate(loadingItemPrefab, loadingItemsParent).GetComponent<CampaignLoadingItem>();
            newItem.title = currentSettings.names[i];
            newItem.titleField.text = newItem.title;
            newItem.path = currentSettings.paths[i];
            newItem.onPress.AddListener(delegate { OpenCampaign(newItem); });
            newItem.onRemove.AddListener(delegate { RemoveCampaign(newItem); });
            items.Add(newItem);
        }
    }

    public void CreateNewCampaign()
    {
        string path = SFB.StandaloneFileBrowser.SaveFilePanel("Create Campaign", "", "the thieves guild", "book");
        if (string.IsNullOrEmpty(path))
            return;
        string name = GetNameFromPath(path);
        GameManager.CreateNewCampaign(name, path);

        currentSettings.names.Add(name);
        currentSettings.paths.Add(path);
        SaveCurrentSettings();

        gameObject.SetActive(false);
    }

    private string GetNameFromPath(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    void ActuallyRemoveCampaign(CampaignLoadingItem item)
    {
        int index = items.IndexOf(item);
        currentSettings.names.RemoveAt(index);
        currentSettings.paths.RemoveAt(index);
        SaveCurrentSettings();
        Destroy(item.gameObject);
        items.Remove(item);
    }

    public void RemoveCampaign(CampaignLoadingItem item)
    {
        WarningNotification.OpenNotificationWindow("Do you really want to delete the Campaign " + item.title + "?", delegate { ActuallyRemoveCampaign(item); });
    }

    public void OpenCampaign(CampaignLoadingItem item)
    {
        if(File.Exists(item.path))
        {
            GameManager.OpenCampaign(item.path);
            gameObject.SetActive(false);
        }
        else
        {
            RemoveCampaign(item);
        }
    }

    public void ImportCampaign()
    {
        string[] paths = SFB.StandaloneFileBrowser.OpenFilePanel("Import Campaign", "", "book", false);
        if(paths.Length > 0)
        {
            string path = paths[0];
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                CampaignData data = formatter.Deserialize(stream) as CampaignData;

                stream.Close();

                GameManager.Instance.currentCampaign = new Campaign(data);
                GameManager.Instance.Load();
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("No File Found in " + path);
            }
        }
    }

    public void ExportCampaign()
    {
        string path = SFB.StandaloneFileBrowser.SaveFilePanel("Export Campaign", "", GameManager.CurrentCampaign.name, "book");
        if(!string.IsNullOrEmpty(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            CampaignData data = new CampaignData(GameManager.CurrentCampaign);

            formatter.Serialize(stream, data);
            stream.Close();
        }
    }
}

public class NotebookSettings
{
    public List<string> paths = new List<string>();
    public List<string> names = new List<string>();
}
