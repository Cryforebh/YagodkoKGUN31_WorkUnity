using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Material _fadeMaterial;
    [SerializeField] private float _fadeDuration = 3f;

    private IEnumerator FadeOut()
    {
        float currentIntensity = 1f; // Начинаем с полного затемнения
        float targetIntensity = 0f; // Заканчиваем полной прозрачностью
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            currentIntensity = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
            _fadeMaterial.SetFloat("_Intensity", currentIntensity);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Финализируем значения
        _fadeMaterial.SetFloat("_Intensity", targetIntensity);
    }

    private IEnumerator FadeShow()
    {
        float currentIntensity = 0f; // Начинаем с полного отсутствия затемнения
        float targetIntensity = 1f;  // Заканчиваем полным затемнением
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            currentIntensity = Mathf.Lerp(0f, 1f, elapsed / _fadeDuration); // Исправленная интерполяция
            _fadeMaterial.SetFloat("_Intensity", currentIntensity);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Финализируем значения
        _fadeMaterial.SetFloat("_Intensity", targetIntensity);
    }

    public void Show()
    {
        StartCoroutine(FadeShow());
    }

    public void Out()
    {
        StartCoroutine(FadeOut());
    }
}
