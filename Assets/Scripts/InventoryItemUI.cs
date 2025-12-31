using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _count;

    public void Setup(Sprite icon, int count)
    {
        _icon.sprite = icon;
        _count.text = $"x{count}";
    }
}
