using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SpaceBattleManagerScript : MonoBehaviour
{
    public PlayerShip playerShip = new PlayerShip { };

    [Header("게이지 컨텐츠 루트")]
    [SerializeField] Transform speedChanceContent;
    [SerializeField] Transform speedDistanceContent;
    [SerializeField] Transform powerDamageContent;
    [SerializeField] Transform powerChanceContent;
    [SerializeField] Transform evasiveContent;
    [SerializeField] Transform shieldContent;

    [SerializeField] GameObject gaugeSlotPrefab;

    // 게이지 단계(0~3) 추적
    int speedChanceStage = 0;
    int speedDistanceStage = 0;
    int powerDamageStage = 0;
    int powerChanceStage = 0;
    int evasiveStage = 0;
    int shieldStage = 0;

    readonly float[] gaugeMultipliers = { 0.5f, 1f, 1.5f, 2f };

    #region 버튼에서 호출할 메서드
    public void OnClickSpeedChanceUp() =>
        AdjustGauge(ref speedChanceStage, +1, speedChanceContent, UpdateSpeedChance);
    public void OnClickSpeedChanceDown() =>
        AdjustGauge(ref speedChanceStage, -1, speedChanceContent, UpdateSpeedChance);

    public void OnClickSpeedDistanceUp() =>
        AdjustGauge(ref speedDistanceStage, +1, speedDistanceContent, UpdateSpeedDistance);
    public void OnClickSpeedDistanceDown() =>
        AdjustGauge(ref speedDistanceStage, -1, speedDistanceContent, UpdateSpeedDistance);

    public void OnClickPowerDamageUp() =>
        AdjustGauge(ref powerDamageStage, +1, powerDamageContent, UpdatePowerDamage);
    public void OnClickPowerDamageDown() =>
        AdjustGauge(ref powerDamageStage, -1, powerDamageContent, UpdatePowerDamage);

    public void OnClickPowerChanceUp() =>
        AdjustGauge(ref powerChanceStage, +1, powerChanceContent, UpdatePowerChance);
    public void OnClickPowerChanceDown() =>
        AdjustGauge(ref powerChanceStage, -1, powerChanceContent, UpdatePowerChance);

    public void OnClickEvasiveUp() =>
        AdjustGauge(ref evasiveStage, +1, evasiveContent, UpdateEvasive);
    public void OnClickEvasiveDown() =>
        AdjustGauge(ref evasiveStage, -1, evasiveContent, UpdateEvasive);

    public void OnClickShieldUp() =>
        AdjustGauge(ref shieldStage, +1, shieldContent, UpdateShield);
    public void OnClickShieldDown() =>
        AdjustGauge(ref shieldStage, -1, shieldContent, UpdateShield);
    #endregion

    #region 내부 로직
    void AdjustGauge(ref int stage, int delta, Transform contentRoot, Action onChanged)
    {
        int newStage = Mathf.Clamp(stage + delta, 0, 3);
        if (newStage == stage) return;

        stage = newStage;
        onChanged?.Invoke();
        RebuildGaugeSlots(contentRoot, stage);
    }

    void RebuildGaugeSlots(Transform contentRoot, int slotCount)
    {
        if (contentRoot == null || gaugeSlotPrefab == null) return;

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);

        for (int i = 0; i < slotCount; i++)
            Instantiate(gaugeSlotPrefab, contentRoot);
    }

    float MultiplierForStage(int stage) =>
        gaugeMultipliers[Mathf.Clamp(stage, 0, gaugeMultipliers.Length - 1)];
    #endregion

    #region 스탯 갱신
    void UpdateSpeedChance()
    {
        playerShip.current_speed_Chance = playerShip.speed_Chance * MultiplierForStage(speedChanceStage);
    }

    void UpdateSpeedDistance()
    {
        playerShip.current_speed_distance = playerShip.speed_distance * MultiplierForStage(speedDistanceStage);
    }

    void UpdatePowerDamage()
    {
        playerShip.current_power_damage = playerShip.power_damage * MultiplierForStage(powerDamageStage);
    }

    void UpdatePowerChance()
    {
        playerShip.current_power_chance = playerShip.power_chance * MultiplierForStage(powerChanceStage);
    }
    void UpdateEvasive()
    {
        playerShip.current_evasive = playerShip.evasive * MultiplierForStage(evasiveStage);
    }

    void UpdateShield()
    {
        // 기본 실드 + (단계-1) * 게이지당 충전량
        playerShip.current_sheild = playerShip.sheild +
                                    Mathf.Max(0, shieldStage - 1) * playerShip.shield_One;
    }
    #endregion
}
