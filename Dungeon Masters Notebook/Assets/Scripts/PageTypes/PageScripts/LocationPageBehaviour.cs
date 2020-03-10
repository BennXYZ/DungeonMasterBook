using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationPageBehaviour : MonoBehaviour
{
    public TMP_InputField type,populance, appearance, log, backstory, information;

    public void SetText(List<string> texts)
    {
        type.text = texts[0];
        populance.text = texts[1];
        appearance.text = texts[2];
        log.text = texts[3];
        backstory.text = texts[4];
        information.text = texts[5];
    }

    public void ChangedType(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[0] = text;
    }
    public void ChangedPopulance(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[1] = text;
    }
    public void ChangedAppearance(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[2] = text;
    }
    public void ChangedLog(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[3] = text;
    }
    public void ChangedBackstory(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[4] = text;
    }
    public void ChangedInformation(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[5] = text;
    }
}
