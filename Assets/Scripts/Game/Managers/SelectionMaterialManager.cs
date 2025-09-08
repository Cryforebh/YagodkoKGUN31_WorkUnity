using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMaterialManager : MonoBehaviour
{
    [SerializeField] private Material _focusMaterialUnit;
    [SerializeField] private Material _selectMaterialUnit;
    [SerializeField] private Material _focusMaterialEnemyUnit;

    public Material GetFocusMaterialUnit => _focusMaterialUnit;
    public Material GetSelectMaterialUnit => _selectMaterialUnit;
    public Material GetFocusMaterialEnemyUnit => _focusMaterialEnemyUnit;

}
