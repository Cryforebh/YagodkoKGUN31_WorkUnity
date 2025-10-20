using UnityEngine;

public class LightPhisicRandom : MonoBehaviour
{
    private Light _light;
    private float _originIntensity;
    private Vector3 _originPosition;
    private float _intensityRandom = 1;
    private bool _isRevert;
    private bool _isRevertPosition;

    [SerializeField] private float _intensityMax = 0.1f;
    [SerializeField] private float _intensityMin = 0.2f;
    [SerializeField] private Light _lightOne;
    private float _intensityOneMax;
    private float _intensityOneMin;
    [SerializeField] private Light _lightTwo;
    private float _intensityTwoMin;
    private float _intensityTwoMax;

    [SerializeField] private bool _isPositionRandom;
    [SerializeField] private Vector3 _positionMax = new Vector3(1, 0, 1);
    [SerializeField] private Vector3 _positionMin = new Vector3(-1, 0, -1);


    private void Awake()
    {
        _light = GetComponent<Light>();
        _originIntensity = _light.intensity;
        _originPosition = transform.position;
    }

    private void Start()
    {
        _intensityMax = _originIntensity + _intensityMax;
        _intensityMin = _originIntensity - _intensityMin;

        if (_lightOne != null)
        {
            _intensityOneMax = _lightOne.intensity + _intensityOneMax;
            _intensityOneMin = _lightOne.intensity - _intensityOneMin;
        }
        if (_lightTwo != null)
        {
            _intensityTwoMax = _lightTwo.intensity + _intensityTwoMax;
            _intensityTwoMin = _lightTwo.intensity - _intensityTwoMin;
        }

        _isRevert = false;

        _positionMax = _originPosition + _positionMax;
        _positionMin = _originPosition - _positionMin;
        _isRevertPosition = false;
    }

    private void Update()
    {
        if (_light == null) return;

        if (!_isRevert)
        {
            _light.intensity += _intensityMax * Time.deltaTime * _intensityRandom;
            if (_lightOne != null) _lightOne.intensity += _intensityOneMax * Time.deltaTime * _intensityRandom;
            if (_lightTwo != null) _lightTwo.intensity += _intensityTwoMax * Time.deltaTime * _intensityRandom;
        }
        else
        {
            _light.intensity -= _intensityMin * Time.deltaTime * _intensityRandom;
            if (_lightOne != null) _lightOne.intensity -= _intensityOneMin * Time.deltaTime * _intensityRandom;
            if (_lightTwo != null) _lightTwo.intensity -= _intensityTwoMin * Time.deltaTime * _intensityRandom;
        }

        if (_light.intensity > _intensityMax)
        {
            _isRevert = true;
            _intensityRandom = Random.Range(0.5f, 1.1f);
        }
        else if (_light.intensity < _intensityMin)
        {
            _isRevert = false;
            _intensityRandom = Random.Range(0.5f, 1.1f);
        }



        if (!_isPositionRandom) return;

        if (!_isRevertPosition)
        {
            transform.position += new Vector3(_positionMax.x + _intensityRandom, _positionMax.z + _intensityRandom, _originPosition.y) * Time.deltaTime;
        }
        else
        {
            transform.position -= new Vector3(_positionMin.x - _intensityRandom, _positionMin.z - _intensityRandom, _originPosition.y) * Time.deltaTime;
        }

        if (transform.position.x > _intensityMax + _originPosition.x || transform.position.z > _intensityMax + _originPosition.z)
        {
            _isRevertPosition = true;
            _intensityRandom = Random.Range(0.5f, 1.1f);
        }
        else if (transform.position.x < _originPosition.x - _intensityMin || transform.position.z < _originPosition.z - _intensityMin)
        {
            _isRevertPosition = false;
            _intensityRandom = Random.Range(0.5f, 1.1f);
        }



    }
}
