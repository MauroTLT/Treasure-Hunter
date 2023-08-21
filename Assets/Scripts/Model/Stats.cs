using System.Collections.Generic;
using UnityEngine;

public class Stats : ScriptableObject {

    [Header("Basic Stuff")]
    public Sprite sprite;
    public string fullname;
    public int level;
    public Element element;

    [Space]
    [Header("Stats")]
    public int maxHp;
    public int maxSp;
    public int attack;
    public int dextery;
    public int defense;
    public int resistance;
    [Space]
    //[HideInInspector]
    public int hp;
    //[HideInInspector]
    public int sp;

    [Space]
    [Header("Skills")]
    public List<Skill> skills = new List<Skill>();

    public void TakeDamage(int damage) {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    public Skill GetRandomSkill() {
        List<Skill> canUse = skills.FindAll(
            delegate (Skill s) {
                return s.cost <= sp;
            }
        );
        return canUse[Random.Range(0, canUse.Count)];
    }

    public bool CanDoAnySkill() {
        foreach (Skill skill in skills) {
            if (skill.cost <= sp) {
                return true;
            }
        }
        return false;
    }

    public Stats Clone() {
        Stats stat = CreateInstance(typeof(Stats)) as Stats;

        stat.sprite = sprite;
        stat.fullname = fullname;
        stat.level = level;
        stat.element = element;

        stat.hp = hp;
        stat.sp = sp;
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