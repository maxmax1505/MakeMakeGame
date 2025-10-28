using UnityEngine;



public interface IItem
{
    ItemType itemType {get; set;}
}
public interface IGun
{
    string Name { get; set; }
    int ActionPoint { get; set; }
    int ShotCountPerTurn { get; set; }
    int ShotDamage { get; set; }
    float AimCorrection { get; set; }
}


//총기 클래스들
public class NormalPistol : IGun, IItem
{
    public string Name { get; set; } = "평범한 권총";
    public int ActionPoint { get; set; } = 6;
    public int ShotCountPerTurn { get; set; } = 3;
    public int ShotDamage { get; set; } = 1;
    public float AimCorrection { get; set; } = 10;
    public ItemType itemType { get; set; } = ItemType.Gun;
    public string itemInformation { get; set; }
}
