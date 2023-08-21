using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager {

    public static PartyMember[] party = new PartyMember[2];
    public static string[] artifactsUnlock = { "Arrow", null, null, null};

    public static bool isGameLoaded = false;
    public static bool gamePaused = false;
    public static bool inBattle = false;
    public static bool showingItem = false;

    public static bool enemies = true;
    public static int battleSpeed = 1;
    public static float musicVolume = 0.5F;
    public static float sfxVolume = 0.5F;


    public static int lastMileStone = 0;
    public static string playTime = "00:00:00";
    public static int gold = 100;

    public static Zone actualZone;
    public static Zone lastZone;
    public static string lastWarp = null;

    private static List<int> chestsOpened = new List<int>();

    /*
     * Method to switch between scenes
     * Needs a Zone object passed by parameter and save the last zone
     */
    public static void ChangeScene(Zone newZone) {
        lastZone = actualZone;
        SceneManager.LoadScene(newZone.zoneName);
    }

    /*
     * Method to switch between scenes
     * Needs only a string passed by parameter
     */
    public static void ChangeScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    /*
     * Method that check if a chest is already open based in the ID passed
     */
    public static bool IsChestOpen(int chestId) {
        return chestsOpened.Contains(chestId);
    }

    /*
     * Method that registers a chest when is opened by the player.
     */
    public static void NewChestOpen(int chestOpen) {
        if (!IsChestOpen(chestOpen)) {
            chestsOpened.Add(chestOpen);
        }
    }

    /*
     * Method to sum an amount of gold to the balance of the player
     */
    public static void SumGold(int amount) {
        gold += amount;
    }

    /*
     * Method to rest an amount of gold to the balance of the player
     */
    public static bool RestGold(int amount) {
        if (gold - amount < 0) {
            return false;
        }
        gold -= amount;
        return true;
    }

    /*
     * Method that registers a new artefact unlocked by the player
     * and prepare it to use it
     */
    public static void NewArtifact(string name) {
        for (int i = 0; i < artifactsUnlock.Length; i++) {
            if (artifactsUnlock[i] == null) {
                artifactsUnlock[i] = name;
                Object.FindObjectOfType<Player>().GetArtefacts();
                Object.FindObjectOfType<ArtifactController>().SetSprites();
                return;
            }
        }
    }

    /*
     * Method that call all the methods needed to save
     * the full game stadistics at the moment of call it
     */
    public static void SaveGame(bool saveZone) {
        SavePlayerPrefs(saveZone);
        SavePartyStats();
        SaveBag();
    }

    /*
     * Sub method that save the player preferences
     * this includes, options, milestone, time and gold,
     * the actual zone and last warp and the chest register
     */
    private static void SavePlayerPrefs(bool saveZone) {
        PlayerPrefs.SetInt("Enemies", (enemies) ? 1 : 0);
        PlayerPrefs.SetInt("BattleSpeed", battleSpeed);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SoundVolume", sfxVolume);

        PlayerPrefs.SetInt("LastMileStone", lastMileStone);

        PlayerPrefs.SetString("PlayTime", playTime);
        PlayerPrefs.SetInt("Gold", gold);

        if (saveZone) {
            PlayerPrefs.SetString("LastWarp", lastWarp);
            PlayerPrefs.SetString("Zone", actualZone.name);
        }

        string chests = "";
        chestsOpened.ForEach((chest) => chests += chest + " ");
        PlayerPrefs.SetString("ChestsOpened", chests);

        Debug.Log("GAME SAVED");
    }

    /*
     * Counterpart of the method before, loads the player preferences
     */
    public static void LoadPlayerPrefs() {
        enemies = PlayerPrefs.GetInt("Enemies", 1) == 1;
        battleSpeed = PlayerPrefs.GetInt("BattleSpeed", 1);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5F);
        sfxVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5F);

        lastMileStone =  PlayerPrefs.GetInt("LastMileStone", 0);

        playTime = PlayerPrefs.GetString("PlayTime", "00:00:00");
        gold = PlayerPrefs.GetInt("Gold", 100);

        lastWarp = PlayerPrefs.GetString("LastWarp", lastWarp);

        string zone = PlayerPrefs.GetString("Zone", "Merix");
        actualZone = Resources.Load("ScriptableObjects/Zones/" + zone) as Zone;


        string chests = PlayerPrefs.GetString("ChestsOpened");
        string[] array = chests.Split(' ');
        foreach (var item in array) {
            if (item != null && !item.Equals("") && !item.Equals(" ")) {
                NewChestOpen(int.Parse(item));
            }
        }
        isGameLoaded = true;
        Debug.Log("GAME LOADED");
    }

    /*
     * Sub method that save the player party stats
     */
    private static void SavePartyStats() {
        for (int i = 0; i < party.Length; i++) {
            PlayerPrefs.SetString("PartyMember_" + i, JsonUtility.ToJson(party[i]));
        }
    }

    /*
     * Sub method that save the player bag of items
     */
    private static void SaveBag() {
        Bag_SO gameBag = GameObject.FindGameObjectWithTag("Player").GetComponent<Bag>().bag;
        string sBag = "";
        for (int i = 0; i < gameBag.slots.Length; i++) {
            sBag += ((gameBag.slots[i].item != null) ? gameBag.slots[i].item.name : " ") + "/" + gameBag.slots[i].amount + "|";
        }

        PlayerPrefs.SetString("SavedBag", sBag);
    }

    /*
     * Counterpart of the method before, loads the player bag of items
     */
    public static void LoadBag() {
        Bag_SO gameBag = GameObject.FindGameObjectWithTag("Player").GetComponent<Bag>().bag;
        string sBag = PlayerPrefs.GetString("SavedBag");
        string[] slots = sBag.Split('|');

        for (int i = 0; i < gameBag.slots.Length; i++) {
            string[] item = slots[i].Split('/');
            if (item[0].Length > 1) {
                gameBag.slots[i].item = Resources.Load<Item>(GetPath(item[0]) + item[0]);
                gameBag.slots[i].amount = int.Parse(item[1]);
            }
        }
    }

    /*
     * Method that returns the path of the ScriptableObject of an item
     */
    private static string GetPath(string item) {
        int id = int.Parse(item.Split('-')[0]);
        if (id < 50) {
            return "ScriptableObjects/Items/Consumables/";
        } else if (id < 100) {
            return "ScriptableObjects/Items/Weapons/";
        } else if (id < 150) {
            return "ScriptableObjects/Items/Helmets/";
        } else if (id < 200) {
            return "ScriptableObjects/Items/Chestplates/";
        } else if (id < 250) {
            return "ScriptableObjects/Items/Shields/";
        } else if (id < 300) {
            return "ScriptableObjects/Items/Necklaces/";
        } else if (id < 305) {
            return "ScriptableObjects/Artifacts/";
        } else {
            return "ScriptableObjects/Items/Specials/";
        }
    }

    /*
     * Method to get the actual Movement of the player, based on the axis
     */
    public static Vector2 GetMov() {
        Vector2 mov = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        return new Vector2(
            (mov.x == 0) ? 0 : (mov.x < 0) ? Mathf.Floor(mov.x) : Mathf.Ceil(mov.x),
            (mov.y == 0) ? 0 : (mov.y < 0) ? Mathf.Floor(mov.y) : Mathf.Ceil(mov.y)
        );
    }

}
