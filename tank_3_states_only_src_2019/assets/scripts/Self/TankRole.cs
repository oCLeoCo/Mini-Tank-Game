using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponType
{
    Cannon,
    MachineGun,
}
public enum ArmorType
{
    Infantry,
    Tank,
    UAV,
    Building,
}
[System.Serializable]
public class Weapon
{
    //==============================()==============================
    public string weaponName;
    public WeaponType weaponType;
    public int price;
    bool isPlayer;
    public int weaponSlot;
    public MeshRenderer[] meshR;
    public Transform shellSpawnPoint;
    public GameObject shellPrefab;
    public float shellSpeed;
    public bool availableToUpgrade;
    public float lifeTime;
    public bool recoil;
    //==============================(Damage)==============================
    public float baseDamage;
    
    public float damagePercentage { get; set; }
    public float f_Damage { get; set; }
    [Range(0, 1f)]
    public float armorPiercing;
    //==============================(Reload & Rate of Fire)==============================
    public float fireElapsedTime { get; set; }

    public float rpm;
    public float rps { get; set; }
    public float reloadElapsedTime { get; set; }
    public float reloadTimeInMinute;
    public float reloadSpeed { get; set; }
    public float reloadSecond { get; set; }
    //==============================(Ammo)==============================
    public int ammoLeftInMag { get; set; }
    public int ammoLeftInStore { get; set; } 
    public int magSize = 30;
    public int maxAmmo = 90;
    //==============================()==============================
    public float effectOnMovement;
    public UnityAction<string,  float, int> GetNewWeaponState;
    public UnityAction<float> GetReloadElapsedTime;
    public UnityAction<int, int> GetAmmoState;
    public UnityAction<int> OnPriceChange;
   public void FireElapsed(float value)
    {
        if (fireElapsedTime < rps)
        {
            fireElapsedTime = Mathf.Clamp(
            fireElapsedTime + value,
            0, rps);
        }
    }
    public void ReloadElapsed(float value)
    {
        if(reloadElapsedTime < reloadSecond &&
             ammoLeftInStore >= 1)
        {
            reloadElapsedTime = Mathf.Clamp(
            reloadElapsedTime + value,
            0, reloadSecond);

            if (isPlayer) GetReloadElapsedTime?.Invoke(reloadElapsedTime);

            if (reloadElapsedTime >= reloadSecond)
            {
                if((ammoLeftInStore + ammoLeftInMag) >= magSize)
                {
                    ammoLeftInStore -= (magSize - ammoLeftInMag);
                    ammoLeftInMag = magSize;
                }
                else
                {
                    ammoLeftInMag += ammoLeftInStore;
                    ammoLeftInStore = 0;
                }
                if (isPlayer || weaponSlot == 0) GetAmmoState?.Invoke(ammoLeftInMag, ammoLeftInStore);
            }
        }
    }
    public void AmmoSupply(float ammoSupplyPercentage)
    {
        ammoLeftInStore = Mathf.Clamp(
            ammoLeftInStore + (int)((float)maxAmmo * ammoSupplyPercentage),
            0, maxAmmo);
        if (isPlayer || weaponSlot == 0) GetAmmoState?.Invoke(ammoLeftInMag, ammoLeftInStore);
    }
    public void SettingUpBaseDate()
    {
        fireElapsedTime = 0;
        reloadElapsedTime = 0;
        rps = 60 / rpm;//Round per Min to Second
        reloadSecond = 60 / reloadTimeInMinute;
        reloadSpeed = 1f;
        damagePercentage = 1f;
        ammoLeftInMag = 0;
        ammoLeftInStore = 0;
        UpdateWeaponDamage();
    }
    public void SettingUpWeapon(bool user)
    {
        isPlayer = user;
        SettingUpBaseDate(); 
        UpdateWeaponDamage();
        if (isPlayer || weaponSlot == 0) 
        {
            GetNewWeaponState?.Invoke(weaponName, reloadSecond, maxAmmo);
            GetAmmoState?.Invoke(ammoLeftInMag, ammoLeftInStore);
        } 
        if(isPlayer)
        {
            GetReloadElapsedTime?.Invoke(reloadElapsedTime);
        }
    }
    public void UpdateWeaponDamage()
    {
        f_Damage = baseDamage *  damagePercentage;
    }
}
[System.Serializable]
public class TankArmor
{
    public string armorName;
    public bool availableToUpgrade;
    public ArmorType armorType = ArmorType.Tank;
    public int price;
    public float baseMaxHealth;
    public float effectOnMovement;
    public MeshRenderer[] meshR;
    [Range(0, 1f)] public float[] baseDef = new float[3];
    public UnityAction<int> OnPriceChange;
}
    [System.Serializable]
public class Engin
{
    public string enginName;
    public bool availableToUpgrade;
    public int price;
    [Range(0, 1000)] public float baseAcceleration = 40f;
    [Range(0, 1000)] public float baseBaking = 90f;
    [Range(0, 1000)] public float baseMaxForwardSpeed = 100f;
    [Range(0, 1000)] public float baseMaxBackwardSpeed = 60f;
    [Range(0, 1000)] public float baseRotSpeed = 35f;
    public UnityAction<int> OnPriceChange;
}
[System.Serializable]
public class Turret
{
    public bool sentryGun;
    public Transform turretTransform;
    public float baseRotSpeed = 5f;
    public float p_rotSpeed { get; set; }
    public float m_rotSpeed { get; set; }
    public float f_rotSpeed { get; set; }
    public void SettingBaseRotSpeed()
    {
        m_rotSpeed = 1f;
        UpdateRotSpeed();
    }
    public void UpdateRotSpeed()
    {
        f_rotSpeed = (baseRotSpeed + p_rotSpeed) * m_rotSpeed;
    }
}

//===========================================================================================================================================================================

public class DataAffect
{
    public int dID;
    public float
     maxHealthPercentage,
     dEFPercentage,
     accelerationPercentage,
     bakingPercentage,
     maxForwardSpeedPercentage,
     maxBackwardSpeedPercentage,
     rotSpeedPercentage,
     compulsoryMoving,
     damagePercentage,
     reloadSpeedPercentage,
     armorPiercingPercentage,
     affectTime;
    public DataAffect()
    {
        maxHealthPercentage = 1f;
        dEFPercentage = 1f;
        accelerationPercentage = 1f;
        bakingPercentage = 1f;
        maxForwardSpeedPercentage = 1f;
        maxBackwardSpeedPercentage = 1f;
        rotSpeedPercentage = 1f;
        compulsoryMoving = 0;
        damagePercentage = 1f;
        reloadSpeedPercentage = 1f;
        armorPiercingPercentage = 1f;
    }
    public DataAffect(int ID)
    {
        dID = ID;
        emptyValue();
    }
    public DataAffect(int ID, float time)
    {
        dID = ID;
        affectTime = time;
        emptyValue();
    }
    public DataAffect(float recoilTime)
    {
        affectTime = recoilTime;
        compulsoryMoving = -100;
    }
    void emptyValue()
    {
        maxHealthPercentage = 0f;
        dEFPercentage = 0f;
        accelerationPercentage = 0f;
        bakingPercentage = 0f;
        maxForwardSpeedPercentage = 0f;
        maxBackwardSpeedPercentage = 0f;
        rotSpeedPercentage = 0f;
        compulsoryMoving = 0;
        damagePercentage = 0f;
        reloadSpeedPercentage = 0f;
        armorPiercingPercentage = 0f;
    }
}
//===========================================================================================================================================================================
public class TimeTrigger
{
    public int tID;
    public float 
        healthChange,
        triggerTime,
        affectTime,
        elapsedTime;
    public TimeTrigger(int ID, float tT, float time)
    {
        tID = ID;
        triggerTime = tT;
        affectTime = time;
    }
}
//===========================================================================================================================================================================
public class TankRole : MonoBehaviour
{
    public GameManager gm { get; set;  }
    public bool selfSetting;
    BuffAndUpgrade bu;
    public string userName = "", tankName = "";
    public bool isAlive = true;
    bool onPanel;
    public bool isPlayer = false;
    public int faction, tankNum;
    int rR = 0;
    public Transform respawnTranfrom { get; set; }
    float health;
    public float f_MaxHealth { get; set; }

    public ArmorInteract[] armorCollider = new ArmorInteract[3];




    float curSpeed, curRotSpeed;


    public float f_AccelerationSpeed { get; set; } public float f_Baking { get; set; } public float f_MaxForwardSpeed { get; set; }
    public float f_MaxBackwardSpeed { get; set; } public float f_RotSpeed { get; set; }
    public TankArmor armor;
    public Engin engin;
    public Turret[] turret;
    public Weapon[] weapon;
    public UnityAction<float> OnSpeedChange;
    public UnityAction<float, float, bool> OnHealthChange;
    public UnityAction<bool> AliveStatus;
    public UnityAction<int> DamageAlert;
    public UnityAction OnWeaponChange;
    DataAffect dataAffect = new DataAffect();
    void Awake() 
    {
        if(selfSetting)
        {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            SignUpArmor();
            SettingBaseMovement();
            for (int x = 0; x < turret.Length; x++) turret[x].SettingBaseRotSpeed();
        }
    }
    void Start()
    {
        if(selfSetting)
        {
            respawnTranfrom = gm.GetTankRespawnTransform(faction, tankNum);
            SettingBaseHealth();
            for (int x = 0; x < weapon.Length; x++) weapon[x].SettingUpWeapon(isPlayer);
            bu = GetComponent<BuffAndUpgrade>();
            if (bu != null) bu.SettingBase();
        }
    }
    public void OnInstantiate()
    {
        if(!isPlayer)
        {
            TankAI ai = GetComponent<TankAI>();
            ai.OnInstantiate();
            TankLabel tankLabel = GetComponentInChildren<TankLabel>();
            tankLabel.SignUp();
        }
        SignUpArmor();
        SettingBaseMovement();
        foreach (Turret x in turret) x.SettingBaseRotSpeed();
        SettingBaseHealth();
        OnWeaponChange?.Invoke();
        foreach (Weapon x in weapon) x.SettingUpWeapon(isPlayer);
        bu = GetComponent<BuffAndUpgrade>();
        if (bu != null) bu.SettingBase();
    }
    void Update()
    {
        if(rR > 0)
        {
            rR -= 1;
            transform.position = respawnTranfrom.position;
            transform.rotation = respawnTranfrom.rotation;
        }
        UpdateTransform();
        foreach(Weapon x in weapon)
        {
            x.FireElapsed(Time.deltaTime);
            x.ReloadElapsed(Time.deltaTime * dataAffect.reloadSpeedPercentage);
        }
    }
    public void UpdateDataAffect(DataAffect newDataAffect)
    {
        dataAffect = newDataAffect;
        UpdateArmor();
        UpdateMovement();
    }
    //=================================================================================(About Health)=================================================================================
    void SettingBaseHealth()
    {
        ResetHealth();
    }
    void ResetHealth()
    {
        isAlive = true;
        f_MaxHealth = armor.baseMaxHealth * dataAffect.maxHealthPercentage;
        health = f_MaxHealth;
        AliveStatus.Invoke(isAlive);
        OnHealthChange?.Invoke(health, f_MaxHealth, false);
    }
    public void UpgradeHealth(float newBaseMaxHealth)
    {
        float healthPercent = health / f_MaxHealth;
        armor.baseMaxHealth = newBaseMaxHealth;
        f_MaxHealth = armor.baseMaxHealth * dataAffect.maxHealthPercentage;
        health = f_MaxHealth * healthPercent;
        OnHealthChange?.Invoke(health, f_MaxHealth, false);
    }
    public void HealthChange(float damage, Transform damageFrom, int shooter, int shooterFaction)
    {
        if (isAlive)
        {

            health = Mathf.Clamp(health - damage, 0, f_MaxHealth);
            if (damage > 0) CalAngleWithDamage(damageFrom);
            OnHealthChange?.Invoke(health, f_MaxHealth, true);
            if (health < 1)
            {
                isAlive = false;
                AliveStatus.Invoke(isAlive);
                gm.ObjectDestroyed(shooterFaction, shooter, 3);
                //ResetMovement();
                Invoke("Respawn", gm.GetRespawnTime(faction, tankNum));
            }
        }
    }
    public void HealthChange(float value)
    {
        if (isAlive)
        {
            health = Mathf.Clamp(health + value, 0, f_MaxHealth);
            OnHealthChange?.Invoke(health, f_MaxHealth, false);
        }
    }
    public void Respawn()
    {
        rR = 3;
        for (int x = 0; x < turret.Length; x++) turret[x].turretTransform.localEulerAngles = new Vector3(0, 0, 0);
        ResetHealth();
    }
    //=================================================================================(About Armor)=================================================================================
    void SignUpArmor()
    {
        for(int x = 0; x < armorCollider.Length; x++)
        {
            float xDef = Mathf.Clamp(
                armor.baseDef[x] * dataAffect.dEFPercentage,
                0, 1);
            armorCollider[x].SetDef(faction,armor.armorType,xDef);
            armorCollider[x].InteractSomeThing += HealthChange;
            armorCollider[x].InteractOther += HealthChange;
            AliveStatus += armorCollider[x].UpdateAlive;
            armorCollider[x].OnReceiveAmmoSupply += AmmoSupply;
        }
        if (armorCollider[0] != null) armorCollider[0].supplyReceiver = true;
    }
    void CalAngleWithDamage(Transform damageTransform)                  //simulate use 12'o clock to find out damage from
    {
        float angle = damageTransform.transform.localEulerAngles.y - transform.localEulerAngles.y;
        int oClock = (int)(angle / 30) + 18;
        while (oClock > 12) oClock -= 12;
        DamageAlert?.Invoke(oClock);
    }
    public void SwitchEquipment(TankArmor newArmor)
    {
        for(int x = 0; x < armor.meshR.Length; x++) armor.meshR[x].enabled = false;
        armor = newArmor;
        UpdateArmor();
        for (int x = 0; x < armor.meshR.Length; x++) armor.meshR[x].enabled = true;
    }
    public void UpdateArmor()
    {
        UpgradeHealth(armor.baseMaxHealth);
        for (int x = 0; x < armorCollider.Length; x++)
            armorCollider[x].armorDef = Mathf.Clamp(
                armor.baseDef[x] * dataAffect.dEFPercentage,
                0, 1);
                
    }
    //=================================================================================(About Movement)=================================================================================
    public void SettingBaseMovement()
    {
        UpdateMovement();
    }
    public void UpdateMovement()
    {
        f_AccelerationSpeed = engin.baseAcceleration * dataAffect.accelerationPercentage;
        f_Baking = engin.baseBaking * dataAffect.bakingPercentage;
        f_MaxForwardSpeed = engin.baseMaxForwardSpeed * dataAffect.maxForwardSpeedPercentage;
        f_MaxBackwardSpeed = engin.baseMaxBackwardSpeed * dataAffect.maxBackwardSpeedPercentage;
        f_RotSpeed = engin.baseRotSpeed * dataAffect.rotSpeedPercentage;
    }
    public void SwitchEquipment(Engin newEngin)
    {
        engin = newEngin;
        UpdateMovement();
    }
    void UpdateTransform()
    {
        if (isAlive)
        {
            transform.Translate(Vector3.forward * curSpeed * Time.deltaTime);
            transform.Rotate(0, curRotSpeed, 0);
        }
        OnSpeedChange?.Invoke(curSpeed);
    }
    public void UpdateTranslate(float input)
    {
        if (dataAffect.compulsoryMoving < 0) { input = 0; }
        else { input += dataAffect.compulsoryMoving; }
        if (input == 0)
        {
            if (curSpeed > -0.1 &&
                curSpeed < 0.1)
                curSpeed = 0;
            else
            {
                input = curSpeed > 0 ? -1 : 1;
                curSpeed = Mathf.Clamp(
                curSpeed + f_Baking * input * Time.deltaTime,
                -f_MaxBackwardSpeed,
                f_MaxForwardSpeed);
            }
        } else
        {
            curSpeed = 0 >= input * curSpeed ? // Checking Input with curSpeed relation
                Mathf.Clamp(
                curSpeed + f_AccelerationSpeed * input * Time.deltaTime,
                -f_MaxBackwardSpeed,
                f_MaxForwardSpeed) :
                Mathf.Clamp(
                curSpeed + f_Baking * input * Time.deltaTime,
                -f_MaxBackwardSpeed,
                f_MaxForwardSpeed);
        }
    }
    public void UpdateRotation(float input)
    {
        if (curSpeed >= f_MaxForwardSpeed / 2 ||
             curSpeed <= f_MaxBackwardSpeed / 2)
        {
            input /= 2;
        }
        curRotSpeed = input * Time.deltaTime * f_RotSpeed;
    }
    void ResetMovement()
    {
        curSpeed = 0f;
        curRotSpeed = 0;
    }
    //=================================================================================(About Turret)=================================================================================
    public void UpdateTurretRotation(bool user, Vector3 target)
    {
        for(int x =0; x < turret.Length; x++)
        {
            if(user != turret[x].sentryGun)
            {
                Vector3 dir = target - turret[x].turretTransform.position;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Lerp(turret[x].turretTransform.rotation, lookRotation, Time.deltaTime * turret[x].f_rotSpeed).eulerAngles;
                turret[x].turretTransform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
        }
    }
    //=================================================================================(About Weapon)=================================================================================
    public void FireWeaponGroup(int weaponSlot)
    {
        if (weapon[weaponSlot].fireElapsedTime >= weapon[weaponSlot].rps &&
             weapon[weaponSlot].ammoLeftInMag >= 1)
        {
            weapon[weaponSlot].fireElapsedTime -= weapon[weaponSlot].rps;
            weapon[weaponSlot].ammoLeftInMag -= 1;
            if (weapon[weaponSlot].ammoLeftInMag <= 0) weapon[weaponSlot].reloadElapsedTime = 0f;
            GameObject shell =
                Instantiate(weapon[weaponSlot].shellPrefab,
                weapon[weaponSlot].shellSpawnPoint.position,
                weapon[weaponSlot].shellSpawnPoint.rotation);

            int aT = 1;
            if (weapon[weaponSlot].weaponType == WeaponType.Cannon) aT = 2;

            float damage = weapon[weaponSlot].f_Damage * dataAffect.damagePercentage;

            float armorPiercing = Mathf.Clamp(
                weapon[weaponSlot].armorPiercing *
                dataAffect.armorPiercingPercentage, 0, 1);

            AmmoObject ammoObject = new AmmoObject(faction,tankNum,aT,
                damage, armorPiercing, weapon[weaponSlot].shellSpeed, 
                weapon[weaponSlot].lifeTime);

            AnyTankSpawnObject ATSO = shell.GetComponent<AnyTankSpawnObject>();
            ATSO.DelimitThisObject(ammoObject);

            if (weapon[weaponSlot].recoil) bu.Recoid();
        }
        if (isPlayer || weaponSlot == 0)
            weapon[weaponSlot].GetAmmoState?.Invoke(
            weapon[weaponSlot].ammoLeftInMag, weapon[weaponSlot].ammoLeftInStore);
    }

    public void AmmoSupply(float ammoSupplyPercentage)
    {
        for (int x = 0; x < weapon.Length; x++) weapon[x].AmmoSupply(ammoSupplyPercentage);
    }
    public Transform GetFirePointForAI()
    {
        return weapon[0].shellSpawnPoint;
    }
    public void SwitchEquipment(Weapon newWeapon, int weaponSlot)
    {
        for (int x = 0; x < weapon[weaponSlot].meshR.Length; x++) weapon[weaponSlot].meshR[x].enabled = false;
        weapon[weaponSlot] = newWeapon;
        for (int x = 0; x < weapon[weaponSlot].meshR.Length; x++) weapon[weaponSlot].meshR[x].enabled = true;
        if(isPlayer || weaponSlot == 0) OnWeaponChange?.Invoke();
        weapon[weaponSlot].SettingUpWeapon(isPlayer);
    }
}

