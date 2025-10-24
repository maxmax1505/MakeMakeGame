using System;
using System.Collections.Generic;
using UnityEngine;


public class CharacterChart : MonoBehaviour
{
    public static List<IMlee> AllMlees = new List<IMlee>{ new PowerATK(), new SpeedATK(), new Defence(), new Dodge() }; //�߰��� ����, ���� ����
}

public interface ICharacter
{
    string Name { get; }
    int HP { get; set; }
    int CurrentHp { get; set; }
    float ShotAtk { get; set; }
    float Speed { get; set; }
    float Distance { get; set; }
    float Perception { get; set; }
    IGun EquipedGun { get; set; }
    void EquipMethod(IGun gun);
    int CharaterRunAI(ICharacter ShouldBePlayer, ICharacter ShouldBeThatEnemy);

    Dictionary<MleeATKType, (bool Active, float weight)> SkillData { get; set; }
    List<IMlee> ActiveSkills { get; set; }
    void SkillCheckMethod();
}

public interface IGun
{
    string Name { get; set; }
    int ActionPoint { get; set; }
    int ShotCountPerTurn { get; set; }
    int ShotDamage { get; set; }
    float AimCorrection { get; set; }
}

public interface IMlee
{
    string Name { get; set; }
    MleeATKType Type { get; }
    float ChanceWeight { get; set; }
    float HitChance(ICharacter attacker);
    float Damage(ICharacter attacker); //�ù� ��������

    Dictionary<MleeATKType, MleePlusMinus> MleeModifiers { get; }
}

public struct MleePlusMinus
{
    public float HitChancePer;     // ���߷� ����
    public float DamagePer;     // ������ ����
    public float StaminaMinus;    // ���¹̳� �Ҹ�

    public MleePlusMinus(float hit, float dmg, float stam)
    {
        this.HitChancePer = hit;
        this.DamagePer = dmg;
        this.StaminaMinus = stam;
    }

    public override string ToString() => $"(Hit:{HitChancePer}, Dmg:{DamagePer}, Stam:{StaminaMinus})";
}

public enum MleeATKType
{
    PowerATK = 0,
    SpeedATK = 1,
    Defence = 2,
    Dodge = 3
}




// Start is called once before the first execution of Update after the MonoBehaviour is created



[Serializable]

//ĳ���� Ŭ������
public class PlayerCharacter : ICharacter
{
    [Header("�⺻ ��ġ��")]

    public string Name { get; set; } = "���";
    public int HP { get; set; } = 10;
    public int CurrentHp { get; set; } = 10;
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
        foreach (var IMleesInList in CharacterChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // ���⼭ ���� �� ����
                ActiveSkills.Add(IMleesInList);                  // ���� ����Ʈ�� �߰�
            }
        }
    }

}

public class Monster1 : ICharacter
{
    [Header("�⺻ ��ġ��")]

    public string Name { get; set; } = "����1";
    public int HP { get; set; } = 10;
    public int CurrentHp { get; set; } = 10;
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
        foreach (var IMleesInList in CharacterChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // ���⼭ ���� �� ����
                ActiveSkills.Add(IMleesInList);                  // ���� ����Ʈ�� �߰�
            }
        }
    }

}

//�ѱ� Ŭ������
public class NormalPistol : IGun
{
    public string Name { get; set; } = "����� ����";
    public int ActionPoint { get; set; } = 6;
    public int ShotCountPerTurn { get; set; } = 3;
    public int ShotDamage { get; set; } = 1;
    public float AimCorrection { get; set; } = 10;
}

//���� ���� Ŭ������

public class PowerATK : IMlee
{
    public string Name { get; set; } = "����";
    public MleeATKType Type { get; } = MleeATKType.PowerATK;
    public float ChanceWeight { get; set; } = 2f; //������ ���� ���� ����ġ
    public float HitChance(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ���߷� ���� ����(�̱���)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ������ ���� ����(�̱���)
    {
        return 10;
    }

    public Dictionary<MleeATKType, MleePlusMinus> MleeModifiers { get; } = new()
    {
        { MleeATKType.SpeedATK, new MleePlusMinus(1.5f, 1.5f, 1f) },
        { MleeATKType.PowerATK, new MleePlusMinus(1f, 0.8f, 1f) },
        { MleeATKType.Defence, new MleePlusMinus(2f, 0f, 1f) },
        { MleeATKType.Dodge, new MleePlusMinus(1.5f, 0f, 1f) }
    };
}
public class SpeedATK : IMlee
{
    public string Name { get; set; } = "�Ӱ�";
    public MleeATKType Type { get; } = MleeATKType.SpeedATK;
    public float ChanceWeight { get; set; } = 2f;
    public float HitChance(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ���߷� ���� ����(�̱���)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ������ ���� ����(�̱���)
    {
        return 10;
    }

    public Dictionary<MleeATKType, MleePlusMinus> MleeModifiers { get; } = new()
    {
        { MleeATKType.SpeedATK, new MleePlusMinus(1.0f, 1.0f, 1f) },
        { MleeATKType.PowerATK, new MleePlusMinus(2f, 1.2f, 1f) },
        { MleeATKType.Defence, new MleePlusMinus(0.4f, 0f, 1f) },
        { MleeATKType.Dodge, new MleePlusMinus(0.2f, 0f, 1f) }
    };
}
public class Defence : IMlee
{
    public string Name { get; set; } = "���";
    public MleeATKType Type { get; } = MleeATKType.Defence;
    public float ChanceWeight { get; set; } = 1f;
    public float HitChance(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ���߷� ���� ����(�̱���)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ������ ���� ����(�̱���)
    {
        return 10;
    }

    public Dictionary<MleeATKType, MleePlusMinus> MleeModifiers { get; } = new()
    {
        { MleeATKType.SpeedATK, new MleePlusMinus(2f, 0.4f, 1f) },
        { MleeATKType.PowerATK, new MleePlusMinus(1f, 0.6f, 1f) },
        { MleeATKType.Defence, new MleePlusMinus(1f, 0f, 1f) },
        { MleeATKType.Dodge, new MleePlusMinus(1f, 0f, 1f) }
    };
}
public class Dodge : IMlee
{
    public string Name { get; set; } = "ȸ��";
    public MleeATKType Type { get; } = MleeATKType.Dodge;
    public float ChanceWeight { get; set; } = 2f;
    public float HitChance(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ���߷� ���� ����(�̱���)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //�÷��̾� ������ �̿��� �� ������ ������ ���� ����(�̱���)
    {
        return 10;
    }

    public Dictionary<MleeATKType, MleePlusMinus> MleeModifiers { get; } = new()
    {
        { MleeATKType.SpeedATK, new MleePlusMinus(0.5f, 1f, 1f) },
        { MleeATKType.PowerATK, new MleePlusMinus(0.3f, 1f, 1f) },
        { MleeATKType.Defence, new MleePlusMinus(1f, 0f, 1f) },
        { MleeATKType.Dodge, new MleePlusMinus(0.4f, 0f, 1f) }
    };
}

