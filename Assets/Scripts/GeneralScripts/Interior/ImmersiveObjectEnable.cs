using UnityEngine;
using Zenject;

public class ImmersiveObjectEnable : MonoBehaviour
{
    private MenuToggleManager _menuToggleManager;

    [SerializeField] private GameObject _immersiveObject;

    private void Start()
    {
        _menuToggleManager.OnEnterToggleMenuEvent += OnToggleUpdate;
        OnToggle();
    }

    private void OnToggle()
    {
        if (_menuToggleManager.ImmersiveObjects == false) _immersiveObject.SetActive(false);
        else _immersiveObject.SetActive(true);
    }

    [Inject]
    private void Construct(MenuToggleManager menuToggleManager)
    {
        _menuToggleManager = menuToggleManager;
    }

    private void OnToggleUpdate(EnumToggleMenu obj)
    {
        OnToggle();
    }

    private void OnDestroy()
    {
        _menuToggleManager.OnEnterToggleMenuEvent -= OnToggleUpdate;
    }
}
