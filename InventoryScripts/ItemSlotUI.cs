using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ItemText;
    [SerializeField] Button button;

    IItem currentItem;
    Action<IItem> onClicked;

    public void Bind(IItem item, Action<IItem> clickHandler)
    {
        currentItem = item;
        onClicked = clickHandler;

        Debug.Log($"[Bind] {name} currentItem={item}");

        switch (item)
        {
            case IGun gun:

                ItemText.text = $"{gun.Name} / 데미지 : {gun.ShotDamage} × {gun.ShotCountPerTurn}";

                break;

            case BodyPart bodyPart:

                ItemText.text = $"-{bodyPart.name}-\n작용: <color=green>{bodyPart.bonuses[0].statId} {bodyPart.bonuses[0].value}</color>\n부작용: <color=red>{bodyPart.penalties[0].statId}{bodyPart.penalties[0].value}</color>";

                break;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Debug.Log($"[Button] clicked {currentItem} from {name}");
            onClicked?.Invoke(currentItem);
        });

        Debug.Log("바인드");
    }

    public void Bind_Inventory(IItem item)
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
    }
}
