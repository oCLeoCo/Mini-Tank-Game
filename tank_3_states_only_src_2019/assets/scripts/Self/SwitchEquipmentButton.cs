using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchEquipmentButton : MonoBehaviour
{
    public UpgradePanel upgradePanel { get; set; }
    public int switichNum { get; set; }
    Image image;
    Text price, equipmentName;
    public void GetAllChildObject()
    {
        image = GetComponent<Image>();
        price = transform.GetChild(0).GetComponent<Text>();
        equipmentName = transform.GetChild(1).GetComponent<Text>();
    }
    public void ChangePrice(int value)
    {
        if (value < 0) { price.text = "Not Available"; }
        else if (value == 0) { price.text = "Purchased"; }
        else { price.text = "" + value; }
    }
    public void ChangeEquipmentName(string name)
    {
        equipmentName.text = name;
    }
    public void SwitchEquipment()
    {
        upgradePanel.UpgradeOrSwtich(switichNum);
    }
}
