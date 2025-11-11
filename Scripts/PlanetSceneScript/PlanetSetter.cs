using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlanetSetter : MonoBehaviour
{
    public UniversManager universManager;

    public TextMeshProUGUI PlanetNameText;

    public void SetUpPlanetData()
    {
        IPlanet curPlanet = universManager.Planets[universManager.CurrentPlanet];

        PlanetNameText.text = curPlanet.Name;
    }
}
