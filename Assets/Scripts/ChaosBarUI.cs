using UnityEngine;
using UnityEngine.UI;

public class ChaosBarUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _speed = 5f;

    private float _maxValue;
    private float _targetValue;

    private void OnEnable()
    {
        _maxValue = ChaosManager.MaxChaos;
        _targetValue = ChaosManager.CurrentChaos;
        
        _fillImage.fillAmount = _targetValue / _maxValue;

        GameEvents.OnChaosChanged += SetValue;
    }

    private void OnDisable()
    {
        GameEvents.OnChaosChanged -= SetValue;
    }

    private void Update()
    {
        var current = _fillImage.fillAmount;
        var target01 = Mathf.Clamp01(_targetValue / _maxValue);

        _fillImage.fillAmount = Mathf.Lerp(
            current,
            target01,
            Time.deltaTime * _speed
        );
    }

    private void SetValue(float value)
    {
        _targetValue = value;
    }
}