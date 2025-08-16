using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueManager;

public class DialogueController : MonoBehaviour
{
    public GameObject player;
    private void Start()
    {
        DialogueManager.instance.ShowDialogue(DialogueName.Intro);
        player = GameObject.Find("Player");
        Invoke("FirstDronD", 0.3f);
    }

    void FirstDronD()
    {
        DialogueManager.instance.ShowBillBoardDialogue(BillboradName.Intro_Monologue, player.transform);
    }
}
