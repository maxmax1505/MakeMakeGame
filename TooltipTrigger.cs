using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tooltipText;
    public TooltipUI tooltip; // �̱��� �Ǵ� �ν����� ����

    public void Awake()
    {
            tooltip = FindObjectOfType<TooltipUI>(true); // ��Ȱ�� ������Ʈ���� �˻�
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(tooltipText)) return;
        tooltip.Show(tooltipText, transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }
}

