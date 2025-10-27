using UnityEngine;

public class TestBattleStartButton : MonoBehaviour
{
    public BattleManager battleManager;
    public Canvas BattleCanvas;
    public Canvas PlanetCanvas;



    public void ClickToStartBattle ()
    {
        BattleCanvas.gameObject.SetActive(true);
        battleManager.LetsStartBattle();

        PlanetCanvas.gameObject.SetActive(false);
    }
}
