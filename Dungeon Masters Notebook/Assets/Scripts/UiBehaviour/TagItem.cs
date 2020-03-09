using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TagItem : MonoBehaviour
{
    public TMP_Text nameDisplay;

    public TMP_InputField nameInput;

    public string memorizedName;

    public bool toggleValue;

    public Toggle toggle;

    [HideInInspector]
    public UnityEvent onToggle;
    [HideInInspector]
    public UnityEvent onNameEdit;
    [HideInInspector]
    public UnityEvent onRemove;

    public TagItem()
    {
        onRemove = new UnityEvent();
        onNameEdit = new UnityEvent();
        onToggle = new UnityEvent();
    }

    public void Toggle(bool val)
    {
        toggleValue = val;
        onToggle.Invoke();
    }

    public void RemoveTag()
    {
        onRemove.Invoke();
    }

    public void MemorizeName()
    {
        memorizedName = nameInput.text;
    }

    public void RenameTag(string newName)
    {
        for (int i = 0; i < GameManager.CurrentCampaign.tags.Count; i++)
        {
            if(GameManager.CurrentCampaign.tags[i].name == memorizedName)
            {
                GameManager.CurrentCampaign.tags[i].name = newName;
                break;
            }
        }
        onNameEdit.Invoke();
    }
}
