using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public enum BodyPartSlot { Head, Body, Arms, Legs }

[Serializable]
public struct StatModifier
{
    public string statId;   // "Perception", "Speed" 등
    public int value;       // +3, -2 …
}

[Serializable]
public class BodyPart : IItem
{
    public BodyPartSlot slot;
    public string name;
    public StatModifier[] bonuses;
    public StatModifier[] penalties;
}
[Serializable]
public struct ModifierOption
{
    public string statId;
    public int minValue;
    public int maxValue;
    public float weight;
}

public class BodyPartGenerator : MonoBehaviour
{
    [SerializeField] List<ModifierOption> headBuffs;
    [SerializeField] List<ModifierOption> headDebuffs;
    [SerializeField] List<ModifierOption> bodyBuffs;
    [SerializeField] List<ModifierOption> bodyDebuffs;
    [SerializeField] List<ModifierOption> legBuffs;
    [SerializeField] List<ModifierOption> legDebuffs;
    [SerializeField] List<ModifierOption> armBuffs;
    [SerializeField] List<ModifierOption> armDebuffs;

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

    public BodyPart GenerateRandomPart()
    {
        BodyPartSlot slot = (BodyPartSlot)UnityEngine.Random.Range(0, 4);

        var (buffTable, debuffTable) = slot switch
        {
            BodyPartSlot.Head => (headBuffs, headDebuffs),
            BodyPartSlot.Body => (bodyBuffs, bodyDebuffs),
            BodyPartSlot.Arms => (armBuffs, armDebuffs),
            _ => (legBuffs, legDebuffs)
        };

        var buff = CreateModifier(PickWeighted(buffTable));
        var debuff = CreateModifier(PickWeighted(debuffTable));

        return new BodyPart
        {
            slot = slot,
            name = $"{slot} Mod Mk.{UnityEngine.Random.Range(100, 999)}",
            bonuses = new[] { buff },
            penalties = new[] { debuff }
        };
    }

    StatModifier CreateModifier(ModifierOption option)
    {
        int value = UnityEngine.Random.Range(option.minValue, option.maxValue + 1);
        return new StatModifier { statId = option.statId, value = value };
    }
}
