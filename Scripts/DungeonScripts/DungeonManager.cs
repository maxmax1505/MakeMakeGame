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

    [SerializeField] Transform PlayerIcon;
    [SerializeField] public float travelTime = 30;
    public bool reachedDestination = false;

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

            if((timer >= travelTime))
            {
                PlayerIcon.position = EndPoint.transform.position;
                reachedDestination = true;
            }

            yield return null;
        }

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
    }
}
