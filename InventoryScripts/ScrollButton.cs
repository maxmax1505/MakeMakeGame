using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ScrollButton : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;

    [SerializeField] float itemHeight = 160f;

    [SerializeField] SlotNumber ThisSlot;

    float Step =>
        itemHeight / Mathf.Max(1f,
            scrollRect.content.rect.height - scrollRect.viewport.rect.height);
    public void ScrollUp()
    {
        scrollRect.verticalNormalizedPosition =
            Mathf.Clamp01(scrollRect.verticalNormalizedPosition + Step);

        if (ThisSlot.CurrentSlotNum != 1)
        {
            ThisSlot.CurrentSlotNum--;
        }

        ThisSlot.UpdateSlotNum();
    }

    public void ScrollDown()
    {
        scrollRect.verticalNormalizedPosition =
            Mathf.Clamp01(scrollRect.verticalNormalizedPosition - Step);

        Debug.Log(ThisSlot);
        Debug.Log(ThisSlot.CurrentSlotNum);

        if (ThisSlot.CurrentSlotNum != ThisSlot.AllSlotNum)
        {
            ThisSlot.CurrentSlotNum++;
        }

        ThisSlot.UpdateSlotNum();
    }
}
