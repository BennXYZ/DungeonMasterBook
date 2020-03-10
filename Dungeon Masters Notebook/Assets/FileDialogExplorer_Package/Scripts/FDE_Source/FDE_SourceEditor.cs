#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(FDE_Source))]
public class FDE_SourceEditor : Editor
{
    FDE_Source fde;

    public Texture MouseIcon;
    public Texture FDE_Logo;

    private bool showChangeNotes = false;

    private List<string> drivers = new List<string>();

    private void OnEnable()
    {
        fde = (FDE_Source)target;

        ActionOBJ_ReadTo3DText = serializedObject.FindProperty("ActionOBJ_ReadTo3DText");
        ActionOBJ_ReadToUIText = serializedObject.FindProperty("ActionOBJ_ReadToUIText");
        ActionOBJ_ReadToSprite = serializedObject.FindProperty("ActionOBJ_ReadToSprite");
        ActionOBJ_ReadToUIImage = serializedObject.FindProperty("ActionOBJ_ReadToUIImage");
        ActionOBJ_ReadToRenderer = serializedObject.FindProperty("ActionOBJ_ReadToRenderer");

        ActionOBJ_ReadToVariableMonoBeh = serializedObject.FindProperty("ActionOBJ_ReadToVariableMonoBeh");
        ActionOBJ_ReadToVariableVar = serializedObject.FindProperty("ActionOBJ_ReadToVariableVar");

        Action_CustomEvent = serializedObject.FindProperty("Action_CustomEvent");

        drivers.Clear();
        foreach (string d in System.IO.Directory.GetLogicalDrives())
        {
            if (System.IO.Directory.Exists(d))
                drivers.Add(d);
        }
        drivers.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
        drivers.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label(FDE_Logo);
        BH();
        if (GUILayout.Button("Documentation"))
            Application.OpenURL("https://docs.google.com/presentation/d/1AEBz5pIbTXIvJsGpspBT1tGYQv1tF5iic65kYndViNA/edit?usp=sharing");
        if (GUILayout.Button("Support"))
            Application.OpenURL("https://matejvanco.com/contact/");
        EH();
        showChangeNotes = EditorGUILayout.Toggle("Show Change Notes", showChangeNotes);
        if(showChangeNotes)
            EditorGUILayout.HelpBox("File Dialog Explorer version 4 [16/01/2020]\n\nChangeLog:\n- Updated API\n- Code Refactoring\n- Added Reading-Types\n- Added History Dialog\n- Added Asynchronous Loading\n- Added Custom Event Trigger\n- Ready for Unity 2020", MessageType.Info);
        GUILayout.Space(10);

        GUILayout.Label("Default Dialog Path");
        DrawProperty("DefaultStartup_ApplicationStartUp", "Application Startup","If enabled, default startup path for Dialog Explorer will be the application's startup path - where is your application located");
        if (!fde.DefaultStartup_ApplicationStartUp)
        {
            DrawProperty("MainPath", "Main Path", "Main default start-up path");
            GUILayout.Label("Available Drivers");
            BH();
            foreach(string driver  in drivers)
            {
                if (GUILayout.Button(driver))
                    fde.MainPath = driver;
            }
            EH();
        }

        GUILayout.Space(10);

        BV();
        BV();
        DrawProperty("EnableDialogAfterStart", "Enable Dialog On Startup", "Show up dialog on startup");
        DrawProperty("KeepDialogAfterAction", "Keep Dialog On Action", "If disabled, dialog will be closed after Action [After Item Click]");
        EV();
        GUILayout.Space(10);
        BV();
        DrawProperty("EnableDataCustomization", "Enable Data Customization", "If enabled, user will be able to use Right Mouse Button to create, edit or copy files/ folders in drives");
        if(fde.EnableDataCustomization)
            DrawProperty("HighProtectionLevel", "High Protection Level", "[Recommended: enabled] If enabled, you won't be able to manipulate with exist files/folders in your computer. But you will be able to manipulate with files/folders created in Dialog Explorer by you");
        EV();
        GUILayout.Space(10);
        BV();
        DrawProperty("EnableHistoryDialog", "Enable History Dialog", "If enabled, user will be able to use history dialog");
        if (fde.EnableHistoryDialog)
        {
            DrawProperty("ShowHistoryDialogOnStart", "Show History Dialog On Start", "If enabled, the History Dialog will be shown on application startup");
            DrawProperty("ShowHistoryFoldersNameOnly", "Show History Folders Name Only", "If enabled, the generated history folders will contain just their name without full path");
        }
        EV();
        GUILayout.Space(10);
        DrawProperty("ShowLoadingPanel", "Show Loading Panel","If enabled, the loading panel with Cancel button will be shown while loading large folders");
        EV();

        GUILayout.Space(10);

        BV();
        DrawProperty("DefaultExtension", "Default Extension", "Default extension while creating a new file WITHOUT dot!");
        DrawProperty("ICON_Files", "Default Files");
        DrawProperty("ICON_Folders", "Default Folders");
        DrawProperty("MaxStoredHistoryFolders", "Max Stored History Folders", "Maximum amount of the recently opened folders");

        GUILayout.Space(5);
        DrawProperty("MaxImageDisplaySize", "Max Image Size [kb]", "When the images are too big, it could take longer to load... Set the size of maximum image file size into dialog[default 1024 kb = 1 mb].Otherwise the image will be replaced by the image below...");
        DrawProperty("ICON_DefaultImageHolder", "Default Image");
        GUILayout.Space(5);
        BV();
        DrawProperty("RegisteredExtensions", "Registered Extensions", "", true);
        EV();
        GUILayout.Space(5);
        DrawProperty("useCustomFont", "Use Custom Font", "Use custom font in the File Dialog");
        if (fde.useCustomFont)
            DrawProperty("customFont");
        EV();

        GUILayout.Space(10);
        BV();
        DrawProperty("FDE_ItemPrefab", "FDE File Prefab", "Prefab of generated files in FDE");
        EV();

        GUILayout.Space(10);
        BV();
        InternalActions();
        EV();
    }

    SerializedProperty ActionOBJ_ReadTo3DText;
    SerializedProperty ActionOBJ_ReadToUIText;
    SerializedProperty ActionOBJ_ReadToSprite;
    SerializedProperty ActionOBJ_ReadToUIImage;
    SerializedProperty ActionOBJ_ReadToRenderer;
    SerializedProperty ActionOBJ_ReadToVariableMonoBeh;
    SerializedProperty ActionOBJ_ReadToVariableVar;
    SerializedProperty Action_CustomEvent;

    private void InternalActions()
    {
        GUILayout.Space(5);
        GUILayout.Label(new GUIContent("Selected Action - " + fde.File_Action.ToString(), MouseIcon));

        DrawProperty("File_Action", "Action Type", "Select an action type that will occur after file click");

        if (fde.File_Action == FDE_Source._FileAction.Open)
            GUILayout.Label("After click - selected file will be opened as file.");
        if (fde.File_Action == FDE_Source._FileAction.OpenInExplorer)
            GUILayout.Label("After click - selected file will open it's directory.");
        if (fde.File_Action == FDE_Source._FileAction.Text_ReadToVariable)
        {
            EditorGUILayout.PropertyField(ActionOBJ_ReadToVariableMonoBeh, new GUIContent("Enter MonoBehaviour Target Script"), true);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(ActionOBJ_ReadToVariableVar, new GUIContent("Enter Variable Name In Target Script"), true);
            serializedObject.ApplyModifiedProperties();
            DrawProperty("ReadType", "Read Type", "Value that will be received from the clicked file");
            GUILayout.Space(5);
            GUILayout.Label("After click - selected file will transfer data to the selected variable value.");
        }
        if (fde.File_Action == FDE_Source._FileAction.Text_ReadTo3DText)
        {
            EditorGUILayout.PropertyField(ActionOBJ_ReadTo3DText, new GUIContent("Enter 3D Text Mesh Object"), true);
            serializedObject.ApplyModifiedProperties();
            DrawProperty("ReadType", "Read Type", "Value that will be received from the clicked file");
            GUILayout.Space(5);
            GUILayout.Label("After click - selected file will transfer data to the 3D text mesh.");
        }
        if (fde.File_Action == FDE_Source._FileAction.Text_ReadToUIText)
        {
            EditorGUILayout.PropertyField(ActionOBJ_ReadToUIText, new GUIContent("Enter UI Text Object"), true);
            serializedObject.ApplyModifiedProperties();
            DrawProperty("ReadType", "Read Type", "Value that will be received from the clicked file");
            GUILayout.Space(5);
            GUILayout.Label("After click - selected file will transfer data to the UI text.");
        }
        if (fde.File_Action == FDE_Source._FileAction.Image_ReadImageToSprite)
        {
            EditorGUILayout.PropertyField(ActionOBJ_ReadToSprite, new GUIContent("Enter Sprite Renderer Object"), true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(5);
            GUILayout.Label("After click - selected file will transfer image data to the Sprite Renderer. \n[Allowed extensions: png, jpg, bmp, gif, tga]");
        }
        if (fde.File_Action == FDE_Source._FileAction.Image_ReadImageToUIImage)
        {
            EditorGUILayout.PropertyField(ActionOBJ_ReadToUIImage, new GUIContent("Enter UI Image Object"), true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(5);
            GUILayout.Label("After click - selected file will transfer image data to the UI image. \n[Allowed extensions: png, jpg, bmp, gif, tga]");
        }
        if (fde.File_Action == FDE_Source._FileAction.Image_ReadImageToRenderer)
        {
            EditorGUILayout.PropertyField(ActionOBJ_ReadToRenderer, new GUIContent("Enter Mesh Renderer Object"), true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(5);
            GUILayout.Label("After click - selected file will transfer image data to the Mesh Renderer. \n[Allowed extensions: png, jpg, bmp, gif, tga]");
        }
        if (fde.File_Action == FDE_Source._FileAction.CustomEvent)
        {
            EditorGUILayout.PropertyField(Action_CustomEvent, new GUIContent("Enter Custom Event"), true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(5);
            GUILayout.Label("After click - the custom event will be rendered immediately.");
        }
    }


    private void DrawProperty(string p, string Text_ = "", string ToolTip = "", bool includeChilds = false)
    {
        if (string.IsNullOrEmpty(Text_))
            Text_ = p;
        EditorGUILayout.PropertyField(serializedObject.FindProperty(p), new GUIContent(Text_, ToolTip), includeChilds, null);
        serializedObject.ApplyModifiedProperties();
    }

    private void BV()
    {
        GUILayout.BeginVertical("Box");
    }
    private void EV()
    {
        GUILayout.EndVertical();
    }
    private void BH()
    {
        GUILayout.BeginHorizontal("Box");
    }
    private void EH()
    {
        GUILayout.EndHorizontal();
    }
}
#endif