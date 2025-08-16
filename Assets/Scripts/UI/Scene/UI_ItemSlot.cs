using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour
{
    public UI_Inventory inventory;

    [Header("ItemData")]
    public ItemData Item = null;
    public int Stack = 0;
    public int index = -1;
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI stackTxt;

    //색상
    private Color _alpha0 = new Color(255, 255, 255, 0);
    private Color _alpha255 = new Color(255, 255, 255, 255);

    // 아이콘 업데이트
    public void UpdateIcon(Sprite icon)
    {
        this.icon.sprite = icon;
        this.icon.color = _alpha255;
    }

    // 텍스트 업데이트
    public void UpdateTMP()
    {
        if (Stack == 1)
        {
            stackTxt.text = "";
        }
        else
        {
            stackTxt.text = Stack.ToString();
        }
    }

    // UI 초기화 함수
    public void ResetSlot()
    {
        Item = null;
        Stack = 0;
        icon.sprite = null;
        icon.color = _alpha0;
        stackTxt.text = "";
    }

    // UI 등당 합수
    public void ShowDetail()
    {
        if (Item != null)
            inventory.ShowDetail(index);
    }
}
