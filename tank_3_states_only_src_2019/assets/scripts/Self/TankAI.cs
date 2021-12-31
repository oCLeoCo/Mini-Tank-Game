using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TankAI : MonoBehaviour
{
    public enum FSMState
    {
        None,   // 0
        Patrol, // 1
        Alert,//2
        Chase,  // 3
        Berserk, // 4
        Attack, // 5
        WimpyPatrol, // 6
        Retreat, // 7
        Upgrade, // 8
        Dead,   // 9
        RepairState, // 10
        RefillAmmo, // 11
    }
    //=============================================================================================================================================================================
    public bool selfSetting;
    public FSMState curState;
    public Transform[] patralLine;
    public Transform[] ammoSupplyStations;
    [Header("Sight"), SerializeField, Range(0, 1500f), Tooltip("Distance, Sight Distance")] float detectRange = 600f;
    [SerializeField, Range(0, 180f), Tooltip("Angle, Sight Angle")] float detectAngle = 60f;
    [SerializeField, Range(0, 2f), Tooltip("Time, detect elapse time")] float detectTime = 0.5f;

    [Header("Attack & Chasing"), SerializeField, Range(0, 1500f), Tooltip("Distance, out of this range will give up chasing")] float giveUpChaseRange = 900f;
    [SerializeField, Range(0, 1500f), Tooltip("Distance, attack enemy within this range")] float attackRange = 500f;
    [SerializeField, Range(0, 1500f), Tooltip("Distance, within this range will stop closing enemy")] float attackStopRange = 300f;
    [SerializeField, Range(0, 180f), Tooltip("Angle, aim target within this angle will Fire")] float allowFireAngle = 10f;
    [SerializeField, Range(0, 100f), Tooltip("Distance, aim target within this distance ignore allowFireAngle")] float ignoreFireAngleRange = 70f;

    [Header("Patrol & Alert"), SerializeField, Range(1, 100f), Tooltip("Distance, with in this distance mean arrive, only use on patrol point & predict enemy pos & back to base")] float defaultArriveDistance = 50f;
    [SerializeField, Range(1, 10f), Tooltip("Time, wait on patrol point time")] float updatePatrolPointWaitTime = 5f;
    [SerializeField, Range(1, 10f), Tooltip("Time, Cool Down on receive new predictEnemyPos")] float coolDownOfPredictEnemyPosTime = 3f;
    [SerializeField, Range(1, 10f), Tooltip("Time, wait on predict enemy fire position")] float predictEnemyPosWaitTime = 3f;

    [Header("Movement Optimization"), SerializeField, Range(0f, 35f), Tooltip("Angle, start on 90'+-")] float rotationPriorityRange = 20f;
    [SerializeField, Range(1, 100f), Tooltip("Distance, add on defaultArriveDistance, between this range will ignore rot front to arrive position")] float useBackwardRange = 50f;

    [Header("Health Change Stage"), SerializeField, Range(0, 1), Tooltip("Percentage, when health below in this percentage will change to retreat state")] float retreatHealthPercentage = 0.2f;
    [SerializeField, Range(0, 1), Tooltip("Percentage, when health below in this percentage will change to wimpy patrol")] float wimpyPatrolHealthPercentage = 0.5f;
    //=================================================================================(About Calculation value)=================================================================================
    bool isAlive, availableToUpgrade, availableToUpgradeSkill;
    int faction, tankNum, patrolPointNUM, mainWeaponMaxAmmo;
    Transform targetTranfrom, patrolTransform, viewPoint, firePoint;
    Vector3 patrolPos, predictEnemyPos;
    public Transform respawnPoint { get; set; }
    public Transform ammoSupplyStation { get; set; }
    float stayAtPatrolPointWaitedTime, elapsedPredictEnemyPosTime, stayAtPredictEnemyPosWaitedTime, elapsedDetectTime;
    public UnityAction<string> OnFSMStateChange;
    public GameManager gm { get; set; }
    TankRole tank;
    BuffAndUpgrade bu;
    ArmorInteract targetArmor;
    NavMeshAgent nav;
    //=================================================================================(About Start)=================================================================================
    void Awake()
    {
        if(selfSetting)
        {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            tank = GetComponent<TankRole>();
            tankNum = tank.tankNum;
            tank.AliveStatus += UpdateAlive;
            tank.OnHealthChange += UpdateHealth;
            tank.DamageAlert += IdentifyDamageFrom;
            nav = GetComponent<NavMeshAgent>();
        }
    }
    void Start()
    {
        if(selfSetting)
        {
            faction = tank.faction;
            respawnPoint = gm.GetTankRespawnTransform(faction, tankNum);
            firePoint = tank.GetFirePointForAI();
            viewPoint = transform.GetChild(transform.childCount - 1);
            SettingPatralLine();
            ChangeFSMStatus(FSMState.Patrol);
        }
    }
    public void OnInstantiate()
    {
        tank = GetComponent<TankRole>();
        bu = GetComponent<BuffAndUpgrade>();
        tankNum = tank.tankNum;
        faction = tank.faction;
        SignUp();
        nav = GetComponent<NavMeshAgent>();
        firePoint = tank.GetFirePointForAI();
        viewPoint = transform.GetChild(transform.childCount - 1);
        SettingPatralLine();
        ChangeFSMStatus(FSMState.Patrol);
    }
    public void SignUp()
    {
        tank.AliveStatus += UpdateAlive;
        tank.OnHealthChange += UpdateHealth;
        tank.DamageAlert += IdentifyDamageFrom;
        tank.OnWeaponChange += UpdateWeapon;
        if(faction == 1)
        {
            gm.OnRedFactionAmmoSupplyStationsChange += UpdateAmmoSupplyStations;
        } else
        {
            gm.OnBlueFactionAmmoSupplyStationsChange += UpdateAmmoSupplyStations;
        }
    }
    //=================================================================================(About Setup & Reset)=================================================================================
    void ClearTargetAndResetPatrol()
    {
        availableToUpgrade = true;
        availableToUpgradeSkill = true;
        ammoSupplyStation = null;
        targetTranfrom = null;
        targetArmor = null;
        patrolPointNUM = 0;
        patrolPos = patralLine[patrolPointNUM].position;
        stayAtPatrolPointWaitedTime = 0;
        stayAtPredictEnemyPosWaitedTime = 0f;
    }
    void ChangeFSMStatus(FSMState state)
    {
        curState = state;
        string stateText = "" + state;
        OnFSMStateChange?.Invoke(stateText);
    }
    void SettingPatralLine()
    {
        patralLine = gm.GetPatrolLine(faction);
        patrolPointNUM = 0;
        patrolPos = patralLine[patrolPointNUM].position;
    }
    void FindNextPoint()
    {
        patrolPointNUM = patrolPointNUM + 1 >= patralLine.Length ? 0 : patrolPointNUM + 1;
        patrolPos = patralLine[patrolPointNUM].position;
    }
    //=================================================================================(About Call Back And Update)=================================================================================
    void UpdateHealth(float health, float maxHealth, bool inDamage)
    {
        float healthPercentage = health / maxHealth;
        if (health >= maxHealth)
        {
            if (curState == FSMState.RepairState ||
                curState == FSMState.Upgrade||
                curState == FSMState.Dead)
            {
                ChangeFSMStatus(FSMState.Patrol);
            }
        }
        if(isAlive)
        {
            if (inDamage &&
                healthPercentage > wimpyPatrolHealthPercentage)
            {
                CheckSkill(SkillFor.RepairTank);
            }
            else if (healthPercentage <= wimpyPatrolHealthPercentage &&
                          healthPercentage > retreatHealthPercentage &&
                          curState != FSMState.RefillAmmo)
            {
                SendSOSMessage();
                ChangeFSMStatus(FSMState.WimpyPatrol);
            }
            else if ((health / maxHealth <= retreatHealthPercentage) ||
                          (healthPercentage <= wimpyPatrolHealthPercentage &&
                          healthPercentage > retreatHealthPercentage &&
                          curState == FSMState.RefillAmmo))
            {
                ClearTargetAndResetPatrol();
                ChangeFSMStatus(FSMState.Retreat);
            }
        }
    }
    void IdentifyDamageFrom(int oClock)
    {
        if (curState == FSMState.Patrol ||
            curState == FSMState.Alert &&
            elapsedPredictEnemyPosTime >= coolDownOfPredictEnemyPosTime)
        {
            oClock = oClock >= 12 ? oClock - 12 : oClock;
            float oClockToAngle = oClock * 30;
            Vector3 predictEnemyRange = transform.forward * attackRange;
            Quaternion predictRotation = Quaternion.AngleAxis(oClockToAngle, Vector3.up);
            predictEnemyPos = predictRotation * predictEnemyRange + transform.position;
            stayAtPatrolPointWaitedTime = 0;
            ChangeFSMStatus(FSMState.Alert);
        }
    }
    void UpdateAlive(bool status)
    {
        isAlive = status;
        if (!isAlive)
        {
            ChangeFSMStatus(FSMState.Upgrade);
            ClearTargetAndResetPatrol();
        }
    }
    void UpdateWeapon()
    {
        tank.weapon[0].GetNewWeaponState += UpdateWeaponState;
        tank.weapon[0].GetAmmoState += UpdateAmmoState;
    }
    void UpdateWeaponState(string name, float time, int maxAmmo)
    {
        mainWeaponMaxAmmo = maxAmmo;
    }
    void UpdateAmmoState(int ammoLeftInMag, int ammoLeftInStore)
    {
        if(isAlive)
        {
            if (ammoLeftInStore <= 0 && curState != FSMState.WimpyPatrol)
            {
                ClearTargetAndResetPatrol();
                ChangeFSMStatus(FSMState.RefillAmmo);
            }
            else if (ammoLeftInStore <= 0 && curState == FSMState.WimpyPatrol)
            {
                ClearTargetAndResetPatrol();
                ChangeFSMStatus(FSMState.Retreat);
            }
            else if (ammoLeftInStore >= mainWeaponMaxAmmo &&
                           curState == FSMState.RefillAmmo)
            {
                ClearTargetAndResetPatrol();
                ChangeFSMStatus(FSMState.Patrol);
            }
        }
    }
    void UpdateAmmoSupplyStations(Transform[] transforms)
    {
        ammoSupplyStations = transforms;
    }
    void FixedUpdate()
    {
        switch(curState)
        {
            case FSMState.Patrol: 
                UpdatePatrolState(); 
                break;
            case FSMState.Alert:
                UpdateAlertState();
                break;
            case FSMState.Chase:
                UpdateChaseState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.WimpyPatrol:
                UpdateWimpyPatrol();
                break;
            case FSMState.Retreat:
                UpdateRetreatState();
                break;
            case FSMState.Upgrade:
                UpdateUpgradeState();
                    break;
            case FSMState.Dead:
            case FSMState.RepairState:
                UpdateRepairState();
                break;
            case FSMState.RefillAmmo:
                UpdateRefillAmmoState();
                break;
            default:                                
                break;
        }
        elapsedPredictEnemyPosTime += Time.fixedDeltaTime;
    }
    //=================================================================================(About Command With Tank)=================================================================================
    void BackupMovingToTarget(bool faceToTarget,bool engagingEnemy, Vector3 betweenTargetTranform, float remainDistance)
    {
        switch(faceToTarget)
        {
            case true:
                RotateToTarget(betweenTargetTranform);
                break;
            default:
                RotateToTarget(-betweenTargetTranform);
                break;
        }
        if (remainDistance > 0)
        {
            float rTWTPAngle = Vector3.Angle(betweenTargetTranform, transform.forward);
            if (rTWTPAngle < 90 + rotationPriorityRange)                                        // Rotation Optimization to "Speed too fast Rot too slow" case tank 
            {
                tank.UpdateTranslate(1);
            }
            else if (rTWTPAngle > 90 - rotationPriorityRange)
            {
                tank.UpdateTranslate(-1);
            }
            else
            {
                tank.UpdateTranslate(0);
            }
        } else
        {
            tank.UpdateTranslate(0);
        }
    }
    void MovingToTarget(bool faceToTarget, bool engagingEnemy, Vector3 betweenTargetTranform, float remainDistance)
    {
        float rTWTPAngle = Vector3.Angle(betweenTargetTranform, transform.forward);
        if ((!faceToTarget &&
              remainDistance >= 0 ) ||                                                  // not arrived
            (!engagingEnemy &&                                                         // not engaging enemy
             useBackwardRange >= remainDistance &&        // tank in use backward range + default arrive range
             remainDistance >= 0 &&                                                 // not arrived
             rTWTPAngle > 90 + rotationPriorityRange))          // in back of tank
        {
            RotateToTarget(-betweenTargetTranform);
            tank.UpdateTranslate(-1);
        } else if 
            ((engagingEnemy &&                                                              //engaging still do rotation
              remainDistance < 0) ||
              (remainDistance >= 0 &&                                                // not arrived
              rTWTPAngle > 90 - rotationPriorityRange))
        {
            RotateToTarget(betweenTargetTranform);
            tank.UpdateTranslate(0);
        } else if(remainDistance < 0)                                           // arrived
        {
            tank.UpdateRotation(0);
            tank.UpdateTranslate(0);
        }  else
        {
            RotateToTarget(betweenTargetTranform);
            tank.UpdateTranslate(1);
        }
    }
    void RotateToTarget(Vector3 betweenTargetTranform)
    {
        float angle = Vector3.SignedAngle(betweenTargetTranform, transform.forward, Vector3.up);
        if (angle > 5f || angle < -5f)
        {
            float x = angle > 0 ? -1f : 1f;
            tank.UpdateRotation(x);
        }
        else
        {
            tank.UpdateRotation(0);
        }
    }
    void Silence()
    {
        tank.UpdateRotation(0);
        tank.UpdateTranslate(0);
    }
    void AimTarget(Vector3 target)
    {
        tank.UpdateTurretRotation(true, target);
    }
    void AimFront()
    {
        Vector3 front = transform.position + transform.forward * 200f;
        tank.UpdateTurretRotation(true, front);
    }
    void Fire(int group)
    {
        tank.FireWeaponGroup(group);
    }
    //=================================================================================(About Function)=================================================================================
    void ScanningEnemy()
    {
        elapsedDetectTime += Time.fixedDeltaTime;
        if (elapsedDetectTime >= detectTime)
        {
            elapsedDetectTime -= detectTime;
            Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, 1 << 8);
            foreach (Collider hit in hits)
            {
                Vector3 hitD = hit.transform.position - viewPoint.position;
                hitD.y = 0f;
                if (Vector3.Angle(hitD, viewPoint.forward) < detectAngle
                    && targetTranfrom == null)
                {
                    RaycastHit hit2;
                    if (Physics.Raycast(viewPoint.position, hit.transform.position - viewPoint.position, out hit2))
                    {
                        switch (hit2.collider.gameObject.layer)
                        {
                            case 8:
                                ArmorInteract findedTank = hit2.collider.GetComponent<ArmorInteract>();
                                if (findedTank.faction != faction &&
                                     findedTank.isAlive)
                                {
                                    targetTranfrom = hit2.transform;
                                    targetArmor = findedTank;
                                    stayAtPredictEnemyPosWaitedTime = 0f;
                                    stayAtPatrolPointWaitedTime = 0f;
                                    ChangeFSMStatus(FSMState.Chase);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
    void CalClosestAmmoSupplyStation()
    {
        ammoSupplyStation = respawnPoint;
        float remainDistanceWithAmmoSupplyStation = (ammoSupplyStation.position - transform.position).magnitude;
        for (int x = 0; x < ammoSupplyStations.Length; x++)
        {
            float remainDistanceWithAmmoSupplyStationX = (ammoSupplyStations[x].position - transform.position).magnitude;
            if (remainDistanceWithAmmoSupplyStationX < remainDistanceWithAmmoSupplyStation)
            {
                remainDistanceWithAmmoSupplyStation = remainDistanceWithAmmoSupplyStationX;
                ammoSupplyStation = ammoSupplyStations[x];
            }
        }
    }
    void CheckSkill(SkillFor skillFor)
    {
        for (int x = 0; x < bu.skills.Length; x++)
        {
            if (bu.skills[x].AvailableToUse() &&
                bu.skills[x].skillFor == skillFor)
            {
                bu.LaunchSkill(x);
            }
        }
    }
    void FireAtWell(int weapon)
    {
        Vector3 aimVector = targetTranfrom.position - firePoint.position;
        if (Vector3.Angle(aimVector, firePoint.forward) < allowFireAngle ||
             aimVector.magnitude < ignoreFireAngleRange)
        {
            for (int x = 0; x <= weapon ; x++) Fire(x);
        }
    }
    void CalUpgradePrice()
    {
        int fund = gm.GetTankStatus(faction, tankNum, 0);
        int wellingToBuyObject = -1;
        int wellingToBuyPrice = -1;
        for (int x = 0; x < bu.upgradeWeapons.Length; x++)
        {
            int objectPrice = bu.upgradeWeapons[x].price;
            if (objectPrice > 0 &&
               bu.upgradeWeapons[x].availableToUpgrade &&
               fund >= objectPrice)
            {
                wellingToBuyObject = 10 + x;
                wellingToBuyPrice = objectPrice;
            }
        }
        for (int x = 0; x < bu.upgradeArmors.Length; x++)
        {
            int objectPrice = bu.upgradeArmors[x].price;
            if (objectPrice > 0 &&
               bu.upgradeArmors[x].availableToUpgrade &&
               fund >= objectPrice &&
               objectPrice > wellingToBuyPrice)
            {
                wellingToBuyObject = 20 + x;
                wellingToBuyPrice = objectPrice;
            }
        }
        for (int x = 0; x < bu.upgradeEngins.Length; x++)
        {
            int objectPrice = bu.upgradeEngins[x].price;
            if (objectPrice > 0 &&
               bu.upgradeEngins[x].availableToUpgrade &&
               fund >= objectPrice &&
               objectPrice > wellingToBuyPrice)
            {
                wellingToBuyObject = 30 + x;
                wellingToBuyPrice = objectPrice;
            }
        }
        if (wellingToBuyObject != -1)
        {
            bu.ChangeEquipment(wellingToBuyObject);
        }
        else
        {
            availableToUpgrade = false;
        }
    }
    void CalUpgradeSkill()
    {
        if (gm.GetTankStatus(faction, tankNum, 3) <= 0)
        {
            availableToUpgradeSkill = false;
        }
        else
        {
            for (int x = 0; x < bu.skills.Length; x++)
                bu.UpgradeSkill(x);
        }
    }
    public void SendSOSMessage()
    {
        if(targetTranfrom != null)
        {
            gm.SOSMessageTranslate(faction, targetTranfrom);
        }
    }
    public void ReceiveSOSMessage(Vector3 pep)
    {
        if(curState == FSMState.Patrol)
        {
            predictEnemyPos = pep;
            stayAtPatrolPointWaitedTime = 0;
            ChangeFSMStatus(FSMState.Alert);
        }
    }
    //=================================================================================(About State)=================================================================================
    void UpdatePatrolState()
    {
        if (targetTranfrom == null)
        {
            ScanningEnemy();
        }
        Vector3 remainTranformWithPT = patrolPos - this.transform.position;
        float remainDistanceWithPT = remainTranformWithPT.magnitude - defaultArriveDistance;
        MovingToTarget(true, false, remainTranformWithPT, remainDistanceWithPT);
        if (remainDistanceWithPT <= 0)
        {
            stayAtPatrolPointWaitedTime += Time.fixedDeltaTime;
            if (stayAtPatrolPointWaitedTime >= updatePatrolPointWaitTime)
            {
                stayAtPatrolPointWaitedTime = 0f;
                FindNextPoint();
            }
        }
    }
    void UpdateAlertState()
    {
        if (targetTranfrom == null)
        {
            ScanningEnemy();
        }
        Vector3 remainTranformWithPET = predictEnemyPos - transform.position;
        float remainDistanceWithPET = remainTranformWithPET.magnitude - defaultArriveDistance;
        MovingToTarget(true, false, remainTranformWithPET, remainDistanceWithPET);
        AimTarget(predictEnemyPos);
        if (remainDistanceWithPET <= 0)
        {
            stayAtPredictEnemyPosWaitedTime += Time.fixedDeltaTime;
            if (stayAtPredictEnemyPosWaitedTime >= predictEnemyPosWaitTime)
            {
                stayAtPredictEnemyPosWaitedTime = 0f;
                ChangeFSMStatus(FSMState.Patrol);
            }
        }
    }
    void UpdateChaseState()
    {
        if (targetTranfrom == null)
        {
            targetArmor = null;
            ChangeFSMStatus(FSMState.Patrol);
        }
        else
        {
            Vector3 remainTranformWithTarget = targetTranfrom.position - this.transform.position;
            float distanceWithTarget = remainTranformWithTarget.magnitude;
            MovingToTarget(true, true, remainTranformWithTarget, distanceWithTarget - attackStopRange);
            AimTarget(targetTranfrom.position);
            CheckSkill(SkillFor.Assault);
            if (distanceWithTarget >= giveUpChaseRange ||
                 !targetArmor.isAlive)
            {
                targetTranfrom = null;
                targetArmor = null;
                ChangeFSMStatus(FSMState.Patrol);
            }
            else if
              (distanceWithTarget < attackRange)
            {
                ChangeFSMStatus(FSMState.Attack);
            }
        }
    }
    void UpdateAttackState()
    {
        if (targetTranfrom == null)
        {
            targetArmor = null;
            ChangeFSMStatus(FSMState.Patrol);
        }
        else
        {
            Vector3 remainTranformWithTarget = targetTranfrom.position - transform.position;
            float distanceWithTarget = remainTranformWithTarget.magnitude;
            if (distanceWithTarget >= giveUpChaseRange ||
                 !targetArmor.isAlive)
            {
                targetTranfrom = null;
                targetArmor = null;
                ChangeFSMStatus(FSMState.Patrol);
            }
            else if (distanceWithTarget > attackRange)
            {
                ChangeFSMStatus(FSMState.Chase);
            }
            else
            {
                AimTarget(targetTranfrom.position);
                MovingToTarget(true, true, remainTranformWithTarget, distanceWithTarget - attackStopRange);
                FireAtWell(0);
                if (distanceWithTarget <= attackRange / 2)
                {
                    CheckSkill(SkillFor.CloseRange);
                }
            }
        }
    }
    void UpdateWimpyPatrol()
    {
        if (targetTranfrom == null)
        {
            ClearTargetAndResetPatrol();
            ChangeFSMStatus(FSMState.Retreat);
        } else
        {
            Vector3 remainTranformWithTarget = targetTranfrom.position - this.transform.position;
            float distanceWithTarget = remainTranformWithTarget.magnitude;
            if (distanceWithTarget >= attackRange ||
                 !targetArmor.isAlive)
            {
                ClearTargetAndResetPatrol();
                CheckSkill(SkillFor.RetreatSkill);
                ChangeFSMStatus(FSMState.Retreat);
            } else
            {
                Vector3 remainTransformWithReSpawnPoint = respawnPoint.position - transform.position;
                MovingToTarget(false, false, remainTransformWithReSpawnPoint, remainTransformWithReSpawnPoint.magnitude - defaultArriveDistance);
                AimTarget(targetTranfrom.position);
                FireAtWell(0);
            }
        }
    }
    void UpdateRetreatState()
    {
        Vector3 remainTransformWithReSpawnPoint = respawnPoint.position - transform.position;
        float remainDistanceWithReSpawnPoint = remainTransformWithReSpawnPoint.magnitude - defaultArriveDistance;
        if(remainDistanceWithReSpawnPoint <= 0)
        {
            ChangeFSMStatus(FSMState.Upgrade);
        }
        MovingToTarget(true, false, remainTransformWithReSpawnPoint, remainDistanceWithReSpawnPoint);
        AimFront();
    }
    void UpdateUpgradeState()
    {
        if (availableToUpgrade) CalUpgradePrice();
        if (availableToUpgradeSkill) CalUpgradeSkill();
        if (!availableToUpgradeSkill && !availableToUpgrade)
        {
            if(isAlive)
            { ChangeFSMStatus(FSMState.RepairState); }
            else
            { ChangeFSMStatus(FSMState.Dead); }
        }
        Silence();
    }
    void UpdateRepairState()
    {
        Vector3 remainTransformWithReSpawnPoint = respawnPoint.position - transform.position;
        float remainDistanceWithReSpawnPoint = remainTransformWithReSpawnPoint.magnitude - defaultArriveDistance;
        if (remainDistanceWithReSpawnPoint <= 0)
        {
            Silence();
        } 
        else
        {
            MovingToTarget(true, false, remainTransformWithReSpawnPoint, remainDistanceWithReSpawnPoint);
            AimFront();
        }
    }
    void UpdateRefillAmmoState()
    {
        if (ammoSupplyStation == null) CalClosestAmmoSupplyStation();
        Vector3 remainTransformWithAmmoSupplyStation = ammoSupplyStation.position - transform.position;
        float remainDistanceWithAmmoSupplyStation = remainTransformWithAmmoSupplyStation.magnitude - defaultArriveDistance;
        MovingToTarget(true, false, remainTransformWithAmmoSupplyStation, remainDistanceWithAmmoSupplyStation);
        AimFront();
    }
    //=================================================================================(About Gizmos)=================================================================================
    private void OnDrawGizmos()
    {
        Vector3 end = transform.position + (transform.forward * detectRange);
        Quaternion left = Quaternion.AngleAxis(-detectAngle, Vector3.up);
        Vector3 endL = left * (transform.forward * detectRange) + transform.position;
        Quaternion right = Quaternion.AngleAxis(detectAngle, Vector3.up);
        Vector3 endR = right * (transform.forward * detectRange) + transform.position;
        Debug.DrawLine(transform.position, end, Color.green);
        Debug.DrawLine(transform.position, endL, Color.green);
        Debug.DrawLine(transform.position, endR, Color.green);
    }
}
