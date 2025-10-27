using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tooltipText;
    public TooltipUI tooltip; // 싱글톤 또는 인스펙터 연결
    public int enemyIndex = -1;      // 마커별로 인덱스를 설정 (버튼이면 -1)
    public bool isPlayerMarker = false;   // 추가
    BattleManager manager;           // BattleManager 싱글톤이나 참조

     
    void Awake()
    {
        tooltip = tooltip != null ? tooltip
                 : Object.FindFirstObjectByType<TooltipUI>();

        if (manager == null)
            manager = Object.FindFirstObjectByType<BattleManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string text = tooltipText;

        if (enemyIndex >= 0 && manager != null)
        {
            text = manager.GetEnemyTooltip(enemyIndex);
        }
        else if (isPlayerMarker && manager != null)
        {
            text = manager.GetPlayerTooltip();
        }

        if (string.IsNullOrEmpty(text)) return;
        tooltip.Show(text, transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }








}

