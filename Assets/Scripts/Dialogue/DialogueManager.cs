using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public DialogueManager.DialogueName dialogueName;
    public DialogueSequence dialogueSequence;
}

[System.Serializable]
public class BillboardEntry
{
    public DialogueManager.BillboradName billboardName;
    public DialogueSequence dialogueSequence;
}

public class DialogueManager : MonoBehaviour
{
    public List<DialogueEntry> dialogueEntries;
    public List<BillboardEntry> billboardEntries;

    public Dictionary<DialogueName, DialogueSequence> dialogueLines = new Dictionary<DialogueName, DialogueSequence>();
    public Dictionary<BillboradName, DialogueSequence> billboardLines = new Dictionary<BillboradName, DialogueSequence>();
    public enum DialogueName
    {
        Intro,
        DronFirst,
        FirstUpdateTower,
    }
    public enum BillboradName
    {
        Intro_Monologue,
        Dron_Monologue,
        Dron_SetWalking,
        Dron_Build,
        Dron_Get,
        Dron_Repair,
    }

    public static DialogueManager instance;
    private void Awake()
    {
        // 싱글톤
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        // List -> Dictionary 초기화
        foreach (var entry in dialogueEntries)
        {
            dialogueLines[entry.dialogueName] = entry.dialogueSequence;
        }

        foreach (var entry in billboardEntries)
        {
            billboardLines[entry.billboardName] = entry.dialogueSequence;
        }
    }


    private UI_Dialogue dialogueObj;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if(dialogueObj == null)
            {
                ShowBillBoardDialogue(BillboradName.Intro_Monologue, InventoryManager.instance.player.transform);

            }
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
                ShowDialogue(DialogueName.DronFirst);
        }
    }
    public void ShowDialogue(DialogueName dialogueName)
    {
        dialogueObj = UIManager.instance.ShowPopupUI("UI_Dialogue").GetComponent<UI_Dialogue>();
        dialogueObj.SetLineAndStartDialogue(dialogueLines[dialogueName]);
    }

    public GameObject ShowBillBoardDialogue(BillboradName dialogueName, Transform root)
    {
       var billboardObj = UIManager.instance.ShowPopupUI("UI_BillBoardDialogue", null, root).GetComponent<UI_BillBoardDialogue>();
        billboardObj.SetLineAndStartDialogue(billboardLines[dialogueName]);
        return billboardObj.gameObject;
    }
}
