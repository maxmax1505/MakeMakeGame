using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tooltipText;
    public TooltipUI tooltip; // 싱글톤 또는 인스펙터 연결

    public void Awake()
    {
            tooltip = FindObjectOfType<TooltipUI>(true); // 비활성 오브젝트까지 검색
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

