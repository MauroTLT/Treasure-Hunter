using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Element", menuName = "Attributes/Element")]
public class Element : ScriptableObject {
    public enum Type { Tenebrae = 1, Lumen = 2, Tonitrus = 3, Solum = 4, Glacies = 5, Ventus = 6, Ignis = 7, Aqua = 8 }

    public Type element;
    public Type[] advantages;
    public Type[] disadvantages;
    public Sprite sprite;

    public bool EffectiveTo(Element other) {
        if (other != null) {
            foreach (Type elem in advantages) {
                if (elem.Equals(other.element)) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool DisadvantageousTo(Element other) {
        if (other != null) {
            foreach (Type elem in disadvantages) {
                if (elem.Equals(other.element)) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool Equals(Element other) {
        if (other == null) return false;
        return element.Equals(other.element);
    }
}
