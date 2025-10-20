using System.Collections;
using UnityEngine;


public class LightStreet : MonoBehaviour
{
    [Header("Настройки ударов")]
    [SerializeField, Tooltip("Мин. задержка между ударами")] private float _minStrikePause = 4f;
    [SerializeField, Tooltip("Макс. задержка между ударами")] private float _maxStrikePause = 12f;

    [Header("Настройки вспышек")]
    [SerializeField, Range(1, 5), Tooltip("Мин. вспышек за удар")] private int _minFlashes = 2;
    [SerializeField, Range(2, 8), Tooltip("Макс. вспышек за удар")] private int _maxFlashes = 4;
    [SerializeField, Tooltip("Пиковая яркость")] private float _peakIntensity = 4f;
    [SerializeField, Tooltip("Скорость нарастания до пиковой яркости")] private float _riseDuration = 0.04f;
    [SerializeField, Tooltip("Скорость спада до базовой яркости")] private float _fallDuration = 0.3f;
    [SerializeField, Tooltip("Мин. интервал между стартами вспышек")] private float _minFlashGap = 0.05f;
    [SerializeField, Tooltip("Макс. интервал между стартами вспышек")] private float _maxFlashGap = 0.15f;

    private Light _light;
    private float _baseBrightness;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _baseBrightness = _light.intensity;
    }

    //private void Start() => StartCoroutine(StrikeController());

    private void OnEnable() => StartCoroutine(StrikeController());

    private IEnumerator StrikeController()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_minStrikePause, _maxStrikePause));
            StartCoroutine(LaunchLightningSequence());
        }
    }

    private IEnumerator LaunchLightningSequence()
    {
        int totalFlashes = Random.Range(_minFlashes, _maxFlashes + 1);

        for (int i = 0; i < totalFlashes; i++)
        {
            StartCoroutine(FlashWave());

            if (i < totalFlashes - 1)
                yield return new WaitForSeconds(Random.Range(_minFlashGap, _maxFlashGap));
        }
    }

    private IEnumerator FlashWave()
    {
        // Фаза нарастания
        yield return AdjustBrightness(_baseBrightness, Random.Range(_baseBrightness, _peakIntensity), _riseDuration);

        // Фаза спада
        yield return AdjustBrightness(Random.Range(_baseBrightness, _peakIntensity), _baseBrightness, _fallDuration);
    }

    private IEnumerator AdjustBrightness(float start, float end, float time)
    {
        float timer = 0f;
        while (timer < time)
        {
            _light.intensity = Mathf.Lerp(start, end, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        _light.intensity = end;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _light.intensity = _baseBrightness;
    }
}

/*
public class LightStreet : MonoBehaviour
{
    private Light _light;
    private float _originIntensity;
    private int _intensityRandom = 1;
    private bool _twoHit;

    [SerializeField] private float _intensityStart = 1f;
    [SerializeField] private float _intensityHit = 4.0f;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _originIntensity = _light.intensity;
    }

    private void Start()
    {
        _intensityHit = _originIntensity + _intensityHit;
    }

    private void Update()
    {
        if (_light == null) return;

        _intensityRandom = Random.Range(1, 1000);

        if (_intensityRandom == 50)
        {
            _light.intensity = _intensityHit;

            _twoHit = true;
        }

        if (_twoHit)
        {
            _intensityRandom = Random.Range(1, 20);

            if (_intensityRandom == 10)
            {
                _light.intensity = _intensityHit;
                _twoHit = false;
            }

        }

        if (_originIntensity >= _light.intensity) return;

        _light.intensity -= _light.intensity * 2 * Time.deltaTime;
    }
}
*/
