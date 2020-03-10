using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestPageBehaviour : MonoBehaviour
{
    public TMP_InputField giver,rewards, information, backstory, log;

    public void SetText(List<string> texts)
    {
        giver.text = texts[0];
        rewards.text = texts[1];
        information.text = texts[2];
        backstory.text = texts[3];
        log.text = texts[4];
    }

    public void ChangedQuestGiver(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[0] = text;
    }
    public void ChangedRewards(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[1] = text;
    }
    public void ChangedInformation(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[2] = text;
    }
    public void ChangedBackstory(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[3] = text;
    }
    public void ChangedLog(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[4] = text;
    }
}
