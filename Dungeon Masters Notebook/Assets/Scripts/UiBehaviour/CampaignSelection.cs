﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class CampaignSelection : MonoBehaviour
{
    const string campaignsCountGetter = "CAMPAIGNS_COUNT";
    const string campaignNamePrefix = "CAMPAIGN_NAME_";

    public Transform loadingItemsParent;

    public GameObject loadingItemPrefab;

    public TMP_InputField inputField;

    List<CampaignLoadingItem> items;

    private void Awake()
    {
        items = new List<CampaignLoadingItem>();
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
        int numberOfCampaigns = PlayerPrefs.GetInt(campaignsCountGetter, 0);
        for (int i = 0; i < numberOfCampaigns; i++)
        {
            CampaignLoadingItem newItem = Instantiate(loadingItemPrefab, loadingItemsParent).GetComponent<CampaignLoadingItem>();
            newItem.Init(PlayerPrefs.GetString(campaignNamePrefix + i, "Not Found"));
            newItem.onPress.AddListener(delegate { OpenCampaign(newItem); });
            newItem.onRemove.AddListener(delegate { RemoveCampaign(newItem); });
            items.Add(newItem);
        }
    }

    public void CreateNewCampaign()
    {
        if(inputField.text != "")
        {
            GameManager.CreateNewCampaign(inputField.text);
            PlayerPrefs.SetString(campaignNamePrefix + PlayerPrefs.GetInt(campaignsCountGetter, 0), inputField.text);
            PlayerPrefs.SetInt(campaignsCountGetter, PlayerPrefs.GetInt(campaignsCountGetter, 0) + 1);
            gameObject.SetActive(false);
        }
    }

    public void RemoveCampaign(CampaignLoadingItem item)
    {
        int index = items.IndexOf(item);
        int numberOfCampaigns = PlayerPrefs.GetInt(campaignsCountGetter, 0);
        for (int i = index; i < numberOfCampaigns - 1; i++)
        {
            PlayerPrefs.SetString(campaignNamePrefix + i, PlayerPrefs.GetString(campaignNamePrefix + (i + 1)));
        }
        PlayerPrefs.SetInt(campaignsCountGetter, PlayerPrefs.GetInt(campaignsCountGetter, 0) - 1);
        Destroy(item.gameObject);
        items.Remove(item);
    }

    public void OpenCampaign(CampaignLoadingItem item)
    {
        GameManager.OpenCampaign(item.title.text);
        gameObject.SetActive(false);
    }
}