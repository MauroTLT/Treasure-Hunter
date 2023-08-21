using UnityEngine;

[CreateAssetMenu(fileName = "New Chestplate", menuName = "Items/Armor/Chestplate")]
public class Chestplate : Armor {

    public Chestplate(int id, string name, int attack, int dextery, int defense, int resistence)
        : base(id, name, attack, dextery, defense, resistence) { }
    public Chestplate(int id, string name, int numHolding, int attack, int dextery, int defense, int resistence)
        : base(id, name, numHolding, attack, dextery, defense, resistence) { }
}
