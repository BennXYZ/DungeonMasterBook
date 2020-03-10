using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroupPageBehaviour : MonoBehaviour
{
    public TMP_InputField hierarchy, goals, information, backstory, log;

    public void SetText(List<string> texts)
    {
        hierarchy.text = texts[0];
        goals.text = texts[1];
        information.text = texts[2];
        backstory.text = texts[3];
        log.text = texts[4];
    }

    public void ChangedHierarchy(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[0] = text;
    }

    public void ChangedGoals(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[1] = text;
    }

    public void InformationChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[2] = text;
    }

    public void BackstoryChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[3] = text;
    }

    public void LogChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[4] = text;
    }
}
