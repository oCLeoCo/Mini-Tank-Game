using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpgradePanel : MonoBehaviour
{
    public BuffAndUpgrade buffAndUpgrade { get; set; }
    UpgradePanel upgradePanel;
    Transform[] layoutGroup = new Transform[3]; 
    public GameObject buttionPrefab;

    void Start()
    {
        upgradePanel = GetComponent<UpgradePanel>();
        layoutGroup[0] = transform.GetChild(0).transform;
        layoutGroup[1] = transform.GetChild(1).transform;
        layoutGroup[2] = transform.GetChild(2).transform;
    }
    public void UpgradeOrSwtich(int select)
    {
        buffAndUpgrade.ChangeEquipment(select);
    }
    public void InstantiateButton()
    {
        for(int x = 0; x < buffAndUpgrade.upgradeWeapons.Length; x++)
        {
            GameObject button = Instantiate(buttionPrefab,layoutGroup[0]);
            SwitchEquipmentButton switichEquipmentButton = button.GetComponent<SwitchEquipmentButton>();
            switichEquipmentButton.GetAllChildObject();
            switichEquipmentButton.upgradePanel = upgradePanel;
            switichEquipmentButton.switichNum = 10 + x;
            switichEquipmentButton.ChangeEquipmentName(buffAndUpgrade.upgradeWeapons[x].weaponName);
            buffAndUpgrade.upgradeWeapons[x].OnPriceChange += switichEquipmentButton.ChangePrice;
        }
        for (int x = 0; x < buffAndUpgrade.upgradeArmors.Length; x++)
        {
            GameObject button = Instantiate(buttionPrefab, layoutGroup[1]);
            SwitchEquipmentButton switichEquipmentButton = button.GetComponent<SwitchEquipmentButton>();
            switichEquipmentButton.GetAllChildObject();
            switichEquipmentButton.upgradePanel = upgradePanel;
            switichEquipmentButton.switichNum = 20 + x;
            switichEquipmentButton.ChangeEquipmentName(buffAndUpgrade.upgradeArmors[x].armorName);
            buffAndUpgrade.upgradeArmors[x].OnPriceChange += switichEquipmentButton.ChangePrice;
        }
        for (int x = 0; x < buffAndUpgrade.upgradeEngins.Length; x++)
        {
            GameObject button = Instantiate(buttionPrefab, layoutGroup[2]);
            SwitchEquipmentButton switichEquipmentButton = button.GetComponent<SwitchEquipmentButton>();
            switichEquipmentButton.GetAllChildObject();
            switichEquipmentButton.upgradePanel = upgradePanel;
            switichEquipmentButton.switichNum = 30 + x;
            switichEquipmentButton.ChangeEquipmentName(buffAndUpgrade.upgradeEngins[x].enginName);
            buffAndUpgrade.upgradeEngins[x].OnPriceChange += switichEquipmentButton.ChangePrice;
        }
    }
}
