using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum ItemType { Gun = 0 }

public class ItemListIem : MonoBehaviour
{
    [SerializeField] Transform contentRoot;
    [SerializeField] ItemSlotUI itemPrefab;
    [SerializeField] BattleManager battleManager;

    void Start()
    {
        Refresh(new List<IItem> { new NormalPistol(), new NormalPistol() });
        Debug.Log(battleManager.guns[0]);
    }

    public void Refresh(List<IItem> itemList)
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var items in itemList)
        {
            var slot = Instantiate(itemPrefab, contentRoot);
            slot.Bind(items);
        }
    }
}


