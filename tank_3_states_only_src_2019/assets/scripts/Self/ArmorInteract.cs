using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArmorInteract : MonoBehaviour
{

    //Damage Role  damage * (1 - (armorDef  * (1 - armorPiercing)))

    public bool supplyReceiver = false;
    public bool isAlive;
    public int faction;
    public ArmorType armorType { get; set; }
    public float armorDef;
    public UnityAction<float, Transform, int, int> InteractSomeThing;
    public UnityAction<float> InteractOther;
    public UnityAction<float> OnReceiveAmmoSupply;
    public UnityAction<DataAffect> OnReceiveDataAffec;
    public void UpdateAlive(bool status)
    {
        isAlive = status;
    }
    public void SetDef(int m_faction, ArmorType m_armorType, float m_armorDef)
    {
        faction = m_faction;
        armorType = m_armorType;
        armorDef = m_armorDef;
    }
    public void DamageDeal(int ammoType, float damage, float armorPiercing, Transform damageFrom, int shooter, int shooterFaction, DataAffect dataAffect)
    {
        OnReceiveDataAffec?.Invoke(dataAffect);
        DamageDeal(ammoType, damage, armorPiercing, damageFrom, shooter, shooterFaction);
    }
    public void DamageDeal(int ammoType,float damage,  float armorPiercing, Transform damageFrom, int shooter, int shooterFaction)
    {
        float damageReduce = -1;
        switch (armorType)
        {
            case ArmorType.Infantry:
                damageReduce = 1 - (armorDef  * (1 - armorPiercing));
                break;
            case ArmorType.Tank:
                if (ammoType == 2)damageReduce = 1 - (armorDef * (1 - armorPiercing));
                break;
            case ArmorType.UAV:
                if(ammoType == 1) damageReduce = 1 - (armorDef * (1 - armorPiercing));
                break;
            case ArmorType.Building:
                damageReduce = 1 - (armorDef * (1 - armorPiercing));
                break;
            default:
                break;
        }
        float f_damage = damageReduce > 0.4f? damage *  damageReduce: 0;
        InteractSomeThing?.Invoke(f_damage, damageFrom, shooter, shooterFaction);
    }  
    public void Supply(float repairAmount, float ammoSupplyPercentage)
    {
        if(repairAmount > 0) InteractOther?.Invoke(repairAmount);
        if (ammoSupplyPercentage > 0) OnReceiveAmmoSupply (ammoSupplyPercentage);
    }
}
