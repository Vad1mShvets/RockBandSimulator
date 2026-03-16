using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        
        var loadOp = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);

        while (!loadOp.isDone)
            yield return null;
        
        var gameplayScene = SceneManager.GetSceneByBuildIndex(2);
        SceneManager.SetActiveScene(gameplayScene);
    }
}