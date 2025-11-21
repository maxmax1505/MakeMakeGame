using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    #region 여러가지 선언
    public ChoiceManager buttonchoice;
    public ButtonValueSelector buttonValueSelector;
    public TooltipUI tooltipUIInstance;
    public ItemListItem itemListItem;
    public BodyPartGenerator PG;
    public SceneChanger sceneChanger;
    public UniversManager universManager;
    public DungeonManager dungeonManager;
    public BodyTargetUi bodyTargetUi;

    [SerializeField] RectTransform uiCanvasRoot;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject laserPrefab;

    public bool ISPlayerNotInBattle = true;
    public bool running;
    public bool ApCheck = false;
    public int cachedMoveChoice;
    public int MleeRange = 30;
    public float ShotRateSpeed = 0.3f;

    public static int TargetEnemy_Int;
    public BodyPartSlot TargetPart;
    [SerializeField] GameObject BodyTragetingBox;
    [SerializeField] GameObject TargetingAlarmBox;
    [SerializeField] GameObject HitOrNotAlarmBox;
    public bool IsTargetPartYes = false;
    bool IsFirstRun = true;

    /// <summary>
    /// 로딩중에 꼬여서 임시로 만든거
    /// </summary>
    public bool FuckYouFuckYou = false;

    public int CurrentMovingEnemy_int;

    public List<RectTransform> Markers;
    public List<RectTransform> EndPoints;
    public List<Slider> Sliders;
    public static List<(RectTransform marker, RectTransform endpoint, ICharacter enemies, Slider slider, Slider manaslider, GameObject animate)> Enemy_WithMarkers;

    /* 이동, 사격 시퀀스에서 UI에 나와 적 표시 */
    public RectTransform minPoint;   // 가상 직선 경로의 시작점 (min)  
    public Slider PlayerSlider;
    public Slider ApSlider;
    public TextMeshProUGUI ApText;

    public float gameMin = 0f;       // enemires[].distance의 선형 보간을 위한 수, 최소 distance
    /// <summary>
    /// 최대 거리
    /// </summary>
    [SerializeField] public static float gameMax = 250f;     // enemires[].distance의 선형 보간을 위한 수, 최대 distance

    public enum MoveIntent { Advance = 0, Keep = 1, Retreat = 2 }
    public enum MoveCaseNine { pAeA = 0, pKeK = 1, pReR = 2, pA = 3, pR = 4, eA = 5, eR = 6, exception = -1 }

    //실행전 캐릭터 객체 초기화
    ICharacter player;
    public List<ICharacter> enemies;
    public List<IGun> guns;
    #endregion

    #region 마커와 기타 등등
    public RectTransform marker0;     // enemies[0] 아이콘
    public RectTransform EndPoint0;   // 끝점   (max)
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
    #endregion

    #region 밀리 버튼들
    public Button MleeButton_1;
    public TextMeshProUGUI MleeButtonText_1;
    public Button MleeButton_2;
    public TextMeshProUGUI MleeButtonText_2;
    public Button MleeButton_3;
    public TextMeshProUGUI MleeButtonText_3;
    public Button MleeButton_4;
    public TextMeshProUGUI MleeButtonText_4;
    #endregion

    #region 스타트, 업데이트, 배틀루프
    void Start()
    {
        //테스트용

        guns = new List<IGun> { new NormalPistol(), new NormalShotgun() };
        //Debug.Log(guns[0].Name);

        //enemies = new List<ICharacter> { new Monster1(guns[0], monLev, riSK), new Monster1(guns[0], monLev, riSK) };
        //IfNameSame();

        player = new PlayerCharacter(guns[1]);
        ApplyModifiers(guns[1].Modifiers, true);
        player.CurrentHp = player.HP;
        itemListItem.Update_StatUI();
        FuckYouFuckYou = true;
        dungeonManager.DungeonPlayer = player;

        //Enemy_WithMakers_RESTART(enemies);

        minPoint.gameObject.GetComponent<TooltipTrigger>().isPlayerMarker = true;

        //테스트용
    }

    private void Update()
    {

        if (!running && ISPlayerNotInBattle == false) { running = true; StartCoroutine(BattleLoop(enemies)); }
    }

    IEnumerator BattleLoop(List<ICharacter> enemiesList)
    {
        Debug.Log("전투 시작");
        ISPlayerNotInBattle = false;

        Enemy_WithMakers_RESTART(enemiesList);

        for (int i = 0; i < Sliders.Count; i++)
        {
            if (Enemy_WithMarkers[i].enemies == null)
            {
                continue;
            }

            Sliders[i].value = 1f;
            Enemy_WithMarkers[i].marker.GetChild(0).GetComponent<Slider>().value = 1f;
            Enemy_WithMarkers[i].marker.GetComponent<Image>().color = Color.white;
        }

        while (!ISPlayerNotInBattle)
        {
            // 1) 플레이어 턴 (입력 대기)
            player.CurrentAp = player.AP;
            Update_ApSlider();
            Update_HpSlider();

            for (int i = 0; i < Enemy_WithMarkers.Count; i++)
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

            bool isInPlayerTurn = true;

            while (isInPlayerTurn == true)
            {
                if (ISPlayerNotInBattle == false)
                {
                    Debug.Log($"현재 AP : {player.CurrentAp} / {player.AP}");

                    TalkManager.Instance.ShowTemp("무엇을 할 것인가?");

                    buttonchoice.choicetrue = false;
                    buttonchoice.choicewhat = -1;

                    buttonchoice.SpawnButtons("총기 사격", "마법 시전", "행동 종료");
                    yield return new WaitUntil(() => buttonchoice.choicetrue);
                }


                if (buttonchoice.choicewhat == 0)
                {
                    yield return StartCoroutine(ShotTargetEnemySelect());
                    yield return StartCoroutine(BodyTargeting());

                    yield return StartCoroutine(ShotingPhase(player, Enemy_WithMarkers[TargetEnemy_Int].enemies));

                }// 여기서 마법 분기************
                else if (buttonchoice.choicewhat == 1)
                {
                    buttonchoice.choicetrue = false;
                    buttonchoice.choicewhat = -1;

                    yield return StartCoroutine(MagicPhase(player, enemies[0])); //뒤에 enemy 인수는 아무거나 넣음 ㅅㄱ
                }
                else
                {
                    isInPlayerTurn = false;
                }
            }

            if (ISPlayerNotInBattle == false)
            {
                for (int i = 0; i < Enemy_WithMarkers.Count; i++)
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
            }
            // 조기 종료 체크
            //if (battleEnd) break;


            /* 3) 승패 판정
            if ( 모두 처치  false) { Debug.Log("Victory"); ISPlayerNotInBattle = true; }
            if ( 전원 사망  false) { Debug.Log("Defeat"); ISPlayerNotInBattle = true; }
            */

            // 템포 조절(선택)
            yield return null;
        }
        Debug.Log("전투 종료");
        running = false;

        yield return ShowThenWait("당신은 아직 살아있다..");
        sceneChanger.WinPageLoad();
    }
    #endregion

    #region 사격 페이즈
    public Dictionary<BodyPartSlot, (float hit, float crit, float partbreak)> bodyBonuses =
        new()
        {
            { BodyPartSlot.Head, (0.4f, 3.0f, 0.1f) },
            { BodyPartSlot.Body, (1.0f, 1.0f, 0.1f) },
            { BodyPartSlot.Arms, (0.7f, 1.0f, 0.5f) },
            { BodyPartSlot.Legs, (0.7f, 1.0f, 0.5f) }
        };

    public IEnumerator ShotingPhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy)
    {
        bool isPlayerInBattle = true;

        while (isPlayerInBattle == true)
        {
            // 대기 들어가기 전 반드시 초기화
            buttonchoice.choicetrue = false;
            buttonchoice.choicewhat = -1;

            // 선택지 표시(UI는 네가 연결)
            if (ShouldBeEnemy.CurrentHp <= 0)
            {
                yield return ShowThenWait("대상이 없다! 대상을 다시 지정해야 한다.");
                yield return StartCoroutine(ShotTargetEnemySelect());
                yield return StartCoroutine(BodyTargeting());
                ShouldBeEnemy = Enemy_WithMarkers[TargetEnemy_Int].enemies;
            }

            TalkManager.Currenttalk = 2;
            buttonchoice.SpawnButtons("대상 재지정", "표준 사격", "조준 사격", "정밀 조준 사격", "연속 사격", "제압 사격", "행동 종료");

            yield return new WaitUntil(() => buttonchoice.choicetrue);

            switch (buttonchoice.choicewhat) //사격 시퀀스
            {
                case 0:

                    Enemy_WithMarkers[TargetEnemy_Int].animate.GetComponent<Image>().color = Color.white;
                    yield return StartCoroutine(ShotTargetEnemySelect());
                    yield return StartCoroutine(BodyTargeting());
                    ShouldBeEnemy = Enemy_WithMarkers[TargetEnemy_Int].enemies;


                    break;

                case 1:

                    if (player.CurrentAp >= 2)
                    {
                        player.CurrentAp -= 2;
                        Update_ApSlider();
                        yield return StartCoroutine(DoFuckingShotTheFAce(ShouldBePlayer, ShouldBeEnemy, 0));

                    }
                    else
                    {
                        yield return ShowThenWait($"행동력이 부족하다! 당신의 행동력 = {player.CurrentAp}");
                    }

                    break;

                case 6:

                    TargetingAlarmBox.SetActive(false);
                    yield return ShowThenWait("당신은 행동을 끝마쳤다.");
                    isPlayerInBattle = false;

                    break;
            }


            DeathOfEnemy();
            if (ISPlayerNotInBattle == true)
            {
                yield break;
            }
        }

        Enemy_WithMarkers[TargetEnemy_Int].animate.GetComponent<Image>().color = Color.white;
        //yield return new WaitForSeconds(0.2f);
    }
    public float CalcShotChance(ICharacter attacker, ICharacter defender)
    {
        // 거리 기준 정규화 (필요한 방식에 맞게 조정)
        float rawDistance = attacker.Distance + defender.Distance - MleeRange;
        float normalized = Mathf.Clamp01(rawDistance / (gameMax - MleeRange));

        // 기본 명중률 (공격자 정확도 - 방어자 회피)
        float baseChance =
            40f
            + attacker.EquipedGun.AimCorrection
            + attacker.Perception
            - defender.Evade;

        float hitByPart = bodyBonuses[TargetPart].hit;
        // 총기 프로필의 거리 곡선 적용
        float hitCurve = attacker.EquipedGun.Profile.hitCurve.Evaluate(normalized);

        return baseChance * hitCurve * hitByPart;
    }
    public void ShotDamageMethod(ICharacter attacker, ICharacter defender)
    {
        // 거리 기준 정규화 (필요한 방식에 맞게 조정)
        float rawDistance = attacker.Distance + defender.Distance - MleeRange;
        float normalized = Mathf.Clamp01(rawDistance / (gameMax - MleeRange));
        float DamageCurve = attacker.EquipedGun.Profile.damageCurve.Evaluate(normalized);
        Debug.Log(DamageCurve);
        float Damage;

        float CritM = 1f;
        bool isCritHit = RollCriticalHit(attacker, defender);
        float critDmgByPart = bodyBonuses[TargetPart].crit;
        Debug.Log(isCritHit);
        if (isCritHit == true) { CritM = attacker.EquipedGun.CritMultiply * critDmgByPart; }
        Damage = (attacker.EquipedGun.ShotDamage + attacker.ShotAtk * 0.5f) * CritM;

        defender.CurrentHp -= Mathf.RoundToInt(Damage * DamageCurve);
    }
    /*
    public float PercFactor(ICharacter attacker)
    {
        return 1f + (attacker.Perception - 10) * 0.04f; // 지각이 10일 때 1.0
    }
    */
    /// <summary>
    /// 공격자/방어자의 치명타 관련 스탯을 사용해 치명타 여부를 판정한다.
    /// </summary>
    public bool RollCriticalHit(ICharacter attacker, ICharacter defender)
    {
        float attCrit = Mathf.Max(1f, attacker.Crit);   // 최소값 보호
        float defCrit = Mathf.Max(1f, defender.CritResist);

        float critByPart = bodyBonuses[TargetPart].crit;
        float ratio = attCrit / defCrit;               // 두 배라면 ratio=2
        float chance = (0.2f + (ratio - 1f) * 0.4f) * critByPart;     // ratio=1 → 0.2, ratio=2 → 0.9
        chance = Mathf.Clamp(chance, 0.05f, 0.5f);    // 지나친 극단값 방지

        return UnityEngine.Random.value < chance;
    }
    public void PartBreak(ICharacter defender)
    {
        float Rv = UnityEngine.Random.value;

        if(Rv < bodyBonuses[TargetPart].partbreak)
        {
            switch (TargetPart)
            {
                case BodyPartSlot.Head:
                    defender.Head_Hp --;
                    Mathf.Clamp(defender.Head_Hp, 0, 3);
                    break;
                case BodyPartSlot.Body:
                    defender.Body_Hp --;
                    Mathf.Clamp(defender.Body_Hp, 0, 3);
                    break;
                case BodyPartSlot.Arms:
                    defender.Arm_Hp --;
                    Mathf.Clamp(defender.Arm_Hp, 0, 3);
                    break;
                case BodyPartSlot.Legs:
                    defender.Leg_Hp --;
                    Mathf.Clamp(defender.Leg_Hp, 0, 3);
                    break;
            }
        }
    }
    public (int hp, string part) Partnow(ICharacter defender)
    {
        switch (TargetPart)
        {
            case BodyPartSlot.Body:
                return (defender.Body_Hp, "몸통");
            case BodyPartSlot.Head:
                return (defender.Head_Hp, "머리");
            case BodyPartSlot.Arms:
                return (defender.Arm_Hp, "팔");
            case BodyPartSlot.Legs:
                return (defender.Leg_Hp, "다리");
            default:
                return (defender.Body_Hp, "몸");
        }
    }
    public IEnumerator ShotTargetEnemySelect()
    {
        TargetEnemy_Int = -1;
        buttonValueSelector.choiceButtonTrue = false;
        buttonValueSelector.SetBindingsInteractable(true);

        TalkManager.Instance.ShowTemp("누구를?");

        yield return new WaitUntil(() => buttonValueSelector.choiceButtonTrue);

        Enemy_WithMarkers[TargetEnemy_Int].animate.GetComponent<Image>().color = Color.blue;
    }
    public IEnumerator BodyTargeting()
    {
        IsTargetPartYes = false;
        BodyTragetingBox.SetActive(true);

        TalkManager.Instance.ShowTemp("어디를?");

        yield return new WaitUntil(() => IsTargetPartYes);

        BodyTragetingBox.SetActive(false);

        TargetingAlarmBox.SetActive(true);
        TextMeshProUGUI targetText = TargetingAlarmBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        string partText = "";

        switch(TargetPart)
        {
            case BodyPartSlot.Head:
                partText = "머리";
                break;
            case BodyPartSlot.Body:
                partText = "몸통";
                break;
            case BodyPartSlot.Arms:
                partText = "팔";
                break;
            case BodyPartSlot.Legs:
                partText = "다리";
                break;
        }

        targetText.text = $"{partText}";
    }
    public IEnumerator DoFuckingShotTheFAce(ICharacter attacker, ICharacter defender, int buttonclick)
    {

        int HowManyShot = 0;
        HitOrNotAlarmBox.SetActive(true);
        TextMeshProUGUI hnBoxtext = HitOrNotAlarmBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Image hnBoxColor = HitOrNotAlarmBox.GetComponent<Image>();
        float ShotChance = CalcShotChance(attacker, defender);

        int calcdamageX = defender.CurrentHp;
        int beforePartHP = Partnow(defender).hp;
        string isPartBreak = "";

        for (int i = 0; i < attacker.EquipedGun.ShotCountPerTurn; i++)
        {

            float Randx = UnityEngine.Random.Range(0, 100);
            Enemy_WithMarkers[TargetEnemy_Int].animate.GetComponent<Image>().color = Color.white;
            if (Randx <= ShotChance)
            {
                HowManyShot++;
                int NowParHp;
                string partmassage = "";
                int dealedHP = defender.HP;

                ShotDamageMethod(attacker, defender);
                NowParHp = Partnow(defender).hp;
                PartBreak(defender);
                if(NowParHp != Partnow(defender).hp)
                {
                    partmassage = Partnow(defender).part + " 파괴!";
                }
                defender.CurrentHp = Mathf.Max(0, defender.CurrentHp);
                hnBoxtext.text = $"명중 x{HowManyShot}";
                hnBoxColor.color = Color.green;
                TalkManager.Instance.ShowTemp($"{i}발째 : 명중! {attacker.Name}은(는) {defender.Name}에게 {dealedHP - defender.CurrentHp} 데미지를 주었다! 확률 : {Mathf.RoundToInt(ShotChance)} {partmassage}");
                FireEffect(minPoint, Enemy_WithMarkers[TargetEnemy_Int].marker, true, attacker);
                Enemy_WithMarkers[TargetEnemy_Int].animate.GetComponent<Image>().color = Color.red;
                Enemy_WithMarkers[TargetEnemy_Int].slider.value = (float)Enemy_WithMarkers[TargetEnemy_Int].enemies.CurrentHp / Enemy_WithMarkers[TargetEnemy_Int].enemies.HP;

                Debug.Log($"{i}발째 : 명중!");
            }
            else
            {
                hnBoxColor.color = Color.yellow;
                hnBoxtext.text = "빗나감";
                TalkManager.Instance.ShowTemp($"{i}발째 : 감나빗! {attacker.Name}의 공격은 빗나갔다! 확률 : {Mathf.RoundToInt(ShotChance)}");
                FireEffect(minPoint, Enemy_WithMarkers[TargetEnemy_Int].marker, false, attacker);

                Debug.Log($"{i}발째 : 감나빗!");
            }

            yield return new WaitForSeconds(ShotRateSpeed);
            Enemy_WithMarkers[TargetEnemy_Int].animate.GetComponent<Image>().color = Color.blue;
        }
        if(beforePartHP != Partnow(defender).hp)
        {
            beforePartHP -= Partnow(defender).hp;
            isPartBreak = Partnow(defender).part + $" -{beforePartHP}";
        }
        hnBoxtext.text = $"명중! x{HowManyShot}";
        hnBoxColor.color = Color.green;
        if(HowManyShot == 0)
        {
            hnBoxtext.text = $"빗나감!";
            hnBoxColor.color = Color.yellow;
        }
        yield return ShowThenWait($"{attacker.EquipedGun.ShotCountPerTurn}발 중 {HowManyShot}발 명중! 확률 : {Mathf.RoundToInt(ShotChance)} 데미지 : {calcdamageX - defender.CurrentHp} {defender.Name}의 남은 HP: {defender.CurrentHp} {isPartBreak}");

        if (defender.CurrentHp <= 0)
        {
            yield return ShowThenWait($"{defender.Name}은 죽음에 이르는 피해를 입었다!");
        }
        HitOrNotAlarmBox.SetActive(false);
        DeathOfEnemy();
        Enemy_WithMakers_RESTART(enemies);
        if (ISPlayerNotInBattle == true)
        {
            yield break;
        }
    }
    public IEnumerator EnemyShotYourFaceFuck(ICharacter attacker, ICharacter defender, int currentEnemy)
    {
        TargetPart = defender.CharacterShotWhereAI(attacker, defender);
        TargetingAlarmBox.SetActive(true);
        HitOrNotAlarmBox.SetActive(true);
        TextMeshProUGUI targetText = TargetingAlarmBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI hnBoxtext = HitOrNotAlarmBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Image hnBoxColor = HitOrNotAlarmBox.GetComponent<Image>();
        string partText = "";

        switch (TargetPart)
        {
            case BodyPartSlot.Head:
                partText = "머리";
                break;
            case BodyPartSlot.Body:
                partText = "몸통";
                break;
            case BodyPartSlot.Arms:
                partText = "팔";
                break;
            case BodyPartSlot.Legs:
                partText = "다리";
                break;
        }

        targetText.text = $"{partText}";
        int HowManyShot = 0;
        float ShotChance = CalcShotChance(attacker, defender);

        int calcdamageX = defender.CurrentHp;
        int beforePartHP = Partnow(defender).hp;
        string isPartBreak = "";

        for (int i = 0; i < attacker.EquipedGun.ShotCountPerTurn; i++)
        {

            float Randx = UnityEngine.Random.Range(0, 100);

            if (Randx <= ShotChance)
            {
                HowManyShot++;

                int NowParHp;
                string partmassage = "";
                int dealedHP = defender.HP;

                ShotDamageMethod(attacker, defender);
                NowParHp = Partnow(defender).hp;
                PartBreak(defender);
                if (NowParHp != Partnow(defender).hp)
                {
                    partmassage = Partnow(defender).part + " 파괴!";
                }
                defender.CurrentHp = Mathf.Max(0, defender.CurrentHp);
                hnBoxtext.text = $"피격! x{HowManyShot}";
                hnBoxColor.color = Color.red;
                TalkManager.Instance.ShowTemp($"{i}발째 : 명중! {attacker.Name}은(는) {defender.Name}에게 {dealedHP - defender.CurrentHp} 데미지를 주었다! 확률 : {Mathf.RoundToInt(ShotChance)} {partmassage}");
                FireEffect(Enemy_WithMarkers[currentEnemy].marker, minPoint, true, attacker);
                minPoint.gameObject.GetComponent<Image>().color = Color.red;
                PlayerSlider.value = (float)player.CurrentHp / player.HP;

                Debug.Log($"{i}발째 : 명중!");
            }
            else
            {
                hnBoxtext.text = $"회피!";
                hnBoxColor.color = Color.green;
                TalkManager.Instance.ShowTemp($"{i}발째 : 감나빗! {attacker.Name}의 공격은 빗나갔다! 확률 : {Mathf.RoundToInt(ShotChance)}");
                FireEffect(Enemy_WithMarkers[currentEnemy].marker, minPoint, false, attacker);

                Debug.Log($"{i}발째 : 감나빗!");
            }

            yield return new WaitForSeconds(ShotRateSpeed);
            minPoint.gameObject.GetComponent<Image>().color = Color.white;
        }
        if (beforePartHP != Partnow(defender).hp)
        {
            beforePartHP -= Partnow(defender).hp;
            isPartBreak = Partnow(defender).part + $" -{beforePartHP}";
        }
        hnBoxtext.text = $"피격! x{HowManyShot}";
        hnBoxColor.color = Color.red;
        if(HowManyShot == 0)
        {
            hnBoxtext.text = $"회피!";
            hnBoxColor.color = Color.green;
        }
        yield return ShowThenWait($"{attacker.EquipedGun.ShotCountPerTurn}발 중 {HowManyShot}발 명중! 확률 : {Mathf.RoundToInt(ShotChance)} 데미지 : {calcdamageX - defender.CurrentHp} {defender.Name}의 남은 HP: {defender.CurrentHp} {isPartBreak}");
        TargetingAlarmBox.SetActive(false);
        HitOrNotAlarmBox.SetActive(false);
    }
    #region 총알발사 UI
    public void FireEffect(RectTransform origin, RectTransform target, bool hit, ICharacter attacker)
    {
        switch (attacker.EquipedGun.gunType)
        {

            case GunType.Pistol:
                FireBullet(origin, target, hit);
                break;

            case GunType.Shotgun:
                FireShotgun(origin, target, hit);
                break;

            case GunType.Laser:
                FireLaser(origin, target, hit);
                break;
        }
    }
    public void FireBullet(RectTransform origin, RectTransform target, bool hit)
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
    public void FireShotgun(RectTransform origin, RectTransform target, bool hit, int pelletCount = 6, float spreadAngle = 6f)
    {
        Vector2 start = origin.anchoredPosition;
        Vector2 dir = (target.anchoredPosition - start).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float baseDistance = Vector2.Distance(start, target.anchoredPosition);

        for (int i = 0; i < pelletCount; i++)
        {
            float offset = Random.Range(-spreadAngle, spreadAngle);
            float angle = baseAngle + offset;

            Vector2 pelletDir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 pelletTarget = start + pelletDir * baseDistance;

            var bullet = Instantiate(bulletPrefab, uiCanvasRoot);
            var rect = bullet.GetComponent<RectTransform>();
            rect.anchoredPosition = start;

            BulletManage bulletManage = bullet.GetComponent<BulletManage>();
            if (bulletManage != null)
            {
                bulletManage.Initialize(pelletTarget, hit);
            }
        }
    }
    public void FireLaser(RectTransform origin, RectTransform target, bool hit)
    {
        GameObject laser = Instantiate(laserPrefab, uiCanvasRoot);
        var laserRect = laser.GetComponent<RectTransform>();
        var laserScript = laser.GetComponent<LaserBullet>();

        if (laserRect != null && laserScript != null)
        {
            Vector2 start = origin.anchoredPosition;
            Vector2 end = target.anchoredPosition;
            laserScript.Initialize(start, end, hit);
        }
    }

    #endregion

    #endregion

    #region 이동 페이즈
    public IEnumerator MovingPhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy)
    {
        for (int i = 0; i < Enemy_WithMarkers.Count; i++)
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
            // 대기 들어가기 전 반드시 초기화
            buttonchoice.choicetrue = false;
            buttonchoice.choicewhat = -1;

            // 선택지 표시(UI는 네가 연결)
            TalkManager.Currenttalk = 2;

            //buttonchoice.SpawnButtons("거리 좁히기", "거리 유지", "거리 벌리기", " 테스트1", "테스트2", "테스트3", "테스트4");
            buttonchoice.SpawnButtonsWithTooltips(
                new List<string> { "거리 좁히기", "거리 유지", "거리 벌리기" },
                new List<string> { "거리를 좁힌다", "거리를 유지한다", "거리를 벌린다" });

            yield return new WaitUntil(() => buttonchoice.choicetrue);

            cachedMoveChoice = buttonchoice.choicewhat;

            IsFirstRun = false;
        }



        // 분기 처리
        switch (cachedMoveChoice) //거리조절 시퀀스
        {
            case 0:

                Debug.Log("거리 좁히기!");

                yield return StartCoroutine(DoRun(ShouldBePlayer, ShouldBeEnemy, 0));

                break;

            case 1: //거리 좁히기

                Debug.Log("거리 유지!");

                yield return StartCoroutine(DoRun(ShouldBePlayer, ShouldBeEnemy, 1));

                break;

            default:

                Debug.Log("거리 벌리기!");

                yield return StartCoroutine(DoRun(ShouldBePlayer, ShouldBeEnemy, 2));

                break;
        }

        // 다음 턴 대비 초기화(선택)
    }
    public IEnumerator DoRun(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy, int button)
    {
        const int DELTA_ENEMY_KEEP = -1;
        const int DELTA_CONTEST_RETREAT_OR_ADVANCE = 0; // 벌리기/좁히기 충돌
        const int DELTA_PLAYER_KEEP = 1;

        int MoveCases = -1;
        Debug.Log(MoveCases);

        MoveIntent Enemyintent = (MoveIntent)ShouldBeEnemy.CharaterRunAI(ShouldBePlayer, ShouldBeEnemy);

        switch (button)
        {
            case 0: //거리 좁히기 선택

                yield return ShowThenWait($"{ShouldBePlayer.Name}은 거리를 좁히기로했다.");

                switch (Enemyintent /*적 ai*/ )
                {
                    case MoveIntent.Advance: //상대도 거리를 좁히기 선택

                        MoveCases = 0; //pAeA

                        yield return ShowThenWait("상대 역시 거리를 좁히고 있다.");

                        break;

                    case MoveIntent.Keep: //상대는 거리 유지

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP) == true) //상대 거리 유지 실패
                        {
                            MoveCases = 3; //pA

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 거리를 유지하려했지만 실패했다. 확률:{Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP))}");
                        }
                        else //상대는 거리 성공
                        {
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 거리를 유지했다! 확률:{Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP))}");
                        }

                        break;

                    case MoveIntent.Retreat: //상대는 거리 벌리기

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE) == true) //상대 거리 벌리기 실패
                        {
                            MoveCases = 3; //pA

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 거리를 벌리려했지만 실패했다. 확률:{Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE))}");
                        }
                        else //상대 거리 벌리기 성공
                        {
                            MoveCases = 6; //eR

                            yield return ShowThenWait($"당신은 거리를 좁히는데 실패했고, {ShouldBeEnemy.Name}은(는) 거리를 벌렸다! 확률:{Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE))}");
                        }

                        break;
                }

                break;

            case 1: //거리 유지 선택

                yield return ShowThenWait($"{ShouldBePlayer.Name}은 거리를 유지하기로했다.");

                switch (Enemyintent /*적 ai*/ )
                {
                    case MoveIntent.Advance: // 나는 유지, 상대는 거리를 좁히기 선택

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP) == true)
                        { // 상대 거리 좁히기 실패
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 거리를 좁히려 했으나 실패했다. 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP))}");
                        }
                        else // 상대 거리 좁히기 성공
                        {
                            MoveCases = 5; //eA

                            yield return ShowThenWait($"거리 유지 실패! 상대는 거리를 좁혔다! 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP))}");
                        }

                        yield return ShowThenWait($"{ShouldBePlayer.Name}과 {ShouldBeEnemy.Name}의 거리는 {ShouldBeEnemy.Distance}(이)가 되었다!");

                        break;

                    case MoveIntent.Keep: //나와 상대 거리 유지

                        MoveCases = 1; //pKeK

                        yield return ShowThenWait($"{ShouldBeEnemy.Name}역시 거리를 유지중이다");

                        break;

                    case MoveIntent.Retreat: //나는 유지, 상대는 거리 벌리기

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP) == true) //상대 거리 벌리기 실패 => 유지
                        {
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 거리를 벌리려했지만 실패했다. 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP))}");

                        }
                        else   //상대 거리 벌리기 성공
                        {
                            MoveCases = 6; //eR 

                            yield return ShowThenWait($"거리 유지 실패! {ShouldBeEnemy.Name}은(는) 거리를 벌렸다! 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_PLAYER_KEEP))}");

                        }

                        break;
                }

                break;

            case 2: //거리 벌리기 선택

                yield return ShowThenWait($"{ShouldBePlayer.Name}은 거리를 벌리기로했다.");

                switch (Enemyintent /*적 ai*/ )
                {
                    case MoveIntent.Advance: //나는 거리 벌리기, 상대는 거리를 좁히기 선택

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE) == true) //상대 거리 좁히기 실패, 나 거리 벌림
                        {
                            MoveCases = 4; //pR

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 거리를 좁히려했지만 실패했다. 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE))}");
                        }
                        else //상대 거리 좁히기 성공
                        {
                            MoveCases = 5; //eA

                            yield return ShowThenWait($"거리 벌리기 실패! {ShouldBeEnemy.Name}은(는) 거리를 좁혔다! 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_CONTEST_RETREAT_OR_ADVANCE))}");
                        }

                        break;

                    case MoveIntent.Keep: //상대는 거리 유지

                        if (VSmeANDyouSPEED(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP) == true) //상대 거리 유지 실패, 나 거리 좁힘
                        {
                            MoveCases = 3; // pA

                            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 거리를 유지하려했지만 실패했다. 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP))}");
                        }
                        else //상대 거리 유지
                        {
                            MoveCases = 1; //pKeK

                            yield return ShowThenWait($"거리 벌리기 실패! {ShouldBeEnemy.Name}은(는) 거리를 유지했다! 확률 : {Mathf.RoundToInt(CalcSpeedChance(ShouldBePlayer, ShouldBeEnemy, DELTA_ENEMY_KEEP))}");
                        }

                        break;

                    case MoveIntent.Retreat: //상대와 나 거리 벌리기

                        MoveCases = 2; //pReR

                        yield return ShowThenWait($"{ShouldBeEnemy.Name} 역시 거리를 벌렸다!");

                        break;
                }

                break;
        }

        MoveCaseNine moveCaseNine = (MoveCaseNine)MoveCases;

        Debug.Log(moveCaseNine);

        float BeforeDistance = ShouldBeEnemy.Distance;

        ShouldBeEnemy.Distance = CalcDistance(ShouldBePlayer, ShouldBeEnemy, moveCaseNine);

        UpdateMarkerForEnemy0(ShouldBeEnemy.Distance, Enemy_WithMarkers[CurrentMovingEnemy_int].marker, Enemy_WithMarkers[CurrentMovingEnemy_int].endpoint);

        TalkManager.Instance.ShowTemp($"{Mathf.Abs(BeforeDistance - ShouldBeEnemy.Distance)}만큼 이동했다. {ShouldBePlayer.Name}과 {ShouldBeEnemy.Name}의 거리는 {ShouldBeEnemy.Distance}(이)가 되었다!");
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
        float ratio = Mathf.Clamp((p.Speed + 0.1f) / (e.Speed + 0.1f), 0.5f, 2f);
        float normalized = (ratio - 1f) / 1f;                // 0.5→-0.5, 2→+1
        float baseChance = 50f + normalized * 30f;           // 20~80 범위
        baseChance += keep * 10f;
        return Mathf.Clamp(baseChance, 20f, 80f);
    }
    public float CalcDistance(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy, MoveCaseNine moveCaseNine)
    {
        float t = Mathf.InverseLerp(gameMin, gameMax, ShouldBeEnemy.Distance);
        t = Mathf.Pow(t, 0.5f); // 뒷 인자로 곡선 휘기 (감마 스케일)
        float multiplier = Mathf.Lerp(1f, 2f, t); //최종 감마 보정치 1배에서 최종 2배까지 보정한다는 뜻

        float DisMax(float distancePM)
        {
            float minStep = gameMax / 10f;
            float maxStep = gameMax / 3f;

            float step = Mathf.Clamp(distancePM, minStep, maxStep); // t=0이면 1/6, t=1이면 1/3

            return step;
        }

        switch (moveCaseNine)
        {
            case MoveCaseNine.pAeA: //더블 거리 좁힘

                ShouldBeEnemy.Distance -= DisMax((ShouldBePlayer.Speed + ShouldBeEnemy.Speed) * multiplier);

                break;

            case MoveCaseNine.pKeK: //거리 유지

                break;

            case MoveCaseNine.pReR: //더블 거리 벌림

                ShouldBeEnemy.Distance += DisMax((ShouldBePlayer.Speed + ShouldBeEnemy.Speed) * multiplier);

                break;

            case MoveCaseNine.pA: //(내가) 거리 좁힘

                ShouldBeEnemy.Distance -= DisMax(ShouldBePlayer.Speed * multiplier);

                break;

            case MoveCaseNine.pR: //(내가) 거리 벌림

                ShouldBeEnemy.Distance += DisMax(ShouldBePlayer.Speed * multiplier);

                break;

            case MoveCaseNine.eA: //(상대가) 거리 좁힘

                ShouldBeEnemy.Distance -= DisMax(ShouldBeEnemy.Speed * multiplier);

                break;

            case MoveCaseNine.eR: //(상대가) 거리 벌림

                ShouldBeEnemy.Distance += DisMax(ShouldBeEnemy.Speed * multiplier);

                break;
        }

        ShouldBeEnemy.Distance = Mathf.Clamp(ShouldBeEnemy.Distance, gameMin, gameMax);

        return Mathf.Round(ShouldBeEnemy.Distance);
    }
    #endregion

    #region 밀리 공격
    public IEnumerator MleePhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy, int ShouldBeEnemy_int)
    {
        // 대기 들어가기 전 반드시 초기화
        buttonchoice.choicetrue = false;
        buttonchoice.choicewhat = -1;

        //최종 선택지
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

        // 선택지 표시(UI는 네가 연결)

        TalkManager.Instance.ShowTemp($"{ShouldBeEnemy.Name}은(는) 당신 바로 앞에 있다! 무엇을 하지?");

        List<IMlee> RandomMleePopOut = new List<IMlee>();

        for (int A = 0; A < 4; A++)
        {
            RandomMleePopOut.Add(RandomMleeByWeight(ShouldBePlayer.ActiveSkills)); //가중치에 따라 알고있는 스킬 중 하나 뽑음
        }

        List<string> MleeAtkSelect = new();

        for (int B = 0; B < 4; B++)
        {
            MleeAtkSelect.Add(RandomMleePopOut[B].Name);
        }

        //공격 선택 시간
        for (int i = 0; i < 2; i++)
        {
            buttonchoice.choicetrue = false;

            Debug.Log(MleeAtkSelect.ToArray());

            buttonchoice.SpawnButtons(MleeAtkSelect.ToArray());
            yield return new WaitUntil(() => buttonchoice.choicetrue);

            if (i == 1)
            {
                TalkManager.Instance.ShowTemp($"{ShouldBeEnemy.Name}은(는) 당신 바로 앞에 있다! 무엇을 하지?");
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



            yield return ShowThenWait($"당신은 {PlayerSelectedMleeList[i].Name}을(를) 했다!");

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
            yield return ShowThenWait($"당신은 {ShouldBeEnemy.Name}의 움직임을 읽었다! {ShouldBeEnemy.Name}는 {EnemySelectedMleeList[i].Name}을(를) 했다");
        }

        // 플레이어, 적 각각 두 번씩 주고받기
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

                yield return ShowThenWait($"{ShouldBeEnemy.Name}의 {EnemySelectedMleeList[i].Name}! {ShouldBePlayer.Name}은 {totalEnemy} 피해를 입었다! 확률 : { totalEnemyHitPer } 원 데미지:{ enemyBase } 기술 연계 : {playerMod}");
            }
            else
            {
                yield return ShowThenWait($"{ShouldBeEnemy.Name}의 {EnemySelectedMleeList[i].Name}은 빗나갔다! 확률 : { totalEnemyHitPer }");
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

                yield return ShowThenWait($"{ShouldBePlayer.Name}의 {PlayerSelectedMleeList[i].Name}! {ShouldBeEnemy.Name}은(는) {totalPlayer} 피해를 입었다! 확률 : { totalPlayerHitPer } 원 데미지:{ playerBase } 기술 연계 : { enemyMod }");
            }
            else
            {
                yield return ShowThenWait($"{ShouldBePlayer.Name}의 {PlayerSelectedMleeList[i].Name}은 빗나갔다! 확률 : { totalPlayerHitPer }");
            }
        }

        MleeButton_1.gameObject.SetActive(false);
        MleeButton_2.gameObject.SetActive(false);
        MleeButton_3.gameObject.SetActive(false);
        MleeButton_4.gameObject.SetActive(false);

        minPoint.gameObject.SetActive(true);
        /*
        for (int i = 0; i < Enemy_WithMarkers.Count; i++)
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
        */

        if (ShouldBeEnemy.CurrentHp <= 0)
        {
            yield return ShowThenWait($"{ShouldBeEnemy.Name}은(는) 죽음에 이르는 피해를 입었다!");
        }

        ShouldBeEnemy.Distance = MleeRange;

        DeathOfEnemy();

        //Enemy_WithMakers_RESTART(enemies);

        if (ISPlayerNotInBattle == true)
        {
            yield break;
        }
    }

    public static IMlee RandomMleeByWeight(IList<IMlee> ActiveMlees)
    {
        float Level = 0; //누적합 계싼
        for (int i = 0; i < ActiveMlees.Count; i++)
        {
            float RangedWeight = ActiveMlees[i].ChanceWeight;
            if (RangedWeight > 0) Level += RangedWeight;
        }
        if (Level <= 0) throw new System.InvalidOperationException("No pickable items (all counts <= 0).");

        // [0, total) 구간에서 난수 1개
        // total이 int 범위를 넘을 수 있으니 double로 계산
        double randomvalue = Random.value * Level;
        float acc = 0; //거름망

        for (int i = 0; i < ActiveMlees.Count; i++)
        {
            float RangedWeight = ActiveMlees[i].ChanceWeight;
            if (RangedWeight <= 0) continue;
            acc += RangedWeight;
            if (randomvalue < acc)
                return ActiveMlees[i];
        }
        // 부동오차 방지용 폴백
        return ActiveMlees[ActiveMlees.Count - 1];
    }
    #endregion

    #region 마법 공격
    public IEnumerator MagicPhase(ICharacter ShouldBePlayer, ICharacter ShouldBeEnemy)
    {
        int requiredSpellAp;
        bool isRightChose = false;

        while (isRightChose == false)
        {
            // 대기 들어가기 전 반드시 초기화
            buttonchoice.choicetrue = false;
            buttonchoice.choicewhat = -1;

            // 선택지 표시(UI는 네가 연결)
            TalkManager.Currenttalk = 2;

            List<string> playerSpellList = new();

            foreach (var spells in player.SpellData)
            {
                playerSpellList.Add(spells.Name);
            }

            int spellCount = playerSpellList.Count;

            playerSpellList.Add("시전 취소");

            buttonchoice.SpawnButtons(playerSpellList.ToArray());
            yield return new WaitUntil(() => buttonchoice.choicetrue);

            if (buttonchoice.choicewhat == spellCount)
            {
                // “행동 끝내기”를 골랐으니 주문 실행 없이 빠져나감
                isRightChose = true;
                yield return ShowThenWait("당신은 주문 시전을 취소했다");
                yield break;
            }


            requiredSpellAp = ShouldBePlayer.SpellData[buttonchoice.choicewhat].ApCost;

            if (requiredSpellAp <= player.CurrentAp)
            {
                break;
            }

            yield return ShowThenWait($"행동력이 부족합니다! 필요 AP: {requiredSpellAp}, 현재 AP: {player.CurrentAp}");
        }

        if (isRightChose == false)
        {
            player.CurrentAp -= ShouldBePlayer.SpellData[buttonchoice.choicewhat].ApCost;
            Update_ApSlider();

            yield return StartCoroutine(ExecuteSpell(ShouldBePlayer.SpellData[buttonchoice.choicewhat], ShouldBePlayer, ShouldBeEnemy));

            for (int i = 0; i < Enemy_WithMarkers.Count; i++)
            {
                if (Enemy_WithMarkers[i].enemies == null) continue;

                if (Enemy_WithMarkers[i].enemies.CurrentHp <= 0)
                {
                    yield return ShowThenWait($"{Enemy_WithMarkers[i].enemies.Name}은(는) 죽음에 이르는 피해를 입었다!");
                }
            }
        }
        DeathOfEnemy();
        Enemy_WithMakers_RESTART(enemies);
        if (ISPlayerNotInBattle == true)
        {
            yield break;
        }
    }
    public IEnumerator ExecuteSpell(ISpell spell, ICharacter caster, ICharacter defender)
    {
        SpellContext ctx = new SpellContext
        {
            caster = caster,
            Target = defender,
            enemySlots = Enemy_WithMarkers,
            casterIsPlayer = caster == player, // 또는 caster.IsPlayerControlled
            PlayerMarker = minPoint
        };

        yield return StartCoroutine(spell.CAST(ctx));
    }
    #endregion

    #region 마커 및 Ui관련
    void UpdateMarkerForEnemy0(float dGame, RectTransform marker, RectTransform endpoint) // dGame = method(distance) 결과
    {
        // 1) 0~1 비율로 변환 (선형)
        float t = Mathf.InverseLerp(gameMin, gameMax, dGame);
        t = Mathf.Clamp01(t);

        // 2) UI 선분(minPoint→maxPoint) 위 위치
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
        위치로 천천히 이동하려면*/

        marker.anchoredPosition = P;
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

        while (enemiesList.Count < EndPoints.Count)
        {
            enemiesList.Add(null);
        }

        for (int i = 0; i < EndPoints.Count; i++)
        {
            Slider manaSlider = Markers[i].GetChild(0)?.GetComponent<Slider>();

            if (enemiesList[i] != null)
            {
                GameObject ani = null;

                if (Markers[i].childCount <= 2)
                {
                    // 슬라이더(0) + 다른 UI(1) + 새 애니만 붙어 있도록 정리
                    ani = Instantiate(enemiesList[i].anime, Markers[i]);
                }
                else
                {
                    ani = Markers[i].GetChild(2).gameObject;
                }

                Enemy_WithMarkers.Add((Markers[i], EndPoints[i], enemiesList[i], Sliders[i], manaSlider, ani));
                Enemy_WithMarkers[i].marker.gameObject.SetActive(true);
            }
            else
            {
                Enemy_WithMarkers.Add((Markers[i], EndPoints[i], null, Sliders[i], manaSlider, null));
            }
        }

        for (int i = 0; i < Enemy_WithMarkers.Count; i++)
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
    public RectTransform GetMarkerFor(ICharacter target)
    {
        for (int i = 0; i < Enemy_WithMarkers.Count; i++)
        {
            if (Enemy_WithMarkers[i].enemies == target)
                return Enemy_WithMarkers[i].marker;
        }
        return null;
    }
    public void Update_ApSlider()
    {
        ApSlider.value = Mathf.InverseLerp(0, player.AP, player.CurrentAp);
        ApText.text = $"{player.CurrentAp}/{player.AP}";
    }
    public void Update_HpSlider()
    {
        PlayerSlider.value = Mathf.InverseLerp(0, player.HP, (float)player.CurrentHp);
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
                enemy.Name += $" ({idx + 1})"; // 8개 넘으면 숫자 붙이는 식으로 예외 처리
            }
        }
    }
    public IEnumerator ShowThenWait(string msg)
    {
        TalkManager.Instance.ShowTemp(msg);
        yield return StartCoroutine(WaitForSpace());
    }
    IEnumerator WaitForSpace()
    {
        // 같은 프레임에 눌려 있던 스페이스 잔상 제거
        yield return null;

        // '새로' 눌리는 순간을 기다린다
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        TalkManager.TempTrue = false;
    }
    #endregion

    #region 툴팁
    public string GetEnemyTooltip(int index)
    {
        if (index < 0 || index >= Enemy_WithMarkers.Count)
            return string.Empty;

        var enemy = Enemy_WithMarkers[index].enemies;
        if (enemy == null)
            return string.Empty;

        bodyTargetUi.Update_BodyTooltip(enemy);
        return $"{enemy.Name}\nHP: {enemy.CurrentHp}/{enemy.HP} MP: {enemy.CurrentMp}/{enemy.MP}\n거리: {enemy.Distance}\n사격 데미지: {enemy.ShotAtk} 스피드: {enemy.Speed}\n인지: {enemy.Perception} 정신력: {enemy.WillPower}";
    }

    public string GetPlayerTooltip()
    {
        bodyTargetUi.Update_BodyTooltip(player);
        return $"{player.Name}\nHP: {player.CurrentHp}/{player.HP}\n거리: {player.Distance}\n사격 데미지: {player.ShotAtk} 스피드: {player.Speed}\n인지: {player.Perception} 정신력: {player.WillPower}";
    }

    #endregion

    #region 기타
    public void LetsStartBattle()
    {
        ISPlayerNotInBattle = false;
    }
    public void ApplyModifiers(IEnumerable<StatModifier> modifiers, bool IsPlus)
    {
        foreach (var mod in modifiers)
        {
            int Valu;

            if (IsPlus == false)
            {
                Valu = mod.value * -1;
            }
            else
            {
                Valu = mod.value;
            }

            switch (mod.statType)
            {
                case StatId.Hp:
                    player.HP += Valu;
                    break;
                case StatId.Speed:
                    player.Speed += Valu;
                    break;
                case StatId.Perseption:
                    player.Perception += Valu;
                    break;
                case StatId.Mp:
                    player.MP += Valu;
                    break;
                case StatId.ShotDamage:
                    player.ShotAtk += Valu;
                    break;
                case StatId.Crit:
                    player.Crit += Valu;
                    break;
            }
        }
    }
    public void PlayerEqquipGun(IGun gun)
    {
        ApplyModifiers(player.EquipedGun.Modifiers, false);
        player.EquipedGun = gun;
    }
    public string PlayerStat()
    {
        string playerstat;

        playerstat = $"플레이어 스탯 \n \n체력 : {player.HP} 정신력 : {player.MP}\n" +
            $"스피드 : {player.Speed} 의지력 : {player.WillPower}\n" +
            $"사격 데미지 : {player.EquipedGun.ShotDamage + player.ShotAtk * 0.5f} 명중률 : {player.Perception}";

        return playerstat;
    }
    public void DeathOfEnemy()
    {
        for (int i = 0; i < Enemy_WithMarkers.Count; i++)
        {
            var entry = Enemy_WithMarkers[i];
            var enemy = entry.enemies;
            if (enemy == null) continue;

            if (enemy.CurrentHp <= 0)
            {
                if (enemy.EquipedGun is IItem thisGun)
                {
                    if (UnityEngine.Random.value < 0.3f)
                    {
                        itemListItem.Get_ItemFromEnemy.Add(thisGun);
                    }
                }

                if (enemy.DropItem.DropWhat == 1)
                {
                    for (int D = 0; D < enemy.DropItem.DropCount; D++)
                    {
                        itemListItem.Get_ItemFromEnemy.Add(PG.GenerateRandomPart(enemy.DropitemLV));
                    }
                }

                Destroy(Enemy_WithMarkers[i].animate);

                Enemy_WithMarkers[i] = (
                    entry.marker,
                    entry.endpoint,
                    null,
                    entry.slider,
                    entry.manaslider,
                    null
                );

                enemies[i] = null;  // 실제 적 리스트도 null 처리
            }

        }

        // 적 정리 후 UI/슬롯 재배치
        Enemy_WithMakers_RESTART(enemies);

        bool IsWin = true;

        buttonchoice.choicetrue = false;
        buttonchoice.choicewhat = -1;

        foreach (var entry in Enemy_WithMarkers)
        {
            if (entry.enemies != null)
                IsWin = false;
        }

        if (IsWin == true)
        {
            TargetingAlarmBox.SetActive(false);
            ISPlayerNotInBattle = true;
        }
    }
    #endregion

}