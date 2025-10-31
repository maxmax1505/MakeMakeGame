using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public BattleManager battleManager;
    public ItemListItem itemListItem;
    public Canvas BattleCanvas;
    public Canvas PlanetCanvas;
    public Canvas CoreInventoryCanvas;
    public Canvas WinCanvas;

    public bool IsInDungeon = false;

    public void ClickToStartBattle ()
    {
        BattleCanvas.gameObject.SetActive(true);
        battleManager.LetsStartBattle();

        PlanetCanvas.gameObject.SetActive(false);
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
