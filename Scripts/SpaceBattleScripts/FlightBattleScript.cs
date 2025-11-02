
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlightBattleScript : MonoBehaviour
{
    [SerializeField]Transform PlayerShip;
    [SerializeField]Transform EnemyShip;

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
    public IEnumerator FlightBattle()
    {
        timer = 0f;

        Vector2 playerStart = playerRect.anchoredPosition;
        Vector2 enemyStart = enemyRect.anchoredPosition;

        Vector2 playerTarget = new Vector2(-600f, 0f);
        Vector2 enemyTarget = new Vector2(600f, 0f);

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

        yield return new WaitForSeconds(1f);

        timer = 0f;
        playerStart = playerRect.anchoredPosition;
        enemyStart = enemyRect.anchoredPosition;

        float ranX = Random.Range(0, PlayerFlight.current_speed_Chance + monsterShip.current_speed_Chance);

        if (ranX < PlayerFlight.current_speed_Chance)
        {
            playerTarget = new Vector2(0f, -250f);
            enemyTarget = new Vector2(0f, 250f);
        }
        else
        {
            playerTarget = new Vector2(0f, 250f);
            enemyTarget = new Vector2(0f, -250f);
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
}
