using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public enum BodyPartSlot { Head, Body, Arms, Legs }
// ���� ������ ������ ���� ���п� ������

[Serializable]
public struct StatModifier
{
    public string statId;   // ������ �� ���� �̸�(��: "Perception")
    public int value;       // ���� ��ġ(+/-)
}

[Serializable]
public class BodyPart : IItem
{
    public BodyPartSlot slot;      // ��� ���Կ� �����Ǵ� ��������
    public string name;            // ���� �̸�
    public StatModifier[] bonuses; // ������ �����ϴ� ���� ���
    public StatModifier[] penalties; // ������ �ִ� ����� ���
    public ItemType itemType { get; set; }
}

[Serializable]
public struct ModifierOption
{
    public string statId;   // �ĺ� ���� �̸�
    public int minValue;    // ���� �ּҰ�
    public int maxValue;    // ���� �ִ밪
    public float weight;    // ���� Ȯ�� ����ġ
}

public class BodyPartGenerator : MonoBehaviour
{
    // ���Ժ� ����/����� �ĺ� ���̺�
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

        // ���� 0~3 �� �ϳ��� �̾� ���� ����

        // ������ ���Կ� �´� ����/����� �ĺ� ���̺� ����

        // �ĺ� �߿��� ���� ����/����� �̾Ƽ� StatModifier ����
        var buff = CreateModifier(PickWeighted(buffTable));
        var debuff = CreateModifier(PickWeighted(debuffTable));

        // �ϼ��� ���� ��ü ��ȯ (����/������� �迭 ����)
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
    }

    StatModifier CreateModifier(ModifierOption option) // �ɼ��� min~max �������� ���� ��ġ 1���� �̾� StatModifier ����
    {
        int value = UnityEngine.Random.Range(option.minValue, option.maxValue + 1);
        return new StatModifier { statId = option.statId, value = value };
    }
}
