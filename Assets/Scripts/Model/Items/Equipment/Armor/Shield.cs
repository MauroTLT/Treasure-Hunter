using UnityEngine;

[CreateAssetMenu(fileName = "New Shield", menuName = "Items/Armor/Shield")]
public class Shield : Armor {

    public Shield(int id, string name, int attack, int dextery, int defense, int resistence)
        : base(id, name, attack, dextery, defense, resistence) { }
    public Shield(int id, string name, int numHolding, int attack, int dextery, int defense, int resistence)
        : base(id, name, numHolding, attack, dextery, defense, resistence) { }
}
