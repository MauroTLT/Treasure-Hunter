using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Attributes/Skill")]
public class Skill : ScriptableObject {

    public int id;
    public string skillName;
    [TextArea]
    public string description;
    public Element element;
    public int cost;
    public int level;
    public int levelToLearn;

    public bool Equals(Skill other) {
        if (other == null) return false;
        return (id.Equals(other.id));
    }
}
