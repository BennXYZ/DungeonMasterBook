using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterPageBehaviour : MonoBehaviour
{
    public TMP_InputField personality, appearance, skills, information, backstoy, log;

    internal void SetText(List<string> texts)
    {
        personality.text = texts[0];
        appearance.text = texts[1];
        skills.text = texts[2];
        information.text = texts[3];
        backstoy.text = texts[4];
        log.text = texts[5];
    }

    public void PersonalityTextChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[0] = text;
    }
    public void AppearanceTextChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[1] = text;
    }
    public void SkillsTextChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[2] = text;
    }
    public void InformationTextChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[3] = text;
    }
    public void BackstoryTextChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[4] = text;
    }
    public void LogTextChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[5] = text;
    }
}
