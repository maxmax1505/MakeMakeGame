using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ItemText;

    IItem currentItem;
    Action<IItem> onClicked;

    ItemListItem owner;

    public void Init(ItemListItem ownerItem)
    {
        owner = ownerItem;
    }


    public void Bind(IItem item, Button slotButton, GameObject slotObject)
    {


        switch (item)
        {
            case IGun gun:

                ItemText.text = $"{gun.Name} / 데미지 : {gun.ShotDamage} × {gun.ShotCountPerTurn}";

                break;

            case BodyPart bodyPart:

                ItemText.text = $"-{bodyPart.name}-\n작용: <color=green>{bodyPart.bonuses[0].statId} {bodyPart.bonuses[0].value}</color>\n부작용: <color=red>{bodyPart.penalties[0].statId}{bodyPart.penalties[0].value}</color>";

                break;
        }

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => owner.UnEqquip(item, slotObject));// 이거 채워야함 ~~
    }

    public void Bind_Inventory(IItem item, Button slotButton, GameObject slotObject)
    {

        switch (item)
        {
            case IGun gun:

                ItemText.text = $"{gun.Name} / 데미지 : {gun.ShotDamage} × {gun.ShotCountPerTurn}";

                break;

            case BodyPart bodyPart:

                ItemText.text = $"-{bodyPart.name}-\n작용: <color=green>{bodyPart.bonuses[0].statId} {bodyPart.bonuses[0].value}</color>\n부작용: <color=red>{bodyPart.penalties[0].statId}{bodyPart.penalties[0].value}</color>";

                break;
        }

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => owner.Eqquip(item, slotObject));
    }
}