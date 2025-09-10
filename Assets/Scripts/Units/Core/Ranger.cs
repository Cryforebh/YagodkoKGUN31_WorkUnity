using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Unit
{
    [SerializeField][Range(1f, 100f)] private float _bHealth = 7f;
    [SerializeField][Range(0f, 10f)] private float _bStrength = 1f;
    [SerializeField][Range(1f, 5f)] private float _bDamageWeapon = 3f;

    private void Start()
    {
        name = "Лучник";
        SetStandardCharacter(_bHealth, _bStrength);
        SetWeaponDamage(_bDamageWeapon);
    }
}
