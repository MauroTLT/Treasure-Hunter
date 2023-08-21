using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyStatsController : MonoBehaviour {

    public Sprite dialogDefault, dialogSelected;
    public GameObject[] partyMembers = new GameObject[2];
    private GameObject[] cursors = new GameObject[2];

    private void Awake() {
        partyMembers[0] = transform.GetChild(0).gameObject;
        partyMembers[1] = transform.GetChild(1).gameObject;
        cursors[0] = partyMembers[0].transform.GetChild(1).GetChild(1).gameObject;
        cursors[1] = partyMembers[1].transform.GetChild(1).GetChild(1).gameObject;
    }

    public void SetAll() {
        SetZone();
        SetPartyStats();
    }

    public void SetZone() {
        GameObject zone = GameObject.Find("CurrentZone");
        if (zone != null) {
            zone.GetComponentInChildren<Text>().text = GameManager.actualZone.zoneName;
        }
    }

    public void SetPartyStats() {
        for (int i = 0; i < partyMembers.Length; i++) {
            PartyMember member = GameManager.party[i];
            Transform stats = partyMembers[i].transform.GetChild(0);
            
            stats.Find("Name").GetComponent<Text>().text = member.fullname;
            stats.Find("Level").GetComponent<Text>().text = "Lvl: " + member.level;

            stats.Find("Health").GetComponent<Slider>().maxValue = member.maxHp;
            stats.Find("Health").GetComponent<Slider>().value = member.hp;
            stats.Find("Health").Find("Text").GetComponent<Text>().text = member.hp + "/" + member.maxHp;

            stats.Find("Magic").GetComponent<Slider>().maxValue = member.maxSp;
            stats.Find("Magic").GetComponent<Slider>().value = member.sp;
            stats.Find("Magic").Find("Text").GetComponent<Text>().text = member.sp + "/" + member.maxSp;
        }
    }

    public void SelectMemberCursor(int i) {
        cursors[i].SetActive(true);
        cursors[Mathf.Abs(i - 1)].SetActive(false);
    }

    public void ShowCursor(int i) {
        cursors[i].SetActive(true);
    }

    public void HideCursor(int i) {
        cursors[i].SetActive(false);
    }

    public void SelectMemberDialog(int i) {
        partyMembers[i].GetComponent<Image>().sprite = dialogSelected;
        partyMembers[Mathf.Abs(i - 1)].GetComponent<Image>().sprite = dialogDefault;
    }

    public void SetDefaultDialog(int i) {
        partyMembers[i].GetComponent<Image>().sprite = dialogDefault;
    }

    public void SetSelectedDialog(int i) {
        partyMembers[i].GetComponent<Image>().sprite = dialogSelected;
    }

    public IEnumerator ShowNumber(int index, int number, Color color) {
        GameObject text = partyMembers[index].transform.GetChild(2).gameObject;
        Vector3 initialPos = text.transform.position;
        text.GetComponent<Text>().text = number.ToString();
        text.GetComponent<Text>().color = color;
        text.SetActive(true);
        for (int i = 0; i < 40; i++) {
            text.transform.position = new Vector3(
                text.transform.position.x,
                text.transform.position.y + 1,
                text.transform.position.z
            );
            yield return new WaitForSeconds(0.025f);
        }

        text.SetActive(false);
        text.transform.position = initialPos;
    }
}
