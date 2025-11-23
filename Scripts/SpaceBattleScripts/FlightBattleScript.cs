
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlightBattleScript : MonoBehaviour
{
    [SerializeField]Transform PlayerShip;
    [SerializeField]Transform EnemyShip;

    [SerializeField] Transform PshipIcon;
    [SerializeField] Transform StartPoint;
    [SerializeField] Transform EndPoint;

    [SerializeField] GameObject EnemyListBox;
    [SerializeField] GameObject BattleListBox;
    [SerializeField] GameObject EnemyListIconPrefab;

    [SerializeField]BattleManager battleManager;

    [SerializeField] Slider PlayerShipHPSlide;
    [SerializeField] Slider PlayerShipShieldSlide;

    [SerializeField] float averageDistance = 400f;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] RectTransform uiCanvasRoot;

    RectTransform playerRect;
    RectTransform enemyRect;

    [SerializeField] RectTransform PlayerRightGun;
    [SerializeField] RectTransform PlayerLeftGun;

    public bool reached = false;
    public bool InSpaceBattle = false;

    float timer;
    Coroutine chaseShake;
    Coroutine ChaseShaketoY;
    enum ChaseResult { None, PlayerAhead, EnemyAhead }
    ChaseResult lastChase = ChaseResult.None;

    IFlight PlayerFlight;
    MonsterShip monsterShip = new MonsterShip { };
    List<(IFlight listship, GameObject listicon)> EneF_InListBox = new();
    List<(IFlight battleship, GameObject battleicon)> Enef_InBattleBox = new();
    public List<IFlight> EneFList = new();

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
    public IEnumerator TravelInSpace()
    {
        reached = false;
        float timer = 0f;
        float currentTimer = 0f;
        float DungeonEncounter = Random.Range(0.5f, 3f);
        float travelTime = 30f;
        GameObject TargetEneIcon = null;
        int TargetEnemyFlight_int;

        while (!reached)
        {
            timer += Time.deltaTime;
            currentTimer = timer;
            PshipIcon.position = Vector3.Lerp(StartPoint.position, EndPoint.position, timer / travelTime);

            if (timer >= DungeonEncounter)
            {
                if (EnemyListBox.transform.childCount > 0)
                {
                    GameObject EneIcon = EneF_InListBox[0].listicon.gameObject;
                    EneIcon.transform.SetParent(BattleListBox.transform);
                    Enef_InBattleBox.Add(EneF_InListBox[0]);
                    EneF_InListBox.RemoveAt(0);
                    TargetEneIcon = Enef_InBattleBox[0].battleicon.gameObject;
                }
                GameObject ficon = Instantiate(EnemyListIconPrefab, EnemyListBox.transform);
                EneF_InListBox.Add((EneFList[0], ficon));
                DungeonEncounter = currentTimer + Random.Range(3f, 8f);
            }

            if (!InSpaceBattle && TargetEneIcon != null)
            {
                InSpaceBattle = true;
                RectTransform IconRect = TargetEneIcon.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
                StartCoroutine(SpaceBattle(IconRect));
            }

            if ((timer >= travelTime))
            {
                PshipIcon.position = EndPoint.transform.position;
                reached = true;
            }

            yield return null;
        }

        yield break;
    }
    public IEnumerator SpaceBattle(RectTransform EnemyShipIcon)
    {
        bool playerturn = true;
        for (int i = 0; i < 20; i++)
        {
            if (playerturn == true)
            {
                if (i % 2 == 0)
                {
                    yield return FlightShoot(PlayerFlight, monsterShip, PlayerRightGun, EnemyShipIcon);
                }
                else
                {
                    yield return FlightShoot(PlayerFlight, monsterShip, PlayerLeftGun, EnemyShipIcon);
                }
                UpdateEnemySlide(EnemyShipIcon, monsterShip);

                if (i == 9)
                {
                    playerturn = false;
                    yield return new WaitForSeconds(0.2f);
                }
            }
            else if (playerturn == false)
            {
                yield return FlightShoot(monsterShip, PlayerFlight, EnemyShipIcon, playerRect);
                UpdatePlayerSlide();
            }

            yield return new WaitForSeconds(0.1f);
        }
        InSpaceBattle = false;
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

        /*
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 1f);

            playerRect.localRotation = Quaternion.Slerp(playerStartRot, playerTargetRot, t);
            enemyRect.localRotation = Quaternion.Slerp(enemyStartRot, enemyTargetRot, t);
            playerRect.anchoredPosition = Vector2.Lerp(playerStart, playerTarget, t);
            enemyRect.anchoredPosition = Vector2.Lerp(enemyStart, enemyTarget, t);
            yield return null;
        }
        */

        playerRect.anchoredPosition = playerTarget;
        enemyRect.anchoredPosition = enemyTarget;

        yield return new WaitForSeconds(1f);

        while (monsterShip.CurrentHull != 0)
        {
            timer = 0f;
            playerStart = playerRect.anchoredPosition;
            enemyStart = enemyRect.anchoredPosition;

            float ranX = Random.Range(0, PlayerFlight.current_speed_Chance + monsterShip.current_speed_Chance);

            ChaseResult currentChase;
            if (ranX < PlayerFlight.current_speed_Chance)
            {
                currentChase = ChaseResult.PlayerAhead;
                playerTarget = new Vector2(-averageDistance + PlayerFlight.current_speed_distance, 0f);
                enemyTarget = new Vector2(averageDistance + monsterShip.speed_distance, 0f);
            }
            else
            {
                currentChase = ChaseResult.EnemyAhead;
                playerTarget = new Vector2(averageDistance + PlayerFlight.current_speed_distance, 0f);
                enemyTarget = new Vector2(-averageDistance + monsterShip.speed_distance, 0f);
            }

            bool skipMove = (currentChase == lastChase && lastChase != ChaseResult.None);

            if (!skipMove)
            {
                while (timer < 1.5f)
                {
                    timer += Time.deltaTime;
                    float t = Mathf.Clamp01(timer / 1.5f);
                    playerRect.anchoredPosition = Vector2.Lerp(playerStart, playerTarget, t);
                    enemyRect.anchoredPosition = Vector2.Lerp(enemyStart, enemyTarget, t);
                    if (lastChase == ChaseResult.None)
                    {
                        playerRect.localRotation = Quaternion.Slerp(playerStartRot, playerTargetRot, t);
                        enemyRect.localRotation = Quaternion.Slerp(enemyStartRot, enemyTargetRot, t);
                    }
                    yield return null;
                }

                playerRect.anchoredPosition = playerTarget;
                enemyRect.anchoredPosition = enemyTarget;
            }

            lastChase = currentChase;

            playerRect.anchoredPosition = playerTarget;
            enemyRect.anchoredPosition = enemyTarget;

            if (ranX < PlayerFlight.current_speed_Chance)
            {
                // 플레이어가 추격 성공 → 적 흔들기 시작
                if (chaseShake != null) StopCoroutine(chaseShake);
                chaseShake = StartCoroutine(BounceUI(enemyRect, 30f, 0.8f));
                if (ChaseShaketoY != null) StopCoroutine(ChaseShaketoY);
                ChaseShaketoY = StartCoroutine(BounceUItoY(playerRect, 30f, 0.8f));
            }
            else
            {
                // 적이 추격 성공 → 플레이어 흔들기
                if (chaseShake != null) StopCoroutine(chaseShake);
                chaseShake = StartCoroutine(BounceUI(playerRect, 30f, 0.8f));
                if (ChaseShaketoY != null) StopCoroutine(ChaseShaketoY);
                ChaseShaketoY = StartCoroutine(BounceUItoY(enemyRect, 30f, 0.8f));
            }

            bool playerturn = true;
            for (int i = 0; i < 20; i++)
            {
                if (playerturn == true)
                {
                    if (i % 2 == 0)
                    {
                        yield return FlightShoot(PlayerFlight, monsterShip, PlayerRightGun, enemyRect);
                    }
                    else
                    {
                        yield return FlightShoot(PlayerFlight, monsterShip, PlayerLeftGun, enemyRect);
                    }
                    UpdateEnemySlide(EnemyShip, monsterShip);

                    if (i == 9)
                    {
                        playerturn = false;
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                else if (playerturn == false)
                {
                    yield return FlightShoot(monsterShip, PlayerFlight, enemyRect, playerRect);
                    UpdatePlayerSlide();
                }

                yield return new WaitForSeconds(0.1f);
            }

            if (chaseShake != null) StopCoroutine(chaseShake);
            if (ChaseShaketoY != null) StopCoroutine(ChaseShaketoY);
        }
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
            FireBullet(attackerMarker, defenderMarker, true);
        }
        else
        {
            FireBullet(attackerMarker, defenderMarker, false);
        }

        yield break;
    }

    public void FireBullet(RectTransform origin, RectTransform target, bool hit)
    {
        if (bulletPrefab == null || origin == null || target == null) return;

        GameObject bullet = Instantiate(bulletPrefab, uiCanvasRoot);
        RectTransform bulletRect = bullet.GetComponent<RectTransform>();

        // origin → 월드 좌표 → 캔버스 로컬 좌표
        Vector3 worldPos = origin.TransformPoint(origin.rect.center); // 축 중심 기준
        Vector3 localPos = uiCanvasRoot.InverseTransformPoint(worldPos);
        bulletRect.localPosition = localPos;

        // 필요하면 회전/스케일도 맞춰 주기
        bulletRect.localRotation = origin.rotation;
        bulletRect.localScale = Vector3.one;

        BulletManage bulletManage = bullet.GetComponent<BulletManage>();
        if (bulletManage != null)
        {
            Vector3 targetWorld = target.TransformPoint(target.rect.center);
            Vector3 targetLocal = uiCanvasRoot.InverseTransformPoint(targetWorld);
            bulletManage.Initialize(targetLocal, hit);
        }
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
        // 기본 전투 거리는 800. 추격자는 자기 speed_distance 만큼 앞으로 붙고,
        // 방어자는 자기 speed_distance 만큼 뒤로 빠지므로 실제 거리 = 800 ± 두 기체의 이동량 차이.
        float distance = Mathf.Abs(averageDistance*2 - attacker.current_speed_distance + defender.current_speed_distance);

        // power_chance 10 → 0.5 (=50%)가 되도록 스케일링.
        float baseHitChance = attacker.power_chance * 0.05f;

        // 실제 거리가 800일 때 bias = 1 → 최종 명중률 50%.
        // 거리가 가까우면 (>1), 멀면 (<1).
        float distanceBias = Mathf.Clamp(distance == 0f ? 2f : averageDistance*2 / distance, 0.25f, 2f);

        // 방어자의 회피율이 높을수록 명중률을 깎는다.
        float defenderEvasion = Mathf.Clamp01(defender.evasive);          // 0 ~ 1
        float evasionFactor = Mathf.Lerp(1f, 0.5f, defenderEvasion);     // 회피 0% → 1, 회피 100% → 0.5

        // 최종 명중률. 거리/회피 둘 다 반영.
        float hitChance = Mathf.Clamp01(baseHitChance * distanceBias * evasionFactor);
        bool isHit = UnityEngine.Random.value <= hitChance;

        // 거리 bias를 데미지 배율에도 사용해, 가까우면 강해지고 멀면 약해지도록.
        float damageMultiplier = Mathf.Clamp(distanceBias, 0.25f, 1.75f);

        // 방어자의 실드 수치가 높을수록 피해 감소.
        float shieldFactor = Mathf.Lerp(1.1f, 0.7f, Mathf.Clamp01(defender.sheild / 100f));

        float damage = isHit
            ? attacker.power_damage * damageMultiplier * shieldFactor
            : 0f;

        return new ShotResult(isHit, damage, hitChance);
    }

    public void UpdatePlayerSlide()
    {
        PlayerShipHPSlide.value = Mathf.InverseLerp(0, PlayerFlight.Hull, PlayerFlight.CurrentHull);
        PlayerShipShieldSlide.value = Mathf.InverseLerp(0, PlayerFlight.sheild, PlayerFlight.current_sheild);
    }

    public void UpdateEnemySlide(Transform enemyMarker, IFlight enemyFlight)
    {
        enemyMarker.GetChild(0).GetComponent<Slider>().value = Mathf.InverseLerp(0, enemyFlight.Hull, enemyFlight.CurrentHull);
        enemyMarker.GetChild(1).GetComponent<Slider>().value = Mathf.InverseLerp(0, enemyFlight.sheild, enemyFlight.current_sheild);
    }

    public IEnumerator BounceUI(RectTransform target, float amplitude, float period)
    {
        if (target == null) yield break;

        Vector2 origin = target.anchoredPosition;
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime;
            float offset = Mathf.Sin((elapsed / period) * Mathf.PI * 2f) * amplitude;
            target.anchoredPosition = origin + Vector2.up * offset;
            yield return null;
        }
    }

    public IEnumerator BounceUItoY(RectTransform target, float amplitude, float period)
    {
        if (target == null) yield break;

        Vector2 origin = target.anchoredPosition;
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime;
            float offset = Mathf.Cos((elapsed / period) * Mathf.PI * 2f) * amplitude;
            target.anchoredPosition = origin + Vector2.up * offset;
            yield return null;
        }
    }
}
