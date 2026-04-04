using TMPro;
using UnityEngine;

public class MoneyBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyText;

    private void OnEnable()
    {
        OnMoneyChanged(MoneyManager.CurrentMoney);

        GameEvents.OnMoneyChanged += OnMoneyChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnMoneyChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int value)
    {
        _moneyText.text = $"{value}";
    }
}
