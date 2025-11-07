using UnityEngine;

public interface IPlanet
{
    Vector2 poSiTion { get; set; }
}

public class NormalPlanet : IPlanet
{
    public Vector2 poSiTion { get; set; }
}

public class PlanetChart : MonoBehaviour
{
}


