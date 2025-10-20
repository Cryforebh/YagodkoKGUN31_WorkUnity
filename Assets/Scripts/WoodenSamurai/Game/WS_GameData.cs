using UnityEngine;

public class WS_GameData : MonoBehaviour, IGameData
{
    public bool Lock { get; set; }

    public bool LockClick { get; set; }

    public WS_Unit TargetUnitEnter { get; set; }

    public WS_HealthBar StatisticUnitVisual { get; set; }

    public WS_ObjectActive TargetObjectActiveEnter { get; set; }
}
