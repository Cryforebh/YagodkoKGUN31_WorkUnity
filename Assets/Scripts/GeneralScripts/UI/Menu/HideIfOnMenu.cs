using UnityEngine;

public class HideIfOnMenu : MonoBehaviour
{
    [SerializeField] private StandardMenu m_Menu;
    [SerializeField] private GameObject _targetHide;


    private void Start()
    {
        if (m_Menu == null) return;
        m_Menu.OnMenuEvent += OnMenuUpdateHide;
    }

    private void OnMenuUpdateHide(bool obj)
    {
        if (obj)
        {
            _targetHide.SetActive(false);
        }
        else _targetHide.SetActive(true);
    }
}
