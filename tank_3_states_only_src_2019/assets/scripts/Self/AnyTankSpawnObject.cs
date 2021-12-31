using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ATSODefinition
{
}
//==================================================================================================================================================================
public enum ATSOType
{
    Ammo,
    Mine,
    Effect,
    UAV,

}
[System.Serializable]
public class MineObject
{
    public int faction { get; }
    public int tankNum { get; }
    [Range(1, 2)]
    public int
        ammoType; // 1=shell, 2= bullet
    [Header("Damage")]
    public float damageLv1;
    public float damageLv2, damageLv3, damageLv4;
    [Header("ArmorPiercing"), Range(0, 1)]
    public float armorPiercingLv1;
    [Range(0, 1)]
    public float armorPiercingLv2, armorPiercingLv3, armorPiercingLv4;
    public float armorPiercing { get; set; }
    public float damage { get; set; }
    public float
        radius,
        lifeTime;
    public DataAffect dataAffect { get; set; }
    public float dectactCD { get; set; }
    public float dectactCDe { get; set; }
    public MineObject(int f, int n)
    {
        faction = f;
        tankNum = n;
        dectactCD = 0.5f;
    }
    public MineObject(int f, int n, int a, float d, float ap, float r, float l, DataAffect da)
    {
        faction = f;
        tankNum = n;
        ammoType = a;
        damage = d;
        armorPiercing = ap;
        radius = r;
        lifeTime = l;
        dataAffect = da;
        dectactCD = 0.5f;
    }
    public void SetWithLevel(int level)
    {
        switch (level)
        {
            case 1: damage = damageLv1; armorPiercing = armorPiercingLv1; break;
            case 2: damage = damageLv2; armorPiercing = armorPiercingLv2; break;
            case 3: damage = damageLv3; armorPiercing = armorPiercingLv3; break;
            case 4: damage = damageLv4; armorPiercing = armorPiercingLv4; break;
        }
    }
}
public class UAVObject
{

}
[System.Serializable]
public class AmmoObject
{
    public int faction { get; }
    public int tankNum { get; }
    [Range (1, 2)]
    public int
        ammoType; // 1=shell, 2= bullet
    [Header("Damage")]
    public float damageLv1;
    public float damageLv2, damageLv3, damageLv4;
    [Header("ArmorPiercing"), Range(0, 1)]
    public float armorPiercingLv1;
    [Range(0, 1)]
    public float armorPiercingLv2, armorPiercingLv3, armorPiercingLv4;
    public float armorPiercing { get; set; }
    public float damage { get; set; }
    public float
        speed,
        lifeTime;
    public AmmoObject(int f, int n)
    {
        faction = f;
        tankNum = n;
    }
    public AmmoObject(int f, int n, int a, float d, float ap, float s, float l)
    {
        faction = f;
        tankNum = n;
        ammoType = a;
        damage = d;
        armorPiercing = ap;
        speed = s;
        lifeTime = l;
    }
    public void SetWithLevel(int level)
    {
        switch(level)
        {
            case 1: damage = damageLv1; armorPiercing = armorPiercingLv1; break;
            case 2: damage = damageLv2; armorPiercing = armorPiercingLv2; break;
            case 3: damage = damageLv3; armorPiercing = armorPiercingLv3; break;
            case 4: damage = damageLv4; armorPiercing = armorPiercingLv4; break;
        }
    }
}

//==================================================================================================================================================================
public class AnyTankSpawnObject : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;
    int objectType = -1;     // 1 = ammo 2 = mine 3 = followUAV
    bool jobDone;
    float lifeTime = 2;
    float movingDistance;
    AmmoObject ammoObject;
    MineObject mineObject;
    DataAffect dataAffect;
    TimeTrigger timeTrigger;



    private void Update()
    {
        if(objectType == 1)
        {
            AmmoUpdate();
        } 
        else if
            (objectType == 2)
        {
            MineUpdate();
        }
        else
        {
            lifeTime -= Time.deltaTime;
        }
        if (lifeTime <= 0) Destroy(gameObject);
    }
    void MineUpdate()
    {
        if (!jobDone)
        {
            if(mineObject.dectactCDe < 0)
            {
                mineObject.dectactCDe += mineObject.dectactCD;
                Collider[] hits = Physics.OverlapSphere(transform.position, mineObject.radius, 1 << 8);
                foreach (var hit in hits)
                {
                    ArmorInteract tank = hit.GetComponent<ArmorInteract>();
                    if (mineObject.faction != tank.faction &&
                        tank.isAlive)
                    {
                        DealDamage(tank);
                        OnJobDone();
                    }
                }
            }
            mineObject.dectactCDe -= Time.deltaTime;
            mineObject.lifeTime -= Time.deltaTime;
            if (mineObject.lifeTime <= 0) OnJobDone();
        }
    }

    void AmmoUpdate()
    {
        if (!jobDone)
        {
            movingDistance = Time.deltaTime * ammoObject.speed;
            RaycastHit hit;
            Vector3 p = transform.position;
            if (Physics.Raycast(p, transform.forward, out hit, movingDistance, 1 << 8 | 1 << 12))
            {
                switch (hit.collider.gameObject.layer)
                {
                    case 8:
                        ArmorInteract tank = hit.collider.gameObject.GetComponent<ArmorInteract>();
                        if (ammoObject.faction != tank.faction)
                        {
                            transform.position += transform.forward * hit.distance;
                            DealDamage(tank);
                            OnJobDone();
                        }
                        break;
                    default:
                        transform.position += transform.forward * hit.distance;
                        OnJobDone();
                        break;
                }
            }
            transform.position +=
                transform.forward * movingDistance;
            ammoObject.lifeTime -= Time.deltaTime;
            if (ammoObject.lifeTime <= 0) OnJobDone();
        }
    }
    void DealDamage(ArmorInteract tank)
    {
        if (objectType == 1)
        {
            tank.DamageDeal(ammoObject.ammoType, ammoObject.damage,
                ammoObject.armorPiercing, transform,
                ammoObject.tankNum,
                ammoObject.faction);
        }
        else
        {
            tank.DamageDeal(mineObject.ammoType, mineObject.damage,
                mineObject.armorPiercing, transform,
                mineObject.tankNum,
                mineObject.faction,
                mineObject.dataAffect);
        }
        OnJobDone();
    }
    void OnJobDone()
    {
        jobDone = true;
        Destroy(objectPrefab);
        objectType = -1;
    }
    public void DelimitThisObject(AmmoObject definition)
    {
        ammoObject = definition;
        objectType = 1;
    }
    public void DelimitThisObject(MineObject definition)
    {
        mineObject = definition;
        objectType = 2;
    }
}
