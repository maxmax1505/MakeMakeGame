using System;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] TMP_Text playerStatUI_Text;

    [SerializeField] SlotNumber gunNUM;
    [SerializeField] SlotNumber headNUM;
    [SerializeField] SlotNumber bodyNUM;
    [SerializeField] SlotNumber armNUM;
    [SerializeField] SlotNumber legNUM;

    public List<IItem> PlayerEquippedList;
    public List<IItem> PlayerInventoryList;

    [SerializeField] public Dictionary<ItemType, SlotNumber> slotNumberLookup;

    void Awake()
    {
        slotNumberLookup = new Dictionary<ItemType, SlotNumber>
        {
            { ItemType.Gun,  gunNUM },
            { ItemType.Head, headNUM },
            { ItemType.Body, bodyNUM },
            { ItemType.Arm,  armNUM },
            { ItemType.Leg,  legNUM }
        };
    }

    void Start()
    {
        PlayerInventoryList = new List<IItem>();
        PlayerEquippedList = new List<IItem>
        {
            new NormalPistol(), new NormalPistol(),
            PG.GenerateRandomPart(), PG.GenerateRandomPart(),
            PG.GenerateRandomPart(), PG.GenerateRandomPart(),
            PG.GenerateRandomPart(), PG.GenerateRandomPart(),
            PG.GenerateRandomPart(), PG.GenerateRandomPart(),
            PG.GenerateRandomPart(), PG.GenerateRandomPart()
        };

        Refresh();
    }

    public void Refresh()
    {
        List<Transform> roots = new List<Transform> { contentRoot_Gun, contentRoot_Arm, contentRoot_Body, contentRoot_Head, contentRoot_Leg };

        foreach (var kv in slotNumberLookup)
        {
            kv.Value.AllSlotNum = 0;
            kv.Value.CurrentSlotNum = Mathf.Clamp(kv.Value.CurrentSlotNum, 1, 1);
            kv.Value.UpdateSlotNum();
        }

        foreach (var root in roots)
        {
            if (root == null) continue;
            foreach (Transform child in root)
                Destroy(child.gameObject);
        }

        foreach (var item in PlayerEquippedList)
        {
            Transform root = item.itemType switch
            {
                ItemType.Gun => contentRoot_Gun,
                ItemType.Head => contentRoot_Head,
                ItemType.Body => contentRoot_Body,
                ItemType.Arm => contentRoot_Arm,
                _ => contentRoot_Leg
            };

            slotNumberLookup[item.itemType].AllSlotNum++;
            slotNumberLookup[item.itemType].UpdateSlotNum();

            var slotInstance = Instantiate(ItemPrefab, root);
            Button slotButton = slotInstance.GetComponent<Button>();
            slotInstance.Init(this);
            slotInstance.Bind(item, slotButton, slotInstance.gameObject);
        }

        foreach (Transform child in ContentInventory)
            Destroy(child.gameObject);

        foreach (var item in PlayerInventoryList)
        {
            var slot = Instantiate(ItemPrefab, ContentInventory);
            Button slotButton = slot.GetComponent<Button>();
            slot.Init(this);
            slot.Bind_Inventory(item, slotButton, slot.gameObject);
        }

        Update_StatUI();

        foreach (var v in slotNumberLookup)
        {
           if(v.Value.AllSlotNum == 0)
            {
                v.Value.CurrentSlotNum = 0;
                v.Value.UpdateSlotNum();
            }
        }
    }

    public void UnEqquip(IItem item, GameObject slotObject)
    {
        if (item == null || slotObject == null) return;

        PlayerEquippedList?.Remove(item);
        PlayerInventoryList.Add(item);

        if (slotNumberLookup.TryGetValue(item.itemType, out var counter))
        {
            counter.AllSlotNum--;
            counter.UpdateSlotNum();
        }

        Destroy(slotObject);
        Refresh();

        if (item is BodyPart bodyPart)
        {
            battleManager.ApplyModifiers(bodyPart.bonuses, false);
            battleManager.ApplyModifiers(bodyPart.penalties, false);
        }

        Refresh();
    }

    public void Eqquip(IItem item, GameObject slotObject)
    {
        if (item == null || slotObject == null) return;

        PlayerInventoryList.Remove(item);
        PlayerEquippedList.Add(item);

        if (slotNumberLookup.TryGetValue(item.itemType, out var counter))
        {
            counter.AllSlotNum++;
            counter.UpdateSlotNum();
        }

        Destroy(slotObject);
        Refresh();

        if (item is BodyPart bodyPart)
        {
            battleManager.ApplyModifiers(bodyPart.bonuses, true);
            battleManager.ApplyModifiers(bodyPart.penalties, true);
        }

        Refresh();
    }


    void Update_StatUI()
    {
        if (playerStatUI_Text != null)
            playerStatUI_Text.text = battleManager.PlayerStat();
    }
}
