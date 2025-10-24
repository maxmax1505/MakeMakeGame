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

    public GameObject buttonPrefab;      // ������ ����
    public RectTransform buttonParent;       // ��ư�� ���� �θ�(Panel)
    //public RectTransform content;
    public ScrollRect scrollrect;
    //public string[] choices;             // ���ڿ� �迭
    public List<string> choices = new List<string>() ;

    public void SpawnButtons(params string[] inputs)
    {
        choices = new List<string>(inputs);

        // Ȥ�� ���� ��ư�� �����ִٸ� ���� ����
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        // �迭�� ���� ��ư ����
        for (int i = 0; i < choices.Count; i++)
        {
            int index = i; // ���� ĸó��

            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);
            TextMeshProUGUI tmpText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = choices[i];

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChoiceSelected(index));

        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        //scrollrect.normalizedPosition = new Vector2(0f, 1f);
    }

    void OnChoiceSelected(int index)
    {
        Debug.Log($"������ ������: {choices[index]} (�ε��� {index})");
        // ���⿡ ��ȭ �б⳪ �̺�Ʈ ó�� �ֱ�
        choicewhat = index;

        choicetrue = true;

        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }
    }
}
