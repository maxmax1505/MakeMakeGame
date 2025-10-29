using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    string Name { get; set; }
    int HP { get; set; }
    int CurrentHp { get; set; }
    int MP { get; set; }
    int CurrentMp { get; set; }
    int AP { get; set; }
    int CurrentAp { get; set; }
    float WillPower { get; set; }
    float ShotAtk { get; set; }
    float Speed { get; set; }
    float Distance { get; set; }
    float Perception { get; set; }

    IGun EquipedGun { get; set; }
    void EquipMethod(IGun gun);
    int CharaterRunAI(ICharacter ShouldBePlayer, ICharacter ShouldBeThatEnemy);

    List<ISpell> SpellData { get; set; }
    Dictionary<MleeATKType, (bool Active, float weight)> SkillData { get; set; }

    List<IMlee> ActiveSkills { get; set; }

    void SkillCheckMethod();
    void Initialize(IGun gun);
}

[Serializable]

//ĳ���� Ŭ������
public class PlayerCharacter : ICharacter
{
    public static PlayerCharacter player { get; set; }
    public PlayerCharacter(IGun equipgun)
    {
        Initialize(equipgun);

        if (player != null)
            throw new System.InvalidOperationException("PlayerCharacter is already created.");

        player = this;
        // ������ �ʱ�ȭ
    }

    [Header("�⺻ ��ġ��")]

    public string Name { get; set; } = "���";
    public int HP { get; set; } = 10;
    public int CurrentHp { get; set; } = 10;
    public int MP { get; set; } = 10;
    public int CurrentMp { get; set; } = 10;
    public int AP { get; set; } = 10;
    public int CurrentAp { get; set; } = 10;
    public float WillPower { get; set; } = 3;
    public float ShotAtk { get; set; } = 1;
    public float Speed { get; set; } = 10;
    public float Distance { get; set; } = 0;
    public float Perception { get; set; } = 10;

    public int CharaterRunAI(ICharacter ShouldBePlayer, ICharacter ShouldBeThatEnemy)
    {
        return -1;
    } //�̱���

    [Header("��� ����")]

    public IGun EquipedGun { get; set; }
    public void EquipMethod(IGun gun)
    {
        EquipedGun = gun;
    }

    [Header("��ų ������ : ��Ƽ�� ���� / ����ġ")]

    //��ų������,��ų�� �˰� �ִ���, �� ����ġ�� ������ �� �ִ�.
    public List<ISpell> SpellData { get; set; } = new() { new MagicSpellChart.Lightening() , new MagicSpellChart.MindShetter() };
    public Dictionary<MleeATKType, (bool Active, float weight)> SkillData { get; set; } = new()
    {
        { MleeATKType.PowerATK, (true, 2.0f) },
        { MleeATKType.SpeedATK, (true, 2.0f) },
        { MleeATKType.Defence, (true, 1.0f) },
        { MleeATKType.Dodge, (true, 2.0f) }
    };
    public List<IMlee> ActiveSkills { get; set; } = new();//�˰��ִ� ��ų���� ����ü�� ��� �ִ� ����Ʈ
    public void SkillCheckMethod()
    {
        foreach (var IMleesInList in MleeChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // ���⼭ ���� �� ����
                ActiveSkills.Add(IMleesInList);                  // ���� ����Ʈ�� �߰�
            }
        }
    }

    public void Initialize(IGun gun)
    {
        SkillCheckMethod();
        EquipMethod(gun);
    }

}

public class Monster1 : ICharacter
{
    public Monster1(IGun gun)
    {
        Initialize(gun);
    }

    [Header("�⺻ ��ġ��")]

    public string Name { get; set; } = "����";
    public int HP { get; set; } = 10;
    public int CurrentHp { get; set; } = 10;
    public int MP { get; set; } = 10;
    public int CurrentMp { get; set; } = 10;
    public int AP { get; set; }
    public int CurrentAp { get; set; }
    public float WillPower { get; set; } = 3;
    public float ShotAtk { get; set; } = 1;
    public float Speed { get; set; } = 10;
    public float Distance { get; set; } = 50;
    public float Perception { get; set; } = 10;


    public int CharaterRunAI(ICharacter ShouldBePlayer, ICharacter ShouldBeThatEnemy)
    {
        int CHP = ShouldBeThatEnemy.CurrentHp;
        int Hp = ShouldBeThatEnemy.HP;

        if (CHP <= Hp / 2f)
        {
            return 2; //����ģ��
        }
        else
        {
            int Randomx = UnityEngine.Random.Range(0, 2);

            if (Randomx == 1)
            {
                return 0; //�Ÿ��� ������.
            }
            else
            {
                return 1; //�Ÿ� ����
            }
        }
    }

    [Header("��� ����")]

    public IGun EquipedGun { get; set; }
    public void EquipMethod(IGun gun)
    {
        EquipedGun = gun;
    }


    [Header("��ų ������ : ��Ƽ�� ���� / ����ġ")]

    //��ų������,��ų�� �˰� �ִ���, �� ����ġ�� ������ �� �ִ�.
    public List<ISpell> SpellData { get; set; } = new() { new MagicSpellChart.Lightening() };
    public Dictionary<MleeATKType, (bool Active, float weight)> SkillData { get; set; } = new()
    {
        { MleeATKType.PowerATK, (true, 1.0f) },
        { MleeATKType.SpeedATK, (true, 1.0f) },
        { MleeATKType.Defence, (true, 1.0f) },
        { MleeATKType.Dodge, (true, 1.0f) }
    };
    public List<IMlee> ActiveSkills { get; set; } = new();//�˰��ִ� ��ų���� ����ü�� ��� �ִ� ����Ʈ
    public void SkillCheckMethod()
    {
        foreach (var IMleesInList in MleeChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // ���⼭ ���� �� ����
                ActiveSkills.Add(IMleesInList);                  // ���� ����Ʈ�� �߰�
            }
        }
    }

    public void Initialize (IGun gun)
    {
        EquipMethod(gun);
        SkillCheckMethod();
    }
}



//���� ���� Ŭ������



