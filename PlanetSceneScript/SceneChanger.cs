using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public BattleManager battleManager;
    public ItemListItem itemListItem;
    public DungeonManager dungeonManager;

    public Canvas BattleCanvas;
    public Canvas PlanetCanvas;
    public Canvas CoreInventoryCanvas;
    public Canvas WinCanvas;
    public Canvas DungeonCanvas;

    public bool IsInDungeon = false;

    public void ClickToStartBattle ()
    {
        BattleCanvas.gameObject.SetActive(true);
        battleManager.LetsStartBattle();

        PlanetCanvas.gameObject.SetActive(false);
    }
    public void GotoDungeon()
    {
        PlanetCanvas.gameObject.SetActive(false);
        DungeonCanvas.gameObject.SetActive(true);

        StartCoroutine(dungeonManager.TravelInDungeon());
    }
    public void DungeonToBattle()
    {
        DungeonCanvas.gameObject.SetActive(false);
        BattleCanvas.gameObject.SetActive(true);

        battleManager.LetsStartBattle();
    }
    public void WinToDungeon()
    {
        WinCanvas.gameObject.SetActive(false);

        DungeonCanvas.gameObject.SetActive(true);

        dungeonManager.NotInBattle = true;
    }
    public void WinPageLoad()
    {
        BattleCanvas.gameObject.SetActive(false);

        WinCanvas.gameObject.SetActive(true);
        itemListItem.Win_Refresh();
    }
    public void ClickToGoInventory()
    {
        CoreInventoryCanvas.gameObject.SetActive(true);

        PlanetCanvas.gameObject.SetActive(false);
    }
    public void BackToPlanetFromInven()
    {
        if (IsInDungeon == false)
        {
            PlanetCanvas.gameObject.SetActive(true);

            CoreInventoryCanvas.gameObject.SetActive(false);
        }
        else
        {

        }
    }
}
