using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BagSlot {

    public Item item;
    public int amount;

    public BagSlot Clone() {
        BagSlot bagSlot = new BagSlot();
        bagSlot.item = this.item;
        bagSlot.amount = this.amount;
        return bagSlot;
    }

    public override string ToString() {
        return item.name + " | " + amount;
    }
}

public class Bag : Singleton<Bag> {

    public Bag_SO bag;

    public bool PutInBag(Item item) {
        for (int i = 0; i < bag.slots.Length; i++) {
            if (bag.slots[i].item != null && bag.slots[i].item.Equals(item)) {
                if (bag.slots[i].amount + 1 > 99) {
                    return false;
                }
                bag.slots[i].amount++;
                return true;
            }
        }
        for (int i = 0; i < bag.slots.Length; i++) {
            if (bag.slots[i].item == null) {
                bag.slots[i].item = item;
                bag.slots[i].amount++;
                return true;
            }
        }
        return false;
    }

    public bool PutInBag(Item item, int amount) {
        if (amount < 100) {
            for (int i = 0; i < bag.slots.Length; i++) {
                if (bag.slots[i].item != null && bag.slots[i].item.Equals(item)) {
                    if (bag.slots[i].amount + amount > 99) {
                        return false;
                    }
                    bag.slots[i].amount += amount;
                    return true;
                }
            }
            for (int i = 0; i < bag.slots.Length; i++) {
                if (bag.slots[i].item == null) {
                    bag.slots[i].item = item;
                    bag.slots[i].amount += amount;
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveFromBag(Item item) {
        if (item != null) {
            for (int i = 0; i < bag.slots.Length; i++) {
                if (bag.slots[i].item != null && bag.slots[i].item.Equals(item)) {
                    bag.slots[i].amount--;
                    if (bag.slots[i].amount <= 0) {
                        bag.slots[i].amount = 0;
                        bag.slots[i].item = null;
                    }
                    return;
                }
            }
        }
    }

    public void RemoveFromBag(Item item, int amount) {
        if (item != null) {
            for (int i = 0; i < bag.slots.Length; i++) {
                if (bag.slots[i].item != null && bag.slots[i].item.Equals(item)) {
                    bag.slots[i].amount -= amount;
                    if (bag.slots[i].amount <= 0) {
                        bag.slots[i].amount = 0;
                        bag.slots[i].item = null;
                    }
                    return;
                }
            }
        }
    }

    public void OrderBag() {
        bool semaforo = false;
        Item aux;
        int aux2;
        for (int i = 0; i < bag.slots.Length - 1; i++) {
            if (bag.slots[i].item == null) {
                semaforo = false;
                for (int j = i + 1; j < bag.slots.Length && !semaforo; j++) {
                    if (bag.slots[j].item != null) {
                        bag.slots[i].item = bag.slots[j].item;
                        bag.slots[j].item = null;
                        bag.slots[i].amount = bag.slots[j].amount;
                        bag.slots[j].amount = 0;
                        semaforo = true;
                    }
                }
            }
        }

        for (int i = 1; i < bag.slots.Length; i++) {
            for (int j = 0; j < bag.slots.Length - 1; j++) {
                if (bag.slots[j].item != null && bag.slots[j + 1].item != null) {
                    if (bag.slots[j].item.id > bag.slots[j + 1].item.id) {
                        aux = bag.slots[j].item;
                        bag.slots[j].item = bag.slots[j + 1].item;
                        bag.slots[j + 1].item = aux;

                        aux2 = bag.slots[j].amount;
                        bag.slots[j].amount = bag.slots[j + 1].amount;
                        bag.slots[j + 1].amount = aux2;
                    }
                }
            }
        }
    }

    public Item[] GetAllOf(int type) {
        List<Item> list = new List<Item>();

        foreach (BagSlot slot in bag.slots) {
            Item i = slot.item;
            if (i != null) {
                if (i is Weapon && type == 0 || // WEAPON
                i is Helmet && type == 1 || // HELMET
                i is Chestplate && type == 2 || // CHESTPLATE
                i is Shield && type == 3 || // SHIELD
                i is Necklace && type == 4) { // NECKLACE
                    list.Add(i);
                }
            }
        }

        return list.ToArray();
    }

    public int GetAmountById(int id) {
        for (int i = 0; i < bag.slots.Length; i++) {
            if (bag.slots[i].item != null && bag.slots[i].item.id == id) {
                return bag.slots[i].amount;
            }
        }
        return 0;
    }

}
