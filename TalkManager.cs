using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalkManager : MonoBehaviour
{
    public TextMeshProUGUI textext;
    Dictionary<int, string[]> talkdata;

    public static TalkManager Instance;

    //이 위로는 변경하면 못고침요. chatgpt 배낌

    int testnum = 0;

    public static int Currenttalk = 1;

    public int CurrentTalkNum = 0;

    public BattleManager BattleManager;

    public ChoiceManager choice;

    //임시 텍스트
    public string TempText;
    public static bool TempTrue = false;

    void Awake()
    {
        //  인스턴스 등록
        if (Instance == null)
        {
            Instance = this;               // 이 오브젝트를 전역 인스턴스로 등록
            DontDestroyOnLoad(gameObject); // (선택사항) 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);           // 중복 방지 (씬에 2개 있으면 하나 삭제)
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
        talkdata.Add(1, new string[] { "안녕" , "bye"});//처음 화면
        talkdata.Add(2, new string[] { "플레이어 선택 대기..." });
        talkdata.Add(3, new string[] { "상대를 선택하라." });
        talkdata.Add(4, new string[] { "당신은 거리를 유지하려 했다" });
        talkdata.Add(5, new string[] { "당신은 거리를 벌리려 했다." });
    }

    public void ShowTemp(string line)
    {
        TempText = line;
        TempTrue = true;
    }
}
