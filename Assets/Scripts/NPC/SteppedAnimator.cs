using UnityEngine;

/// <summary>
/// Gives the NPC a stop-motion / stepped animation look by sampling
/// the Animator at a fixed frame rate (default 12 FPS) instead of every frame.
/// Attach to any NPC alongside its Animator.
/// </summary>
public class SteppedAnimator : MonoBehaviour
{
    public static bool GlobalEnabled = true;

    [SerializeField] private float _fps = 12f;

    private Animator _animator;
    private float _accumulator;

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        
        if (_animator != null && GlobalEnabled)
            _animator.speed = 0f;
    }

    private void OnDisable()
    {
        if (_animator != null)
            _animator.speed = 1f;
    }

    private void LateUpdate()
    {
        if (_animator == null || !_animator.enabled)
            return;

        if (!GlobalEnabled)
        {
            if (_animator.speed == 0f)
                _animator.speed = 1f;

            return;
        }
        
        if (_animator.speed != 0f)
            _animator.speed = 0f;

        _accumulator += Time.deltaTime;

        var stepInterval = 1f / _fps;

        if (_accumulator < stepInterval)
            return;
        
        var dt = _accumulator;
        _accumulator = 0f;

        _animator.speed = 1f;
        _animator.Update(dt);
        _animator.speed = 0f;
    }
}
