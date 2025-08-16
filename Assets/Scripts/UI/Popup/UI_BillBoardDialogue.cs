using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BillBoardDialogue : UI_Popup
{
    [SerializeField] private float charInterval = 0.05f;
    [SerializeField] private float delayAfterLine = 1f; 
    private DialogueLine[] lines;

    [SerializeField] private int currentLineIndex = 0;

    private Coroutine typingCoroutine;
    private TextMeshProUGUI dialogueTmp;

    enum TMP
    {
        DialogueTxt,
    }
    public override void Init()
    {

    }
    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
    // 다음줄 출력
    private void DisplayNextLine()
    {
        if (currentLineIndex >= lines.Length)
        {
            dialogueTmp.text = "";
            Destroy(this.gameObject);
            return; // 끝남
        }

        string line = lines[currentLineIndex].dialogueText;

        currentLineIndex++;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeTextCoroutine(line));
    }

    // 한글자씩 나오게 하는 코루틴
    private IEnumerator TypeTextCoroutine(string line)
    {
        dialogueTmp.text = "";

        foreach (char c in line)
        {
            dialogueTmp.text += c;
            yield return new WaitForSeconds(charInterval);
        }

        typingCoroutine = null;

        yield return new WaitForSeconds(delayAfterLine);
        DisplayNextLine();
    }

    public void SetLineAndStartDialogue(DialogueSequence lines)
    {
        Bind<TextMeshProUGUI>(typeof(TMP));
        dialogueTmp = Get<TextMeshProUGUI>((int)TMP.DialogueTxt);

        this.lines = lines.dialogueLines;
        currentLineIndex = 0;

        DisplayNextLine();
    }

}
