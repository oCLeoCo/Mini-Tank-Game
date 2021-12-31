using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairArea : MonoBehaviour
{
    public bool isUpgradeStation;
    public int faction;
    public float repairAmount, repairRange, repairIntervalTime, ammoSupplyPercentage;
    float elapsedRepairTime;

    void Repair()
    {
        elapsedRepairTime += Time.fixedDeltaTime;
        if(elapsedRepairTime >= repairIntervalTime)
        {
            elapsedRepairTime -= repairIntervalTime;
            Collider[] hits = Physics.OverlapSphere(this.transform.position, repairRange, 1 << 8);
                foreach(Collider hit in hits)
            {
                ArmorInteract tank = hit.transform.GetComponent<ArmorInteract>();
                if(tank.faction == faction &&
                     tank.supplyReceiver)
                {
                    tank.Supply(repairAmount, ammoSupplyPercentage);
                }
            }
        }
    }
    void FixedUpdate()
    {
        Repair();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, repairRange);
    }
}
