using UnityEngine;
using UnityEngine.UI;

public abstract class AnimatedBarBaseUI : MonoBehaviour
{
    [Header("Fills")]
    [SerializeField] protected Image _barNewFill;
    [SerializeField] protected Image _barPreviousFill;

    [Header("Animation")]
    [SerializeField] protected float _newFillSpeed = 20f;
    [SerializeField] protected float _previousFillSpeed = 10f;
    [SerializeField] protected float _previousFillDelay = 0.1f;

    protected float _min;
    protected float _max = 1f;
    protected float _targetValue;

    private float _delayTimer;

    protected virtual void Update()
    {
        var target01 = GetTarget01();

        if (_barNewFill)
        {
            _barNewFill.fillAmount = Mathf.Lerp(
                _barNewFill.fillAmount,
                target01,
                Time.deltaTime * _newFillSpeed
            );
        }

        if (_barPreviousFill == null)
            return;

        var newFillReachedTarget = Mathf.Abs(_barNewFill.fillAmount - target01) < 0.001f;

        if (!newFillReachedTarget)
        {
            _delayTimer = _previousFillDelay;
            return;
        }

        if (_delayTimer > 0f)
        {
            _delayTimer -= Time.deltaTime;
            return;
        }

        _barPreviousFill.fillAmount = Mathf.Lerp(
            _barPreviousFill.fillAmount,
            target01,
            Time.deltaTime * _previousFillSpeed
        );
    }

    protected float GetTarget01()
    {
        if (Mathf.Approximately(_max, _min))
            return 0f;

        return Mathf.Clamp01((_targetValue - _min) / (_max - _min));
    }

    protected void SetRange(float min, float max)
    {
        _min = min;
        _max = max;
    }

    protected void SetTargetValueInternal(float value)
    {
        _targetValue = Mathf.Clamp(value, _min, _max);
    }

    protected void SnapToValue(float value)
    {
        SetTargetValueInternal(value);

        var value01 = GetTarget01();

        if (_barNewFill != null)
            _barNewFill.fillAmount = value01;

        if (_barPreviousFill != null)
            _barPreviousFill.fillAmount = value01;

        _delayTimer = 0f;
    }
}