using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlankPageBehaviour : PageBehaviour
{
    [SerializeField]
    TMP_InputField noteHeader;

    [SerializeField]
    TMP_InputField noteBody;

    List<NoteBehaviour> currentNotes = new List<NoteBehaviour>();

    public NoteBehaviour notePrefab;

    public Transform notesParent;

    int currentNoteId = 0;

    public override void SetText(List<string> texts)
    {
        base.SetText(texts);
        ClearNotes();
        for (int i = 1; i < texts.Count; i= i + 2)
        {
            if(i + 1 >= texts.Count)
            {
                Debug.LogWarning("[BlankPageBehaviour] Not enough texts for header and content");
            }
            else
                AddNote(texts[i], texts[i + 1]);
        }
        if (currentNotes.Count > 0)
            OpenNote(currentNotes[0]);
        UpdateNoteFieldInteractable();
    }

    public override void Awake()
    {
        //base.Awake();
    }

    public void OpenNote(int i)
    {
        if(currentNotes.Count > i && i >= 0)
        {
            currentNoteId = i;
            noteHeader.text = currentNotes[i].title;
            noteBody.text = currentNotes[i].text;
        }
        else
        {
            currentNoteId = currentNotes.Count > 0 ? currentNotes[currentNotes.Count - 1].id : -1;
            noteHeader.text = currentNotes.Count > 0 ? currentNotes[currentNotes.Count - 1].title : "";
            noteBody.text = currentNotes.Count > 0 ? currentNotes[currentNotes.Count - 1].text : "";
        }
    }

    public void OpenNote(NoteBehaviour note)
    {
        if(note != null)
        {
            currentNoteId = note.id;
            noteHeader.text = note.title;
            noteBody.text = note.text;
        }
        else
        {
            currentNoteId = -1;
            noteHeader.text = "";
            noteBody.text = "";
        }
    }

    public void RemoveNote(int index)
    {
        for (int i = index + 1; i < currentNotes.Count; i++)
        {
            currentNotes[i].id--;
        }
        if(currentNotes.Count > index)
        {
            GameObject.Destroy(currentNotes[index].gameObject);
            currentNotes.RemoveAt(index);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Tried Deleting a Note but List to short");
        }
        RemoveText(index * 2 + 1);
        RemoveText(index * 2 + 1);
        if (currentNoteId == index)
            OpenNote(index);
        UpdateNoteFieldInteractable();
    }

    public void AddNote()
    {
        OpenNote(AddNote("New Note", ""));
    }

    public NoteBehaviour AddNote(string title, string text)
    {
        NoteBehaviour newItem = Instantiate(notePrefab, notesParent);
        newItem.id = currentNotes.Count;
        newItem.title = title;
        newItem.titleObject.text = title;
        newItem.onSelectNoteEvent.AddListener(delegate { OpenNote(newItem.id); });
        newItem.OnRemoveNoteEvent.AddListener(delegate { RemoveNote(newItem.id); });
        currentNotes.Add(newItem);
        EditNoteTitle(title, newItem.id);
        EditNoteText(text, newItem.id);
        UpdateNoteFieldInteractable();
        return newItem;
    }

    void UpdateNoteFieldInteractable()
    {
        noteBody.interactable = currentNotes.Count > 0;
        noteHeader.interactable = currentNotes.Count > 0;
        if(currentNotes.Count <= 0)
        {
            noteBody.text = "Please add a <b>Note</b> to edit this text";
            noteHeader.text = "No Notes";
        }
    }

    public void EditMainText(string newText)
    {
        TextChanged(newText, 0);
    }

    public void EditNoteTitle(string newText)
    {
        EditNoteTitle(newText, currentNoteId);
    }

    public void EditNoteTitle(string newText, int tartedNoteId)
    {
        TextChanged(newText, tartedNoteId * 2 + 1);
        if (currentNotes.Count > tartedNoteId)
        {
            currentNotes[tartedNoteId].titleObject.text = newText;
            currentNotes[tartedNoteId].title = newText;
        }
        else
            Debug.LogWarning("Tried to edit Title of non existing Note");
    }

    public void EditNoteText(string newText)
    {
        EditNoteText(newText, currentNoteId);
    }

    public void EditNoteText(string newText, int tartedNoteId)
    {
        TextChanged(newText, tartedNoteId * 2 + 2);
        if (currentNotes.Count > tartedNoteId)
        {
            currentNotes[tartedNoteId].gameObject.GetComponent<NoteBehaviour>().text = newText;
        }
        else
            Debug.LogWarning("Tried to edit Title of non existing Note");
    }

    void ClearNotes()
    {
        foreach(Transform child in notesParent)
        {
            GameObject.Destroy(child.gameObject);
        }
        currentNotes.Clear();
        noteBody.text = string.Empty;
        noteHeader.text = string.Empty;
    }
}