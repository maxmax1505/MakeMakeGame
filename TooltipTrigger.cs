using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tooltipText;
    public TooltipUI tooltip; // �̱��� �Ǵ� �ν����� ����
    public int enemyIndex = -1;      // ��Ŀ���� �ε����� ���� (��ư�̸� -1)
    public bool isPlayerMarker = false;   // �߰�
    BattleManager manager;           // BattleManager �̱����̳� ����

     
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

