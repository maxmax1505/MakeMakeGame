using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] Transform StartPoint;
    [SerializeField] Transform EndPoint;
    [SerializeField] BattleManager battleManager;
    [SerializeField] SceneChanger sceneChanger;
    [SerializeField] UniversManager universManager;

    [SerializeField] GameObject restPopup;
    [SerializeField] Slider HpBar;
    [SerializeField] Button restYesButton;
    [SerializeField] Button restNoButton;

    public enum DungeonChoice { Go, Rest, Run };

    [SerializeField] Transform PlayerIcon;
    [SerializeField] public float travelTime = 30;
    public bool reachedDestination = false;

    public ICharacter DungeonPlayer;

    public bool NotInBattle = false;

    public IEnumerator TravelInDungeon()
    {
        float timer = 0f;
        float currentTimer = 0f;
        float DungeonEncounter = Random.Range(3f, 8f);

        while (!reachedDestination)
        {
            timer += Time.deltaTime;
            currentTimer = timer;
            PlayerIcon.position = Vector3.Lerp(StartPoint.position, EndPoint.position, timer / travelTime);

            if (timer >= DungeonEncounter)
            {
                NotInBattle = false;

                yield return StartCoroutine(StartBattle());

                DungeonEncounter = currentTimer + Random.Range(3f, 8f);
            }

            if ((timer >= travelTime))
            {
                PlayerIcon.position = EndPoint.transform.position;
                reachedDestination = true;
            }

            yield return null;
        }

        yield break;
    }

    public IEnumerator RestInDungeon()
    {
        float timer = 0f;
        float restDuration = 5f;                       // 휴식 시간
        float targetHp = Mathf.Min(DungeonPlayer.CurrentHp + DungeonPlayer.HP / 2f, DungeonPlayer.HP);
        float healSpeed = DungeonPlayer.HP / 10f;
        float encounterTime = Random.Range(3f, 5f);    // 휴식 중 전투 시점
        float IsIncountered = UnityEngine.Random.value;
        float chp = DungeonPlayer.CurrentHp;

        while (timer < restDuration)
        {
            timer += Time.deltaTime;

            chp = Mathf.MoveTowards(chp, targetHp, healSpeed * Time.deltaTime);
            DungeonPlayer.CurrentHp = Mathf.RoundToInt(chp);
            Debug.Log(DungeonPlayer.CurrentHp);
            Debug.Log(DungeonPlayer.HP);

            if (IsIncountered <= 0.3 && timer >= encounterTime)
            {
                NotInBattle = false;
                yield return StartCoroutine(StartBattle());
                // 전투 후 휴식 계속할지 결정
                yield break;
            }

            yield return null;
        }

        // 5초 휴식 후 종료
        yield break;
    }

    public IEnumerator StartBattle()
    {
        int monLev = 1;
        float riSK = universManager.ComputeRisk(monLev);
        battleManager.enemies = new List<ICharacter> { new Monster1(monLev, riSK), new Monster1(monLev, riSK) };
        battleManager.IfNameSame();
        sceneChanger.DungeonToBattle();
        yield return new WaitUntil(() => NotInBattle);
        yield return StartCoroutine(AskRestChoice());
    }

    public IEnumerator AskRestChoice()
    {
        DungeonChoice? wantRest = null;
        restPopup.SetActive(true);

        restYesButton.onClick.RemoveAllListeners();
        restYesButton.onClick.AddListener(() => wantRest = DungeonChoice.Rest);

        restNoButton.onClick.RemoveAllListeners();
        restNoButton.onClick.AddListener(() => wantRest = DungeonChoice.Go);

        // 선택이 들어올 때까지 대기
        yield return new WaitUntil(() => wantRest.HasValue);

        restPopup.SetActive(false);

        switch (wantRest.Value)
        {
            case DungeonChoice.Rest:

                yield return StartCoroutine(RestInDungeon());
                break;

            case DungeonChoice.Go:

                break;
        }
    }
}
