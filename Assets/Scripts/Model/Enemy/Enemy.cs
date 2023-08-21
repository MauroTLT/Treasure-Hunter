using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy")]
public class Enemy : Stats {

    [Space]
    [Header("Enemy Data")]
    public int gold;
    public int xpDrop;
    public Item drop;
    [Range(0, 100)]
    public int probItemDrop;
    public bool isBoss;

    public new Enemy Clone() {
        Enemy stat = CreateInstance(typeof(Enemy)) as Enemy;

        stat.level = level;
        stat.gold = gold;
        stat.xpDrop = xpDrop;
        stat.drop = drop;

        stat.sprite = sprite;
        stat.fullname = fullname;
        stat.hp = maxHp;
        stat.sp = maxSp;
        stat.maxHp = maxHp;
        stat.maxSp = maxSp;
        stat.attack = attack;
        stat.dextery = dextery;
        stat.defense = defense;
        stat.resistance = resistance;

        stat.skills = skills;

        return stat;
    }
}
