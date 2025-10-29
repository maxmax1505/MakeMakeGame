using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SlotNumber : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI NUMtext;

    public int AllSlotNum = 0;
    public int CurrentSlotNum = 1;

    public void UpdateSlotNum()
    {
        NUMtext.text = $"{CurrentSlotNum} / {AllSlotNum}";
    }
}