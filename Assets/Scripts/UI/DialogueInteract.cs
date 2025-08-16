using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueManager;

public class DialogueInteract : MonoBehaviour
{    bool isInteract = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isInteract)
            {
                isInteract = true;
                DialogueManager.instance.ShowDialogue(DialogueName.DronFirst);
            }

        }
    }
}
