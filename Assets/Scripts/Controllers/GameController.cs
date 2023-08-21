using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Map where the player starts by default in every zone
    public GameObject initialMap;
    // The Zone object of the actual zone
    public Zone zone;
    public MileStones mileStones;
    public Bag_SO gameBag;
    public PartyMember[] stats = new PartyMember[2];
    public static SoundController soundController;
    public static DialogController dialogController;
    public static GameObject itemDescriptor;

    private GameObject player;
    private static float constA = 4.8F; // Level gap rise, decrease goes up
    private static float constB = -20.5F; // Initial level Gap
    private static float constC = Mathf.Exp((1 - constB) / constA);// Not change;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        GameManager.actualZone = zone;

        // Load the bag at the start of a zone
        if (player.GetComponent<Bag>().bag == null) {
            player.GetComponent<Bag>().bag = gameBag;
        }

        if (GameManager.isGameLoaded) {
            GameManager.isGameLoaded = false;

            GameManager.LoadBag();

            GameObject room = GameObject.Find(GameManager.actualZone.saveRoom.name);
            Camera.main.GetComponent<MainCamera>().SetBounds(room);
            player.transform.position = GameManager.actualZone.savePos;

            Debug.Log("PLAYER LOADED");
        } else {
            Camera.main.GetComponent<MainCamera>().SetBounds(initialMap);
            if (GameManager.lastZone != null) {
                SpawnPoint();
            } else if (GameManager.lastWarp != null && GameManager.lastWarp.Length > 0) {
                GameObject.Find(GameManager.lastWarp).GetComponent<Warp>().TransportPlayer(player);
            }
        }

        // ONLY FOR DEBUGING PURPOSES
        if (GameManager.party[0] == null) {
            GameManager.party[0] = stats[0].Clone();
            GameManager.party[1] = stats[1].Clone();
            Debug.Log("DEBUGING");
        }
    }

    private void Start() {
        soundController = Camera.main.GetComponent<SoundController>();
        dialogController = FindObjectOfType<DialogController>();
        itemDescriptor = GameObject.FindGameObjectWithTag("ItemDescriptor");
        itemDescriptor.SetActive(false);
        soundController.SetVolume();
        soundController.GetComponent<RandomBattleController>().enabled = GameManager.enemies;
        StartCoroutine(Timer());
    }

    /*
     * Transport the player to the correct warp if it detects that has change the scene 
     */
    private void SpawnPoint() {
        Debug.Log("Moving between Zones: " + GameManager.actualZone + " - " + GameManager.lastZone);
        GameObject warpGO = GameObject.Find(GameManager.actualZone.zoneName + "-" + GameManager.lastZone.zoneName);
        if (warpGO != null) {
            Warp warp = warpGO.GetComponent<Warp>();
            warp.TransportPlayer(player);
        }
        GameManager.lastZone = null;
        GameManager.lastWarp = null;
    }

    /*
     * This routine controls the time played
     */
    private IEnumerator Timer() {
        string[] time = GameManager.playTime.Split(':');
        int hour = int.Parse(time[0]);
        int min = int.Parse(time[1]);
        int sec = int.Parse(time[2]);

        while (true) {
            yield return new WaitForSecondsRealtime(1F);
            sec++;
            if (sec == 60) {
                sec = 0;
                min++;
                if (min == 60) {
                    min = 0;
                    hour++;
                }
            }
            GameManager.playTime = hour + ":" +
                ((min < 10) ? "0" + min : min.ToString()) + ":" +
                ((sec < 10) ? "0" + sec : sec.ToString());
        }
    }

    /*
     * Method that calculates the damage done from one Character to another
     */
    public static int CalculateDamage(Stats you, Stats target) {
        int damage = ((Random.Range(1, 4) + 2) * you.attack / target.defense) + 1;
        return Mathf.Clamp(Mathf.CeilToInt(damage), 0, 999);
    }

    /*
     * Method that calculates the damage done from one Character to another
     * based on a skill used
     */
    public static int CalculateDamage(Stats you, Stats target, Skill skill) {
        int damage = ((Random.Range(1, 4) * 2) * you.dextery / target.resistance) + 1;
        damage *= skill.level;
        if (skill.element != null) {
            if (skill.element.EffectiveTo(target.element)) {
                damage = Mathf.CeilToInt(float.Parse(damage.ToString()) * 1.5f);
            } else if (skill.element.DisadvantageousTo(target.element)) {
                damage = Mathf.CeilToInt(float.Parse(damage.ToString()) / 1.5f);
            }
        }
        return Mathf.Clamp(damage, 0, 999);
    }

    /*
     * Method that returns the level based of an amount of experience
     */
    public static int XpToLvl(int xp) {
        return (int) Mathf.Max(Mathf.Floor(constA * Mathf.Log(xp + constC) + constB), 1);
    }

    /*
     * Returns the experience needed to level up
     */
    public static int XpToNextLvl(int totalXp) {
        int memberLvl = XpToLvl(totalXp);

        for (int xp = totalXp; xp < 999999; xp++) {
            int lvl = XpToLvl(xp);
            if (memberLvl != lvl) {
                return (xp - totalXp);
            }
        }
        return 0;
    }
}
