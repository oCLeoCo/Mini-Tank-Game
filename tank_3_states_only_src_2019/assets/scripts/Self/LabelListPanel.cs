using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelListPanel : MonoBehaviour
{
    public GameObject tankLabelPrefab;
    Transform redFactionLabelList, blueFactionLabelList;
    void Start()
    {
        redFactionLabelList = transform.GetChild(0).transform;
        blueFactionLabelList = transform.GetChild(1).transform;
    }
    public void InstantiateLabel(TankRole tank, TankAI ai)
    {
        Transform listToInstantiate;
        switch(tank.faction)
        {
            case 1:
                listToInstantiate = redFactionLabelList;
                break;
            default:
                listToInstantiate = blueFactionLabelList;
                break;
        }
        GameObject newTankLabel = Instantiate(tankLabelPrefab, listToInstantiate);
        TankLabel tankLabel = newTankLabel.GetComponent<TankLabel>();
        FSMLabel fSMLabel = newTankLabel.GetComponent<FSMLabel>();
        tankLabel.tank = tank;
        fSMLabel.tank = ai;
        tankLabel.SignUp();
        fSMLabel.SignUp();
    }
}
