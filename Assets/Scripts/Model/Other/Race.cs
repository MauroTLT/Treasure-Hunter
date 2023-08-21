using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Race", menuName = "Attributes/Race")]
public class Race : ScriptableObject {

    public enum Type { Humanoid = 1, Spectral = 2, Undead = 3, Beast = 4, Insect = 5, Acuatic = 6, Bird = 7, Plant = 8 }

    public Type race;
    public Element[] disadvantages = new Element[2];

    public bool Equals(Race other) {
        if (other == null) return false;
        return race.Equals(other.race);
    }
}
