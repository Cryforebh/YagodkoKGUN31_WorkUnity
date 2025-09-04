using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Material _focusMaterialUnit;
    [SerializeField] private Material _selectMaterialUnit;

    public Material GetFocusMaterialUnit => _focusMaterialUnit;
    public Material GetSelectMaterialUnit => _selectMaterialUnit;

}
