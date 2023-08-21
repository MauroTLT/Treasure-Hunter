using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zone", menuName = "Zone")]
public class Zone : ScriptableObject {
    
    [Space]
    [Header("Zone Attributtes")]
    public string zoneName;

    [Space]
    [Header("Save Point")]
    public GameObject saveRoom;
    public Vector2 savePos;

    [Space]
    [Header("Enemys")]
    public MonsterTable enemyTable;
    public Sprite[] battleBackgrounds;

    [Space]
    [Header("Booleans")]
    public bool canUseArtifacts;

    public Sprite GetBattleBackground() {
        return battleBackgrounds[Random.Range(0, battleBackgrounds.Length)];
    }
}
