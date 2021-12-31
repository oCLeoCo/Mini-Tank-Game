using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraControl : MonoBehaviour
{
    public GameObject cam ;
    public Transform tank, playerTankPos;
    bool followTank;
    public float speed = 500;
    public float rotSpeed = 40f;
    public float forwardTranslation { get; set; }
    public float sideTranslation { get; set; }
    public float heightTranslation { get; set; }
    public float horizontalRotation { get; set; }
    public float verticalRotatiom { get; set; }
    public void DefaultRotation()
    {
        cam.transform.localEulerAngles = new Vector3(45f, 0, 0);
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    public void TopView()
    {
        cam.transform.localEulerAngles = new Vector3(90f, 0, 0);
    }
    public void TankView()
    {
        this.transform.position = playerTankPos.position;
        cam.transform.localEulerAngles = new Vector3(17.5f, 0, 0);
        this.transform.localEulerAngles = new Vector3(0, tank.localEulerAngles.y , 0);
    }
    public void FollowTank()
    {
        followTank = !followTank;
        if(followTank)
        {
            TankView();
            this.transform.SetParent(playerTankPos);
        } else this.transform.SetParent(null);
    }    
    void Update() 
    {
        this.transform.Translate(
            -sideTranslation * speed * Time.deltaTime,                                  //1 to Left, -1 to Right
            heightTranslation * speed * Time.deltaTime,                                 //1 to Up, -1 to Down
            forwardTranslation * speed * Time.deltaTime);                              // 1 to forward, -1 to back
        cam.transform.Rotate(-horizontalRotation * rotSpeed * Time.deltaTime, 0, 0);     // 1 to Up, -1 to Down
        this.transform.Rotate(0, -verticalRotatiom * rotSpeed * Time.deltaTime, 0);     // 1 to Left, -1 to right
    }
}
