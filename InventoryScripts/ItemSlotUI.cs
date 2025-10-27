using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ItemText;

    public void Bind(IItem item)
    {
        switch (item)
        {
            case IGun gun:

                ItemText.text = $"{gun.Name} / µ¥¹ÌÁö : {gun.ShotDamage} ¡¿ {gun.ShotCountPerTurn}";

                break;
        }
    }
}
