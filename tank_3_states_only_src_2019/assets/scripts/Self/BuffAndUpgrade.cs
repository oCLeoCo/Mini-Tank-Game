using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public enum SkillType
{
    Active,
    Mode,
    Passive,
}
public enum SkillFor
{
    Assault,
    CloseRange,
    RepairTank,
    RetreatSkill,
}
public enum AffectType
{
    MaxHealthPercentage,
    DEFPercentage,
    AccelerationPercentage,
    BakingPercentage,
    MaxForwardSpeedPercentage,
    MaxBackwardSpeedPercentage,
    RotSpeedPercentage,
    CompulsoryMoving,
    DamagePercentage,
    ReloadSpeedPercentage,
    ArmorPiercingPercentage,
}
public enum TimeTriggerType
{
    HealthChange,
}
[System.Serializable]
public class Skill
{
    public string skillName;
    [System.Serializable]
    public class DataAffectValue
    {
        public AffectType affectType;
        [SerializeField, Range(-100, 100)] float lv1Value, lv2Value, lv3Value, lv4Value;
        public float GetValue(int sLv)
        {
            float v;
            switch(sLv)
            {
                case 1: v = lv1Value; break;
                case 2: v = lv2Value; break;
                case 3: v = lv3Value; break;
                case 4: v = lv4Value; break;
                default:v = 0; break;
            }
            return v/100;
        }
    }
    [System.Serializable]
    public class TimeTriggerValue
    {
        public TimeTriggerType timeTriggerType;
        [SerializeField] float lv1Value, lv2Value, lv3Value, lv4Value;
        public float GetValue(int sLv)
        {
            float v;
            switch (sLv)
            {
                case 1: v = lv1Value; break;
                case 2: v = lv2Value; break;
                case 3: v = lv3Value; break;
                case 4: v = lv4Value; break;
                default: v = 0; break;
            }
            return v;
        }
    }
    public SkillType skillType;
    public SkillFor skillFor;
    public int skillID;
    public Sprite skillIcon;
    public int skillLevel { get; set; }
    int skillRequireLevel = 1;
    public float
    coolDownTime;
    public int useTime;
    public int usedTime { get; set; }
    public float coolDownElapsedTime { get; private set; }
    float effectElapsedTime; 
    float eachCoolDownTime;
    public MeshRenderer[] meshR;
    public MeshRenderer[] lanuchHide;
    public GameObject spawnprefab;
    public Transform[] spawnTransform;
    public ATSOType spawnObjectType;
    [SerializeField] AmmoObject ammoObject;
    [SerializeField] MineObject mineObject;
    [SerializeField] DataAffectValue[] mineAffectValue;
    [SerializeField] DataAffectValue[] dataAffectValue;
    [SerializeField] TimeTriggerValue[] timeTriggerValues;
    [SerializeField] float timeToTrigger;
    [SerializeField] float affectTime;
    [SerializeField] TrailRenderer[] trailRenderer;
    public UnityAction<int> OnSkillLevelChange;
    public UnityAction<float> OnSkillStateChange;
    public UnityAction<float> OnSkillElapsedTimeChange;
    public void CoolDown(float time)
    {
        if(effectElapsedTime != 0)
        {
            if(effectElapsedTime - time <= 0)
            {
                effectElapsedTime = 0f;
                foreach (TrailRenderer x in trailRenderer) x.enabled = false;
            }
            else
            {
                effectElapsedTime -= time;
            }
        }
        if(coolDownElapsedTime != 0)
        {
            if (coolDownElapsedTime - time <= 0)
            {
                coolDownElapsedTime = 0;
                usedTime = 0;
                LanuchHide();
            }
            else
            {
                coolDownElapsedTime -= time;
                if (coolDownElapsedTime <= eachCoolDownTime * (usedTime - 1))
                {
                    usedTime -= 1;
                    LanuchHide();
                }
            }
            OnSkillElapsedTimeChange?.Invoke(coolDownElapsedTime);
        }
    }
    public bool HaveDateAffect()
    {
        bool hda = false;
        if (dataAffectValue.Length > 0) hda = true;
        return hda;
    }
    public bool HaveTimeTriggerLength()
    {
        bool htt = false;
        if (timeTriggerValues.Length > 0) htt = true;
        return htt;
    }
    public TimeTrigger GetTimeTrigger()
    {
        TimeTrigger tT = new TimeTrigger(skillID, timeToTrigger, affectTime);
        foreach(TimeTriggerValue x in timeTriggerValues)
        {
            switch(x.timeTriggerType)
            {
                case TimeTriggerType.HealthChange:
                    tT.healthChange += x.GetValue(skillLevel);
                    break;
            }
        }
        return tT;
    }
    public DataAffect CreateDataAffect(DataAffectValue[] re)
    {
        DataAffect dA = new DataAffect(skillID, affectTime);
        foreach (DataAffectValue x in re)
        {
            switch (x.affectType)
            {
                case AffectType.MaxHealthPercentage:
                    dA.maxHealthPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.DEFPercentage:
                    dA.dEFPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.AccelerationPercentage:
                    dA.accelerationPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.BakingPercentage:
                    dA.bakingPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.MaxForwardSpeedPercentage:
                    dA.maxForwardSpeedPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.MaxBackwardSpeedPercentage:
                    dA.maxBackwardSpeedPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.RotSpeedPercentage:
                    dA.rotSpeedPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.CompulsoryMoving:
                    dA.compulsoryMoving += x.GetValue(skillLevel);
                    break;
                case AffectType.DamagePercentage:
                    dA.damagePercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.ReloadSpeedPercentage:
                    dA.reloadSpeedPercentage += x.GetValue(skillLevel);
                    break;
                case AffectType.ArmorPiercingPercentage:
                    dA.armorPiercingPercentage += x.GetValue(skillLevel);
                    break;
            }
        }
        return dA;
    }
    public DataAffect GetDateAffect()
    {
        return CreateDataAffect(dataAffectValue);
    }
    public ATSOType GetATSOType()
    {
        return spawnObjectType;
    }
    public MineObject GetMineObject(int f, int n)
    {
        MineObject atsod = new MineObject
            (f, n, mineObject.ammoType, mineObject.damage,
            mineObject.armorPiercing,
            mineObject.radius, mineObject.lifeTime,
            CreateDataAffect(mineAffectValue));
        return atsod;
    }
    public AmmoObject GetAmmoObject(int f, int n)
    {
        AmmoObject atsod = new AmmoObject
            (f,n, ammoObject.ammoType, ammoObject.damage,
            ammoObject.armorPiercing, ammoObject.speed,
            ammoObject.lifeTime);
        return atsod;
    }
    public void UpgradeSkill()
    {
        skillLevel++;
        skillRequireLevel += 2;
        eachCoolDownTime = coolDownTime / useTime;
        foreach (MeshRenderer x in meshR)  x.enabled = true;
        ammoObject.SetWithLevel(skillLevel);
        mineObject.SetWithLevel(skillLevel);
        OnSkillLevelChange?.Invoke(skillLevel);
    }
    public bool AvailableToUpgrade(int level)
    {
        if(level >= skillRequireLevel &&
            skillLevel < 4)
        { return true; } else { return false; }
    }
    public void LaunchSkill()
    {
        coolDownElapsedTime += eachCoolDownTime;
        usedTime += 1;
        LanuchHide();
        effectElapsedTime += affectTime;
        foreach (TrailRenderer x in trailRenderer) x.enabled = true;
    }
    public bool AvailableToUse()
    {
        if (skillLevel > 0 && usedTime < useTime)
        { return true; } else { return false; }
    }
    public Transform GetTransform()
    {
        int x = usedTime - 1;
        while (x >= spawnTransform.Length) x--;
        return spawnTransform[x];
    }
    void LanuchHide()
    {
        for (int x = 0; x < lanuchHide.Length; x++)
        {
            if (usedTime > x)
            {
                lanuchHide[x].enabled = false;
             }
            else
            {
                lanuchHide[x].enabled = true;
            }
        }
    }
}
//===========================================================================================================================================================================

public class BuffAndUpgrade : MonoBehaviour
{

    public GameManager gm { get; set; }
    TankRole tank;
    int faction, tankNum;
    public bool selfSetting, isPlayer;
    public Weapon[] upgradeWeapons;
    public TankArmor[] upgradeArmors;
    public Engin[] upgradeEngins;
    public Skill[] skills;
    List<DataAffect> dataAffects = new List<DataAffect>();
    List<TimeTrigger> timeTriggers = new List<TimeTrigger>();

    public void BeforeTank()
    {
        tank = GetComponent<TankRole>();
        upgradeWeapons[0] = tank.weapon[0];
        upgradeArmors[0] = tank.armor;
        upgradeEngins[0] = tank.engin;
    }
    public void SettingBase()
    {
        faction = tank.faction;
        tankNum = tank.tankNum;
        for (int x = 0; x < upgradeWeapons.Length; x++) upgradeWeapons[x].SettingUpBaseDate();
        for (int x = 0; x <= 1; x++) upgradeWeapons[x].availableToUpgrade = true;
        for (int x = 0; x <= 1; x++) upgradeArmors[x].availableToUpgrade =true;
        for (int x = 0; x <= 1; x++) upgradeEngins[x].availableToUpgrade = true;
        for (int x = 0; x < upgradeWeapons.Length; x++)
        {
            if(upgradeWeapons[x].availableToUpgrade)
            {
                upgradeWeapons[x].OnPriceChange?.Invoke(upgradeWeapons[x].price);
            } else
            {
                upgradeWeapons[x].OnPriceChange?.Invoke(-1);
            }
        }
        for (int x = 0; x < upgradeArmors.Length; x++)
        {
            if (upgradeArmors[x].availableToUpgrade)
            {
                upgradeArmors[x].OnPriceChange?.Invoke(upgradeArmors[x].price);
            }
            else
            {
                upgradeArmors[x].OnPriceChange?.Invoke(-1);
            }
        }
        for (int x = 0; x < upgradeEngins.Length; x++)
        {
            if (upgradeEngins[x].availableToUpgrade)
            {
                upgradeEngins[x].OnPriceChange?.Invoke(upgradeEngins[x].price);
            }
            else
            {
                upgradeEngins[x].OnPriceChange?.Invoke(-1);
            }
        }
        foreach (ArmorInteract x in tank.armorCollider)
        {
            x.OnReceiveDataAffec += AddDataAffect;
        }
    }
    void ChangeWeapon(int weaponNUM)
    {
        if(weaponNUM < upgradeWeapons.Length &&
            upgradeWeapons[weaponNUM].availableToUpgrade &&
            gm.GetTankStatus(faction, tankNum, 0) >= upgradeWeapons[weaponNUM].price)
        {
            gm.Purchase(faction, tankNum, upgradeWeapons[weaponNUM].price);
            upgradeWeapons[weaponNUM].price = 0;
            upgradeWeapons[weaponNUM].OnPriceChange?.Invoke(upgradeWeapons[weaponNUM].price);
            if (weaponNUM + 1 < upgradeWeapons.Length)
            {
                upgradeWeapons[weaponNUM + 1].availableToUpgrade = true;
                upgradeWeapons[weaponNUM + 1].OnPriceChange?.Invoke(upgradeWeapons[weaponNUM + 1].price);
            }
            tank.SwitchEquipment(upgradeWeapons[weaponNUM], upgradeWeapons[weaponNUM].weaponSlot);
        }
    }


    void ChangeArmor(int armorNUM)
    {
        if(armorNUM < upgradeArmors.Length &&
            upgradeArmors[armorNUM].availableToUpgrade &&
            gm.GetTankStatus(faction, tankNum, 0) >= upgradeArmors[armorNUM].price)
        {
            gm.Purchase(faction, tankNum, upgradeArmors[armorNUM].price);
            upgradeArmors[armorNUM].price = 0;
            upgradeArmors[armorNUM ].OnPriceChange?.Invoke(upgradeArmors[armorNUM].price);
            if (armorNUM + 1 < upgradeArmors.Length)
            {
                upgradeArmors[armorNUM + 1].availableToUpgrade = true;
                upgradeArmors[armorNUM + 1].OnPriceChange?.Invoke(upgradeArmors[armorNUM + 1].price);
            }
            tank.SwitchEquipment(upgradeArmors[armorNUM]);
        }
    }

    void ChangeEngin(int enginNUM)
    {
        if(enginNUM < upgradeEngins.Length &&
            upgradeEngins[enginNUM].availableToUpgrade &&
            gm.GetTankStatus(faction, tankNum, 0) >= upgradeEngins[enginNUM].price)
        {
            gm.Purchase(faction, tankNum, upgradeEngins[enginNUM].price);
            upgradeEngins[enginNUM].price = 0;
            upgradeEngins[enginNUM].OnPriceChange?.Invoke(upgradeEngins[enginNUM].price);
            if (enginNUM + 1 < upgradeEngins.Length)
            {
                upgradeEngins[enginNUM + 1].availableToUpgrade = true;
                upgradeEngins[enginNUM + 1].OnPriceChange?.Invoke(upgradeEngins[enginNUM + 1].price);
            }
            tank.SwitchEquipment(upgradeEngins[enginNUM]);
        }
    }
    public void ChangeEquipment(int Num)
    {
        int type = Num / 10;
        switch(type)
        {
            case 1:
                ChangeWeapon(Num - type * 10);
                break;
            case 2:
                ChangeArmor(Num - type * 10);
                break;
            case 3:
                ChangeEngin(Num - type * 10);
                break;
            default:
                break;
        }
    }





    //===========================================================================================================================================================================
    public void UpgradeSkill(int slot)
    {
        if (gm.GetTankStatus(faction, tankNum, 3) > 0 &&
             skills[slot].AvailableToUpgrade
             (gm.GetTankStatus(faction, tankNum, 2)))
        {
            gm.UpgradeSkill(faction, tankNum);
            skills[slot].UpgradeSkill();
        }
    }
    public void LaunchSkill(int slot)
    {
        if (skills[slot].AvailableToUse())
        {
            skills[slot].LaunchSkill();
            switch (skills[slot].skillType)
            {
                case SkillType.Active:
                    if (skills[slot].spawnTransform != null &&
                        skills[slot].spawnprefab != null)
                    {
                        GameObject sgo = Instantiate(
                            skills[slot].spawnprefab,
                            skills[slot].GetTransform().position,
                            skills[slot].GetTransform().rotation);
                        AnyTankSpawnObject atso = 
                            sgo.GetComponent<AnyTankSpawnObject>();
                        if(skills[slot].GetATSOType() == ATSOType.Ammo)
                        {
                            atso.DelimitThisObject
                                (skills[slot].GetAmmoObject(faction, tankNum));
                        }
                        else if (skills[slot].GetATSOType() == ATSOType.Mine)
                        {
                            atso.DelimitThisObject
                                (skills[slot].GetMineObject(faction, tankNum));
                        }
                    }
                    if (skills[slot].HaveDateAffect()) 
                        AddDataAffect(skills[slot].GetDateAffect());
                    if(skills[slot].HaveTimeTriggerLength())
                        AddTimeTrigger(skills[slot].GetTimeTrigger());
                    break;
                case SkillType.Mode:
                    break;
                case SkillType.Passive:
                    break;
                default:
                    break;
            }
        }
    }
    //===========================================================================================================================================================================
    public void Recoid()
    {
        DataAffect recoilAffect = new DataAffect(0.8f);
        AddDataAffect(recoilAffect);
    }
    void AddDataAffect(DataAffect newDataAffect)
    {
        bool same = false;
        foreach(DataAffect x in dataAffects)
        {
            if (newDataAffect.dID == x.dID)
            {
                if (newDataAffect.affectTime > x.affectTime)
                {
                    x.affectTime = newDataAffect.affectTime;
                }
                same = true;
            }
        }
        if(!same)
        {
            dataAffects.Add(newDataAffect);
            CalDataAffect();
        } 
    }
    void CalDataAffect()
    {
        DataAffect newDataAffects = new DataAffect();
        foreach (DataAffect x in dataAffects)
        {
            newDataAffects.maxHealthPercentage += x.maxHealthPercentage;
            newDataAffects.dEFPercentage += x.dEFPercentage;
            newDataAffects.accelerationPercentage += x.accelerationPercentage;
            newDataAffects.bakingPercentage += x.bakingPercentage;
            newDataAffects.maxForwardSpeedPercentage += x.maxForwardSpeedPercentage;
            newDataAffects.maxBackwardSpeedPercentage += x.maxBackwardSpeedPercentage;
            newDataAffects.rotSpeedPercentage += x.rotSpeedPercentage;
            newDataAffects.compulsoryMoving += x.compulsoryMoving;
            newDataAffects.damagePercentage += x.damagePercentage;
            newDataAffects.reloadSpeedPercentage += x.reloadSpeedPercentage;
            newDataAffects.armorPiercingPercentage += x.armorPiercingPercentage;
        }
        tank.UpdateDataAffect(newDataAffects);
    }
    void BackupUpdateDataAffect()
    {
        foreach (DataAffect x in dataAffects)
        {
            if (x.affectTime < 100)
            {
                x.affectTime -= Time.fixedDeltaTime;
                if (x.affectTime <= 0)
                {
                    dataAffects.Remove(x);
                    CalDataAffect();
                }
            }
        }
    }
    void UpdateDataAffect()
    {
        int removeAffect = -1;
        foreach (DataAffect x in dataAffects)
        {
            if (x.affectTime < 100) // if affectTime >= 100 that mean is a Passive or Mode use ID to remove
            {
                x.affectTime -= Time.fixedDeltaTime;
                if (x.affectTime <= 0)
                {
                    removeAffect = dataAffects.IndexOf(x);
                }
                if (!tank.isAlive) removeAffect = dataAffects.IndexOf(x);
            }
        }
        if (removeAffect != -1)
        {
            dataAffects.RemoveAt(removeAffect);
            CalDataAffect();
        }
    }

    //===========================================================================================================================================================================
    void AddTimeTrigger(TimeTrigger timeTrigger)
    {
        bool same = false;
        foreach (TimeTrigger x in timeTriggers)
        {
            if (timeTrigger.tID == x.tID)
            {
                if (timeTrigger.affectTime > x.affectTime)
                {
                    x.affectTime = timeTrigger.affectTime;
                }
                same = true;
            }
        }
        if (!same)
        {
            timeTriggers.Add(timeTrigger);
        }
    }
    void UpdateTimeTrigger()
    {
        int removeTimeTrigger = -1;
        foreach (TimeTrigger x in timeTriggers)
        {
            if(x.affectTime < 100) // if affectTime >= 100 that mean is a Passive or Mode use ID to remove
            {
                x.affectTime -= Time.fixedDeltaTime;
                if (!tank.isAlive) removeTimeTrigger = timeTriggers.IndexOf(x);
                if (x.affectTime <= 0) removeTimeTrigger = timeTriggers.IndexOf(x);
            }
            x.elapsedTime -= Time.fixedDeltaTime;
            if (x.elapsedTime <= 0)
            {
                x.elapsedTime += x.triggerTime;
                if (tank.isAlive) tank.HealthChange(x.healthChange);
            }
        }
        if (removeTimeTrigger != -1)
        {
            timeTriggers.RemoveAt(removeTimeTrigger);
        }
    }
    //===========================================================================================================================================================================
    private void FixedUpdate()
    {
        UpdateDataAffect();
        foreach (Skill x in skills) x.CoolDown(Time.fixedDeltaTime);
        UpdateTimeTrigger();
    }
}
