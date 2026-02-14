using System;
using UnityEngine;

public class Bassist : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _animationState;

    private void Start()
    {
        _animator.Play(_animationState);
    }
}