using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    //플레이어(주인공) 대사인지 아닌지
    public bool isPlayer;

    public string characterName;

    [TextArea(2, 5)]
    public string dialogueText;

    public Sprite characterSprite;
}
