using UnityEngine;

[CreateAssetMenu(fileName = "MileStones", menuName = "Attributes/MileStones")]
public class MileStones : ScriptableObject {
    [Space]
    [Header("List of Milestones of the Game")]
    public string[] mileStones;

    [Space]
    [Header("Minimum number to use Artifacts")]
    public int bombMileStone;
    public int hookMilestone;

    public string GetName(int index) {
        return mileStones[index];
    }

    public bool CanUseBomb(int mile) {
        return mile >= bombMileStone;
    }

    public bool CanUseHook(int mile) {
        return mile >= hookMilestone;
    }

}
