using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LevelRestartPanel : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _fillSpeed = 0.5f;

    public float fillAmount { get; private set; }

    public void ShowPanel() => gameObject.SetActive(true);
    
    public void HidePanel()
    {
        gameObject.SetActive(false);
        fillAmount = 0f;
        _fillImage.fillAmount = 0f;
    }

    public void FillScale(float deltaTime)
    {
        fillAmount = Mathf.Clamp01(fillAmount + deltaTime * _fillSpeed);
        _fillImage.fillAmount = fillAmount;
    }

    private void Awake()
    {
        _fillImage = gameObject.GetComponent<Image>();
        _fillImage.fillAmount = 0f;
    }
}
