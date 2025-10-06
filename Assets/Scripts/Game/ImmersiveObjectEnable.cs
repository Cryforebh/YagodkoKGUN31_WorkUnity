using UnityEngine;
using Zenject;

public class ImmersiveObjectEnable : MonoBehaviour
{
    [Inject] private MenuToggleManager _menuToggleManager;

    [SerializeField] private GameObject _immersiveObject;

    private void Awake()
    {
        OnToggle();
    }

    private void Update()
    {
        OnToggle();
    }

    private void OnToggle()
    {
        if (_menuToggleManager.ImmersiveObjects == false) _immersiveObject.SetActive(false);
        else _immersiveObject.SetActive(true);
    }
}
