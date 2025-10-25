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
            // ���� ������ �����ؾ� Ŭ���� ������ �� ����
            var capturedValue = binding.value;
            binding.button.onClick.AddListener(() => HandleSelection(capturedValue));
        }
    }

    void HandleSelection(int value)
    {
        BattleManager.TargetEnemy_Int = value;
        Debug.Log($"���õ� ��: {value}");

        choiceButtonTrue = true;
        SetBindingsInteractable(false);
        // �ʿ� �� ���⼭ �ٸ� ���� ����
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

