using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlightChart : MonoBehaviour
{
}

public interface IFlight
{
    #region 스탯
    string name { get; set; }
    int Hull { get; set; }
    int CurrentHull { get; set; }
    float speed_Chance { get; set; }
    float speed_distance { get; set; }
    float power_damage { get; set; }
    float power_chance { get; set; }
    float evasive { get; set; }
    float sheild { get; set; }
    float current_sheild { get; set; }
    float shield_One { get; set; }
    float current_speed_Chance { get; set; }
    float current_speed_distance { get; set; }
    float current_power_damage { get; set; }
    float current_power_chance { get; set; }
    float current_evasive { get; set; }
    #endregion
    void Refresh(); 
}

public class PlayerShip : IFlight
{
    #region 플레이어 스탯
    public string name { get; set; } = "플레이어 기함";
    public int Hull { get; set; } = 100;
    public int CurrentHull { get; set; } = 100;
    public float speed_Chance { get; set; } = 10f;
    public float speed_distance { get; set; } = 100f;
    public float power_damage { get; set; } = 2f;
    public float power_chance { get; set; } = 10f;
    public float evasive { get; set; } = 10f;
    public float sheild { get; set; } = 0;
    public float current_sheild { get; set; } = 0;
    public float shield_One { get; set; } = 10f;
    public float current_speed_Chance { get; set; }
    public float current_speed_distance { get; set; }
    public float current_power_damage { get; set; }
    public float current_power_chance { get; set; }
    public float current_evasive { get; set; }
    #endregion
    public void Refresh()
    {
        CurrentHull = Hull;

        current_speed_Chance = speed_Chance;
        current_speed_distance = speed_distance;

        current_power_damage = power_damage;
        current_power_chance = power_chance;

        current_evasive = evasive;

        current_sheild = sheild;   // 기본 실드로 초기화
                                   // shield_One은 게이지당 충전량이므로 초기화하지 않고 보존
    }
}

public class MonsterShip : IFlight
{
    #region 플레이어 스탯
    public string name { get; set; } = "적 기함";
    public int Hull { get; set; } = 100;
    public int CurrentHull { get; set; } = 100;
    public float speed_Chance { get; set; } = 10f;
    public float speed_distance { get; set; } = 100f;
    public float power_damage { get; set; } = 2f;
    public float power_chance { get; set; } = 10f;
    public float evasive { get; set; } = 10f;
    public float sheild { get; set; } = 0;
    public float current_sheild { get; set; } = 0;
    public float shield_One { get; set; } = 10f;
    public float current_speed_Chance { get; set; }
    public float current_speed_distance { get; set; }
    public float current_power_damage { get; set; }
    public float current_power_chance { get; set; }
    public float current_evasive { get; set; }
    #endregion
    public void Refresh()
    {
        CurrentHull = Hull;

        current_speed_Chance = speed_Chance;
        current_speed_distance = speed_distance;

        current_power_damage = power_damage;
        current_power_chance = power_chance;

        current_evasive = evasive;

        current_sheild = sheild;   // 기본 실드로 초기화
                                   // shield_One은 게이지당 충전량이므로 초기화하지 않고 보존
    }
}