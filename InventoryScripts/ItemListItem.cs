using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum ItemType { Gun, Head, Leg, Body, Arm }


public class ItemListItem : MonoBehaviour
{

    [SerializeField] Transform contentRoot_Gun;
    [SerializeField] Transform contentRoot_Head;
    [SerializeField] Transform contentRoot_Body;
    [SerializeField] Transform contentRoot_Arm;
    [SerializeField] Transform contentRoot_Leg;

    [SerializeField] Transform ContentInventory;

    [SerializeField] ItemSlotUI ItemPrefab;
    [SerializeField] BattleManager battleManager;
    [SerializeField] BodyPartGenerator PG;

    [SerializeField] public List<IItem> PlayerItemList;

    void Start()
    {
        PlayerItemList = new List<IItem>();

        Refresh(new List<IItem> { new NormalPistol(), new NormalPistol(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart(), PG.GenerateRandomPart() });
        Debug.Log(battleManager.guns[0]);
    }

    public void Refresh(List<IItem> itemList)
    {
        List<Transform> roots = new List<Transform> { contentRoot_Gun, contentRoot_Arm, contentRoot_Body, contentRoot_Head, contentRoot_Leg };

        foreach (var root in roots)
        {
            foreach (Transform child in root)
                Destroy(child.gameObject); // 이때 child는 root의 자식
        }

        foreach (var item in itemList)
        {
            Transform root = item.itemType switch
            {
                ItemType.Gun => contentRoot_Gun,
                ItemType.Head => contentRoot_Head,
                ItemType.Body => contentRoot_Body,
                ItemType.Arm => contentRoot_Arm,
                _ => contentRoot_Leg
            };

            var slotInstance = Instantiate(ItemPrefab, root);
            var slotGO = slotInstance.gameObject;   // 각 슬롯 오브젝트를 별도 변수에 저장
            var capturedItem = item;                      // 반복문 아이템도 별도 변수에 저장

            slotInstance.Bind(capturedItem,
                clickedItem => UnEqquip(clickedItem, slotGO));
        }

        foreach (Transform child in ContentInventory)
            Destroy(child.gameObject);

        foreach (var item in PlayerItemList)
        {
            Debug.Log("장비");
            var slot = Instantiate(ItemPrefab, ContentInventory);

            slot.Bind_Inventory(item);
        }
    }

    public void EquipBodyPart(BodyPart part)
    {
        // 슬롯 교체 등 기존 장착 처리…0

        // 버프 적용
        battleManager.ApplyModifiers(part.bonuses);

        // 디버프 적용
        battleManager.ApplyModifiers(part.penalties);
    }

    public void UnEqquip(IItem item, GameObject button)
    {
        Debug.Log($"[UnEquip] {item} remove {button.GetInstanceID()}");
        PlayerItemList.Add(item);
        Destroy(button);

        foreach (Transform child in ContentInventory)
            Destroy(child.gameObject);

        foreach (var invItem in PlayerItemList)
        {
            var slot = Instantiate(ItemPrefab, ContentInventory);
            slot.Bind_Inventory(invItem);
        }
    }
    
}


