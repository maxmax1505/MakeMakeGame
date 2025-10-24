using System;
using System.Collections.Generic;
using UnityEngine;


public class CharacterChart : MonoBehaviour
{
    public static List<IMlee> AllMlees = new List<IMlee>{ new PowerATK(), new SpeedATK(), new Defence(), new Dodge() }; //추가만 가능, 수정 금지
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
    float Damage(ICharacter attacker); //시발 어지러워

    Dictionary<MleeATKType, MleePlusMinus> MleeModifiers { get; }
}

public struct MleePlusMinus
{
    public float HitChancePer;     // 명중률 보정
    public float DamagePer;     // 데미지 배율
    public float StaminaMinus;    // 스태미나 소모

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

//캐릭터 클래스들
public class PlayerCharacter : ICharacter
{
    [Header("기본 수치들")]

    public string Name { get; set; } = "당신";
    public int HP { get; set; } = 10;
    public int CurrentHp { get; set; } = 10;
    public float ShotAtk { get; set; } = 1;
    public float Speed { get; set; } = 10;
    public float Distance { get; set; } = 0;
    public float Perception { get; set; } = 10;
    public int CharaterRunAI(ICharacter ShouldBePlayer, ICharacter ShouldBeThatEnemy)
    {
        return -1;
    } //미구현

    [Header("장비 장착")]

    public IGun EquipedGun { get; set; }
    public void EquipMethod(IGun gun)
    {
        EquipedGun = gun;
    }

    [Header("스킬 데이터 : 액티브 상태 / 가중치")]

    //스킬데이터,스킬을 알고 있는지, 그 가중치는 얼마인지 써 있다.
    public Dictionary<MleeATKType, (bool Active, float weight)> SkillData { get; set; } = new()
    {
        { MleeATKType.PowerATK, (true, 2.0f) },
        { MleeATKType.SpeedATK, (true, 2.0f) },
        { MleeATKType.Defence, (true, 1.0f) },
        { MleeATKType.Dodge, (true, 2.0f) }
    };
    public List<IMlee> ActiveSkills { get; set; } = new();//알고있는 스킬들의 구현체를 담고 있는 리스트
    public void SkillCheckMethod()
    {
        foreach (var IMleesInList in CharacterChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // 여기서 개별 값 주입
                ActiveSkills.Add(IMleesInList);                  // 소유 리스트에 추가
            }
        }
    }

}

public class Monster1 : ICharacter
{
    [Header("기본 수치들")]

    public string Name { get; set; } = "몬스터1";
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
            return 2; //도망친다
        }
        else
        {
            int Randomx = UnityEngine.Random.Range(0, 2);

            if (Randomx == 1)
            {
                return 0; //거리를 좁힌다.
            }
            else
            {
                return 1; //거리 유지
            }
        }
    }

    [Header("장비 장착")]

    public IGun EquipedGun { get; set; }
    public void EquipMethod(IGun gun)
    {
        EquipedGun = gun;
    }


    [Header("스킬 데이터 : 액티브 상태 / 가중치")]

    //스킬데이터,스킬을 알고 있는지, 그 가중치는 얼마인지 써 있다.
    public Dictionary<MleeATKType, (bool Active, float weight)> SkillData { get; set; } = new()
    {
        { MleeATKType.PowerATK, (true, 1.0f) },
        { MleeATKType.SpeedATK, (true, 1.0f) },
        { MleeATKType.Defence, (true, 1.0f) },
        { MleeATKType.Dodge, (true, 1.0f) }
    };
    public List<IMlee> ActiveSkills { get; set; } = new();//알고있는 스킬들의 구현체를 담고 있는 리스트
    public void SkillCheckMethod()
    {
        foreach (var IMleesInList in CharacterChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // 여기서 개별 값 주입
                ActiveSkills.Add(IMleesInList);                  // 소유 리스트에 추가
            }
        }
    }

}

//총기 클래스들
public class NormalPistol : IGun
{
    public string Name { get; set; } = "평범한 권총";
    public int ActionPoint { get; set; } = 6;
    public int ShotCountPerTurn { get; set; } = 3;
    public int ShotDamage { get; set; } = 1;
    public float AimCorrection { get; set; } = 10;
}

//근접 공격 클래스들

public class PowerATK : IMlee
{
    public string Name { get; set; } = "강공";
    public MleeATKType Type { get; } = MleeATKType.PowerATK;
    public float ChanceWeight { get; set; } = 2f; //선택지 랜덤 출현 가중치
    public float HitChance(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 적중률 산출 공식(미구현)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 데미지 산출 공식(미구현)
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
    public string Name { get; set; } = "속공";
    public MleeATKType Type { get; } = MleeATKType.SpeedATK;
    public float ChanceWeight { get; set; } = 2f;
    public float HitChance(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 적중률 산출 공식(미구현)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 데미지 산출 공식(미구현)
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
    public string Name { get; set; } = "방어";
    public MleeATKType Type { get; } = MleeATKType.Defence;
    public float ChanceWeight { get; set; } = 1f;
    public float HitChance(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 적중률 산출 공식(미구현)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 데미지 산출 공식(미구현)
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
    public string Name { get; set; } = "회피";
    public MleeATKType Type { get; } = MleeATKType.Dodge;
    public float ChanceWeight { get; set; } = 2f;
    public float HitChance(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 적중률 산출 공식(미구현)
    {
        return 50;
    }
    public float Damage(ICharacter attacker) //플레이어 스탯을 이용한 이 공격의 데미지 산출 공식(미구현)
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

