using UnityEngine;

public interface IUnitMain
{
    public EnumPlayers Player { get; }
    public bool IsEnemy { get; set; }

    public Transform CurrentTransform { get; }
}
