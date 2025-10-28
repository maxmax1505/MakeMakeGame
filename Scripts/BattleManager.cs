using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public ChoiceManager buttonchoice;
    public ButtonValueSelector buttonValueSelector;
    public TooltipUI tooltipUIInstance;

    [SerializeField] RectTransform uiCanvasRoot;
    [SerializeField] GameObject bulletPrefab;

    public bool ISPlayerNotInBattle = true;
    public bool running;
    public int cachedMoveChoice;
    public int MleeRange = 20;
    public float ShotRateSpeed = 0.3f;

    public static int TargetEnemy_Int;
    bool IsFirstRun = true;

    public int CurrentMovingEnemy_int;

    public List<RectTransform> Markers;
    public List<RectTransform> EndPoints;
    public List<Slider> Sliders;
    public static List<(RectTransform marker, RectTransform endpoint, ICharacter enemies, Slider slider, Slider manaslider)> Enemy_WithMarkers;

    /* �̵�, ��� ���������� UI�� ���� �� ǥ�� */
    public RectTransform minPoint;   // ���� ���� ����� ������ (min)  
    public Slider PlayerSlider;

    public RectTransform marker0;     // enemies[0] ������
    public RectTransform EndPoint0;   // ����   (max)
    public Slider EnemySlider0;
    public RectTransform marker1;
    public RectTransform EndPoint1;
    public Slider EnemySlider1;
    public RectTransform marker2;
    public RectTransform EndPoint2;
    public Slider EnemySlider2;
    public RectTransform marker3;
    public RectTransform EndPoint3;
    public Slider EnemySlider3;
    public RectTransform marker4;
    public RectTransform EndPoint4;
    public Slider EnemySlider4;
    public RectTransform marker5;
    public RectTransform EndPoint5;
    public Slider EnemySlider5;
    public RectTransform marker6;
    public RectTransform EndPoint6;
    public Slider EnemySlider6;
    public RectTransform marker7;
    public RectTransform EndPoint7;
    public Slider EnemySlider7;



    public float gameMin = 0f;       // enemires[].distance�� ���� ������ ���� ��, �ּ� distance
    [SerializeField] public static float gameMax = 200f;     // enemires[].distance�� ���� ������ ���� ��, �ִ� distance

    public Button MleeButton_1;
    public TextMeshProUGUI MleeButtonText_1;
    public Button MleeButton_2;
    public TextMeshProUGUI MleeButtonText_2;
    public Button MleeButton_3;
    public TextMeshProUGUI MleeButtonText_3;
    public Button MleeButton_4;
    public TextMeshProUGUI MleeButtonText_4;




    //������ ĳ���� ��ü �ʱ�ȭ
    ICharacter player;
    List<ICharacter> enemies;
    public List<IGun> guns;

    public enum MoveIntent { Advance = 0, Keep = 1, Retreat = 2 }
    public enum MoveCaseNine { pAeA = 0, pKeK = 1, pReR = 2, pA = 3, pR = 4, eA = 5, eR = 6, exception = -1 }


    void Start()
    {
        //�׽�Ʈ��

        guns = new List<IGun> { new NormalPistol() };
        Debug.Log(guns[0].Name);
        enemies = new List<ICharacter> { new Monster1(guns[0]), new Monster1(guns[0]) };
        IfNameSame();

        player = new PlayerCharacter(guns[0]);

        Enemy_WithMakers_RESTART(enemies);

        minPoint.gameObject.GetComponent<TooltipTrigger>().isPlayerMarker = true;

        //�׽�Ʈ��
    }

    private void Update()
    {

        if (!running && ISPlayerNotInBattle == false) { running = true; StartCoroutine(BattleLoop(enemies)); }
    }

    IEnumerator WaitForSpace()
    {
        // ���� �����ӿ� ���� �ִ� �����̽� �ܻ� ����
        yield return null;

        // '����' ������ ������ ��ٸ���
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        TalkManager.TempTrue = false;
    }

    IEnumerator BattleLoop(List<ICharacter> enemiesList)
    {
        Debug.Log("���� ����");
        ISPlayerNotInBattle = false;

        Enemy_WithMakers_RESTART(enemiesList);

        while (!ISPlayerNotInBattle)
        {
            // 1) �÷��̾� �� (�Է� ���)

            for (int i = 0; i < 8; i++)
            {
                CurrentMovingEnemy_int = i;

                if (Enemy_WithMarkers[i].enemies != null)
                {

                    yield return StartCoroutine(MovingPhase(player, Enemy_WithMarkers[i].enemies));

                    if (Enemy_WithMarkers[i].enemies.Distance < MleeRange)
                    {
                        yield return StartCoroutine(MleePhase(player, Enemy_WithMarkers[i].enemies, i));
                    }
                }
                else
                {
                    continue;
                }
            }

            IsFirstRun = true;

            TalkManager.Instance.ShowTemp("������ �� ���ΰ�?");

            buttonchoice.choicetrue = false;
            buttonchoice.choicewhat = -1;

            buttonchoice.SpawnButtons("�ѱ� ���", "���� ����");
            yield return new WaitUntil(() => buttonchoice.choicetrue);

            if (buttonchoice.choicewhat == 0)
            {
                yield return StartCoroutine(ShotTargetEnemySelect());

                yield return StartCoroutine(ShotingPhase(player, Enemy_WithMarkers[TargetEnemy_Int].enemies));

            }// ���⼭ ���� �б�************
            else if (buttonchoice.choicewhat == 1)
            {
                buttonchoice.choicetrue = false;
                buttonchoice.choicewhat = -1;

                yield return StartCoroutine(MagicPhase(player, enemies[0])); //�ڿ� enemy �μ��� �ƹ��ų� ���� ����
            }

            for (int i = 0; i < 8; i++)
            {
                if (Enemy_WithMarkers[i].enemies != null)
                {
                    yield return StartCoroutine(EnemyShotYourFaceFuck(Enemy_WithMarkers[i].enemies, player, i));
                }
                else
                {
                    continue;
                }
            }
            // ���� ���� üũ
            //if (battleEnd) break;


            // 3) ���� ����
            if (/* ��� óġ */ false) { Debug.Log("Victory"); ISPlayerNotInBattle = true; }
            if (/* ���� ��� */ false) { Debug.Log("Defeat"); ISPlayerNotInBattle = true; }

            // ���� ����(����)
            yield return null;
        }
        Debug.Log("���� ����");
        running = false;
    }

    public IEnumerator MovingPhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy)
    {
        for (int i = 0; i < 8; i++)
        {
            if (Enemy_WithMarkers[i].enemies != null)
            {
                UpdateMarkerForEnemy0(Enemy_WithMarkers[i].enemies.Distance, Enemy_WithMarkers[i].marker, Enemy_WithMarkers[i].endpoint);
            }
            else
            {
                continue;
            }

        }

        if (IsFirstRun == true)
        {
            // ��� ���� �� �ݵ�� �ʱ�ȭ
            buttonchoice.choicetrue = false;
            buttonchoice.choicewhat = -1;

            // ������ ǥ��(UI�� �װ� ����)
            TalkManager.Currenttalk = 2;

            //buttonchoice.SpawnButtons("�Ÿ� ������", "�Ÿ� ����", "�Ÿ� ������", " �׽�Ʈ1", "�׽�Ʈ2", "�׽�Ʈ3", "�׽�Ʈ4");
            buttonchoice.SpawnButtonsWithTooltips(
                new List<string> { "�Ÿ� ������", "�Ÿ� ����", "�Ÿ� ������" },
                new List<string> { "�Ÿ��� ������", "�Ÿ��� �����Ѵ�", "�Ÿ��� ������" });

            yield return new WaitUntil(() => buttonchoice.choicetrue);

            cachedMoveChoice = buttonchoice.choicewhat;

            IsFirstRun = false;
        }



        // �б� ó��
        switch (cachedMoveChoice) //�Ÿ����� ������
        {
            case 0:

                Debug.Log("�Ÿ� ������!");

                yield return StartCoroutine(DoRun(ShouldBePlayer, ShouldBeEnemy, 0));

                break;

            case 1: //�Ÿ� ������

                Debug.Log("�Ÿ� ����!");

                yield return StartCoroutine(DoRun(ShouldBePlayer, ShouldBeEnemy, 1));

                break;

            default:

                Debug.Log("�Ÿ� ������!");

                yield return StartCoroutine(DoRun(ShouldBePlayer, ShouldBeEnemy, 2));

                break;
        }

        // ���� �� ��� �ʱ�ȭ(����)
    }

    public IEnumerator ShotingPhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy)
    {
        // ��� ���� �� �ݵ�� �ʱ�ȭ
        buttonchoice.choicetrue = false;
        buttonchoice.choicewhat = -1;

        // ������ ǥ��(UI�� �װ� ����)
        TalkManager.Currenttalk = 2;

        buttonchoice.SpawnButtons("ǥ�� ���", "���� ���", "���� ���� ���", "���� ���", "���� ���");

        yield return new WaitUntil(() => buttonchoice.choicetrue);

        switch (buttonchoice.choicewhat) //��� ������
        {
            case 0:

                yield return StartCoroutine(DoFuckingShotTheFAce(ShouldBePlayer, ShouldBeEnemy, 0));

                break;

            case 1:

                break;
        }
        //yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator DoRun(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy, int button)
    {
        const int DELTA_ENEMY_KEEP = -1;
        const int DELTA_CONTEST_RETREAT_OR_ADVANCE = 0; // ������/������ �浹
        const int DELTA_PLAYER_KEEP = 1;

        int MoveCases = -1;
        Debug.Log(MoveCases);

        MoveIntent Enemyintent = (MoveIntent)ShouldBeEnemy.CharaterRunAI(ShouldBePlayer, ShouldBeEnemy);

        switch (button)
        {
            case 0: //�Ÿ� ������ ����

                yield return ShowThenWait($"{ShouldBePlayer.Name}�� �Ÿ��� ��������ߴ�.");

                switch (Enemyintent /*�� ai*/ )
                {
                    case MoveIntent.Advance: //��뵵 �Ÿ��� ������ ����

                        MoveCases = 0; //pAeA

                        yield return ShowThenWait("��� ���� �Ÿ��� ������ �ִ�.");

                        break;

                    case MoveIntent.Keep: //���� �Ÿ� ����

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP) == true) //��� �Ÿ� ���� ����
                        {
                            MoveCases = 3; //pA

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}��(��) �Ÿ��� �����Ϸ������� �����ߴ�. Ȯ��:{CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP)}");
                        }
                        else //���� �Ÿ� ����
                        {
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}��(��) �Ÿ��� �����ߴ�! Ȯ��:{CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP)}");
                        }

                        break;

                    case MoveIntent.Retreat: //���� �Ÿ� ������

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE) == true) //��� �Ÿ� ������ ����
                        {
                            MoveCases = 3; //pA

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}��(��) �Ÿ��� ������������ �����ߴ�. Ȯ��:{CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE)}");
                        }
                        else //��� �Ÿ� ������ ����
                        {
                            MoveCases = 6; //eR

                            yield return ShowThenWait($"����� �Ÿ��� �����µ� �����߰�, {ShouldBeEnemy.Name}��(��) �Ÿ��� ���ȴ�! Ȯ��:{CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE)}");
                        }

                        break;
                }

                break;

            case 1: //�Ÿ� ���� ����

                yield return ShowThenWait($"{ShouldBePlayer.Name}�� �Ÿ��� �����ϱ���ߴ�.");

                switch (Enemyintent /*�� ai*/ )
                {
                    case MoveIntent.Advance: // ���� ����, ���� �Ÿ��� ������ ����

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP) == true)
                        { // ��� �Ÿ� ������ ����
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}��(��) �Ÿ��� ������ ������ �����ߴ�. Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP)}");
                        }
                        else // ��� �Ÿ� ������ ����
                        {
                            MoveCases = 5; //eA

                            yield return ShowThenWait($"�Ÿ� ���� ����! ���� �Ÿ��� ������! Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP)}");
                        }

                        yield return ShowThenWait($"{ShouldBePlayer.Name}�� {ShouldBeEnemy.Name}�� �Ÿ��� {ShouldBeEnemy.Distance}(��)�� �Ǿ���!");

                        break;

                    case MoveIntent.Keep: //���� ��� �Ÿ� ����

                        MoveCases = 1; //pKeK

                        yield return ShowThenWait($"{ShouldBeEnemy.Name}���� �Ÿ��� �������̴�");

                        break;

                    case MoveIntent.Retreat: //���� ����, ���� �Ÿ� ������

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP) == true) //��� �Ÿ� ������ ���� => ����
                        {
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}��(��) �Ÿ��� ������������ �����ߴ�. Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP)}");

                        }
                        else   //��� �Ÿ� ������ ����
                        {
                            MoveCases = 6; //eR 

                            yield return ShowThenWait($"�Ÿ� ���� ����! {ShouldBeEnemy.Name}��(��) �Ÿ��� ���ȴ�! Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP)}");

                        }

                        break;
                }

                break;

            case 2: //�Ÿ� ������ ����

                yield return ShowThenWait($"{ShouldBePlayer.Name}�� �Ÿ��� ��������ߴ�.");

                switch (Enemyintent /*�� ai*/ )
                {
                    case MoveIntent.Advance: //���� �Ÿ� ������, ���� �Ÿ��� ������ ����

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE) == true) //��� �Ÿ� ������ ����, �� �Ÿ� ����
                        {
                            MoveCases = 4; //pR

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}��(��) �Ÿ��� ������������ �����ߴ�. Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE)}");
                        }
                        else //��� �Ÿ� ������ ����
                        {
                            MoveCases = 5; //eA

                            yield return ShowThenWait($"�Ÿ� ������ ����! {ShouldBeEnemy.Name}��(��) �Ÿ��� ������! Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE)}");
                        }

                        break;

                    case MoveIntent.Keep: //���� �Ÿ� ����

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP) == true) //��� �Ÿ� ���� ����, �� �Ÿ� ����
                        {
                            MoveCases = 3; // pA

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}��(��) �Ÿ��� �����Ϸ������� �����ߴ�. Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP)}");
                        }
                        else //��� �Ÿ� ����
                        {
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"�Ÿ� ������ ����! {ShouldBeEnemy.Name}��(��) �Ÿ��� �����ߴ�! Ȯ�� : {CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP)}");
                        }

                        break;

                    case MoveIntent.Retreat: //���� �� �Ÿ� ������

                        MoveCases = 2; //pReR

                        yield return ShowThenWait($"{ShouldBeEnemy.Name} ���� �Ÿ��� ���ȴ�!");

                        break;
                }

                break;
        }

        MoveCaseNine moveCaseNine = (MoveCaseNine)MoveCases;

        Debug.Log(moveCaseNine);

        float BeforeDistance = ShouldBeEnemy.Distance;

        ShouldBeEnemy.Distance = CalcDistance(ShouldBePlayer, ShouldBeEnemy, moveCaseNine);

        UpdateMarkerForEnemy0(ShouldBeEnemy.Distance, Enemy_WithMarkers[CurrentMovingEnemy_int].marker, Enemy_WithMarkers[CurrentMovingEnemy_int].endpoint);

        TalkManager.Instance.ShowTemp($"{Mathf.Abs(BeforeDistance - ShouldBeEnemy.Distance)}��ŭ �̵��ߴ�. {ShouldBePlayer.Name}�� {ShouldBeEnemy.Name}�� �Ÿ��� {ShouldBeEnemy.Distance}(��)�� �Ǿ���!");
        yield return StartCoroutine(WaitForSpace());
    }

    public bool VSmeANDyouSPEED(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy, int IsSomeoneKeepDistance)
    {
        float Randx = UnityEngine.Random.Range(0, 100);
        float Speedcheck = CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, IsSomeoneKeepDistance);

        if (Randx <= Speedcheck)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float CalcSpeedChance(ICharacter p, ICharacter e, int keep)
    {
        return 50 + (p.Speed - e.Speed) * 10 + keep * 20;
    }

    public float CalcDistance(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy, MoveCaseNine moveCaseNine)
    {
        float t = Mathf.InverseLerp(gameMin, gameMax, ShouldBeEnemy.Distance);
        t = Mathf.Pow(t, 0.5f); // �� ���ڷ� � �ֱ� (���� ������)
        float multiplier = Mathf.Lerp(1f, 2f, t); //���� ���� ����ġ 1�迡�� ���� 2����� �����Ѵٴ� ��

        switch (moveCaseNine)
        {
            case MoveCaseNine.pAeA: //���� �Ÿ� ����

                ShouldBeEnemy.Distance -= (ShouldBePlayer.Speed + ShouldBeEnemy.Speed) * multiplier;

                break;

            case MoveCaseNine.pKeK: //�Ÿ� ����

                break;

            case MoveCaseNine.pReR: //���� �Ÿ� ����

                ShouldBeEnemy.Distance += (ShouldBePlayer.Speed + ShouldBeEnemy.Speed) * multiplier;

                break;

            case MoveCaseNine.pA: //(����) �Ÿ� ����

                ShouldBeEnemy.Distance -= ShouldBePlayer.Speed * multiplier;

                break;

            case MoveCaseNine.pR: //(����) �Ÿ� ����

                ShouldBeEnemy.Distance += ShouldBePlayer.Speed * multiplier;

                break;

            case MoveCaseNine.eA: //(��밡) �Ÿ� ����

                ShouldBeEnemy.Distance -= ShouldBeEnemy.Speed * multiplier;

                break;

            case MoveCaseNine.eR: //(��밡) �Ÿ� ����

                ShouldBeEnemy.Distance += ShouldBeEnemy.Speed * multiplier;

                break;
        }

        ShouldBeEnemy.Distance = Mathf.Clamp(ShouldBeEnemy.Distance, gameMin, gameMax);

        return Mathf.Round(ShouldBeEnemy.Distance);
    }

    public IEnumerator ShowThenWait(string msg)
    {
        TalkManager.Instance.ShowTemp(msg);
        yield return StartCoroutine(WaitForSpace());
    }

    public float CalcShotChance(ICharacter attacker, ICharacter defender)
    {
        return 40 + attacker.EquipedGun.AimCorrection + attacker.Perception - (defender.Speed + defender.Perception / 2);
    }

    public float PercFactor(ICharacter attacker)
    {
        return 1f + (attacker.Perception - 10) * 0.04f; // ������ 10�� �� 1.0
    }

    public void ShotDamageMethod(ICharacter attacker, ICharacter defender)
    {
        float Damage;

        Damage = (attacker.EquipedGun.ShotDamage + attacker.ShotAtk) * PercFactor(attacker);

        defender.CurrentHp -= Mathf.RoundToInt(Damage);
    }

    public IEnumerator ShotTargetEnemySelect()
    {
        TargetEnemy_Int = -1;
        buttonValueSelector.choiceButtonTrue = false;
        buttonValueSelector.SetBindingsInteractable(true);

        TalkManager.Instance.ShowTemp("������?");

        yield return new WaitUntil(() => buttonValueSelector.choiceButtonTrue);

        Enemy_WithMarkers[TargetEnemy_Int].marker.gameObject.GetComponent<Image>().color = Color.blue;
    }

    public IEnumerator DoFuckingShotTheFAce(ICharacter attacker, ICharacter defender, int buttonclick)
    {

        int HowManyShot = 0;
        float ShotChance = CalcShotChance(attacker, defender);

        int calcdamageX = defender.CurrentHp;
        ShotDamageMethod(attacker, defender);
        calcdamageX = calcdamageX - defender.CurrentHp;
        defender.CurrentHp += calcdamageX;

        for (int i = 0; i < attacker.EquipedGun.ShotCountPerTurn; i++)
        {

            float Randx = UnityEngine.Random.Range(0, 100);
            Enemy_WithMarkers[TargetEnemy_Int].marker.gameObject.GetComponent<Image>().color = Color.white;
            if (Randx <= ShotChance)
            {
                HowManyShot++;

                ShotDamageMethod(attacker, defender);
                defender.CurrentHp = Mathf.Max(0, defender.CurrentHp);
                TalkManager.Instance.ShowTemp($"{i}��° : ����! {attacker.Name}��(��) {defender.Name}���� {calcdamageX} �������� �־���! Ȯ�� : {ShotChance}");
                FireBullet(minPoint, Enemy_WithMarkers[TargetEnemy_Int].marker, true);
                Enemy_WithMarkers[TargetEnemy_Int].marker.gameObject.GetComponent<Image>().color = Color.red;
                Enemy_WithMarkers[TargetEnemy_Int].slider.value = (float)Enemy_WithMarkers[TargetEnemy_Int].enemies.CurrentHp / Enemy_WithMarkers[TargetEnemy_Int].enemies.HP;

                Debug.Log($"{i}��° : ����!");
            }
            else
            {
                TalkManager.Instance.ShowTemp($"{i}��° : ������! {attacker.Name}�� ������ ��������! Ȯ�� : {ShotChance}");
                FireBullet(minPoint, Enemy_WithMarkers[TargetEnemy_Int].marker, false);

                Debug.Log($"{i}��° : ������!");
            }

            yield return new WaitForSeconds(ShotRateSpeed);
            Enemy_WithMarkers[TargetEnemy_Int].marker.gameObject.GetComponent<Image>().color = Color.white;
        }



        yield return ShowThenWait($"{attacker.EquipedGun.ShotCountPerTurn}�� �� {HowManyShot}�� ����! Ȯ�� : {ShotChance} ������ : {calcdamageX * HowManyShot} {defender.Name}�� ���� HP: {defender.CurrentHp}");

    }

    public IEnumerator EnemyShotYourFaceFuck(ICharacter attacker, ICharacter defender, int currentEnemy)
    {
        int calcdamageX = defender.CurrentHp;
        ShotDamageMethod(attacker, defender);
        calcdamageX = calcdamageX - defender.CurrentHp;
        defender.CurrentHp += calcdamageX;

        int HowManyShot = 0;
        float ShotChance = CalcShotChance(attacker, defender);

        for (int i = 0; i < attacker.EquipedGun.ShotCountPerTurn; i++)
        {

            float Randx = UnityEngine.Random.Range(0, 100);

            if (Randx <= ShotChance)
            {
                HowManyShot++;

                ShotDamageMethod(attacker, defender);
                defender.CurrentHp = Mathf.Max(0, defender.CurrentHp);
                TalkManager.Instance.ShowTemp($"{i}��° : ����! {attacker.Name}��(��) {defender.Name}���� {calcdamageX} �������� �־���! Ȯ�� : {ShotChance}");
                FireBullet(Enemy_WithMarkers[currentEnemy].marker, minPoint, true);
                minPoint.gameObject.GetComponent<Image>().color = Color.red;
                PlayerSlider.value = (float)player.CurrentHp / player.HP;

                Debug.Log($"{i}��° : ����!");
            }
            else
            {
                TalkManager.Instance.ShowTemp($"{i}��° : ������! {attacker.Name}�� ������ ��������! Ȯ�� : {ShotChance}");
                FireBullet(Enemy_WithMarkers[currentEnemy].marker, minPoint, false);

                Debug.Log($"{i}��° : ������!");
            }

            yield return new WaitForSeconds(ShotRateSpeed);
            minPoint.gameObject.GetComponent<Image>().color = Color.white;
        }

        yield return ShowThenWait($"{attacker.EquipedGun.ShotCountPerTurn}�� �� {HowManyShot}�� ����! Ȯ�� : {ShotChance} ������ : {calcdamageX * HowManyShot} {defender.Name}�� ���� HP: {defender.CurrentHp}");

    }

    public IEnumerator MleePhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy, int ShouldBeEnemy_int)
    {
        // ��� ���� �� �ݵ�� �ʱ�ȭ
        buttonchoice.choicetrue = false;
        buttonchoice.choicewhat = -1;

        //���� ������
        List<IMlee> PlayerSelectedMleeList = new();
        List<IMlee> EnemySelectedMleeList = new();

        marker0.gameObject.SetActive(false);
        marker1.gameObject.SetActive(false);
        marker2.gameObject.SetActive(false);
        marker3.gameObject.SetActive(false);
        marker4.gameObject.SetActive(false);
        marker5.gameObject.SetActive(false);
        marker6.gameObject.SetActive(false);
        marker7.gameObject.SetActive(false);
        minPoint.gameObject.SetActive(false);

        // ������ ǥ��(UI�� �װ� ����)

        TalkManager.Instance.ShowTemp($"{ShouldBeEnemy.Name}��(��) ��� �ٷ� �տ� �ִ�! ������ ����?");

        List<IMlee> RandomMleePopOut = new List<IMlee>();

        for (int A = 0; A < 4; A++)
        {
            RandomMleePopOut.Add(RandomMleeByWeight(ShouldBePlayer.ActiveSkills)); //����ġ�� ���� �˰��ִ� ��ų �� �ϳ� ����
        }

        List<string> MleeAtkSelect = new();

        for (int B = 0; B < 4; B++)
        {
            MleeAtkSelect.Add(RandomMleePopOut[B].Name);
        }

        //���� ���� �ð�
        for (int i = 0; i < 2; i++)
        {
            buttonchoice.choicetrue = false;

            Debug.Log(MleeAtkSelect.ToArray());

            buttonchoice.SpawnButtons(MleeAtkSelect.ToArray());
            yield return new WaitUntil(() => buttonchoice.choicetrue);

            if (i == 1)
            {
                TalkManager.Instance.ShowTemp($"{ShouldBeEnemy.Name}��(��) ��� �ٷ� �տ� �ִ�! ������ ����?");
            }

            PlayerSelectedMleeList.Add(RandomMleePopOut[buttonchoice.choicewhat]);

            yield return new WaitUntil(() => buttonchoice.choicetrue);

            if (i == 0)
            {
                MleeButton_1.gameObject.SetActive(true);
                MleeButtonText_1.text = PlayerSelectedMleeList[i].Name;
            }
            else
            {
                MleeButton_3.gameObject.SetActive(true);
                MleeButtonText_3.text = PlayerSelectedMleeList[i].Name;

                Debug.Log(MleeAtkSelect[i]);
            }



            yield return ShowThenWait($"����� {PlayerSelectedMleeList[i].Name}��(��) �ߴ�!");

            EnemySelectedMleeList.Add(RandomMleeByWeight(ShouldBeEnemy.ActiveSkills));

            if (i == 0)
            {
                MleeButton_2.gameObject.SetActive(true);
                MleeButtonText_2.text = EnemySelectedMleeList[i].Name;
            }
            else
            {
                MleeButton_4.gameObject.SetActive(true);
                MleeButtonText_4.text = EnemySelectedMleeList[i].Name;
            }
            yield return ShowThenWait($"����� {ShouldBeEnemy.Name}�� �������� �о���! {ShouldBeEnemy.Name}�� {EnemySelectedMleeList[i].Name}��(��) �ߴ�");
        }

        // �÷��̾�, �� ���� �� ���� �ְ�ޱ�
        for (int i = 0; i < 2; i++)
        {
            float RandX = UnityEngine.Random.value * 100;

            float enemyBaseHitPer = EnemySelectedMleeList[i].HitChance(ShouldBeEnemy);
            float playerModHitPer = PlayerSelectedMleeList[0].MleeModifiers[EnemySelectedMleeList[0].Type].HitChancePer *
                                    PlayerSelectedMleeList[1].MleeModifiers[EnemySelectedMleeList[1].Type].HitChancePer;
            int totalEnemyHitPer = Mathf.RoundToInt(enemyBaseHitPer * playerModHitPer);

            if (RandX < totalEnemyHitPer)
            {
                Debug.Log(RandX);
                Debug.Log(totalEnemyHitPer);

                float enemyBase = EnemySelectedMleeList[i].Damage(ShouldBeEnemy);
                float playerMod = PlayerSelectedMleeList[0].MleeModifiers[EnemySelectedMleeList[0].Type].DamagePer *
                                  PlayerSelectedMleeList[1].MleeModifiers[EnemySelectedMleeList[1].Type].DamagePer;
                int totalEnemy = Mathf.RoundToInt(enemyBase * playerMod);

                ShouldBePlayer.CurrentHp -= totalEnemy;
                PlayerSlider.value = (float)ShouldBePlayer.CurrentHp / ShouldBePlayer.HP;

                yield return ShowThenWait($"{ShouldBeEnemy.Name}�� {EnemySelectedMleeList[i].Name}! {ShouldBePlayer.Name}�� {totalEnemy} ���ظ� �Ծ���! Ȯ�� : { totalEnemyHitPer } �� ������:{ enemyBase } ��� ���� : {playerMod}");
            }
            else
            {
                yield return ShowThenWait($"{ShouldBeEnemy.Name}�� {EnemySelectedMleeList[i].Name}�� ��������! Ȯ�� : { totalEnemyHitPer }");
            }

            float RandY = UnityEngine.Random.value * 100;

            float PlayerBaseHitPer = PlayerSelectedMleeList[i].HitChance(ShouldBePlayer);
            float EnemyModHitPer = EnemySelectedMleeList[0].MleeModifiers[PlayerSelectedMleeList[0].Type].HitChancePer *
                                    EnemySelectedMleeList[1].MleeModifiers[PlayerSelectedMleeList[1].Type].HitChancePer;
            int totalPlayerHitPer = Mathf.RoundToInt(PlayerBaseHitPer * EnemyModHitPer);

            if (RandY < totalPlayerHitPer)
            {
                Debug.Log(RandY);
                Debug.Log(totalPlayerHitPer);

                float playerBase = PlayerSelectedMleeList[i].Damage(ShouldBePlayer);
                float enemyMod = EnemySelectedMleeList[0].MleeModifiers[PlayerSelectedMleeList[0].Type].DamagePer *
                                 EnemySelectedMleeList[1].MleeModifiers[PlayerSelectedMleeList[1].Type].DamagePer;
                int totalPlayer = Mathf.RoundToInt(playerBase * enemyMod);

                ShouldBeEnemy.CurrentHp -= totalPlayer;
                Enemy_WithMarkers[ShouldBeEnemy_int].slider.value = (float)ShouldBeEnemy.CurrentHp / ShouldBeEnemy.HP;

                yield return ShowThenWait($"{ShouldBePlayer.Name}�� {PlayerSelectedMleeList[i].Name}! {ShouldBeEnemy.Name}��(��) {totalPlayer} ���ظ� �Ծ���! Ȯ�� : { totalPlayerHitPer } �� ������:{ playerBase } ��� ���� : { enemyMod }");
            }
            else
            {
                yield return ShowThenWait($"{ShouldBePlayer.Name}�� {PlayerSelectedMleeList[i].Name}�� ��������! Ȯ�� : { totalPlayerHitPer }");
            }
        }

        MleeButton_1.gameObject.SetActive(false);
        MleeButton_2.gameObject.SetActive(false);
        MleeButton_3.gameObject.SetActive(false);
        MleeButton_4.gameObject.SetActive(false);

        minPoint.gameObject.SetActive(true);

        for (int i = 0; i < 8; i++)
        {
            if (Enemy_WithMarkers[i].enemies != null)
            {
                Enemy_WithMarkers[i].marker.gameObject.SetActive(true);
            }
            else
            {
                continue;
            }
        }
    }

    public static IMlee RandomMleeByWeight(IList<IMlee> ActiveMlees)
    {
        float Level = 0; //������ ���
        for (int i = 0; i < ActiveMlees.Count; i++)
        {
            float RangedWeight = ActiveMlees[i].ChanceWeight;
            if (RangedWeight > 0) Level += RangedWeight;
        }
        if (Level <= 0) throw new System.InvalidOperationException("No pickable items (all counts <= 0).");

        // [0, total) �������� ���� 1��
        // total�� int ������ ���� �� ������ double�� ���
        double randomvalue = Random.value * Level;
        float acc = 0; //�Ÿ���

        for (int i = 0; i < ActiveMlees.Count; i++)
        {
            float RangedWeight = ActiveMlees[i].ChanceWeight;
            if (RangedWeight <= 0) continue;
            acc += RangedWeight;
            if (randomvalue < acc)
                return ActiveMlees[i];
        }
        // �ε����� ������ ����
        return ActiveMlees[ActiveMlees.Count - 1];
    }

    public IEnumerator MagicPhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy)
    {
        // ��� ���� �� �ݵ�� �ʱ�ȭ
        buttonchoice.choicetrue = false;
        buttonchoice.choicewhat = -1;

        // ������ ǥ��(UI�� �װ� ����)
        TalkManager.Currenttalk = 2;

        List<string> playerSpellList = new();

        foreach (var spells in player.SpellData)
        {
            playerSpellList.Add(spells.Name);
        }

        buttonchoice.SpawnButtons(playerSpellList.ToArray());

        yield return new WaitUntil(() => buttonchoice.choicetrue);

        yield return StartCoroutine(ExecuteSpell(ShouldBePlayer.SpellData[buttonchoice.choicewhat], ShouldBePlayer, ShouldBeEnemy));
    }

    void UpdateMarkerForEnemy0(float dGame, RectTransform marker, RectTransform endpoint) // dGame = method(distance) ���
    {
        // 1) 0~1 ������ ��ȯ (����)
        float t = Mathf.InverseLerp(gameMin, gameMax, dGame);
        t = Mathf.Clamp01(t);

        // 2) UI ����(minPoint��maxPoint) �� ��ġ
        Vector2 A = minPoint.anchoredPosition;
        Vector2 B = endpoint.anchoredPosition;
        Vector2 P = Vector2.Lerp(A, B, t);
        /*
        if(IsFirstMove == true)
        {
            marker.anchoredPosition = P;
            IsFirstMove = false;
        }

        marker.anchoredPosition = Vector2.MoveTowards(marker.anchoredPosition, P, 800f * 0.1f);
        ��ġ�� õõ�� �̵��Ϸ���*/

        marker.anchoredPosition = P;
    }

    void FireBullet(RectTransform origin, RectTransform target, bool hit)
    {
        if (bulletPrefab == null || origin == null || target == null) return;

        GameObject bullet = Instantiate(bulletPrefab, uiCanvasRoot);
        RectTransform bulletRect = bullet.GetComponent<RectTransform>();
        bulletRect.anchoredPosition = origin.anchoredPosition;

        int index = uiCanvasRoot.GetSiblingIndex();
        bulletRect.SetSiblingIndex(index + 2);


        BulletManage bulletManage = bullet.GetComponent<BulletManage>();
        if (bulletManage != null)
        {
            bulletManage.Initialize(target.anchoredPosition, hit);
        }
    }

    public void IfNameSame()
    {
        string[] suffixes = { " A", " B", " C", " D", " E", " F", " G", " H" };
        Dictionary<System.Type, int> counters = new();

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var type = enemy.GetType();
            if (!counters.ContainsKey(type))
                counters[type] = 0;

            int idx = counters[type];
            if (idx < suffixes.Length)
            {
                enemy.Name += suffixes[idx];
                counters[type] = idx + 1;
            }
            else
            {
                enemy.Name += $" ({idx + 1})"; // 8�� ������ ���� ���̴� ������ ���� ó��
            }
        }
    }

    public string GetEnemyTooltip(int index)
    {
        if (index < 0 || index >= Enemy_WithMarkers.Count)
            return string.Empty;

        var enemy = Enemy_WithMarkers[index].enemies;
        if (enemy == null)
            return string.Empty;

        return $"{enemy.Name}\nHP: {enemy.CurrentHp}/{enemy.HP} MP: {enemy.CurrentMp}/{enemy.Mp}\nDistance: {enemy.Distance}";
    }

    public string GetPlayerTooltip()
    {
        return $"{player.Name}\nHP: {player.CurrentHp}/{player.HP}\nDistance: {player.Distance}";
    }

    public void Enemy_WithMakers_RESTART(List<ICharacter> enemiesList)
    {
        Markers = new List<RectTransform> { marker0, marker1, marker2, marker3, marker4, marker5, marker6, marker7 };
        EndPoints = new List<RectTransform> { EndPoint0, EndPoint1, EndPoint2, EndPoint3, EndPoint4, EndPoint5, EndPoint6, EndPoint7 };
        Sliders = new List<Slider> { EnemySlider0, EnemySlider1, EnemySlider2, EnemySlider3, EnemySlider4, EnemySlider5, EnemySlider6, EnemySlider7 };

        marker0.gameObject.SetActive(false);
        marker1.gameObject.SetActive(false);
        marker2.gameObject.SetActive(false);
        marker3.gameObject.SetActive(false);
        marker4.gameObject.SetActive(false);
        marker5.gameObject.SetActive(false);
        marker6.gameObject.SetActive(false);
        marker7.gameObject.SetActive(false);
        MleeButton_1.gameObject.SetActive(false);
        MleeButton_2.gameObject.SetActive(false);
        MleeButton_3.gameObject.SetActive(false);
        MleeButton_4.gameObject.SetActive(false);

        Enemy_WithMarkers = new();

        while (enemiesList.Count < 8)
        {
            enemiesList.Add(null);
        }

        

        for (int i = 0; i < 8; i++)
        {
            Slider manaSlider = Markers[i].GetChild(0)?.GetComponent<Slider>();

            if (enemiesList[i] != null)
            {
                Enemy_WithMarkers.Add((Markers[i], EndPoints[i], enemiesList[i], Sliders[i], manaSlider));
                Enemy_WithMarkers[i].marker.gameObject.SetActive(true);
            }
            else
            {
                Enemy_WithMarkers.Add((Markers[i], EndPoints[i], null, Sliders[i], manaSlider));
            }
        }

        for (int i = 0; i < 8; i++)
        {
            if (Enemy_WithMarkers[i].enemies != null)
            {
                UpdateMarkerForEnemy0(Enemy_WithMarkers[i].enemies.Distance, Enemy_WithMarkers[i].marker, Enemy_WithMarkers[i].endpoint);
            }
            else
            {
                continue;
            }

            var trigger = Enemy_WithMarkers[i].marker.GetComponent<TooltipTrigger>();
            if (trigger != null)
            {

                if (trigger != null)
                {
                    trigger.enemyIndex = i;
                    trigger.tooltip = tooltipUIInstance;
                }
            }

            if (Enemy_WithMarkers[i].enemies != null)
                UpdateMarkerForEnemy0(Enemy_WithMarkers[i].enemies.Distance, Enemy_WithMarkers[i].marker, Enemy_WithMarkers[i].endpoint);
        }
    }

    public void LetsStartBattle()
    {
        ISPlayerNotInBattle = false;
    }

    public RectTransform GetMarkerFor(ICharacter target)
    {
        for (int i = 0; i < Enemy_WithMarkers.Count; i++)
        {
            if (Enemy_WithMarkers[i].enemies == target)
                return Enemy_WithMarkers[i].marker;
        }
        return null;
    }

    public IEnumerator ExecuteSpell(ISpell spell, ICharacter caster, ICharacter defender)
    {
        SpellContext ctx = new SpellContext
        {
            caster = caster,
            Target = defender,
            enemySlots = Enemy_WithMarkers,
            casterIsPlayer = caster == player, // �Ǵ� caster.IsPlayerControlled
            PlayerMarker = minPoint
        };

        yield return StartCoroutine(spell.CAST(ctx));
    }

    public void ApplyModifiers(IEnumerable<StatModifier> modifiers)
    {
        foreach (var mod in modifiers)
        {
            switch (mod.statId)
            {
                case "ü��":
                    player.HP += mod.value;
                    break;
                case "���ǵ�":
                    player.Speed += mod.value;
                    break;
                case "����":
                    player.Perception += mod.value;
                    break;
                case "����":
                    player.Mp += mod.value;
                    break;
                case "��� ������":
                    player.ShotAtk += mod.value;
                    break;
            }
        }
    }


}

