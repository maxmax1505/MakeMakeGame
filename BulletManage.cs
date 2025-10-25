using UnityEngine;

public class BulletManage : MonoBehaviour
{
    [SerializeField] float speed = 900f;          // �ʴ� �̵� �ȼ� (UI�ϱ� px ���� ����)
    [SerializeField] float maxTravelDistance = 1400f;

    RectTransform _rect;
    Vector2 _spawnPos;
    Vector2 _target;
    Vector2 _direction;
    float _targetDistance;
    bool _initialized;
    bool _hitOnReach;

    Canvas _canvas;
    Camera _uiCamera;

    void Awake()
    {
        _rect = (RectTransform)transform;
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            _uiCamera = _canvas.worldCamera;
    }

    /// <summary>
    /// UI�� �Ѿ� �ʱ�ȭ. target�� ���� Canvas ������ anchoredPosition�� �Ѱ��ִ� �� ���� �����մϴ�.
    /// </summary>
    public void Initialize(Vector2 targetAnchoredPos, bool ifHitOrNot)
    {
        _spawnPos = _rect.anchoredPosition;
        _target = targetAnchoredPos;
        _direction = (_target - _spawnPos).normalized;
        _targetDistance = Vector2.Distance(_spawnPos, _target);
        _hitOnReach = ifHitOrNot;
        _initialized = true;

        if (_direction.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            _rect.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    /// <summary>
    /// ���� ��ǥ�� �޾� UI ĵ���� �������� ��ȯ�ϰ� ���� �� ����ϼ���.
    /// </summary>
    public void InitializeFromWorld(Vector3 targetWorldPos, bool ifHitOrNot)
    {
        Vector2 anchored;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)_rect.parent,
            RectTransformUtility.WorldToScreenPoint(_uiCamera, targetWorldPos),
            _uiCamera,
            out anchored);

        Initialize(anchored, ifHitOrNot);
    }

    void Update()
    {
        if (!_initialized) return;

        _rect.anchoredPosition += _direction * speed * Time.deltaTime;
        float travelled = Vector2.Distance(_spawnPos, _rect.anchoredPosition);

        if (_hitOnReach)
        {
            if (travelled >= _targetDistance)
                Destroy(gameObject);
        }
        else
        {
            if (travelled >= maxTravelDistance)
                Destroy(gameObject);
        }
    }
}
