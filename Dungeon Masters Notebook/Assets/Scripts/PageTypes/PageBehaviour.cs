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
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[textId] = newText;
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

        if (texts.Count > inputFields.Count) //There are more Texts in Page than in Behaviour
        {
            Debug.LogWarning("There are more Text-Fields than Texts in the Page. Are you sure the Page has enough texts for this?");
        }
        else if(texts.Count < inputFields.Count) //There are more Inputfields than Page-Texts
        {
            Debug.LogWarning("There are more Texts than Text-Fields in the Page. Are you sure the Behaviour has enough Text-Fields for this?");
        }
    }
}