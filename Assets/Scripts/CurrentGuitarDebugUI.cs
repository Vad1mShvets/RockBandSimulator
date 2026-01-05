using System;
using TMPro;
using UnityEngine;

public class CurrentGuitarDebugUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _guitarDebugName;

    private void Awake()
    {
        GameEvents.OnGuitarUpdate += GuitarUpdated;
    }

    private void GuitarUpdated(GuitarType guitarType)
    {
        _guitarDebugName.text = guitarType.ToString();
    }
}
