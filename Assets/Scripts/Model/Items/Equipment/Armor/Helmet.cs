using UnityEngine;

[CreateAssetMenu(fileName = "New Helmet", menuName = "Items/Armor/Helmet")]
public class Helmet : Armor {

    public Helmet(int id, string name, int attack, int dextery, int defense, int resistance)
        : base(id, name, attack, dextery, defense, resistance) { }
    public Helmet(int id, string name, int numHolding, int attack, int dextery, int defense, int resistance)
        : base(id, name, numHolding, attack, dextery, defense, resistance) { }

}


