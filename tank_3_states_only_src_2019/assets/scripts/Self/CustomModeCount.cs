using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomModeCount : MonoBehaviour
{
    public GameObject mainMenu;
    public GameManager gm;
    public Count redFactionCount, blueFactionCount;
    public Dropdown playerTankType;
    public Toggle toggle;
    public void StartGame(int faction)
    {
        gm.SetCustomGame(redFactionCount.countNUM, blueFactionCount.countNUM, playerTankType.value, faction, toggle.isOn);
        mainMenu.SetActive(false);
    }
}
