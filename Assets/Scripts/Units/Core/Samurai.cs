using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai : Unit
{
    [SerializeField][Range(1f, 100f)] private float _bHealth = 15f;
    [SerializeField][Range(0f, 10f)] private float _bStrength = 3f;
    [SerializeField][Range(1f, 5f)] private float _bDamageWeapon = 3f;

    private void Start()
    {
        name = "Самурай";
        SetStandardCharacter(_bHealth, _bStrength);
        SetWeaponDamage(_bDamageWeapon);
    }

}
