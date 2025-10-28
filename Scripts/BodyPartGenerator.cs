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
    public string statId;   // "Perception", "Speed" ��
    public int value;       // +3, -2 ��
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

    //���������� ������̾� �ϳ��� ����
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
        /* options[^1]�� C# 8���� �߰��� �ε��� ��������, ������ ù ��° ���(�� ������ ���)�� �ǹ��ؿ�. 
         * ���� ���� �ȿ��� �ƹ� �ɼǵ� ��ȯ���� ������ ������ġ�� ������ ��Ҹ� �����ֱ� ���� ���� �̴ϴ�. 
         * (��� ����ġ�� 0 �̻��̶�� ���� �ȿ��� �ݵ�� ������ �Ͼ������, ���� ��Ȳ�� ����� ó���Դϴ�.) */
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
