using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UniversManager : MonoBehaviour
{
    //public int CurrentLevel = 1;

    [SerializeField] float minSpacing = 100;
    [SerializeField] float targetCount = 30;
    [SerializeField] public int minLevel = 10;
    [SerializeField] public int maxLevel = 50;
    [SerializeField] public float riskCurve = 1.2f;   // 곡선 조절
    public float ComputeRisk(int level)
    {
        float t = Mathf.Clamp01((level - minLevel) / (float)(maxLevel - minLevel));
        float leveled = Mathf.Pow(t, riskCurve);
        return Mathf.Lerp(1f, 100f, leveled);
    }

    List<Vector2> planetsPositionList = new();
    public List<IPlanet> Planets = new();

    [SerializeField] GameObject PlanetPrefab;
    [SerializeField] RectTransform uiParent;
    [SerializeField] TextMeshProUGUI planetNameText;

    public int CurrentPlanet = 0;

    public void Start()
    {
        LetThereBeLight();
        BuildPlanetData();
        //RenderPlanetButtons();
    }
    /* 푸아송 방법
    public void LetThereBeLight()
    {
        List<Vector2> active = new();

        Vector2 first = Random.insideUnitCircle * 10f;

        planetsPositionList.Add(first);
        active.Add(first);

        while (planetsPositionList.Count < targetCount && active.Count > 0)
        {
            int index = Random.Range(0, active.Count);
            Vector2 basePoint = active[index];
            bool found = false;

            for (int attempt = 0; attempt < 30; attempt++)
            {
                Vector2 candidate = basePoint + Random.insideUnitCircle.normalized *
                                    Random.Range(minSpacing, minSpacing * 2f);

                if (!IsInsideMap(candidate)) continue;
                if (IsFarEnough(planetsPositionList, candidate, minSpacing))
                {
                    planetsPositionList.Add(candidate);
                    active.Add(candidate);
                    found = true;
                    break;
                }
            }

            if (!found)
                active.RemoveAt(index);
        }

        foreach (var pos in planetsPositionList)
        {
            Debug.Log($"Planet at ({pos.x:F2}, {pos.y:F2})");
        }
    }
    */

    public void LetThereBeLight()
    {
        float minX = -1280f;
        float maxX = 1280f;
        float minY = -720f;
        float maxY = 720f;

        int attempts = 0;
        int maxAttempts = 2000;

        while (planetsPositionList.Count < targetCount && attempts < maxAttempts)
        {
            var candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if (IsFarEnough(planetsPositionList, candidate, minSpacing))
            {
                planetsPositionList.Add(candidate);
            }

            attempts++;
        }
    }

    void BuildPlanetData()
    {
        Planets.Clear();

        for (int i = 0; i < planetsPositionList.Count; i++)
        {
            Planets.Add(new NormalPlanet
            {
                poSiTion = planetsPositionList[i],
                PlanetIndex = i,
                Name = RandomPlanetName()
            });
        }
    }

    void RenderPlanetButtons()
    {
        foreach (Transform child in uiParent)
            Destroy(child.gameObject);

        for (int i = 0; i < Planets.Count; i++)
        {
            var planetUI = Instantiate(PlanetPrefab, uiParent);
            planetUI.GetComponent<RectTransform>().anchoredPosition = Planets[i].poSiTion;

            int index = i;
            var button = planetUI.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
                Debug.Log($"Planet index: {index}, 행성명: {Planets[index].Name}")
            );
            button.onClick.AddListener(() => CurrentPlanet = index);
        }
    }

    public bool IsInsideMap(Vector2 point)
    {
        // 맵을 원형이라고 가정한 경우
        //return point.magnitude <= mapRadius;
        // 사각형 맵이라면:
        return point.x >= -1280 && point.x <= 1280 && point.y >= -720 && point.y <= 720;
    }

    public bool IsFarEnough(List<Vector2> existing, Vector2 candidate, float minSpacing)
    {
        float minSqr = minSpacing * minSpacing;
        foreach (var pos in existing)
        {
            if ((candidate - pos).sqrMagnitude < minSqr)
                return false;
        }
        return true;
    }

    public string RandomPlanetName()
    {
        string[] syllable1 = { "Ara", "Cal", "Eon", "Lys", "Or", "Zan", "San", "Psy" };
        string[] syllable2 = { "nos", "thar", "dora", "phus", "mer", "vara", "saar", "zar", "ray" };

        string randomNum = UnityEngine.Random.Range(0, 999).ToString();
        string RandomName() =>
            syllable1[Random.Range(0, syllable1.Length)] +
            syllable2[Random.Range(0, syllable2.Length)] + "-" + randomNum;

        return RandomName();
    }

}
