using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Party Member", menuName = "PartyMember")]
public class PartyMember : Stats {

    [Space]
    [Header("Experiencie")]
    public int totalXp;

    [Space]
    [Header("Equipment")]
    public Weapon weapon;
    public Helmet helmet;
    public Chestplate chestplate;
    public Shield shield;
    public Necklace necklace;

    [Space]
    [Header("Skill That Can Learn")]
    public List<Skill> canLearn = new List<Skill>();

    public PartyMember() { }
    public PartyMember(string fullname, int maxHp, int maxSp, int attack, int dextery, int defense, int resistance) {
        this.fullname = fullname;
        this.level = 1;
        this.hp = maxHp/2;
        this.maxHp = maxHp;
        this.sp = maxSp/2;
        this.maxSp = maxSp;
        this.attack = attack;
        this.dextery = dextery;
        this.defense = defense;
        this.resistance = resistance;
    }

    public bool SumXP(int value) {
        this.totalXp += value;
        int level = GameController.XpToLvl(totalXp);
        if (level != this.level) {
            for (int i = level - this.level; i > 0; i--) {
                this.level++;
                LevelUp();
            }
            return true;
        }
        return false;
    }

    private void LevelUp() {
        int points = Random.Range(20, 31);
        int toHp = Mathf.RoundToInt(points / 3F);
        if (maxHp + toHp <= 999) {
            maxHp += toHp;
            points -= toHp;
        } else {
            points -= 999 - maxHp;
            maxHp = 999;
        }
        int toSp = Random.Range(4, 7);
        if (maxSp + toSp <= 99) {
            maxSp += toSp;
            points -= toSp;
        } else {
            points -= 999 - maxSp;
            maxSp = 999;
        }
        for ( ; points > 0 ; points--) {
            int prob = Random.Range(0, 4);
            switch (prob) {
                case 0: attack++; break;
                case 1: defense++; break;
                case 2: dextery++; break;
                case 3: resistance++; break;
            }
        }
        if (canLearn.Count != 0) {
            Skill skill = canLearn.Find(s => s.levelToLearn == level);
            if (skill != null) {
                LearnSkill(skill);
            }
        }
    }

    public void LearnSkill(Skill skill) {
        // Checks the id attribute since we override the Equals method
        if (!skills.Contains(skill)) {
            skills.Add(skill);
        }
    }

    public void SetWeapon(Weapon value) {
        if (weapon != null) {
            attack -= weapon.attack;
            dextery -= weapon.dextery;
            Bag.Instance.PutInBag(weapon);
        }
        if (value != null) {
            attack += value.attack;
            dextery += value.dextery;
            Bag.Instance.RemoveFromBag(value);
        }
        weapon = value;
    }

    public void SetHelmet(Helmet value) {
        if (helmet != null) {
            attack -= helmet.attack;
            dextery -= helmet.dextery;
            defense -= helmet.defense;
            resistance -= helmet.resistance;
            Bag.Instance.PutInBag(helmet);
        }
        if (value != null) {
            attack += value.attack;
            dextery += value.dextery;
            defense += value.defense;
            resistance += value.resistance;
            Bag.Instance.RemoveFromBag(value);
        }
        helmet = value;
    }

    public void SetChestplate(Chestplate value) {
        if (chestplate != null) {
            attack -= chestplate.attack;
            dextery -= chestplate.dextery;
            defense -= chestplate.defense;
            resistance -= chestplate.resistance;
            Bag.Instance.PutInBag(chestplate);
        }
        if (value != null) {
            attack += value.attack;
            dextery += value.dextery;
            defense += value.defense;
            resistance += value.resistance;
            Bag.Instance.RemoveFromBag(value);
        }
        chestplate = value;
    }

    public void SetShield(Shield value) {
        if (shield != null) {
            attack -= shield.attack;
            dextery -= shield.dextery;
            defense -= shield.defense;
            resistance -= shield.resistance;
            Bag.Instance.PutInBag(shield);
        }
        if (value != null) {
            attack += value.attack;
            dextery += value.dextery;
            defense += value.defense;
            resistance += value.resistance;
            Bag.Instance.RemoveFromBag(value);
        }
        shield = value;
    }

    public void SetNecklace(Necklace value) {
        if (necklace != null) {
            attack -= necklace.attack;
            dextery -= necklace.dextery;
            defense -= necklace.defense;
            resistance -= necklace.resistance;
            element = null;
            Bag.Instance.PutInBag(necklace);
        }
        if (value != null) {
            attack += value.attack;
            dextery += value.dextery;
            defense += value.defense;
            resistance += value.resistance;
            element = value.element;
            Bag.Instance.RemoveFromBag(value);
        }
        necklace = value;
    }

    public void HealAll() {
        hp = maxHp;
        sp = maxSp;
    }

    public new PartyMember Clone() {
        PartyMember stat = CreateInstance(typeof(PartyMember)) as PartyMember;

        stat.sprite = sprite;
        stat.fullname = fullname;
        stat.element = element;

        stat.hp = hp;
        stat.sp = sp;
        stat.maxHp = maxHp;
        stat.maxSp = maxSp;
        stat.attack = attack;
        stat.dextery = dextery;
        stat.defense = defense;
        stat.resistance = resistance;

        stat.level = GameController.XpToLvl(totalXp);
        stat.totalXp = totalXp;

        stat.skills = new List<Skill>();
        foreach (var item in skills) {
            stat.skills.Add(item);
        }

        stat.canLearn = new List<Skill>();
        foreach (var item in canLearn) {
            stat.canLearn.Add(item);
        }

        stat.weapon = weapon;
        stat.helmet = helmet;
        stat.chestplate = chestplate;
        stat.shield = shield;
        stat.necklace = necklace;

        return stat;
    }

}
