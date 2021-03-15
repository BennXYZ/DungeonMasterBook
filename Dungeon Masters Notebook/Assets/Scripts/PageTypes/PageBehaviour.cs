using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageBehaviour : MonoBehaviour
{
    [SerializeField]
    List<TMP_InputField> inputFields;

    public virtual void Awake()
    {
        for (int i = 0; i < inputFields.Count; i++)
        {
            int thisId = i;
            inputFields[i].onEndEdit.AddListener(newText => { TextChanged(newText, thisId); });
        }
    }

    public virtual void TextChanged(string newText, int textId)
    {
        if(GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Count <= textId)
        {
            Debug.LogWarning("You are trying to write into a text that doesn't exist. Are you sure the Page has enough texts for this?");
        }
        if(textId >= GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Count)
        {
            GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Add(newText);
        }
        else
        {
            GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[textId] = newText;
        }
    }

    public virtual void RemoveText(int textId)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.RemoveAt(textId);
    }

    public virtual void SetText(List<string> texts)
    {
        for (int i = 0; i < inputFields.Count; i++)
        {
            if(i < texts.Count)
            {
                inputFields[i].text = texts[i]; //Apply Saved Texts to inpuFields.
            }
        }
    }
}