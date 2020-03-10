using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.IO;
using System.Diagnostics;

//---------------File Dialog Explorer Written by Matej Vanco 8/8/2017 - d/m/yyyy
//---------------Last Update 16.01.2020

[AddComponentMenu("Matej Vanco/File Dialog Explorer")]
[System.Serializable]
public class FDE_Source : MonoBehaviour {

    public bool DefaultStartup_ApplicationStartUp = true;
    public string MainPath = "C:/";

    public bool EnableDialogAfterStart = false;
    public bool KeepDialogAfterAction = false;
    public bool EnableDataCustomization = true;
    public bool HighProtectionLevel = true;

    public bool EnableHistoryDialog = true;
    public bool ShowHistoryDialogOnStart = true;
    public bool ShowHistoryFoldersNameOnly = true;
    public int MaxStoredHistoryFolders = 25;

    public bool ShowLoadingPanel = true;

    public enum _FileAction : int 
    {
        Open = 0, 
        OpenInExplorer = 1, 
        Text_ReadToVariable = 2, 
        Text_ReadTo3DText = 3, 
        Text_ReadToUIText = 4, 
        Image_ReadImageToSprite = 5, 
        Image_ReadImageToUIImage = 6, 
        Image_ReadImageToRenderer = 7,
        CustomEvent = 8
    };

    public enum _ReadType : int
    {
        ReadFileContent,
        ReadFileName,
        ReadFileNameWithoutExtension,
        ReadFileExtensionOnly,
        ReadFullFilePath,
        ReadFullFilePathWithoutFileName,
        ReadFileSizeInBytes,
        ReadFileSizeInKilobytes,
        ReadFileSizeInMegabytes
    };

    public Sprite ICON_Files;
    public Sprite ICON_Folders;
    public int MaxImageDisplaySize = 1024;
    public Sprite ICON_DefaultImageHolder;
    public string DefaultExtension = "txt";

    public bool useCustomFont = false;
    public Font customFont;

    [System.Serializable]
    public class _RegisteredExtensions
    {
        [Header("Extension Name")]
        [Tooltip("Write extension WITHOUT dot (txt, exe, png, bmp etc)")]
        public string Extension = "txt";
        [Header("Extension Icon")]
        public Sprite Icon;
    }

    public _RegisteredExtensions[] RegisteredExtensions;

    private GameObject FDE_SourceObject;
    public GameObject FDE_ItemPrefab;

    public _FileAction File_Action = _FileAction.Open;
    public _ReadType ReadType = _ReadType.ReadFileContent;

    protected List<string> Disallowed_Folders = new List<string> { "$recycle.bin", "system volume information", "documents and settings", "recovery", "hiberfil", "pagefile" };

    //----------Script content - UI requirements
    #region UI Content
    private InputField UI_FullPath;
    private Button UI_BackButton;
    private RectTransform UI_DialogContent;
    private Text UI_Info;
    private Slider UI_DialogSize;
    private Text UI_DialogSizeInfo;
    private Dropdown UI_Drivers;

    private GameObject UI_LoadingPanel;
    private Slider UI_LoadingPanel_Progress;
    private Text UI_LoadingPanel_ProgressText;
    private Button UI_LoadingPanel_Cancel;

    private GameObject UI_ScrollDialog;
    private Text UI_DialogInfo;
    private Button UI_ScrollDialog_Copy;
    private Button UI_ScrollDialog_Paste;
    private Button UI_ScrollDialog_Duplicate;
    private Button UI_ScrollDialog_Delete;
    private Button UI_ScrollDialog_CreateFile;
    private Button UI_ScrollDialog_CreateFolder;
    private Button UI_ScrollDialog_Delete2;
    private InputField UI_ScrollDialog_CreatingSomethingInputField;
    private Button UI_ScrollDialog_AcceptInputField;
    private Button UI_ScrollDialog_Rename;
    private Button UI_ScrollDialog_Rename2;
    private GameObject UI_ScrollDialog_ProtectionQuestion;

    private Transform UI_HistoryDialog;
    private Transform UI_History_Content;
    private Transform UI_History_ItemPrefab;
    private Transform UI_History_OpenHistory;

    private bool UIPassed = false;
    #endregion

    //----------Internal Functions
    #region Internal Variables
    private List<string> ListOfPathes = new List<string>();
    private List<string> ListOfCreatedFileFolders = new List<string>();
    private string SelectedPath;
    private string PathToPaste;
    #endregion

    //----------Action Requirements
    #region Required Actions
    [HideInInspector]
    public TextMesh ActionOBJ_ReadTo3DText;
    [HideInInspector]
    public Text ActionOBJ_ReadToUIText;
    [HideInInspector]
    public SpriteRenderer ActionOBJ_ReadToSprite;
    [HideInInspector]
    public Image ActionOBJ_ReadToUIImage;
    [HideInInspector]
    public Renderer ActionOBJ_ReadToRenderer;

    [HideInInspector]
    public MonoBehaviour ActionOBJ_ReadToVariableMonoBeh;
    [HideInInspector]
    public string ActionOBJ_ReadToVariableVar;

    [HideInInspector]
    public UnityEvent Action_CustomEvent;
    #endregion

    //----------Start - Setting up dialog
    void Awake () 
    {
        FDE_SourceObject = this.gameObject;
        if (!FDE_ItemPrefab)
        {
            UnityEngine.Debug.LogError("File Dialog Explorer - Error - FDE_ItemPrefab is missing. FDE has been deleted.");
            DestroyImmediate(this);
        }

        UIPassed = A_Internal_GetUI();
        if (!UIPassed)
        {
            UnityEngine.Debug.LogError("File Dialog Explorer - Error - FDE_SourceObject doesn't exist or the content is missing. FDE has been deleted.");
            DestroyImmediate(this);
        }

        A_Internal_InitializeFunctions();

        if (MaxStoredHistoryFolders > 100) //---You can change the value, but this is recommended due to the performance drop
            MaxStoredHistoryFolders = 100;

        if (DefaultStartup_ApplicationStartUp)
            MainPath = Application.dataPath;
        ListOfPathes.Add(MainPath);
        A_Internal_RefreshContent();

        if (!EnableDialogAfterStart)
            Action_CLOSE_DIALOG();
	}

    //----------Public functions
    #region Actions - Public
    /// <summary>
    /// Show dialog panel with starter path (leave it empty if the starter path is already defined)
    /// </summary>
    public void Action_SHOW_DIALOG(string StarterPath = "")
    {
        if (!string.IsNullOrEmpty(StarterPath))
            MainPath = StarterPath;

        FDE_SourceObject.SetActive(true);
        A_Internal_RefreshContent();
    }
    /// <summary>
    /// Close dialog panel
    /// </summary>
    public void Action_CLOSE_DIALOG()
    {
        FDE_SourceObject.SetActive(false);
    }

    /// <summary>
    /// Change click file action by enum (Read to text? Read to renderer?...)
    /// </summary>
    public void Action_ChangeClickFileAction(_FileAction Action)
    {
        File_Action = Action;
    }
    /// <summary>
    /// Change click file action by index (Read to text? Read to renderer?...)
    /// </summary>
    public void Action_ChangeClickFileAction(int ActionIndex)
    {
        File_Action = (_FileAction)ActionIndex;
    }

    /// <summary>
    /// Change read file type by enum (Read file content? Read just file name?...)
    /// </summary>
    public void Action_ChangeReadFileAction(_ReadType readType)
    {
        ReadType = readType;
    }
    /// <summary>
    /// Change read file type by index (Read file content? Read just file name?...)
    /// </summary>
    public void Action_ChangeReadFileAction(int readIndex)
    {
        ReadType = (_ReadType)readIndex;
    }
    #endregion


    //----------Movement of the dialog
    #region Dialog Drag Drop
    float XOff = 0;
    float YOff = 0;
    private void A_Internal_UI_BeginDrag()
    {
        XOff = FDE_SourceObject.transform.position.x - Input.mousePosition.x;
        YOff = FDE_SourceObject.transform.position.y - Input.mousePosition.y;
    }
    private void A_Internal_UI_Drag()
    {
        FDE_SourceObject.transform.position = new Vector3(XOff+Input.mousePosition.x,YOff+Input.mousePosition.y,0);
    }
    #endregion

    //----------Correction of Dialog UI & Functionality
    #region Internal - Set Up Dialog Content
    private bool A_Internal_GetUI()
    {
        try
        {
            UI_DialogInfo = FDE_SourceObject.transform.Find("DialogInfo").GetComponent<Text>();

            UI_FullPath = FDE_SourceObject.transform.Find("FullPath").GetComponent<InputField>();
            UI_BackButton = FDE_SourceObject.transform.Find("BackButton").GetComponent<Button>();
            UI_DialogContent = FDE_SourceObject.transform.Find("PCContent").transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
            UI_Info = FDE_SourceObject.transform.Find("FileInfo").GetComponent<Text>();
            UI_DialogSize = FDE_SourceObject.transform.Find("FilesSize").GetComponent<Slider>();
            UI_DialogSizeInfo = FDE_SourceObject.transform.Find("FilesSize").transform.Find("Text").GetComponent<Text>();
            UI_Drivers = FDE_SourceObject.transform.Find("Drivers").GetComponent<Dropdown>();

            UI_ScrollDialog = FDE_SourceObject.transform.Find("ScrollDialog").gameObject;
            UI_ScrollDialog_Copy = UI_ScrollDialog.transform.Find("File").transform.Find("Copy").GetComponent<Button>();
            UI_ScrollDialog_Paste = UI_ScrollDialog.transform.Find("Folder").transform.Find("Paste").GetComponent<Button>();
            UI_ScrollDialog_Duplicate = UI_ScrollDialog.transform.Find("File").transform.Find("Duplicate").GetComponent<Button>();
            UI_ScrollDialog_Delete = UI_ScrollDialog.transform.Find("File").transform.Find("Delete").GetComponent<Button>();

            UI_ScrollDialog_CreateFile = UI_ScrollDialog.transform.Find("Folder").transform.Find("Create File").GetComponent<Button>();
            UI_ScrollDialog_CreateFolder = UI_ScrollDialog.transform.Find("Folder").transform.Find("Create Folder").GetComponent<Button>();
            UI_ScrollDialog_Delete2 = UI_ScrollDialog.transform.Find("Folder").transform.Find("Delete").GetComponent<Button>();

            UI_ScrollDialog_CreatingSomethingInputField = UI_ScrollDialog.transform.Find("Input").GetComponent<InputField>();
            UI_ScrollDialog_AcceptInputField = UI_ScrollDialog_CreatingSomethingInputField.transform.Find("Accept").GetComponent<Button>();

            UI_ScrollDialog_Rename = UI_ScrollDialog.transform.Find("File").transform.Find("Rename").GetComponent<Button>();
            UI_ScrollDialog_Rename2 = UI_ScrollDialog.transform.Find("Folder").transform.Find("Rename").GetComponent<Button>();

            UI_HistoryDialog = FDE_SourceObject.transform.Find("HistoryPanel");
            UI_History_Content = FDE_SourceObject.transform.Find("HistoryPanel").Find("Scroll View").transform.GetChild(0).GetChild(0);
            UI_History_ItemPrefab = FDE_SourceObject.transform.Find("HistoryPanel").Find("PrefabButton");
            UI_History_ItemPrefab.gameObject.SetActive(false);
            UI_History_OpenHistory = FDE_SourceObject.transform.Find("OpenHistory");
            UI_HistoryDialog.gameObject.SetActive(false);
            if (!EnableHistoryDialog)
                UI_History_OpenHistory.gameObject.SetActive(false);
            else if (ShowHistoryDialogOnStart)
                UI_HistoryDialog.gameObject.SetActive(true);

            UI_LoadingPanel = FDE_SourceObject.transform.Find("LoadingPanel").gameObject;
            UI_LoadingPanel_Progress = UI_LoadingPanel.transform.Find("LoadingValue").GetComponent<Slider>();
            UI_LoadingPanel_ProgressText = UI_LoadingPanel.transform.Find("Text").GetComponent<Text>();
            UI_LoadingPanel_Cancel = UI_LoadingPanel.transform.Find("Cancel").GetComponent<Button>();
            UI_LoadingPanel.SetActive(false);

            UI_ScrollDialog_ProtectionQuestion = UI_ScrollDialog.transform.Find("ProtectionQuestion").gameObject;

            UI_ScrollDialog_ProtectionQuestion.SetActive(false);
            UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(false);

            UI_ScrollDialog.SetActive(false);

            if(useCustomFont && customFont)
            {
                foreach (Text t in GetComponentsInChildren<Text>(true))
                    t.font = customFont;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    private void A_Internal_InitializeFunctions()
    {
        UI_FullPath.onEndEdit.AddListener(delegate
        {
            A_Internal_SearchByAddress();
        });

        UI_BackButton.onClick.AddListener(delegate
        {
            A_Internal_Back();
        });

        UI_DialogSize.onValueChanged.AddListener(delegate
        {
            A_Internal_ChangeSize();
        });

        UI_Drivers.ClearOptions();

        foreach (string d in Directory.GetLogicalDrives())
        {
            if (Directory.Exists(d))
                UI_Drivers.options.Add(new Dropdown.OptionData() { text = d });
        }

        UI_Drivers.onValueChanged.AddListener(delegate
        {
            A_Internal_ChangeDriver();
        });
        UI_Drivers.captionText.text = UI_Drivers.options[0].text;
        UI_Drivers.value = 0;

        if (string.IsNullOrEmpty(MainPath) || !Directory.Exists(MainPath))
            MainPath = UI_Drivers.captionText.text;

        EventTrigger.Entry e = new EventTrigger.Entry();
        e.eventID = EventTriggerType.PointerClick;
        e.callback.AddListener(delegate
        {
            if (EnableDataCustomization)
                A_Internal_RefreshProtectionList();
            A_Internal_InitializeScrollDia("");
        });
        EventTrigger.Entry e2 = new EventTrigger.Entry();
        e2.eventID = EventTriggerType.Drag;
        e2.callback.AddListener(delegate
        {
            A_Internal_UI_Drag();
        });
        EventTrigger.Entry e3 = new EventTrigger.Entry();
        e3.eventID = EventTriggerType.BeginDrag;
        e3.callback.AddListener(delegate
        {
            A_Internal_UI_BeginDrag();
        });
        FDE_SourceObject.transform.Find("PCContent").GetComponent<EventTrigger>().triggers.Add(e);
        FDE_SourceObject.transform.Find("PCContent").GetComponent<EventTrigger>().triggers.Add(e2);
        FDE_SourceObject.transform.Find("PCContent").GetComponent<EventTrigger>().triggers.Add(e3);

        UI_ScrollDialog_Copy.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(0);
        });
        UI_ScrollDialog_Paste.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(1);
        });
        UI_ScrollDialog_Duplicate.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(2);
        });
        UI_ScrollDialog_Delete.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(3);
        });



        UI_ScrollDialog_CreateFile.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(4);
        });
        UI_ScrollDialog_CreateFolder.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(5);
        });
        UI_ScrollDialog_Delete2.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(3);
        });
        UI_ScrollDialog_Rename.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(6);
        });
        UI_ScrollDialog_Rename2.transform.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_ScrollButton(7);
        });


        UI_ScrollDialog_AcceptInputField.GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_EnterInputField();
        });


        UI_ScrollDialog_ProtectionQuestion.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate
        {
            A_Internal_EnterInputField();
        });
        UI_ScrollDialog_ProtectionQuestion.transform.Find("No").GetComponent<Button>().onClick.AddListener(delegate
        {
            UI_ScrollDialog_ProtectionQuestion.SetActive(false);
        });


        UI_LoadingPanel_Cancel.onClick.AddListener(delegate
        {
            StopAllCoroutines();
            UI_LoadingPanel.SetActive(false);
        });

    }
    #endregion

    //----------DIALOG Content Refresher [Back, Add file etc]
    #region Dialog Content Refresher
    private void A_Internal_RefreshContent()
    {
        if (File_Action == _FileAction.Open || File_Action == _FileAction.OpenInExplorer)
            UI_DialogInfo.text = "Browse Files & Folders";
        else if (File_Action == _FileAction.Image_ReadImageToRenderer || File_Action == _FileAction.Image_ReadImageToSprite || File_Action == _FileAction.Image_ReadImageToUIImage)
            UI_DialogInfo.text = "Select Image File";
        else if (File_Action == _FileAction.Text_ReadTo3DText || File_Action == _FileAction.Text_ReadToUIText || File_Action == _FileAction.Text_ReadToVariable)
            UI_DialogInfo.text = "Select Text File";

        bool ImFile = A_Internal_GetFileType_ImFile(MainPath);

        UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(false);

        if (ImFile)
        {
            if (!File.Exists(MainPath))
                FDE_Error("File doesn't exist");
        }
        else
        {
            if (!Directory.Exists(MainPath))
                FDE_Error("Directory doesn't exist");
        }

        UI_FullPath.text = MainPath;

        //---------------------------Generating files & folders
        StopAllCoroutines();
        StartCoroutine(A_Internal_RefreshAsyncContent());
       
        if(EnableHistoryDialog)
        {
            if (UI_History_Content.transform.childCount > 0)
                for (int i = UI_History_Content.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(UI_History_Content.transform.GetChild(i).gameObject);
                }

            int c = 0;
            List<string> copy = new List<string>(ListOfPathes);
            foreach(string s in copy)
            {
                if(Directory.Exists(s) == false)
                {
                    ListOfPathes.RemoveAt(c);
                    continue;
                }
                GameObject newButton = Instantiate(UI_History_ItemPrefab.gameObject, UI_History_Content);
                newButton.gameObject.SetActive(true);
                newButton.name = c.ToString();
                if(!ShowHistoryFoldersNameOnly)
                    newButton.GetComponentInChildren<Text>().text = s;
                else
                    newButton.GetComponentInChildren<Text>().text = string.IsNullOrEmpty(Path.GetFileName(s)) ? s : Path.GetFileName(s);
                newButton.GetComponent<Button>().onClick.AddListener(delegate { A_Internal_Back(int.Parse(newButton.name, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture)); });
                c++;
            }
        }

        UI_DialogContent.transform.localScale = Vector3.one;
        UI_DialogContent.transform.localRotation = Quaternion.identity;
        UI_DialogContent.transform.localPosition = Vector3.zero;

        return;
    }

    private IEnumerator A_Internal_RefreshAsyncContent()
    {
        if (ShowLoadingPanel)
            UI_LoadingPanel.SetActive(true);

        if (UI_DialogContent.transform.childCount > 0)
        {
            UI_LoadingPanel_Progress.minValue = 0;
            UI_LoadingPanel_Progress.maxValue = UI_DialogContent.transform.childCount;
            for (int i = UI_DialogContent.transform.childCount - 1; i >= 0; i--)
            {
                UI_LoadingPanel_Progress.value = i;
                UI_LoadingPanel_ProgressText.text = "Refreshing " + i.ToString() + "/" + UI_LoadingPanel_Progress.maxValue.ToString();
                Destroy(UI_DialogContent.transform.GetChild(i).gameObject);
                yield return null;
            }
        }

        UI_LoadingPanel_Progress.minValue = 0;
        UI_LoadingPanel_Progress.maxValue = Directory.GetDirectories(MainPath).Length;

        //-----------------------------------------------------------------Generating Folders
        for (int i = 0; i < Directory.GetDirectories(MainPath).Length; i++)
        {
            UI_LoadingPanel_Progress.value = i;
            UI_LoadingPanel_ProgressText.text = "Loading Folders " + i.ToString() + "/" + Directory.GetDirectories(MainPath).Length.ToString();
            string currentPath = Directory.GetDirectories(MainPath)[i];
            try
            {
                if (Disallowed_Folders.Contains(Path.GetFileName(currentPath).ToLower()))
                    continue;
                try
                {
                    string testAccess = (Directory.GetDirectories(currentPath).Length).ToString();
                }
                catch { continue; }

                GameObject newDirectory = Instantiate(FDE_ItemPrefab, UI_DialogContent.transform);
                newDirectory.transform.Find("Icon").GetComponent<Image>().sprite = ICON_Folders;
                newDirectory.transform.Find("Name").GetComponent<Text>().text = Path.GetFileName(currentPath);
                if (useCustomFont && customFont)
                    newDirectory.transform.Find("Name").GetComponent<Text>().font = customFont;
                newDirectory.transform.Find("Path").GetComponent<Text>().text = currentPath;
                newDirectory.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    A_ProcessFile(newDirectory.transform.Find("Path").GetComponent<Text>().text);
                });

                FDE_CustomEventTrigger cE = newDirectory.GetComponentInChildren<Button>().gameObject.AddComponent<FDE_CustomEventTrigger>();
                cE.OnEnter = delegate
                {
                    A_GetInfo(newDirectory.transform.Find("Path").GetComponent<Text>().text);
                };
                cE.OnExit = delegate
                {
                    UI_Info.enabled = false;
                };
                cE.OnClick = delegate
                {
                    A_Internal_InitializeScrollDia(newDirectory.transform.Find("Path").GetComponent<Text>().text);
                };
            }
            catch { continue;  }
            yield return null;
        }

        UI_LoadingPanel_Progress.minValue = 0;
        UI_LoadingPanel_Progress.maxValue = Directory.GetFiles(MainPath).Length;

        //-----------------------------------------------------------------Generating Files
        for (int i = 0; i < Directory.GetFiles(MainPath).Length; i++)
        {
            UI_LoadingPanel_Progress.value = i;
            UI_LoadingPanel_ProgressText.text = "Loading Files " + i.ToString() + "/" + Directory.GetFiles(MainPath).Length.ToString();

            GameObject newFile = Instantiate(FDE_ItemPrefab, UI_DialogContent.transform);
            string currentPath = Directory.GetFiles(MainPath)[i];

            bool GotIcon = false;
            if (RegisteredExtensions.Length > 0)
            {
                foreach (_RegisteredExtensions regExts in RegisteredExtensions)
                {
                    if ("." + regExts.Extension == Path.GetExtension(currentPath))
                    {
                        GotIcon = true;
                        newFile.transform.Find("Icon").GetComponent<Image>().sprite = regExts.Icon;
                        break;
                    }
                }
            }

            if (!GotIcon)
            {
                string ext = Path.GetExtension(currentPath);
                if (ext == ".png" || ext == ".jpg")
                {
                    Image img = newFile.transform.Find("Icon").GetComponent<Image>();
                    Texture2D txt = new Texture2D(100, 100);
                    FileInfo f = new FileInfo(currentPath);
                    long size_ = f.Length;
                    float calculatedSize = MaxImageDisplaySize * 1024;
                    if (size_ <= calculatedSize)
                    {
                        byte[] b = File.ReadAllBytes(currentPath);
                        txt.LoadImage(b);
                        txt.Apply();
                        Sprite sp = Sprite.Create(txt, new Rect(Vector2.zero, new Vector2(txt.width, txt.height)), Vector2.zero);
                        newFile.transform.Find("Icon").GetComponent<Image>().sprite = sp;
                    }
                    else if (ICON_DefaultImageHolder != null)
                        newFile.transform.Find("Icon").GetComponent<Image>().sprite = ICON_DefaultImageHolder;
                }
                else
                    newFile.transform.Find("Icon").GetComponent<Image>().sprite = ICON_Files;
            }
            newFile.transform.Find("Name").GetComponent<Text>().text = Path.GetFileName(currentPath);
            if (useCustomFont && customFont)
                newFile.transform.Find("Name").GetComponent<Text>().font = customFont;
            newFile.transform.Find("Path").GetComponent<Text>().text = currentPath;
            newFile.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                A_ProcessFile(newFile.transform.Find("Path").GetComponent<Text>().text);
            });

            FDE_CustomEventTrigger cE = newFile.GetComponentInChildren<Button>().gameObject.AddComponent<FDE_CustomEventTrigger>();
            cE.OnEnter = delegate
            {
                A_GetInfo(newFile.transform.Find("Path").GetComponent<Text>().text);
            };
            cE.OnExit = delegate
            {
                UI_Info.enabled = false;
            };
            cE.OnClick = delegate
            {
                A_Internal_InitializeScrollDia(newFile.transform.Find("Path").GetComponent<Text>().text);
            };

            yield return null;
        }

        UI_LoadingPanel.SetActive(false);
    }

    private void A_Internal_Back()
    {
        if (ListOfPathes.Count - 1 > 0)
            ListOfPathes.RemoveAt(ListOfPathes.Count - 1);

        if (Directory.Exists(ListOfPathes[ListOfPathes.Count - 1]))
            MainPath = ListOfPathes[ListOfPathes.Count - 1];
        else
            MainPath = UI_Drivers.options[0].text;
        
        A_Internal_RefreshContent();
    }
    private void A_Internal_Back(int toIndex)
    {
        if (ListOfPathes.Count <= 1)
            return;
        if(toIndex < 0 || toIndex >= ListOfPathes.Count)
        {
            FDE_Error("Back index is too high (higher than directory history array) or lower than 0");
            return;
        }
        if (Directory.Exists(ListOfPathes[toIndex]))
            MainPath = ListOfPathes[toIndex];
        else
            MainPath = UI_Drivers.options[0].text;

        A_Internal_RefreshContent();
    }
    private void A_Internal_SearchByAddress()
    {
        if (!string.IsNullOrEmpty(UI_FullPath.text) && UI_FullPath.text != MainPath)
            A_ProcessFile(UI_FullPath.text);
        else
            FDE_Error("Address is empty or already opened");
    }

    private void A_Internal_ChangeDriver()
    {
        if (!string.IsNullOrEmpty(UI_Drivers.captionText.text))
            A_ProcessFile(UI_Drivers.captionText.text);
        else
            FDE_Error("Address is empty or already opened");
    }
    private void A_Internal_ChangeSize()
    {
        UI_DialogContent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(UI_DialogSize.value, UI_DialogSize.value);
        UI_DialogSizeInfo.text = "Size: " + UI_DialogSize.value.ToString("0");
    }

    private bool A_Internal_GetFileType_ImFile(string Path_)
    {
        try
        {
            FileAttributes at = File.GetAttributes(Path_);
            if ((at & FileAttributes.Directory) == FileAttributes.Directory)
                return false;
            else
                return true;
        }
        catch { return false; }
    }
    private void A_Internal_InitializeScrollDia(string Path_)
    {
        if (!EnableDataCustomization)
            return;
        if (!string.IsNullOrEmpty(Path_) && Input.GetMouseButtonUp(1))
        {
            UI_ScrollDialog.SetActive(true);

            SelectedPath = Path_;

            if (A_Internal_GetFileType_ImFile(SelectedPath))
            {
                UI_ScrollDialog.transform.Find("File").gameObject.SetActive(true);
                UI_ScrollDialog.transform.Find("Folder").gameObject.SetActive(false);
            }
            else
            {
                UI_ScrollDialog.transform.Find("File").gameObject.SetActive(false);
                UI_ScrollDialog.transform.Find("Folder").gameObject.SetActive(true);
            }

            UI_ScrollDialog_Delete.interactable = true;
            UI_ScrollDialog_Delete2.interactable = true;
            UI_ScrollDialog_Rename.interactable = true;
            UI_ScrollDialog_Rename2.interactable = true;
            UI_ScrollDialog_Duplicate.interactable = true;
            UI_ScrollDialog_Copy.interactable = true;
            UI_ScrollDialog_Paste.interactable = false;
            UI_ScrollDialog.transform.position = Input.mousePosition;
        }
        else
        {
            if (string.IsNullOrEmpty(Path_))
            {
                UI_ScrollDialog_Delete2.interactable = false;
                UI_ScrollDialog_Rename2.interactable = false;
                UI_ScrollDialog.transform.Find("File").gameObject.SetActive(false);
                UI_ScrollDialog.transform.Find("Folder").gameObject.SetActive(true);
            }
                
            if (Input.GetMouseButtonUp(1))
                UI_ScrollDialog.SetActive(true);
            else
            {
                UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(false);
                UI_ScrollDialog.SetActive(false);
            }

            if (!string.IsNullOrEmpty(PathToPaste))
                UI_ScrollDialog_Paste.interactable = true;
            else
                UI_ScrollDialog_Paste.interactable = false;

            UI_ScrollDialog_Delete.interactable = false;
            UI_ScrollDialog_Duplicate.interactable = false;
            UI_ScrollDialog_Copy.interactable = false;
            UI_ScrollDialog.transform.position = Input.mousePosition;

            SelectedPath = MainPath;
        }

        if (!A_Internal_CheckFileForProtection(Path_))
        {
            UI_ScrollDialog_Delete.interactable = false;
            UI_ScrollDialog_Delete2.interactable = false;
            UI_ScrollDialog_Rename.interactable = false;
            UI_ScrollDialog_Rename2.interactable = false;
            UI_ScrollDialog_Duplicate.interactable = false;
            UI_ScrollDialog_Copy.interactable = false;
        }
    }

    private bool A_Internal_CheckFileForProtection(string currentPath)
    {
        if (!HighProtectionLevel)
            return true;
        else
        {
            foreach(string s in ListOfCreatedFileFolders)
            {
                if (s.Replace("/", "").Replace(@"\", "") == currentPath.Replace("/", "").Replace(@"\", ""))
                    return true;
            }
            return false;
        }
    }
    private void A_Internal_RefreshFileForProtection(string filepath, int addAction = 0, string SecondPath = "")
    {
        if (filepath.Length > 4 && (filepath.Substring(3,1) == "/" || filepath.Substring(3, 1) == @"\"))
            filepath = filepath.Remove(3, 1);

        filepath = filepath.Replace("/", @"\");
        SecondPath = SecondPath.Replace("/", @"\");
        if (addAction == 1)
        {
            int index = 0;
            foreach(string s in ListOfCreatedFileFolders)
            {
                if (s.Replace("/", "").Replace(@"\", "") == filepath.Replace("/", "").Replace(@"\", ""))
                {
                    ListOfCreatedFileFolders[index] = SecondPath;
                    break;
                }
                index++;
            }
        }
        else if (addAction == 2)
            ListOfCreatedFileFolders.Add(filepath);
        else if (addAction == 3)
            ListOfCreatedFileFolders.Remove(filepath);
    }
    private void A_Internal_RefreshProtectionList()
    {
        for(int i = 0; i< ListOfCreatedFileFolders.Count;i++)
        {
            string s = ListOfCreatedFileFolders[i];
            if (A_Internal_GetFileType_ImFile(s))
            {
                if (!File.Exists(s))
                    ListOfCreatedFileFolders.RemoveAt(i);
            }
            else
            {
                if (!Directory.Exists(s))
                    ListOfCreatedFileFolders.RemoveAt(i);
            }
        }
    }
        
    private void A_Internal_ScrollButton(int index)
    {
        switch(index)
        {
            case 0:
                PathToPaste = SelectedPath;
                break;

            case 1:
                try
                {
                    string Name = Path.GetFileName(PathToPaste);

                    if (File.Exists(SelectedPath + "/" + Path.GetFileName(PathToPaste)))
                        Name = Path.GetFileNameWithoutExtension(Name) + Directory.GetFiles(SelectedPath).Length.ToString() + Path.GetExtension(Name);
                    File.Copy(PathToPaste, SelectedPath + "/" + Name);
                    A_Internal_RefreshFileForProtection(SelectedPath + "/" + Name, 2);
                    PathToPaste = "";
                }
                catch
                { }

                A_Internal_RefreshContent();
                break;

            case 2:
                try
                {
                    string path = Path.GetDirectoryName(SelectedPath) + "/" + Path.GetFileNameWithoutExtension(SelectedPath) + Directory.GetFiles(Path.GetDirectoryName(SelectedPath)).Length.ToString() + Path.GetExtension(SelectedPath);
                    File.Copy(SelectedPath,path);
                    A_Internal_RefreshFileForProtection(path, 2);
                }
                catch
                { }

                A_Internal_RefreshContent();
                break;

            case 3:
                actionIndex = 4;
                UI_ScrollDialog_ProtectionQuestion.SetActive(true);
                break;

            case 4:
                actionIndex = 0;
                UI_ScrollDialog_CreatingSomethingInputField.text = "";
                UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(true);
                break;

            case 5:
                actionIndex = 1;
                UI_ScrollDialog_CreatingSomethingInputField.text = "";
                UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(true);
                break;

            case 6:
                actionIndex = 2;
                UI_ScrollDialog_CreatingSomethingInputField.text = Path.GetFileName(SelectedPath);
                UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(true);
                break;

            case 7:
                actionIndex = 3;
                UI_ScrollDialog_CreatingSomethingInputField.text = Path.GetFileName(SelectedPath);
                UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(true);
                break;
        }
       
        if(index<3)
        UI_ScrollDialog.SetActive(false);
    }
    int actionIndex;
    private void A_Internal_EnterInputField()
    {
        UI_ScrollDialog_CreatingSomethingInputField.gameObject.SetActive(false);
        UI_ScrollDialog_ProtectionQuestion.SetActive(false);
        UI_ScrollDialog.SetActive(false);

        switch (actionIndex)
        {
            //----------CreateNewFile & Folder
            case 0:
                if (string.IsNullOrEmpty(UI_ScrollDialog_CreatingSomethingInputField.text) || File.Exists(UI_ScrollDialog_CreatingSomethingInputField.text))
                    return;

                string fileName = UI_ScrollDialog_CreatingSomethingInputField.text;
                if (string.IsNullOrEmpty(DefaultExtension))
                    DefaultExtension = "txt";
                if (DefaultExtension.Contains("."))
                    DefaultExtension = DefaultExtension.Replace(".", "");
                if (!fileName.Contains("."))
                    fileName += "."+ DefaultExtension;
                File.Create(MainPath + "/" + fileName).Dispose();
                A_Internal_RefreshFileForProtection(MainPath + "/" + fileName, 2);

                A_Internal_RefreshContent();
                break;

            case 1:
                if (string.IsNullOrEmpty(UI_ScrollDialog_CreatingSomethingInputField.text) || Directory.Exists(UI_ScrollDialog_CreatingSomethingInputField.text))
                    return;

                Directory.CreateDirectory(MainPath + "/" + UI_ScrollDialog_CreatingSomethingInputField.text);
                A_Internal_RefreshFileForProtection(MainPath + "/" + UI_ScrollDialog_CreatingSomethingInputField.text, 2);

                A_Internal_RefreshContent();
                break;


            //---------Renaming files & folders
            case 2:
                if (string.IsNullOrEmpty(UI_ScrollDialog_CreatingSomethingInputField.text) || File.Exists(UI_ScrollDialog_CreatingSomethingInputField.text))
                    UI_ScrollDialog_CreatingSomethingInputField.text += Random.Range(1, 999);

                File.Move(SelectedPath, MainPath + "/" + UI_ScrollDialog_CreatingSomethingInputField.text);
                A_Internal_RefreshFileForProtection(SelectedPath, 1, MainPath + "/" + UI_ScrollDialog_CreatingSomethingInputField.text);

                A_Internal_RefreshContent();
                break;

            case 3:
                if (string.IsNullOrEmpty(UI_ScrollDialog_CreatingSomethingInputField.text) || Directory.Exists(UI_ScrollDialog_CreatingSomethingInputField.text))
                    UI_ScrollDialog_CreatingSomethingInputField.text += Random.Range(1, 999);

                Directory.Move(SelectedPath, MainPath + "/" + UI_ScrollDialog_CreatingSomethingInputField.text);
                A_Internal_RefreshFileForProtection(SelectedPath, 1, MainPath + "/" + UI_ScrollDialog_CreatingSomethingInputField.text);

                A_Internal_RefreshContent();
                break;


            //---------Deleting files & folders
            case 4:
                try
                {
                    if (A_Internal_GetFileType_ImFile(SelectedPath))
                        File.Delete(SelectedPath);
                    else
                        Directory.Delete(SelectedPath, true);

                    A_Internal_RefreshFileForProtection(SelectedPath, 3);
                }
                catch(IOException e) { FDE_Error("Could not delete the file/ directory ["+ e.Message +"] - " + SelectedPath); }
                A_Internal_RefreshContent();
                break;
        }
    }
    #endregion

    //----------After File click functions
    #region AfterFile Click processes
    private void A_ProcessFile(string Path_)
    {
        try
        {
            bool ImFile = A_Internal_GetFileType_ImFile(Path_);

            UI_ScrollDialog.SetActive(false);

            if (ImFile)
            {
                switch(File_Action)
                {

                    //----------------------------Basic File Actions
                    case _FileAction.Open:
                        Process.Start(Path_);
                        break;
                    case _FileAction.OpenInExplorer:
                        Process.Start(Path.GetDirectoryName(Path_));
                        break;

                    //----------------------------Logical-Variable Related File Actions
                    case _FileAction.Text_ReadToVariable:
                        if (!ActionOBJ_ReadToVariableMonoBeh)
                        {
                            UnityEngine.Debug.LogError("FDE error - missing required object.");
                            return;
                        }

                        try
                        {
                            ActionOBJ_ReadToVariableMonoBeh.GetType().GetField(ActionOBJ_ReadToVariableVar).SetValue(ActionOBJ_ReadToVariableMonoBeh, A_ReturnReadType(Path_));
                        }
                        catch
                        {
                            UnityEngine.Debug.LogError("FDE error - variable could not be found.");
                            return;
                        }
                        break;

                    //----------------------------Visual File Actions
                    case _FileAction.Image_ReadImageToRenderer:
                        if (!ActionOBJ_ReadToRenderer)
                        {
                            UnityEngine.Debug.LogError("FDE error - missing required object.");
                            return;
                        }
                        if (Path.GetExtension(Path_) != ".png" && Path.GetExtension(Path_) != ".jpg" && Path.GetExtension(Path_) != ".bmp" && Path.GetExtension(Path_) != ".tga" && Path.GetExtension(Path_) != ".gif")
                            return;

                        Texture2D t = new Texture2D(1, 1);
                        t.LoadImage(File.ReadAllBytes(Path_));
                        t.name = Path.GetFileNameWithoutExtension(Path_);
                        t.Apply();
                        ActionOBJ_ReadToRenderer.material.mainTexture = (Texture)t;
                        break;

                    case _FileAction.Image_ReadImageToSprite:
                        if (!ActionOBJ_ReadToSprite)
                        {
                            UnityEngine.Debug.LogError("FDE error - missing required object.");
                            return;
                        }
                        if (Path.GetExtension(Path_) != ".png" && Path.GetExtension(Path_) != ".jpg" && Path.GetExtension(Path_) != ".bmp" && Path.GetExtension(Path_) != ".tga" && Path.GetExtension(Path_) != ".gif")
                            return;

                        t = new Texture2D(1, 1);
                        t.LoadImage(File.ReadAllBytes(Path_));
                        t.name = Path.GetFileNameWithoutExtension(Path_);
                        t.Apply();
                        ActionOBJ_ReadToSprite.sprite = Sprite.Create(t, new Rect(Vector2.zero, new Vector2(t.width, t.height)), Vector2.zero);
                        break;

                    case _FileAction.Image_ReadImageToUIImage:
                        if (!ActionOBJ_ReadToUIImage)
                        {
                            UnityEngine.Debug.LogError("FDE error - missing required object.");
                            return;
                        }
                        if (Path.GetExtension(Path_) != ".png" && Path.GetExtension(Path_) != ".jpg" && Path.GetExtension(Path_) != ".bmp" && Path.GetExtension(Path_) != ".tga" && Path.GetExtension(Path_) != ".gif")
                            return;

                        t = new Texture2D(1, 1);
                        t.LoadImage(File.ReadAllBytes(Path_));
                        t.name = Path.GetFileNameWithoutExtension(Path_);
                        t.Apply();
                        ActionOBJ_ReadToUIImage.sprite = Sprite.Create(t, new Rect(Vector2.zero, new Vector2(t.width, t.height)), Vector2.zero);
                        break;

                    //----------------------------ASCII/ Text File Actions
                    case _FileAction.Text_ReadTo3DText:
                        if (!ActionOBJ_ReadTo3DText)
                        {
                            UnityEngine.Debug.LogError("FDE error - missing required object.");
                            return;
                        }
                        ActionOBJ_ReadTo3DText.text = A_ReturnReadType(Path_);
                        break;

                    case _FileAction.Text_ReadToUIText:
                        if (!ActionOBJ_ReadToUIText)
                        {
                            UnityEngine.Debug.LogError("FDE error - missing required object.");
                            return;
                        }
                        ActionOBJ_ReadToUIText.text = A_ReturnReadType(Path_);
                        break;

                    //----------------------------Custom File Actions
                    case _FileAction.CustomEvent:
                        if (Action_CustomEvent != null)
                            Action_CustomEvent.Invoke();
                        else
                            UnityEngine.Debug.LogError("FDE error - missing required object.");
                        break;
                }

                if (!KeepDialogAfterAction)
                    Action_CLOSE_DIALOG();
            }
            else
            {
                if (MaxStoredHistoryFolders > ListOfPathes.Count)
                    ListOfPathes.Add(Path_);
                else if (ListOfPathes.Count > 0)
                {
                    ListOfPathes.RemoveAt(0);
                    ListOfPathes.Add(Path_);
                }
                MainPath = Path_;
                A_Internal_RefreshContent();
            }
        }
        catch { UnityEngine.Debug.LogError("FDE_Source - Could not process the clicked file..."); }
    }
    private void A_GetInfo(string Path_)
    {
        try
        {
            bool ImFile = A_Internal_GetFileType_ImFile(Path_);

            UI_Info.enabled = true;

            if (ImFile)
            {
                long fileSize = new FileInfo(Path_).Length;
                UI_Info.text = "Type: File    Size: " + (fileSize/1024).ToString() + "kb    Last Access: " + File.GetLastAccessTime(Path_).ToShortDateString();
            }
            else
                UI_Info.text = "Type: Folder    Size: " + (Directory.GetDirectories(Path_).Length).ToString() + " Folders    Last Access: " + Directory.GetLastAccessTime(Path_).ToShortDateString();
        }
        catch { UI_Info.text = ""; }
    }
    private string A_ReturnReadType(string Path_)
    {
        switch(ReadType)
        {
            case _ReadType.ReadFileContent:
                return File.ReadAllText(Path_);
            case _ReadType.ReadFileName:
                return Path.GetFileName(Path_);
            case _ReadType.ReadFileNameWithoutExtension:
                return Path.GetFileNameWithoutExtension(Path_);
            case _ReadType.ReadFileExtensionOnly:
                return Path.GetExtension(Path_);
            case _ReadType.ReadFullFilePathWithoutFileName:
                return Path.GetDirectoryName(Path_);
            case _ReadType.ReadFullFilePath:
                return Path_;
            case _ReadType.ReadFileSizeInBytes:
                return File.ReadAllBytes(Path_).Length.ToString();
            case _ReadType.ReadFileSizeInKilobytes:
                return (File.ReadAllBytes(Path_).Length / 1024).ToString();
            case _ReadType.ReadFileSizeInMegabytes:
                return (File.ReadAllBytes(Path_).Length / (1024*2)).ToString();
        }

        return "ERROR";
    }
    #endregion

    //----------Error
    private void FDE_Error(string Exception)
    {
        UnityEngine.Debug.LogError("FDE Error-Warning: " + Exception);
        return;
    }
}