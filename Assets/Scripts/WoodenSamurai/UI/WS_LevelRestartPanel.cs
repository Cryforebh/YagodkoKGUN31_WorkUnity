using UnityEngine;
using UnityEngine.UI;

public class WS_LevelRestartPanel : MonoBehaviour
{
    [SerializeField] private Image _backGround;
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _fillSpeed = 0.5f;

    public float fillAmount { get; private set; }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        _backGround.enabled = true;
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
        _backGround.enabled = false;
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
        _backGround.enabled = false;
    }
}
