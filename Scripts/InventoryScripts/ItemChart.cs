using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public interface IItem
{
    ItemType itemType {get; set;}
}
public interface IGun
{
    string Name { get; set; }
    int ActionPoint { get; set; }
    int ShotCountPerTurn { get; set; }
    int ShotDamage { get; set; }
    float AimCorrection { get; set; }
    IReadOnlyList<StatModifier> Modifiers { get; }
}


//총기 클래스들
public class NormalPistol : IGun, IItem
{
    public string Name { get; set; } = "평범한 권총";
    public int ActionPoint { get; set; } = 6;
    public int ShotCountPerTurn { get; set; } = 3;
    public int ShotDamage { get; set; } = 1;
    public float AimCorrection { get; set; } = 10;
    public ItemType itemType { get; set; } = ItemType.Gun;
    public string itemInformation { get; set; }

    static readonly StatModifier[] modifiers =
    {
        new StatModifier { statType = StatId.ShotDamage,  value = +5 },
        new StatModifier { statType = StatId.Hp, value = -3 },
        new StatModifier { statType = StatId.Speed, value =  0 }
    };
    public IReadOnlyList<StatModifier> Modifiers => modifiers;
}
