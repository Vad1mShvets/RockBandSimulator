using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] _bodies;

    private void Awake()
    {
        _bodies = GetComponentsInChildren<Rigidbody>();
        SetRagdoll(false);
    }

    public void EnableRagdoll()
    {
        SetRagdoll(true);
    }

    private void SetRagdoll(bool enabled)
    {
        foreach (var rb in _bodies)
        {
            rb.isKinematic = !enabled;

            if (enabled)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}