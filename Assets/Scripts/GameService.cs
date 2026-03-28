using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameService : MonoBehaviour
{
    public static GameService Instance { get; private set; }

    [SerializeField] private int _uiSceneIndex = 1;
    [SerializeField] private int _gameplaySceneIndex = 2;

    private bool _restarting;

    private void Awake()
    {
        Instance = this;
        
        GameEvents.OnChaosFilled += RestartLevel;
    }

    public IEnumerator StartGame()
    {
        ChaosManager.Init();
        ConcertScoreManager.Init(); 
        SoundsManager.Init();
        ReputationManager.Init();
        
        yield return LoadUI();
        yield return LoadGameplay();
        
        yield return FadeScreenUI.Instance.FadeOut();
        
        InputStateController.Instance.SetGameplay();
    }

    public void RestartLevel()
    {
        if (_restarting)
            return;

        _restarting = true;
        
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return FadeScreenUI.Instance.FadeIn();

        yield return UnloadGameplay();
        yield return LoadGameplay();

        yield return FadeScreenUI.Instance.FadeOut();

        _restarting = false;
    }

    private IEnumerator LoadUI()
    {
        var scene = SceneManager.GetSceneByBuildIndex(_uiSceneIndex);

        if (!scene.isLoaded)
            yield return SceneManager.LoadSceneAsync(_uiSceneIndex, LoadSceneMode.Additive);
    }

    private IEnumerator LoadGameplay()
    {
        yield return SceneManager.LoadSceneAsync(_gameplaySceneIndex, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(_gameplaySceneIndex));

        GameEvents.OnGameplayStarted?.Invoke();
    }

    private IEnumerator UnloadGameplay()
    {
        var scene = SceneManager.GetSceneByBuildIndex(_gameplaySceneIndex);

        if (scene.isLoaded)
            yield return SceneManager.UnloadSceneAsync(scene);
    }
}