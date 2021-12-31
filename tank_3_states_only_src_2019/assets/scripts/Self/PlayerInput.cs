using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    TankRole tank;
    BuffAndUpgrade bU;
    bool isAlive; 
    void Awake()
    {
        tank = GetComponent<TankRole>();
        bU = GetComponent<BuffAndUpgrade>();
        tank.AliveStatus += UpdateAlive;
    }
    void Update()
    {
        if(isAlive)
        {
            UpdateKeyBoard();
            UpdateMouse();
        }
    }
    void UpdateMouse()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position + new Vector3(0, 0, 0));
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
        float HitDist = 0;
        if (playerPlane.Raycast(RayCast, out HitDist))
        {
            Vector3 RayHitPoint = RayCast.GetPoint(HitDist);
            tank.UpdateTurretRotation(true, RayHitPoint);
        }
    }
    void UpdateKeyBoard()
    {
        tank.UpdateTranslate(Input.GetAxis("Vertical"));
        if(Input.GetAxis("Vertical") < 0)
        {
            tank.UpdateRotation(-Input.GetAxis("Horizontal"));
        } else
        {
            tank.UpdateRotation(Input.GetAxis("Horizontal"));
        }
        if (Input.GetMouseButtonDown(0)) tank.FireWeaponGroup(0);
        if (Input.GetMouseButton(1))
        {
            tank.FireWeaponGroup(1);
            //tank.FireWeaponGroup(2);
            //tank.FireWeaponGroup(3);
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) bU.UpgradeSkill(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) bU.UpgradeSkill(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) bU.UpgradeSkill(2);
        } else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) bU.LaunchSkill(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) bU.LaunchSkill(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) bU.LaunchSkill(2);
        }
    }
    void UpdateAlive(bool status)
    {
        isAlive = status;
    }
}
