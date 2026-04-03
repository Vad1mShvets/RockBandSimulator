using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance { get; private set; }
    
    [Header("Location Buttons")]
    [SerializeField] private Button _garageLocationButton;
    [SerializeField] private Button _shopLocationButton;

    [Header("Root")]
    [SerializeField] private GameObject _root;

    private bool _isTraveling;

    private void Awake()
    {
        Instance = this;
        
        _garageLocationButton.onClick.AddListener(() => Travel(LocationType.Garage));
        _shopLocationButton.onClick.AddListener(() => Travel(LocationType.Shop));

        _root.SetActive(false);
    }

    public void Show()
    {
        if (_isTraveling) return;

        _root.SetActive(true);

        PlayerStateController.TryEnterBusy();
        InputStateController.Instance.SetUI();
    }

    public void Hide()
    {
        _root.SetActive(false);

        InputStateController.Instance.SetGameplay();
        PlayerStateController.ExitBusy();
    }

    private void Travel(LocationType destination)
    {
        if (_isTraveling) return;
        if (destination == TravelSystem.CurrentLocation) return;

        StartCoroutine(TravelRoutine(destination));
    }

    private IEnumerator TravelRoutine(LocationType destination)
    {
        _isTraveling = true;
        
        _root.SetActive(false);

        yield return FadeScreenUI.Instance.FadeIn();
        
        TravelSystem.SwitchLocation(destination);
        
        yield return FadeScreenUI.Instance.FadeOut();

        InputStateController.Instance.SetGameplay();
        PlayerStateController.ExitBusy();

        _isTraveling = false;
    }
}
