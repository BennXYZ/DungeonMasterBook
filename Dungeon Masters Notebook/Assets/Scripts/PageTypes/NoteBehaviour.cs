using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NoteBehaviour : MonoBehaviour
{
    public string title;

    public string text;

    public TMPro.TMP_Text titleObject;

    public int id;

    public UnityEvent onSelectNoteEvent = new UnityEvent();

    public UnityEvent OnRemoveNoteEvent = new UnityEvent();

    public void SelectNote()
    {
        onSelectNoteEvent.Invoke();
    }

    public void RemoveNote()
    {
        OnRemoveNoteEvent.Invoke();
    }
}
