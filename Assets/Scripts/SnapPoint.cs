using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward);
    }
}