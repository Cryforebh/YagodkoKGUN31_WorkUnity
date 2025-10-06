using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private Unit _unit;
    public Unit Unit => _unit;

    public void CurrentUnit(Unit unit)
    {
        _unit = unit;
    }
}
