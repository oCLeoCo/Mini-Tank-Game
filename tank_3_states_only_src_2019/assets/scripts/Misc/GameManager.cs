using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GameManager : MonoBehaviour 
{
    public GameObject gameUIObject;
    public GameObject[] testUIObject;
    public Transform camBase;
    Transform cam;
    public GameObject[] tankPrefabs;
    GameManager gm;
    public float gameSpeed = 1.0f;
    public Transform[] 
        faction1TankRespawnTransform, faction2TankRespawnTransform, 
        line1, line2, line3;
    public Transform[] redFactionAmmoSupplyStations, blueFactionAmmoSupplyStations;
    public int[] f1AIOnLineNUM = new int[3], f2AIOnLineNUM = new int[3];
    GameObject[] redFactionTankList, blueFactionTankList;
    int[,] redTankStatus, blueTankStatus; // 0= Fund, 1= Exp, 2= Level, 3 = SkillPoint
    int fundUplimit = 9999, expUpLimit = 9999, levelUpLimit = 12, skillPointUpLimit = 12, playerFaction = 0, playerTankNum = 0;
    public int redFactionTankNum, blueFactionTankNum,
        levelUpRequireExp,
        eachTankFund, eachTankExp,
        eachInfantryFund, eachInfantryExp,
        eachUAVFund, eachUAVExp,
        eachTurretFund, eachTurretExp;
    public float baseRespawnTime = 8f, eachLvRTI = 2f;
    public UnityAction<int> OnPlayerFundChange;
    public UnityAction<int> OnPlayerLevelChange;
    public UnityAction<int, int> OnPlayerExpChange;
    public UnityAction<int> OnPlayerSkillPointChange;
    public UnityAction<Transform[]> OnRedFactionAmmoSupplyStationsChange, OnBlueFactionAmmoSupplyStationsChange;
    GameUI gameUI;
    bool testMode;

    // Transform[][] lines;
    void Awake()
    {
        gm = this.GetComponent<GameManager>();
        cam = camBase.GetChild(0);
        SettingRespawnTransform();
        SettingWandarPoints();
        SettingAmmoSupplyStations();
        gameUI = gameUIObject.GetComponent<GameUI>();
    }

    //void Start()
    //{
    //    Physics.gravity = new Vector3(0, -500.0f, 0);
    //}
    //
    //void Update()
    //{
    //    Time.timeScale = gameSpeed;
    //}
    //===========================================================================================================================================================================
    void SettingRespawnTransform()
    {
        SettingTransformArray(ref faction1TankRespawnTransform, 0, 0);
        SettingTransformArray(ref faction2TankRespawnTransform, 0, 1);
    }
    void SettingWandarPoints()
    {
        SettingTransformArray(ref line1, 1, 0);
        SettingTransformArray(ref line2, 1, 1);
        SettingTransformArray(ref line3, 1, 2);
        //lines = new Transform[this.transform.GetChild(1).childCount][];
        //for (int x = 0; x < this.transform.GetChild(1).childCount; x++)SettingTransformArray(ref lines[x], 1, x);
    }
    void SettingAmmoSupplyStations()
    {
        SettingTransformArray(ref redFactionAmmoSupplyStations, 3, 0);
        SettingTransformArray(ref blueFactionAmmoSupplyStations, 3, 1);
    }
    void SettingTransformArray(ref Transform[] transformsArray, int firstIndex, int secIndex)
    {
        Transform transformCollection = this.transform.GetChild(firstIndex).GetChild(secIndex);
        transformsArray = new Transform[transformCollection.childCount];
        for (int x = 0; x < transformCollection.childCount; x++)
        {
            if (transformCollection.GetChild(x) != null)
                transformsArray[x] = transformCollection.GetChild(x);
        }
    }
    //===========================================================================================================================================================================
    public Transform GetTankRespawnTransform(int faction, int tankNUM)
    {
        switch (faction)
        {
            case 1:
                while (tankNUM > faction1TankRespawnTransform.Length) tankNUM -= faction1TankRespawnTransform.Length;
                return faction1TankRespawnTransform[tankNUM - 1];
            default:
                while (tankNUM > faction2TankRespawnTransform.Length) tankNUM -= faction2TankRespawnTransform.Length;
                return faction2TankRespawnTransform[tankNUM - 1];
        }
    }
    //===========================================================================================================================================================================
    public Transform[] GetPatrolLine(int faction)
    {
        Transform[] line = ChooseWhichLineAIGO(faction);
        Transform[] fixedLine = new Transform[line1.Length + 1];
        switch (faction)
        {
            case 1:
                for (int x = 0; x < line1.Length; x++) fixedLine[x] = line[x];
                break;
            case 2:
                for (int x = 0; x < line1.Length; x++) fixedLine[x] = line[line.Length - 1 - x];
                break;
            default:
                break;
        }
        fixedLine[fixedLine.Length - 1] = this.transform.GetChild(2).GetChild(faction - 1);
        return fixedLine;
    }
    Transform[] ChooseWhichLineAIGO(int faction)
    {
        int whichLine = -1;
        switch (faction)
        {
            case 1:
                for (int x = 0; x < f1AIOnLineNUM.Length; x++)
                {
                    int y = x + 1 >= f1AIOnLineNUM.Length ? 0 : x + 1;
                    if (whichLine == -1 &&
                         f1AIOnLineNUM[x] < f1AIOnLineNUM[y])
                        whichLine = x;
                }
                whichLine = whichLine == -1 ? Random.Range(0, f1AIOnLineNUM.Length) : whichLine;
                f1AIOnLineNUM[whichLine]++;
                break;
            default:
                for (int x = 0; x < f2AIOnLineNUM.Length; x++)
                {
                    int y = x + 1 >= f2AIOnLineNUM.Length ? 0 : x + 1;
                    if (whichLine == -1 &&
                         f2AIOnLineNUM[x] < f2AIOnLineNUM[y])
                        whichLine = x;
                }
                whichLine = whichLine == -1 ? Random.Range(0, f2AIOnLineNUM.Length) : whichLine;
                f2AIOnLineNUM[whichLine]++;
                break;
        }
        switch (whichLine + 1)
        {
            case 1:
                return line1;
            case 2:
                return line2;
            default:
                return line3;
        }
    }
    //===========================================================================================================================================================================


    // 0= Fund, 1= Exp, 2= Level, 3 = SkillPoint
    public int GetTankStatus(int faction, int tankNum, int which)
    {
        which = Mathf.Clamp(which, 0, 4);
        int statusValue = 0;
        int indexNum = tankNum - 1;
        if(faction == 1)
        {
            statusValue = redTankStatus[which, indexNum];
        } 
        else
        {
            statusValue = blueTankStatus[which, indexNum];
        }
        return statusValue;
    }
    void ChangeTankStatus(int faction, int tankNum, int value, int which)
    {
        // 0= Fund, 1= Exp, 2= Level, 3 = SkillPoint
        which = Mathf.Clamp(which, 0, 4);
        int indexNum = tankNum - 1;
        int uplimit = 0;
        int afterValue = 0;
        int tLv = GetTankStatus(faction, tankNum, 2);
        switch (which)
        {
            case 0: uplimit = fundUplimit; break;
            case 1: uplimit = expUpLimit; break;
            case 2: uplimit = levelUpLimit; break;
            default: uplimit = skillPointUpLimit; break;
        }
        if (faction == 1)
        {
            redTankStatus[which, indexNum] = Mathf.Clamp(
                redTankStatus[which, indexNum] + value
                , 0, uplimit);
            afterValue = redTankStatus[which, indexNum];
        }
        else
        {
            blueTankStatus[which, indexNum]  = Mathf.Clamp(
                blueTankStatus[which, indexNum] + value
                , 0, uplimit);
            afterValue = blueTankStatus[which, indexNum];
        }
        if(which == 1 &&
            afterValue >= levelUpRequireExp * tLv)
        {
            if(afterValue >= levelUpRequireExp * tLv &&
                tLv < levelUpLimit)
            {
                ChangeTankStatus(faction, tankNum, -levelUpRequireExp * tLv, which);
                ChangeTankStatus(faction, tankNum, 1, 2);
                ChangeTankStatus(faction, tankNum, 1, 3);
            }
        }
        if (faction == playerFaction &&
             tankNum == playerTankNum)
        {
            switch(which)
            {
                case 0: OnPlayerFundChange?.Invoke(afterValue); break;
                case 1: OnPlayerExpChange?.Invoke(afterValue , levelUpRequireExp * tLv); break;
                case 2: OnPlayerLevelChange?.Invoke(afterValue); break;
                default: OnPlayerSkillPointChange?.Invoke(afterValue); break;
            }
        }
    }
    public void Purchase(int faction, int tankNum, int value)
    {
        ChangeTankStatus(faction, tankNum, -value, 0);
    }
    public void UpgradeSkill(int faction, int tankNum)
    {
        ChangeTankStatus(faction, tankNum, -1, 3);
    }
    public void ObjectDestroyed(int faction, int tankNum, int destroyedObjectType) //by which Tank destroy
    {   //Type 1 = Infantry, 2 = UAV, 3 = Tank, 4 = Turret
        // 0= Fund, 1= Exp, 2= Level, 3 = SkillPoint
        int dFund = 0;
        int dExp = 0;
        switch(destroyedObjectType)
        {
            case 1: dFund = eachInfantryFund; dExp = eachInfantryExp; break;
            case 2: dFund = eachUAVFund; dExp = eachUAVExp; break;
            case 3: dFund = eachTankFund; dExp = eachTankExp; break;
            case 4: dFund = eachTurretFund; dExp = eachTurretExp; break;
            default: break;
        }
        if (tankNum > 0 && // only tank can get reward
             destroyedObjectType < 4)
        {
            ChangeTankStatus(faction, tankNum, dFund, 0);
            ChangeTankStatus(faction, tankNum, dExp, 1);
        } 
        else if (destroyedObjectType == 4)
        {   // Turret Destroy is team reward
            int fTN = 0;
            if (faction == 1) { fTN = redFactionTankNum; } else { fTN = blueFactionTankNum; }
            for (int x = 0; x < fTN; x++)
            {
                ChangeTankStatus(faction, x, dFund, 0);
                ChangeTankStatus(faction, x, dFund, 1);
            }
        }
    }
    public void InitializeTankStatus()
    {
        redTankStatus = new int[4, redFactionTankNum];
        blueTankStatus = new int[4, blueFactionTankNum];
    }
    public void InitializeTankList()
    {
        redFactionTankList = new GameObject[redFactionTankNum];
        blueFactionTankList = new GameObject[blueFactionTankNum];
    }
    public void AddTank(int faction, int tankNum, GameObject tank)
    {
        if(faction == 1)
        {
            redFactionTankList[tankNum - 1] = tank;
        }
        else
        {
            blueFactionTankList[tankNum - 1] = tank;
        }
    }
    public float GetRespawnTime(int faction, int tankNum)
    {
        float rT = baseRespawnTime;
        rT += ((float)GetTankStatus(faction, tankNum, 2) - 1) * eachLvRTI;
        return rT;
    }
    void Test(int value, int which)
    {
        which = Mathf.Clamp(which, 0, 4);
        ChangeTankStatus(playerFaction, playerTankNum, value, which);
    }
    public void TestMoney(int value)
    {
        Test(value, 0);
    }
    public void TestExp(int value)
    {
        Test(value, 1);
    }
    //===========================================================================================================================================================================
    public void SetCustomGame(int redFactionAINUM, int blueFactionAINUM, int playerTankType, int playerInWhichFaction, bool mode)
    {
        testMode = mode;
        // Total Tank Number
        if(playerInWhichFaction == 1)
        {
            redFactionTankNum = redFactionAINUM + 1;
            blueFactionTankNum = blueFactionAINUM;
        }
        else
        {
            redFactionTankNum = redFactionAINUM;
            blueFactionTankNum = blueFactionAINUM + 1;
        }
        //Setup Status
        InitializeTankStatus();
        InitializeTankList();
        //player Tank
        gameUI.gm = gm;
        InitializeTank(playerInWhichFaction, 1, playerTankType, true, GetTankRespawnTransform(playerInWhichFaction, 1));

        //other Tank
        int redFactionAITankNum = playerInWhichFaction == 1? 2: 1;
        int blueFactionAITankNum = playerInWhichFaction == 2? 2: 1;
        int tankType;
        for (int x =0; x < redFactionAINUM; x++)
        {
            tankType = Random.Range(0, tankPrefabs.Length);
            InitializeTank(1, redFactionAITankNum, tankType, false, GetTankRespawnTransform(1, redFactionAITankNum));
            redFactionAITankNum += 1;
        }
        for (int x = 0; x < blueFactionAINUM; x++)
        {
            tankType = Random.Range(0, tankPrefabs.Length);
            InitializeTank(2, blueFactionAITankNum, tankType, false, GetTankRespawnTransform(2, blueFactionAITankNum));
            blueFactionAITankNum += 1;
        }

        for (int x = 0; x < redFactionTankNum; x++)
        {
            ChangeTankStatus(1, x + 1, 0, 0);
            ChangeTankStatus(1, x + 1, 1, 2);
            ChangeTankStatus(1, x + 1, 0, 1);
            ChangeTankStatus(1, x + 1, 1, 3);
        }
        for (int x = 0; x < blueFactionTankNum; x++)
        {
            ChangeTankStatus(2, x + 1, 0, 0);
            ChangeTankStatus(2, x + 1, 1, 2);
            ChangeTankStatus(2, x + 1, 0, 1);
            ChangeTankStatus(2, x + 1, 1, 3);
        }

        UpdateAmmoSupplyStations();

        if (testMode)
        {
            TestMoney(3000);
            TestExp(300);
            for(int x =0; x < testUIObject.Length; x++)
            {
                testUIObject[x].SetActive(true);
            }
        }
    }

    void InitializeTank(int faction,int tankNum,  int tankType, bool isPlayer, Transform spawnPoint)
    {
        GameObject tank = Instantiate(tankPrefabs[tankType], spawnPoint.position, spawnPoint.rotation);
        TankRole tankCs = tank.GetComponent<TankRole>();
        tankCs.gm = gm;
        tankCs.respawnTranfrom = spawnPoint;
        tankCs.faction = faction;
        tankCs.tankNum = tankNum;
        tankCs.isPlayer = isPlayer;


        BuffAndUpgrade buffAndUpgrade = tank.GetComponent<BuffAndUpgrade>();
        buffAndUpgrade.BeforeTank();
        buffAndUpgrade.gm = gm;

        TankLabelRotation tankLabelR = tank.GetComponentInChildren<TankLabelRotation>();
        tankLabelR.camBase = camBase;

        if(isPlayer)
        {
            tankCs.userName = "Player";
            playerFaction = faction;
            playerTankNum = tankNum;

            gameUIObject.SetActive(true);

            DefaultCameraControl camCs = camBase.GetComponent<DefaultCameraControl>();
            camCs.tank = tank.transform;
            camCs.playerTankPos = tank.transform.GetChild(tank.transform.childCount - 1);
            if (!testMode) camCs.FollowTank();
            PlayerInput playerInput = tank.GetComponent<PlayerInput>();
            playerInput.enabled = true;
            

        } else
        {
            TankAI tankAI = tank.GetComponent<TankAI>();
            tankAI.gm = gm;
            tankAI.enabled = true;
            tankAI.respawnPoint = spawnPoint;
            string aiName = faction == playerFaction ? "Friendly ": "Enemy ";
            tankCs.userName = aiName + tankNum;
        }
        AddTank(faction, tankNum, tank);
        gameUI.InstantiateLabel(tank);

        tankCs.OnInstantiate();
    }
    void UpdateAmmoSupplyStations()
    {
        OnRedFactionAmmoSupplyStationsChange?.Invoke(redFactionAmmoSupplyStations);
        OnBlueFactionAmmoSupplyStationsChange?.Invoke(blueFactionAmmoSupplyStations);
    }
    public void SOSMessageTranslate(int faction, Transform transform)
    {
        if( faction == 1)
        {
            foreach(GameObject x in redFactionTankList)
            {
                TankAI ai = x.GetComponent<TankAI>();
                ai.ReceiveSOSMessage(transform.position);
            }
        }
        else
        {
            foreach (GameObject x in blueFactionTankList)
            {
                TankAI ai = x.GetComponent<TankAI>();
                ai.ReceiveSOSMessage(transform.position);
            }
        }
    }
}
