using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Table", menuName = "Enemy/MonsterTable")]

public class MonsterTable : ScriptableObject {

    private const int MAX_GROUP_NUM = 4;

    public Enemy[] possibleEncounters;
    [Range(0, 100)]
    public int[] appearProb;
    [Range(1, MAX_GROUP_NUM)]
    public int[] groupAmount;

    public Enemy[] NewEncounter() {
        Enemy[] enemysInBattle = new Enemy[MAX_GROUP_NUM];
        List<int> pendingToAprove = CheckProbs();
        int alreadyInBattle = 0;
        bool addMore = true;
        while (alreadyInBattle != MAX_GROUP_NUM && addMore) {
            int statIndex = pendingToAprove[Random.Range(0, pendingToAprove.Count)];
            Enemy enemy = possibleEncounters[statIndex];
            int amount = groupAmount[statIndex];
            pendingToAprove.Remove(statIndex);

            int amountToPut = Random.Range(1, amount + 1);
            if ((amountToPut + alreadyInBattle) > MAX_GROUP_NUM) {
                amountToPut = MAX_GROUP_NUM - alreadyInBattle;
            }
            for (int i = 0; i < amountToPut; i++) {
                for (int j = 0; j < MAX_GROUP_NUM; j++) {
                    if (enemysInBattle[j] == null) {
                        enemysInBattle[j] = enemy.Clone();
                        enemysInBattle[j].fullname = enemysInBattle[j].fullname + " " + (j + 1);
                        break;
                    }
                }
            }
            addMore = (pendingToAprove.Count > 0) ? (Random.Range(0, 2) == 0) : false;
        }
        
        return enemysInBattle;
    }

    private List<int> CheckProbs() {
        List<int> pendingToAprove = new List<int>();
        while (pendingToAprove.Count == 0) {
            for (int i = 0; i < possibleEncounters.Length; i++) {
                int prob = appearProb[i];

                if ((Random.Range(0, 100) <= prob)) {
                    pendingToAprove.Add(i);
                }
            }
        }
        return pendingToAprove;
    }
}
