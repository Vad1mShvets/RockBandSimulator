using UnityEngine;

public struct DamageData
{
    public float Amount;
    public Transform Attacker;

    public DamageData(float amount, Transform attacker)
    {
        Amount = amount;
        Attacker = attacker;
    }
}