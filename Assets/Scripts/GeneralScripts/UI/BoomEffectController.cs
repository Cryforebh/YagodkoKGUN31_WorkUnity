using System.Collections;
using UnityEngine;

public class BoomEffectController : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;

    [Header("Настройки взрыва")]
    [SerializeField] private Sprite[] _explosionSprites; // Массив спрайтов анимации
    [SerializeField] private float _frameDuration = 0.05f; // Длительность одного кадра
    [SerializeField] private float _offsetY = 1.5f; // Смещение по высоте относительно юнита

    [Header("Визуальные настройки")]
    [SerializeField] private Material _billboardMaterial; // Материал с шейдером билбординга
    [SerializeField] private Vector2 _spriteSize = new Vector2(2, 2); // Размер спрайта

    private Transform _mainCamera;

    public void PlayExplosion(IUnitMain unit)
    {
        _mainCamera = m_Camera.transform;
        GameObject explosionObject = new GameObject("ExplosionSprite");
        explosionObject.transform.SetParent(transform);

        SpriteRenderer _spriteRenderer = explosionObject.AddComponent<SpriteRenderer>();
        _spriteRenderer.material = _billboardMaterial;
        _spriteRenderer.enabled = false;

        StartCoroutine(ExplosionAnimation(unit, _spriteRenderer));
    }

    private IEnumerator ExplosionAnimation(IUnitMain unit, SpriteRenderer spriteRenderer)
    {
        UpdatePositionAndRotation(unit, spriteRenderer);
        spriteRenderer.enabled = true;

        foreach (Sprite frame in _explosionSprites)
        {
            spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(_frameDuration);
        }

        spriteRenderer.enabled = false;
        Destroy(spriteRenderer.gameObject); // Уничтожаем объект после завершения анимации
    }

    private void UpdatePositionAndRotation(IUnitMain unit, SpriteRenderer spriteRenderer)
    {
        // Позиционирование над юнитом
        Vector3 newPosition = unit.CurrentTransform.position;
        newPosition.y += _offsetY;
        spriteRenderer.transform.position = newPosition;

        // Ориентация в сторону камеры
        spriteRenderer.transform.LookAt(spriteRenderer.transform.position + _mainCamera.forward, _mainCamera.up);

        // Установка размера
        spriteRenderer.transform.localScale = new Vector3(_spriteSize.x, _spriteSize.y, 1f);
    }


    //private SpriteRenderer _spriteRenderer;
    //private Transform _mainCamera;
    //private IUnitMain _unit;
    ////private bool _isPlaying;

    //void Awake()
    //{
    //    InitializeComponents();
    //}

    //private void InitializeComponents()
    //{
    //    _mainCamera = m_Camera.transform;
    //    GameObject explosionObject = new GameObject("ExplosionSprite");
    //    explosionObject.transform.SetParent(transform);

    //    _spriteRenderer = explosionObject.AddComponent<SpriteRenderer>();
    //    _spriteRenderer.material = _billboardMaterial;
    //    _spriteRenderer.enabled = false;
    //}

    //public void PlayExplosion(IUnitMain unit)
    //{
    //    _unit = unit;
    //    //if (!_isPlaying)
    //    //{
    //        StartCoroutine(ExplosionAnimation());
    //    //}
    //}

    //private IEnumerator ExplosionAnimation()
    //{
    //    //_isPlaying = true;
    //    UpdatePositionAndRotation();
    //    _spriteRenderer.enabled = true;

    //    foreach (Sprite frame in _explosionSprites)
    //    {
    //        _spriteRenderer.sprite = frame;
    //        yield return new WaitForSeconds(_frameDuration);
    //    }

    //    _spriteRenderer.enabled = false;
    //    //_isPlaying = false;
    //}

    //private void UpdatePositionAndRotation()
    //{
    //    // Позиционирование над юнитом
    //    Vector3 newPosition = _unit.CurrentTransform.position;
    //    newPosition.y += _offsetY;
    //    _spriteRenderer.transform.position = newPosition;

    //    // Ориентация в сторону камеры
    //    _spriteRenderer.transform.LookAt(
    //        _spriteRenderer.transform.position + _mainCamera.forward,
    //        _mainCamera.up
    //    );

    //    // Установка размера
    //    _spriteRenderer.transform.localScale = new Vector3(
    //        _spriteSize.x,
    //        _spriteSize.y,
    //        1f
    //    );
    //}

    //public void ResetExplosion()
    //{
    //    StopAllCoroutines();
    //    //_isPlaying = false;
    //    _spriteRenderer.enabled = false;
    //}
}
