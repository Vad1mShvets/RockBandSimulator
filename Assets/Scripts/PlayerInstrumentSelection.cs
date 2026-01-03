using System;
using UnityEngine;

public class PlayerInstrumentSelection : MonoBehaviour
{
    //[SerializeField] private GameObject _guitarsParent;
    
    private void Start()
    {
        //DisableGuitar();
        //DisableGuitar();
        //DisableGuitar();
    }

    private void OnEnable()
    {
        // GameEvents.OnCallingConcertStart += EnableGuitar;
        // GameEvents.OnCallingRehearsalStart += EnableGuitar;
    }

    private void OnDisable()
    {
        // GameEvents.OnCallingConcertStart -= DisableGuitar;
        // GameEvents.OnCallingRehearsalStart -= DisableGuitar;
    }

    private void EnableGuitar()
    {
        //_guitarsParent.SetActive(true);
    }
    
    private void DisableGuitar()
    {
        //_guitarsParent.SetActive(false);
    }
}
