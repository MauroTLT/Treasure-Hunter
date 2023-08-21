
public class Armor : Item {

    public int attack;
    public int dextery;
    public int defense;
    public int resistance;

    public Armor(int id, string name, int attack, int dextery, int defense, int resistance) : base(id, name) {
        this.attack = attack;
        this.dextery = dextery;
        this.defense = defense;
        this.resistance = resistance;
    }

    public Armor(int id, string name, int numHolding, int attack, int dextery, int defense, int resistance) : base(id, name, numHolding) {
        this.attack = attack;
        this.dextery = dextery;
        this.defense = defense;
        this.resistance = resistance;
    }
}
