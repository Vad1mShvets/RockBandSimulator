using System.Collections;
using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return GameService.Instance.StartGame();
    }
}