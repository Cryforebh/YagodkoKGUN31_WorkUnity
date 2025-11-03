using UnityEngine;

public class C_Setting : MonoBehaviour
{
    [Header("Настройки игры:")]
    [SerializeField, Tooltip("Рубить обязательно?")] private bool _isNecessaryToAttack = true;
    [SerializeField, Tooltip("При поглощении вражеской фишки, дамка занимает клетку за убитой фишкой?")] private bool _isLimitedMoveDamka = true;

    public bool IsNecessaryToAttack => _isNecessaryToAttack;
    public bool IsLimitedMoveDamka => _isLimitedMoveDamka;
}
