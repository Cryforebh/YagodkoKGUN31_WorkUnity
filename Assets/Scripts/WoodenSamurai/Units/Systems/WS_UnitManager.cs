using UnityEngine;

public class WS_UnitManager : MonoBehaviour
{
    private WS_Unit _unit;
    public WS_Unit Unit => _unit;

    public void CurrentUnit(WS_Unit unit)
    {
        _unit = unit;
    }
}
