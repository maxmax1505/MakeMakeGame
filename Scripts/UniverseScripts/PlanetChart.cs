using UnityEngine;

public interface IPlanet
{
    Vector2 poSiTion { get; set; }
    string Name { get; set; }
    int PlanetIndex { get; set; }
}

public class NormalPlanet : IPlanet
{
    public Vector2 poSiTion { get; set; }
    public string Name { get; set; }
    public int PlanetIndex { get; set; }
}

public class PlanetChart : MonoBehaviour
{
}


