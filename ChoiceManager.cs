using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class ChoiceManager : MonoBehaviour
{
    public bool choicetrue = false;
    public int choicewhat = -1;

    public GameObject buttonPrefab;      // 프리팹 연결
    public RectTransform buttonParent;       // 버튼을 넣을 부모(Panel)
    //public RectTransform content;
    public ScrollRect scrollrect;
    //public string[] choices;             // 문자열 배열
    public List<string> choices = new List<string>() ;

    public void SpawnButtons(params string[] inputs)
    {
        choices = new List<string>(inputs);

        // 혹시 기존 버튼이 남아있다면 전부 삭제
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        // 배열을 돌며 버튼 생성
        for (int i = 0; i < choices.Count; i++)
        {
            int index = i; // 람다 캡처용

            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);
            TextMeshProUGUI tmpText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = choices[i];

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChoiceSelected(index));

        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        //scrollrect.normalizedPosition = new Vector2(0f, 1f);
    }

    public void SpawnButtonsWithTooltips(
    IList<string> labels,
    IList<string> tooltips)
    {
        if (labels == null || tooltips == null) return;

        choices = new List<string>();
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        int count = Mathf.Min(labels.Count, tooltips.Count);
        for (int i = 0; i < count; i++)
        {
            int index = i;
            string label = labels[i];
            string tip = tooltips[i];

            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = label;

            var trigger = btnObj.GetComponent<TooltipTrigger>();
            if (trigger != null)
                trigger.tooltipText = tip;

            btnObj.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(index));
            choices.Add(label);
        }
    }

    void OnChoiceSelected(int index)
    {
        Debug.Log($"선택한 선택지: {choices[index]} (인덱스 {index})");
        // 여기에 대화 분기나 이벤트 처리 넣기
        choicewhat = index;

        choicetrue = true;

        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
            TooltipUI tooltip = FindObjectOfType<TooltipUI>();
            if (tooltip != null)
                tooltip.Hide();

        }
    }
}
