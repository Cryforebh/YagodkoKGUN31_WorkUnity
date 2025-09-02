using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CellPaletteSettings : ScriptableObject
{
    [field: SerializeField, Space(15f)]
    [field: Tooltip("Клетка под выбранным юнитом")]
    public Material selectedMaterial;


    public Material moveMaterial;
    public Material attackMaterial;
    public Material moveAndAttackMaterial;
}
