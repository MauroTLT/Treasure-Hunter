using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class Weapon : Item {

    public enum Type { Human = 1, Animal = 2, Both = 3}

    public Race effectiveTo;
    public Element element;

    public int attack;
    public int dextery;

    public Type canUse;

    public Weapon(int id, string name, int attack, int dextery) : base(id, name) {
        this.attack = attack;
        this.dextery = dextery;
    }

    public Weapon(int id, string name, int numHolding, int attack, int dextery) : base(id, name, numHolding) {
        this.attack = attack;
        this.dextery = dextery;
    }
}
