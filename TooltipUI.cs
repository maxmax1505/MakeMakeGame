using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tooltipLabel;
    [SerializeField] RectTransform background;
    RectTransform _rect;

    void Awake() => _rect = (RectTransform)transform;

    public void Show(string message, RectTransform target)
    {
        tooltipLabel.text = message;
        gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        // ���콺 ��ġ�� target ��ġ�� ���� �̵�
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        _rect.position = screenPos + new Vector2(10f, -10f);
        background.position = _rect.position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
