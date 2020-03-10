using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlankPageBehaviour : MonoBehaviour
{
    public TMP_InputField inputField;

    public void SetText(string text)
    {
        inputField.text = text;
    }

    public void TextChanged(string text)
    {
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[0] = text;
    }
}
