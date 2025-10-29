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

    Dictionary<ItemType, SlotNumber> slotNumberLookup;

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
        ResetSlotCounters();
        ClearAllSlots();
        PopulateEquippedSlots();
        PopulateInventorySlots();
        UpdateStatUI();
        UpdateCountersUI();
    }

    void ResetSlotCounters()
    {
        foreach (var kv in slotNumberLookup)
        {
            kv.Value.AllSlotNum = 0;
            kv.Value.CurrentSlotNum = Mathf.Clamp(kv.Value.CurrentSlotNum, 1, 1);
            kv.Value.UpdateSlotNum();
        }
    }

    Transform GetRootForItem(IItem item)
    {
        return item.itemType switch
        {
            ItemType.Gun  => contentRoot_Gun,
            ItemType.Head => contentRoot_Head,
            ItemType.Body => contentRoot_Body,
            ItemType.Arm  => contentRoot_Arm,
            _             => contentRoot_Leg
        };
    }

    void PopulateEquippedSlots()
    {
        foreach (var item in PlayerEquippedList)
        {
            var root = GetRootForItem(item);
            AdjustCounter(item.itemType, +1);

            var slotInstance = Instantiate(ItemPrefab, root);
            var button = slotInstance.GetComponent<Button>();
            slotInstance.Init(this);
            slotInstance.Bind(item, button, slotInstance.gameObject);
        }
    }

    void PopulateInventorySlots()
    {
        foreach (var item in PlayerInventoryList)
        {
            var slot = Instantiate(ItemPrefab, ContentInventory);
            var button = slot.GetComponent<Button>();
            slot.Init(this);
            slot.Bind_Inventory(item, button, slot.gameObject);
        }
    }

    void ClearAllSlots()
    {
        var roots = new[]
        {
            contentRoot_Gun, contentRoot_Arm,
            contentRoot_Body, contentRoot_Head,
            contentRoot_Leg, ContentInventory
        };

        foreach (var root in roots)
        {
            if (root == null) continue;
            foreach (Transform child in root)
                Destroy(child.gameObject);
        }
    }

    void AdjustCounter(ItemType type, int delta)
    {
        if (slotNumberLookup.TryGetValue(type, out var slotNumber))
        {
            slotNumber.AllSlotNum += delta;
            if (slotNumber.AllSlotNum <= 0)
                slotNumber.CurrentSlotNum = 0;
        }
    }

    public void UnEqquip(IItem item, GameObject slotObject)
    {
        if (item == null || slotObject == null) return;

        PlayerEquippedList?.Remove(item);
        PlayerInventoryList.Add(item);
        AdjustCounter(item.itemType, -1);

        Destroy(slotObject);
        Refresh();

        if (item is BodyPart bodyPart)
        {
            battleManager.ApplyModifiers(bodyPart.bonuses, false);
            battleManager.ApplyModifiers(bodyPart.penalties, false);
        }
    }

    public void Eqquip(IItem item, GameObject slotObject)
    {
        if (item == null || slotObject == null) return;

        PlayerInventoryList.Remove(item);
        PlayerEquippedList.Add(item);
        AdjustCounter(item.itemType, +1);

        Destroy(slotObject);
        Refresh();

        if (item is BodyPart bodyPart)
        {
            battleManager.ApplyModifiers(bodyPart.bonuses, true);
            battleManager.ApplyModifiers(bodyPart.penalties, true);
        }
    }

    void UpdateStatUI()
    {
        if (playerStatUI_Text != null)
            playerStatUI_Text.text = battleManager.PlayerStat();
    }

    void UpdateCountersUI()
    {
        foreach (var kv in slotNumberLookup)
        {
            var slot = kv.Value;
            if (slot.AllSlotNum <= 0)
                slot.CurrentSlotNum = 0;
            else
                slot.CurrentSlotNum = Mathf.Clamp(slot.CurrentSlotNum, 1, slot.AllSlotNum);

            slot.UpdateSlotNum();
        }
    }
}
