
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlightBattleScript : MonoBehaviour
{
    [SerializeField]Transform PlayerShip;
    [SerializeField]Transform EnemyShip;

    [SerializeField]BattleManager battleManager;

    [SerializeField] float averageDistance = 400f;

    RectTransform playerRect;
    RectTransform enemyRect;
    float timer;

    IFlight PlayerFlight;
    MonsterShip monsterShip = new MonsterShip { };

    public void Awake()
    {
        playerRect = PlayerShip.GetComponent<RectTransform>();
        enemyRect = EnemyShip.GetComponent<RectTransform>();

        monsterShip.Refresh();
    }
    public void SetPlayer(IFlight player)
    {
        this.PlayerFlight = player;
    }
    public IEnumerator FlightMove()
    {
        timer = 0f;

        Vector2 playerStart = playerRect.anchoredPosition;
        Vector2 enemyStart = enemyRect.anchoredPosition;

        Vector2 playerTarget = new Vector2(0f, -250f);
        Vector2 enemyTarget = new Vector2(0f, 250f);

        Quaternion playerStartRot = playerRect.localRotation;
        Quaternion enemyStartRot = enemyRect.localRotation;

        Quaternion playerTargetRot = playerStartRot * Quaternion.Euler(0f, 0f, -90f);
        Quaternion enemyTargetRot = enemyStartRot * Quaternion.Euler(0f, 0f, -90f);

        while (timer < 3f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 3f);

            playerRect.localRotation = Quaternion.Slerp(playerStartRot, playerTargetRot, t);
            enemyRect.localRotation = Quaternion.Slerp(enemyStartRot, enemyTargetRot, t);
            playerRect.anchoredPosition = Vector2.Lerp(playerStart, playerTarget, t);
            enemyRect.anchoredPosition = Vector2.Lerp(enemyStart, enemyTarget, t);
            yield return null;
        }

        playerRect.anchoredPosition = playerTarget;
        enemyRect.anchoredPosition = enemyTarget;

        yield return new WaitForSeconds(1f);

        timer = 0f;
        playerStart = playerRect.anchoredPosition;
        enemyStart = enemyRect.anchoredPosition;

        float ranX = Random.Range(0, PlayerFlight.current_speed_Chance + monsterShip.current_speed_Chance);


        if (ranX < PlayerFlight.current_speed_Chance)
        {
            playerTarget = new Vector2(-averageDistance + PlayerFlight.current_speed_distance, 0f);
            enemyTarget = new Vector2(averageDistance + monsterShip.speed_distance, 0f);
        }
        else
        { 
            playerTarget = new Vector2(averageDistance + PlayerFlight.current_speed_distance, 0f);
            enemyTarget = new Vector2(-averageDistance + monsterShip.speed_distance, 0f);
        }
        Debug.Log(ranX);

        while (timer < 3f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 3f);

            playerRect.anchoredPosition = Vector2.Lerp(playerStart, playerTarget, t);
            enemyRect.anchoredPosition = Vector2.Lerp(enemyStart, enemyTarget, t);
            yield return null;
        }
        playerRect.anchoredPosition = playerTarget;
        enemyRect.anchoredPosition = enemyTarget;
    }

    public IEnumerator FlightShoot(IFlight attaker, IFlight defender, RectTransform attackerMarker, RectTransform defenderMarker)
    {
        ShotResult meShot = ResolveShot(attaker, defender);

        float remainingDamage = meShot.damage;

        if (meShot.hit == true)
        {
            if (defender.current_sheild > 0f)
            {
                float absorbed = Mathf.Min(defender.current_sheild, remainingDamage);
                defender.current_sheild -= absorbed;
                remainingDamage -= absorbed;
            }
            if (remainingDamage > 0f)
            {
                defender.CurrentHull = Mathf.Max(defender.CurrentHull - Mathf.RoundToInt(remainingDamage), 0);
            }
            FireFlightBullet(attackerMarker, defenderMarker, true);
        }
        else
        {
            FireFlightBullet(attackerMarker, defenderMarker, false);
            Debug.Log("감나빗!");
        }

        yield break;
    }

    public void FireFlightBullet(RectTransform origin, RectTransform target, bool hit)
    {
        battleManager.FireBullet(origin, target, hit);
    }

    public struct ShotResult
    {
        public bool hit;
        public float damage;
        public float hitChance;

        public ShotResult(bool hit, float damage, float hitChance)
        {
            this.hit = hit;
            this.damage = damage;
            this.hitChance = hitChance;
        }
    }

    public ShotResult ResolveShot(IFlight attacker, IFlight defender)
    {
        // 두 기체 사이 실제 거리(원하는 계산식으로 대체 가능)
        float distance = Mathf.Abs(attacker.current_speed_distance - defender.current_speed_distance);

        // power_chance 10f -> 0.5 (=50%)가 되도록 스케일
        float baseHitChance = Mathf.Clamp01(attacker.power_chance * 0.05f);

        // distance == speed_distance일 때 1, 더 가까우면 >1, 멀면 <1
        float distanceFactor = attacker.speed_distance / Mathf.Max(distance, 1f);
        distanceFactor = Mathf.Clamp(distanceFactor, 0.25f, 2f);

        float hitChance = Mathf.Clamp01(baseHitChance * distanceFactor);

        bool isHit = UnityEngine.Random.value <= hitChance;

        // 거리Factor를 그대로 데미지 배율로 사용 (==1이면 1배)
        float damageMultiplier = Mathf.Clamp(distanceFactor, 0.25f, 1.5f);
        float damage = isHit ? attacker.power_damage * damageMultiplier : 0f;

        return new ShotResult(isHit, damage, hitChance);
    }
}
