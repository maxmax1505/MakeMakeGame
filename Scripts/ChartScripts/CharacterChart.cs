using System;
using System.Collections.Generic;
using UnityEngine;


public interface ICharacter
{
    #region 여러가지 스탯
    string Name { get; set; }
    int Level { get; set; }
    int DropitemLV { get; set; }
    int HP { get; set; }
    int CurrentHp { get; set; }
    int MP { get; set; }
    int CurrentMp { get; set; }
    int AP { get; set; }
    int CurrentAp { get; set; }
    float WillPower { get; set; }
    float base_WillPower { get; set; }
    float ShotAtk { get; set; }
    float base_ShotAtk { get; set; }
    float Speed { get; set; }
    float base_Speed { get; set; }
    float Distance { get; set; }
    float base_Distance { get; set; }
    float Perception { get; set; }
    float base_Perception { get; set; }
    float Crit { get; set; }
    float base_Crit { get; set; }
    float CritResist { get; set; }
    float base_CritResist { get; set; }
    #endregion

    #region 총 관련
    IGun EquipedGun { get; set; }
    void EquipMethod(IGun gun);
    void Initialize(IGun gun);
    #endregion

    #region 애니메이션 관련
    GameObject anime { get; set; }
    int animeIndex { get; set; }
    #endregion

    int CharaterRunAI(ICharacter ShouldBePlayer, ICharacter ShouldBeThatEnemy);
    (int DropWhat, int DropCount) DropItem { get; set; }

    List<ISpell> SpellData { get; set; }
    Dictionary<MleeATKType, (bool Active, float weight)> SkillData { get; set; }

    List<IMlee> ActiveSkills { get; set; }

    void SkillCheckMethod();

}

[Serializable]


//캐릭터 클래스들
public class PlayerCharacter : ICharacter
{
    public static PlayerCharacter player { get; set; }
    public PlayerCharacter(IGun equipgun)
    {
        Initialize(equipgun);

        if (player != null)
            throw new System.InvalidOperationException("PlayerCharacter is already created.");

        player = this;
        // 나머지 초기화
    }

    #region 기본 수치들
    [Header("기본 수치들")]
    public string Name { get; set; } = "당신";
    public int Level { get; set; } = 1;
    public int DropitemLV { get; set; }
    public int HP { get; set; } = 30;
    public int CurrentHp { get; set; } = 10;
    public int MP { get; set; } = 10;
    public int CurrentMp { get; set; } = 10;
    public int AP { get; set; } = 10;
    public int CurrentAp { get; set; } = 10;
    public float WillPower { get; set; } = 3;
    public float base_WillPower { get; set; } = 3;
    public float ShotAtk { get; set; } = 1;
    public float base_ShotAtk { get; set; } = 1;
    public float Speed { get; set; } = 10;
    public float base_Speed { get; set; } = 10;
    public float Distance { get; set; } = 0;
    public float base_Distance { get; set; } = 0;
    public float Perception { get; set; } = 10;
    public float base_Perception { get; set; } = 10;
    public float Crit { get; set; } = 10;
    public float base_Crit { get; set; } = 10;
    public float CritResist { get; set; } = 10;
    public float base_CritResist { get; set; } = 10;

    #endregion

    public int CharaterRunAI(ICharacter ShouldBePlayer, ICharacter ShouldBeThatEnemy)
    {
        return -1;
    } //미구현
    public (int DropWhat, int DropCount) DropItem { get; set; }

    #region 총기 관련
    [Header("장비 장착")]

    public IGun EquipedGun { get; set; }
    public void EquipMethod(IGun gun)
    {
        EquipedGun = gun;
    }
    #endregion

    #region 애니메이션 관련
    public GameObject anime { get; set; }
    public int animeIndex { get; set; }
    #endregion

    [Header("스킬 데이터 : 액티브 상태 / 가중치")]

    //스킬데이터,스킬을 알고 있는지, 그 가중치는 얼마인지 써 있다.
    public List<ISpell> SpellData { get; set; } = new() { new MagicSpellChart.Lightening(), new MagicSpellChart.MindShetter() };
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
        foreach (var IMleesInList in MleeChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // 여기서 개별 값 주입
                ActiveSkills.Add(IMleesInList);                  // 소유 리스트에 추가
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
    public Monster1()
    {
        Initialize(new NormalPistol());
        DropitemLV = UnityEngine.Random.Range(1, 6);
        Debug.Log(DropitemLV);
        Distance = UnityEngine.Random.Range(70, 151);
    }
    float Round(float value, int decimals = 2)
    {
        float factor = Mathf.Pow(10f, decimals);
        return Mathf.Round(value * factor) / factor;
    }
    /*
    void ApplyLevel(float risk)
    {
        HP = CurrentHp = Mathf.RoundToInt(HP * (1f + risk * 0.1f));
        MP = CurrentMp = Mathf.RoundToInt(MP * (1f + risk * 0.15f));
        ShotAtk = Round(base_ShotAtk * (1f + risk * 0.12f));
        Speed = base_Speed * (1f + risk * 0.08f); //round를 모두 적용할것
        Perception = base_Perception * (1f + risk * 0.10f);
        Crit = base_Crit * (1f + risk * 0.12f);
        CritResist = base_CritResist * (1f + risk * 0.10f);
        WillPower = 3f + risk * 0.2f;
        Distance = UnityEngine.Random.Range(70, 151);
        //Distance = Mathf.Lerp(50f, 20f, risk);   // 고레벨일수록 더 가까운 거리에서 시작 등
    }
    */
    #region 기본 수치들

    public string Name { get; set; } = "몬스터";
    public int Level { get; set; } = 1;
    public int DropitemLV { get; set; }
    public int HP { get; set; } = 10;
    public int CurrentHp { get; set; } = 10;
    public int MP { get; set; } = 10;
    public int CurrentMp { get; set; } = 10;
    public int AP { get; set; }
    public int CurrentAp { get; set; }
    public float WillPower { get; set; } = 3;
    public float base_WillPower { get; set; } = 3;
    public float ShotAtk { get; set; } = 1;
    public float base_ShotAtk { get; set; } = 1;
    public float Speed { get; set; } = 10;
    public float base_Speed { get; set; } = 10;
    public float Distance { get; set; } = 50;
    public float base_Distance { get; set; } = 50;
    public float Perception { get; set; } = 10;
    public float base_Perception { get; set; } = 10;
    public float Crit { get; set; } = 10;
    public float base_Crit { get; set; } = 10;
    public float CritResist { get; set; } = 10;
    public float base_CritResist { get; set; } = 10;

    #endregion

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

    public (int DropWhat, int DropCount) DropItem { get; set; } = new(1, 1);

    #region 장비 장착

    public IGun EquipedGun { get; set; }
    public void EquipMethod(IGun gun)
    {
        EquipedGun = gun;
    }
    #endregion

    #region 애니메이션 관련
    public GameObject anime { get; set; }
    public int animeIndex { get; set; } = 0;
    #endregion

    #region 스킬 데이터 : 액티브 상태 / 가중치

    //스킬데이터,스킬을 알고 있는지, 그 가중치는 얼마인지 써 있다.
    public List<ISpell> SpellData { get; set; } = new() { new MagicSpellChart.Lightening() };
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
        foreach (var IMleesInList in MleeChart.AllMlees)
        {
            var d = SkillData[IMleesInList.Type];          // (has, weight)
            if (d.Active == true)
            {
                IMleesInList.ChanceWeight = d.weight;    // 여기서 개별 값 주입
                ActiveSkills.Add(IMleesInList);                  // 소유 리스트에 추가
            }
        }
    }
    #endregion

    public void Initialize(IGun gun)
    {
        EquipMethod(gun);
        SkillCheckMethod();
        this.anime = AnimationList.enemyAnimations[this.animeIndex];
    }
}




//근접 공격 클래스들



