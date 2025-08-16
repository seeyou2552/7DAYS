using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class UI_Dialogue : UI_Popup
{
    [SerializeField] private float charInterval = 0.05f;
    public DialogueLine[] lines;

    [SerializeField] private bool isTyping = false;
    [SerializeField] private int currentLineIndex = 0;
    private bool waitingForNextInput = false;

    private Coroutine typingCoroutine;
    private TextMeshProUGUI dialogueTmp;
    private TextMeshProUGUI nameTmp;

    private Image playerImage;
    private Image npcImage;

    public RectTransform rect;
    private CanvasGroup canvasGroup;

    enum TMP
    {
        DialogueTxt,
        NameTxt
    }
    enum Images
    {
        PlayerImage,
        NpcImage
    }

    private void Update()
    {
        // 추후 키 입력은 변경해야함
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else if (waitingForNextInput)
            {
                DisplayNextLine();
            }
        }

    }
    // 다음줄 출력
    private void DisplayNextLine()
    {
        if (currentLineIndex >= lines.Length)
        {
            dialogueTmp.text = "";
            AnimateOut();
            return; // 끝남
        }
        ResetDetails();

        string line = lines[currentLineIndex].dialogueText;
        //이미지와 name박스 설정
        SetDetails();

        currentLineIndex++;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeTextCoroutine(line));
    }

    // 한글자씩 나오게 하는 코루틴
    private IEnumerator TypeTextCoroutine(string line)
    {
        isTyping = true;
        waitingForNextInput = false;
        dialogueTmp.text = "";

        foreach (char c in line)
        {
            dialogueTmp.text += c;
            yield return new WaitForSecondsRealtime(charInterval);
        }

        isTyping = false;
        waitingForNextInput = true;
        typingCoroutine = null;
    }

    // 스킵
    private void SkipTyping()
    {
        if (!isTyping) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueTmp.text = lines[currentLineIndex - 1].dialogueText;
        isTyping = false;
        waitingForNextInput = true;
        typingCoroutine = null;
    }

    public void SetLineAndStartDialogue(DialogueSequence lines)
    {
       // base.Init();
        rect = transform.GetChild(0).GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        Bind<TextMeshProUGUI>(typeof(TMP));
        Bind<Image>(typeof(Images));
        dialogueTmp = Get<TextMeshProUGUI>((int)TMP.DialogueTxt);
        nameTmp = Get<TextMeshProUGUI>((int)TMP.NameTxt);

        playerImage = Get<Image>((int)Images.PlayerImage);
        npcImage = Get<Image>((int)Images.NpcImage);
        ResetDetails();

        this.lines = lines.dialogueLines;
        currentLineIndex = 0;

        AnimateIn();

    }

    // 이미지와, 이름 초기화(setFalse)
    void ResetDetails()
    {
        nameTmp.transform.parent.gameObject.SetActive(false);
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);
    }

    // 등장 애니메이션
    private void AnimateIn()
    {
        SetDetails();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rect.anchoredPosition = new Vector2(0, -50f); // 아래에서 시작
        canvasGroup.alpha = 0f; // 투명하게 시작

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, 0.4f)).SetUpdate(true);
        seq.Join(rect.DOAnchorPosY(0f, 0.4f).SetEase(Ease.OutCubic)).SetUpdate(true);
        seq.OnComplete(() =>
        {
            DisplayNextLine();
        });
    }

    private void AnimateOut()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(0f, 0.4f)).SetUpdate(true);
        seq.Join(rect.DOAnchorPosY(-50f, 0.4f).SetEase(Ease.OutCubic)).SetUpdate(true);
        seq.OnComplete(() =>
        {
            UIManager.instance.ClosePopupUI(this);
        });
    }

    private void SetDetails()
    {
        var curLine = lines[currentLineIndex];

        if (curLine.isPlayer)
        {
            playerImage.gameObject.SetActive(transform);
            playerImage.sprite = curLine.characterSprite;
        }
        else
        {
            npcImage.gameObject.SetActive(transform);
            npcImage.sprite = curLine.characterSprite;
            nameTmp.transform.parent.gameObject.SetActive(true);
            nameTmp.text = curLine.characterName;
        }
    }

}
