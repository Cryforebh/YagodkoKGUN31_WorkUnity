using UnityEngine;

public class GameData : MonoBehaviour
{
    public bool Lock { get; set; }

    public bool LockClick { get; set; }

    public Unit TargetUnitEnter { get; set; }

    public HealthBar StatisticUnitVisual { get; set; }

    public ObjectActive TargetObjectActiveEnter { get; set; }
}
