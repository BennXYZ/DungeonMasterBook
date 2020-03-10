using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPageBehaviour : MonoBehaviour
{
    public TMP_InputField type, magic, appearance, effects, backstory;

    public void SetText(List<string> texts)
    {
        type.text = texts[0];
        magic.text = texts[1];
        appearance.text = texts[2];
        effects.text = texts[3];
        backstory.text = texts[4];
    }

    public void ChangedType(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[0] = text;
    }
    public void ChangedMagic(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[1] = text;
    }
    public void ChangedAppearance(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[2] = text;
    }
    public void ChangedEffects(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[3] = text;
    }
    public void ChangedBackstory(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[4] = text;
    }
}
