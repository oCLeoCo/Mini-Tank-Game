using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLabelRotation : MonoBehaviour
{
    TankRole tank;
    bool isPlayer;
    public Transform camBase;
    Transform cam, labelCanvas;
    void Start()
    {
        tank = GetComponentInParent<TankRole>();
        cam = camBase.GetChild(0).transform;
        labelCanvas = this.transform.GetChild(0).transform;
        isPlayer = tank.isPlayer;
        if (isPlayer) gameObject.SetActive(false);
    }
    void Update()
    {
        //this.transform.localEulerAngles = new Vector3(0, camBase.localEulerAngles.y , 0);
        labelCanvas.localEulerAngles = new Vector3(cam.localEulerAngles.x, 0, 0);
        Vector3 direction = (cam.transform.position - this.transform.position).normalized;
        direction.y = 0f;
        this.transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
    }
}
