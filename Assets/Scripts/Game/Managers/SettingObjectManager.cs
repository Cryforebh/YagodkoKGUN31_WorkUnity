
using UnityEngine;

public class SettingObjectManager : MonoBehaviour
{
    [Header("Настройки Модификаций")]
    [SerializeField] private float _mofifiSushiHealthUp = 5f;

    public float MofifiSushiHealthUp => _mofifiSushiHealthUp;
}
