using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalkManager : MonoBehaviour
{
    public TextMeshProUGUI textext;
    Dictionary<int, string[]> talkdata;

    public static TalkManager Instance;

    //�� ���δ� �����ϸ� ����ħ��. chatgpt �賦

    int testnum = 0;

    public static int Currenttalk = 1;

    public int CurrentTalkNum = 0;

    public BattleManager BattleManager;

    public ChoiceManager choice;

    //�ӽ� �ؽ�Ʈ
    public string TempText;
    public static bool TempTrue = false;

    void Awake()
    {
        //  �ν��Ͻ� ���
        if (Instance == null)
        {
            Instance = this;               // �� ������Ʈ�� ���� �ν��Ͻ��� ���
            DontDestroyOnLoad(gameObject); // (���û���) �� ��ȯ���� ����
        }
        else
        {
            Destroy(gameObject);           // �ߺ� ���� (���� 2�� ������ �ϳ� ����)
        }

        talkdata = new Dictionary<int, string[]>();
        GenerateData();
        textext.text = talkdata[1][testnum];
    }

    private void Update()
    {
        if (!TempTrue)
        {
            textext.text = talkdata[Currenttalk][CurrentTalkNum];
        }
        else
        {
            textext.text = TempText;
        }
    }

    void GenerateData()
    {
        talkdata.Add(1, new string[] { "�ȳ�" , "bye"});//ó�� ȭ��
        talkdata.Add(2, new string[] { "�÷��̾� ���� ���..." });
        talkdata.Add(3, new string[] { "��븦 �����϶�." });
        talkdata.Add(4, new string[] { "����� �Ÿ��� �����Ϸ� �ߴ�" });
        talkdata.Add(5, new string[] { "����� �Ÿ��� ������ �ߴ�." });
    }

    public void ShowTemp(string line)
    {
        TempText = line;
        TempTrue = true;
    }
}
