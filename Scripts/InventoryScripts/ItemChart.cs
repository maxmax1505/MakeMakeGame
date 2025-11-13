using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum GunType { Pistol, Shotgun, Sniper }

[System.Serializable]
public class GunProfile
{
    public AnimationCurve damageCurve = AnimationCurve.Linear(0f, 0.8f, 1f, 0.5f);
    public AnimationCurve hitCurve = AnimationCurve.Linear(0f, 0.6f, 1f, 0.4f);
}

public interface IItem
{
    ItemType itemType { get; set; }
}
public interface IGun
{
    string Name { get; set; }
    GunType gunType { get; set; }
    int ActionPoint { get; set; }
    int ShotCountPerTurn { get; set; }
    int ShotDamage { get; set; }
    float AimCorrection { get; set; }
    GunProfile Profile { get; }
    IReadOnlyList<StatModifier> Modifiers { get; }
}


//총기 클래스들
public class NormalPistol : IGun, IItem
{
    public NormalPistol()
    {
        UpdatProfile();
    }

    public string Name { get; set; } = "평범한 권총";
    public GunType gunType { get; set; } = GunType.Pistol;
    public int ActionPoint { get; set; } = 6;
    public int ShotCountPerTurn { get; set; } = 3;
    public int ShotDamage { get; set; } = 1;
    public float AimCorrection { get; set; } = 10;
    public ItemType itemType { get; set; } = ItemType.Gun;
    public string itemInformation { get; set; }

    public GunProfile Profile { get; private set; } = new GunProfile();
    void UpdatProfile()
    {
        Profile.damageCurve = new AnimationCurve(
        new Keyframe(0f, 0.6f), // 근거리
        new Keyframe(0.5f, 1.2f), // 중거리 최고점
        new Keyframe(1f, 0.5f)  // 장거리에서 다시 하락
        );

        Profile.hitCurve = new AnimationCurve(
        new Keyframe(0f, 0.6f), // 근거리
        new Keyframe(0.5f, 1.2f), // 중거리 최고점
        new Keyframe(1f, 0.5f)  // 장거리에서 다시 하락
        );
    }

    static readonly StatModifier[] modifiers =
    {
        new StatModifier { statType = StatId.ShotDamage,  value = +5 },
        new StatModifier { statType = StatId.Hp, value = -3 },
        new StatModifier { statType = StatId.Speed, value =  0 }
    };
    public IReadOnlyList<StatModifier> Modifiers => modifiers;
}
