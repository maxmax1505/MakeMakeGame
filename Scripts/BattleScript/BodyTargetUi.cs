using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class BodyTargetUi : MonoBehaviour
{
    [SerializeField]BattleManager battleManager;

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
}
