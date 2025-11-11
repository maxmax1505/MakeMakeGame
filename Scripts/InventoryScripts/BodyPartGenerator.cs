using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public enum BodyPartSlot { Head, Body, Arms, Legs }
// └─ 파츠가 장착될 슬롯 구분용 열거형


[Serializable]
public struct StatModifier
{
    public string statName;   // 영향을 줄 스탯 이름(예: "Perception")
    public StatId statType;
    public int value;       // 증감 수치(+/-)
}

[Serializable]
public class BodyPart : IItem
{
    public BodyPartSlot slot;      // 어느 슬롯에 장착되는 파츠인지
    public string name;            // 파츠 이름
    public StatModifier[] bonuses; // 파츠가 제공하는 버프 목록
    public StatModifier[] penalties; // 파츠가 주는 디버프 목록
    public ItemType itemType { get; set; }
}

[Serializable]
public class SlotStatPool
{
    public BodyPartSlot slot;

    [Header("버프 가능한 스탯")]
    public StatId[] buffStats;

    [Header("디버프 가능한 스탯")]
    public StatId[] debuffStats;

    public AnimationCurve buffCurve = AnimationCurve.Linear(0, 2, 1, 12);
    public AnimationCurve debuffCurve = AnimationCurve.Linear(0, 1, 1, 8);
}


[Serializable]
public struct ModifierOption
{
    public string statNm;   // 후보 스탯 이름
    public StatId statid;
    public int minValue;    // 증감 최소값
    public int maxValue;    // 증감 최대값
    public float weight;    // 등장 확률 가중치
}



public class BodyPartGenerator : MonoBehaviour
{
    public UniversManager universManager;
    public ItemListItem itemListItem;

    [SerializeField] SlotStatPool[] slotPools;

    /*
    // 슬롯별 버프/디버프 후보 테이블
    [SerializeField] List<ModifierOption> headBuffs;
    [SerializeField] List<ModifierOption> headDebuffs;
    [SerializeField] List<ModifierOption> bodyBuffs;
    [SerializeField] List<ModifierOption> bodyDebuffs;
    [SerializeField] List<ModifierOption> legBuffs;
    [SerializeField] List<ModifierOption> legDebuffs;
    [SerializeField] List<ModifierOption> armBuffs;
    [SerializeField] List<ModifierOption> armDebuffs;
    */

    //누적합으로 모디파이어 하나를 뽑음
    ModifierOption PickWeighted(List<ModifierOption> options)
    {
        float total = 0f;
        foreach (var opt in options) total += opt.weight;
        float r = UnityEngine.Random.value * total;
        foreach (var opt in options)
        {
            r -= opt.weight;
            if (r <= 0f) return opt;
        }
        return options[^1];
        /* options[^1]는 C# 8에서 추가된 인덱서 문법으로, 끝에서 첫 번째 요소(즉 마지막 요소)를 의미해요. 
         * 만약 루프 안에서 아무 옵션도 반환되지 않으면 안전장치로 마지막 요소를 돌려주기 위해 붙인 겁니다. 
         * (사실 가중치가 0 이상이라면 루프 안에서 반드시 선택이 일어나겠지만, 예외 상황을 대비한 처리입니다.) */
    }

    public BodyPart GenerateRandomPart(int Lev)
    {
        float risk = universManager.ComputeRisk(Lev);

        BodyPartSlot slot = (BodyPartSlot)UnityEngine.Random.Range(0, 4);
        SlotStatPool pool = slotPools[(int)slot];

        var buff = CreateModifier(pool, risk, isBuff: true);
        var debuff = CreateModifier(pool, risk, isBuff: false);

        return new BodyPart
        {
            slot = slot,
            name = $"{slot} Mod Mk.{UnityEngine.Random.Range(100, 999)}",
            bonuses = new[] { buff },
            penalties = new[] { debuff },
            itemType = slot switch
            {
                BodyPartSlot.Head => ItemType.Head,
                BodyPartSlot.Body => ItemType.Body,
                BodyPartSlot.Arms => ItemType.Arm,
                _ => ItemType.Leg
            }
        };

        #region 과거의 잔재
        /*
        var (buffTable, debuffTable) = slot switch
        {
            BodyPartSlot.Head => (headBuffs, headDebuffs),
            BodyPartSlot.Body => (bodyBuffs, bodyDebuffs),
            BodyPartSlot.Arms => (armBuffs, armDebuffs),
            _ => (legBuffs, legDebuffs)
        };

        // └─ 0~3 중 하나를 뽑아 슬롯 결정

        // 결정된 슬롯에 맞는 버프/디버프 후보 테이블 선택

        // 후보 중에서 실제 버프/디버프 뽑아서 StatModifier 생성
        var buff = CreateModifier(PickWeighted(buffTable));
        var debuff = CreateModifier(PickWeighted(debuffTable));

        // 완성된 파츠 객체 반환 (버프/디버프는 배열 형태)
        return new BodyPart
        {
            slot = slot,
            name = $"{slot} Mod Mk.{UnityEngine.Random.Range(100, 999)}",
            bonuses = new[] { buff },
            penalties = new[] { debuff },
            itemType = slot switch
            {
                BodyPartSlot.Head => ItemType.Head,
                BodyPartSlot.Body => ItemType.Body,
                BodyPartSlot.Arms => ItemType.Arm,
                _ => ItemType.Leg
            }
        };
        */
        #endregion
    }

    /*
    StatModifier CreateModifier(ModifierOption option) // 옵션의 min~max 범위에서 실제 수치 1개를 뽑아 StatModifier 생성
    {
        int value = UnityEngine.Random.Range(option.minValue, option.maxValue + 1);
        return new StatModifier { statName = option.statNm, statType = option.statid , value = value };
    }
    */

    StatModifier CreateModifier(SlotStatPool pool, float risk, bool isBuff)
    {
        StatId[] candidates = isBuff ? pool.buffStats : pool.debuffStats;
        StatId stat = candidates[UnityEngine.Random.Range(0, candidates.Length)];

        AnimationCurve curve = isBuff ? pool.buffCurve : pool.debuffCurve;
        int value = Mathf.RoundToInt(curve.Evaluate(risk));
        if (!isBuff) value = -Mathf.Abs(value);

        return new StatModifier
        {
            statType = stat,
            statName = itemListItem.GetStatLabel(stat),
            value = value
        };
    }
}
