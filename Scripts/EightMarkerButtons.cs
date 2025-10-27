using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonValueSelector : MonoBehaviour
{
    public bool choiceButtonTrue = false;

    [System.Serializable]
    struct ButtonBinding
    {
        public Button button;
        public int value;
    }

    [SerializeField] ButtonBinding[] bindings;

    void Awake()
    {
        foreach (var binding in bindings)
        {
            // 지역 변수에 복사해야 클로저 문제가 안 생김
            var capturedValue = binding.value;
            binding.button.onClick.AddListener(() => HandleSelection(capturedValue));
        }
    }

    void HandleSelection(int value)
    {
        BattleManager.TargetEnemy_Int = value;
        Debug.Log($"선택된 값: {value}");

        choiceButtonTrue = true;
        SetBindingsInteractable(false);
        // 필요 시 여기서 다른 로직 실행
    }

    public void SetBindingsInteractable(bool isInteractable)
    {
        foreach (var binding in bindings)
        {
            if (binding.button != null)
                binding.button.interactable = isInteractable;
        }
    }
}

