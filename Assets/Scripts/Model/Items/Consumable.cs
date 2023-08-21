
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class Consumable : Item {

    public enum Stat { HP = 1, MaxHP = 2, SP = 3, MaxSP = 4, Atk = 5, Def = 6, Dex = 7, Res = 8 }
    public Stat statUp;
    public int amount;

    public Consumable(int id, string name) : base(id, name) {}

    public Consumable(int id, string name, int numHolding) : base(id, name, numHolding) {}

    public bool Use(PartyMember member, bool inBattle) {
        if (inBattle) {
            Bag.Instance.RemoveFromBag(this);
        }
        switch (statUp) {
            case Stat.HP:
                if (member.hp == member.maxHp) {
                    return false;
                } else if ((member.hp + amount) > member.maxHp) {
                    member.hp = member.maxHp;
                } else {
                    member.hp += amount;
                }
                break;
            case Stat.SP:
                if (member.sp == member.maxSp) {
                    return false;
                } else if ((member.sp + amount) > member.maxSp) {
                    member.sp = member.maxSp;
                } else {
                    member.sp += amount;
                }
                break;
            case Stat.MaxHP: member.maxHp += amount; break;
            case Stat.MaxSP: member.maxSp += amount; break;
            case Stat.Atk: member.attack += amount; break;
            case Stat.Def: member.defense += amount; break;
            case Stat.Dex: member.dextery += amount; break;
            case Stat.Res: member.resistance += amount; break;
        }
        if (!inBattle) {
            Bag.Instance.RemoveFromBag(this);
        }
        return true;
    }
}
