using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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
public interface IMlee
{
    string Name { get; set; }
    MleeATKType Type { get; }
    float ChanceWeight { get; set; }
    float HitChance(ICharacter attacker);
    float Damage(ICharacter attacker); //�ù� ��������

    Dictionary<MleeATKType, MleePlusMinus> MleeModifiers { get; }
}

public class MleeChart : MonoBehaviour
{
    public static List<IMlee> AllMlees = new List<IMlee> { new PowerATK(), new SpeedATK(), new Defence(), new Dodge() }; //�߰��� ����, ���� ����

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
}
