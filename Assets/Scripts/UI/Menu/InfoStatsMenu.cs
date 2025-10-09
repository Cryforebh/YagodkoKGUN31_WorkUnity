using TMPro;
using UnityEngine;

public class InfoStatsMenu : MonoBehaviour
{
    [SerializeField] private EnumInfoStatsType _typeStat;
    [SerializeField] private Unit _unit;

    private TMP_Text _tmpText;
    private string _text = "Error!";

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        ShowInfo();
    }

    private void ShowInfo()
    {
        switch (_typeStat)
        {
            case EnumInfoStatsType.Health:
                _text = $"(LvL up +{_unit.LvlUpHealthCounts}) \\ {_unit.GetMaxHealth}";
                break;
            case EnumInfoStatsType.Damage:
                _text = $"(LvL up +{_unit.LvlUpDamageCounts}) \\ {_unit.GetDamage}";
                break;
            case EnumInfoStatsType.MoveRange:
                _text = $"(LvL up +{_unit.LvlUpMoveRangeCount}) \\ {_unit.MoveRange}";
                break;
            case EnumInfoStatsType.AttackRange:
                _text = $"{_unit.AttackRange}";
                break;
            default:
                break;
        }

        _tmpText.text = _text;
    }
}

public enum EnumInfoStatsType
{
    Health = 0,
    Damage = 1,
    MoveRange = 2,
    AttackRange = 3,
}
