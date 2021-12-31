using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameManager gm { get; set; }
    Transform playerTransform;
    Vector3 upgradeStation;
    public float upgradeStationRange;
    TankLabel tankLabel;
    NewWeaponInformation mainWeapon, subWeapon;
    UpgradePanel upgradePanel;
    FundPanel fundPanel;
    SkillPanel skillPanel;
    LabelListPanel labelListPanel;
    SpeedPanel speedPanel;
    GameObject upgradePanelObject;
    private void Start()
    {
        tankLabel = transform.GetChild(0).GetComponent<TankLabel>();
        var weaponPanelLayout = transform.GetChild(1).GetChild(0);
        mainWeapon = weaponPanelLayout.GetChild(0).GetComponent<NewWeaponInformation>();
        subWeapon = weaponPanelLayout.GetChild(1).GetComponent<NewWeaponInformation>();
        upgradePanelObject = transform.GetChild(2).gameObject;
        upgradePanel = upgradePanelObject.GetComponent<UpgradePanel>();
        fundPanel = transform.GetChild(3).GetComponent<FundPanel>();
        skillPanel = transform.GetChild(4).GetComponent<SkillPanel>();
        labelListPanel = transform.GetChild(5).GetComponent<LabelListPanel>();
        speedPanel = transform.GetChild(6).GetComponent<SpeedPanel>();
    }
    public void SetPlayer(GameObject player)
    {
        TankRole playerCs = player.GetComponent<TankRole>();
        BuffAndUpgrade playerBU = player.GetComponent<BuffAndUpgrade>();
        tankLabel.tank = playerCs;
        tankLabel.SignUp();


        mainWeapon.barNum = 0;
        subWeapon.barNum = 1;
        mainWeapon.SignUp(playerCs, playerBU);
        subWeapon.SignUp(playerCs, playerBU);


        upgradePanel.buffAndUpgrade = playerBU;
        upgradePanel.InstantiateButton();
        SetUpgradeStation(player.transform);

        fundPanel.SignUp(gm);

        skillPanel.InstantiateButton(playerBU);
        skillPanel.SignUp(gm);

        speedPanel.SignUp(playerCs);
    }
    public void InstantiateLabel(GameObject tank)
    {
        TankRole tankCs = tank.GetComponent<TankRole>();
        TankAI tankAI = tank.GetComponent<TankAI>();
        if(tankCs.isPlayer)
        {
            SetPlayer(tank);
        }
        labelListPanel.InstantiateLabel(tankCs, tankAI);
    }
    public void SetUpgradeStation(Transform player)
    {
        playerTransform = player;
        upgradeStation = playerTransform.position;
    }
    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            if ((playerTransform.position - upgradeStation).magnitude < upgradeStationRange)
            {
                upgradePanelObject.SetActive(true);
            }
            else
            {
                upgradePanelObject.SetActive(false);
            }
        }
    }
}
