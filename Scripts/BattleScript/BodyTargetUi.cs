using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class BodyTargetUi : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;

    [SerializeField] GameObject Headtooltip;
    [SerializeField] GameObject Arm1tooltip;
    [SerializeField] GameObject Arm2tooltip;
    [SerializeField] GameObject Bodytooltip;
    [SerializeField] GameObject Legstooltip;

    public void HeadTargeting()
    {
        battleManager.TargetPart = BodyPartSlot.Head;

        battleManager.IsTargetPartYes = true;
    }
    public void ArmTargeting()
    {
        battleManager.TargetPart = BodyPartSlot.Arms;

        battleManager.IsTargetPartYes = true;
    }
    public void BodyTargeting()
    {
        battleManager.TargetPart = BodyPartSlot.Body;

        battleManager.IsTargetPartYes = true;
    }
    public void LegTargeting()
    {
        battleManager.TargetPart = BodyPartSlot.Legs;

        battleManager.IsTargetPartYes = true;
    }

    public Color BodyColor(int partsHP)
    {
        switch (partsHP)
        {
            case 3:
                return Color.green;
            case 2:
                return Color.yellow;
            case 1:
                return Color.orange;
            case 0:
                return Color.red;
            default:
                Debug.Log(partsHP);
                return Color.white;
        }
    }

    public void Update_BodyTooltip(ICharacter character)
    {
        var map = new Dictionary<GameObject, int>
        {
            { Headtooltip, character.Head_Hp },
            { Arm1tooltip, character.Arm_Hp },
            { Arm2tooltip, character.Arm_Hp },
            { Bodytooltip, character.Body_Hp },
            { Legstooltip, character.Leg_Hp },
        };

        foreach (var Bodyttip in map)
        {
            var go = Bodyttip.Key;
            int hp = Bodyttip.Value;

            go.SetActive(true);
            go.GetComponent<Image>().color = BodyColor(hp);
        }
    }

    public void Disable_BodyTooltipUi()
    {
        List<GameObject> PartTooltips = new() { Headtooltip, Arm1tooltip, Arm2tooltip, Bodytooltip, Legstooltip };

        foreach (GameObject parttool in PartTooltips)
        {
            parttool.SetActive(false);
        }
    }
}
