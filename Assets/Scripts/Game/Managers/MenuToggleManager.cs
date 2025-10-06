using UnityEngine;

public class MenuToggleManager : MonoBehaviour
{
    private bool _dynamicCamera;
    private bool _madnessMode;
    private bool _immersiveObjects = true;

    private bool _isEnterMenu = false;

    public bool DynamicCamera { get => _dynamicCamera; set => _dynamicCamera = value; }
    public bool MadnessMode { get => _madnessMode; set { _madnessMode = value; } }
    public bool ImmersiveObjects { get => _immersiveObjects; set => _immersiveObjects = value; }

    public bool IsEnterMenu { get => _isEnterMenu; set { _isEnterMenu = value; } }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

public enum EnumToggleMenu
{
    None = 0,
    DynamicCamera = 1,
    Woman = 2,
    MadnessMode = 3,
    ImmersiveObjects = 4,
}
