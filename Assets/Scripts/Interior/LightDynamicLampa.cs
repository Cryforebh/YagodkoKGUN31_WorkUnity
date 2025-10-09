using System.Collections;
using UnityEngine;

public class LightDynamicLampa : MonoBehaviour
{
    [Header("Настройки источников света освещающих фанарь")]
    [SerializeField] private Light _lightOne;
    [SerializeField] private Light _lightTwo;
    [SerializeField] private float _minIntensity = 1.0f;
    [SerializeField] private float _maxIntensity = 1.1f;

    [Header("Настройки мерцания")]
    [SerializeField] private float _flickerSpeed = 3f;
    //[SerializeField] private float _randomness = 0.3f;

    private float _baseIntensityOne;
    private float _baseIntensityTwo;
    private float _seed;
    private Coroutine _flickerCoroutine;

    private void Awake()
    {
        _seed = Random.Range(0f, 100f);
        Construct();
    }

    private void Construct()
    {
        _baseIntensityOne = _lightOne.intensity;
        _baseIntensityTwo = _lightTwo.intensity;
    }

    private void OnEnable()
    {
        _flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    private void OnDisable()
    {
        if (_flickerCoroutine != null)
        {
            StopCoroutine(_flickerCoroutine);
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Основной цикл мерцания
            float noise = Mathf.PerlinNoise(Time.time * _flickerSpeed, _seed);
            float lerpValue = Mathf.PingPong(noise, 1f);

            _lightOne.intensity = Mathf.Lerp(
                _baseIntensityOne * _minIntensity,
                _baseIntensityOne * _maxIntensity,
                lerpValue
            );

            _lightTwo.intensity = Mathf.Lerp(
                _baseIntensityTwo * _maxIntensity,
                _baseIntensityTwo * _minIntensity,
                lerpValue
            );

            //// Случайные всплески
            //if (Random.value < 0.1f * _randomness)
            //{
            //    _lightOne.intensity *= 1.02f;
            //    _lightTwo.intensity *= 1.005f;
            //    yield return new WaitForSeconds(0.05f); // Короткая вспышка
            //}

            yield return null;
        }
    }

    // Для ручного управления (если нужно)
    public void RestartFlicker()
    {
        if (_flickerCoroutine != null)
        {
            StopCoroutine(_flickerCoroutine);
        }
        _flickerCoroutine = StartCoroutine(FlickerRoutine());
    }
}
