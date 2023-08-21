using UnityEngine;

[CreateAssetMenu(fileName = "New Necklace", menuName = "Items/Armor/Necklace")]
public class Necklace : Armor {

    public Element element;
    public Necklace(int id, string name, int attack, int dextery, int defense, int resistence)
        : base(id, name, attack, dextery, defense, resistence) { }
    public Necklace(int id, string name, int numHolding, int attack, int dextery, int defense, int resistence)
        : base(id, name, numHolding, attack, dextery, defense, resistence) { }
}
