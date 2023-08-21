using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Bag", menuName = "Items/Bag", order = -1)]
public class Bag_SO : ScriptableObject {

    public BagSlot[] slots = new BagSlot[80];

    public Bag_SO Clone() {
        Bag_SO newBag = CreateInstance<Bag_SO>();

        for (int i = 0; i < slots.Length; i++) {
            newBag.slots[i] = slots[i].Clone();
        }

        return newBag;
    }

}
