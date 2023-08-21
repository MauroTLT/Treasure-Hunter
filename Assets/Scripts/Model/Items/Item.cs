using UnityEngine;

public abstract class Item : ScriptableObject {

    public int id;
    public string itemName;
    [TextArea]
    public string description;
    public int price;
    public Sprite sprite;

    public Item() { }

    public Item(int id, string name) {
        this.id = id;
        this.name = name;
    }

    public Item(int id, string name, int numHolding) {
        this.id = id;
        this.name = name;
    }

    public bool Equals(Item other) {
        if (other == null) return false;
        return id.Equals(other.id);
    }

}
