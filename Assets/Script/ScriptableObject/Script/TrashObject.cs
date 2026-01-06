using UnityEngine;

[CreateAssetMenu(fileName = "New Trash Ojbect", menuName = "Inventory System/Items/Trash")]
public class TrashObject : ItemObject
{
    [Header("Trash Settings")]
    public TrashType trashType;
    public int minSellValue = 1;
    public int maxSellValue = 10;

    public bool isBig = true;
    private void Awake()
    {
        type = ItemType.Trash;
        GenerateRandomValue();
    }

    public void GenerateRandomValue()
    {
        price = Random.Range(minSellValue, maxSellValue + 1); // Inclusive of maxValue
    }
}
