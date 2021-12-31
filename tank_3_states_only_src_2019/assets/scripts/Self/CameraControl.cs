using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject cam;
    public float speed = 500;
    public float rotSpeed = 40f;
    float zTranslation, xTranslation, yTranslation, xRotation, yRotatiom;
    public void ForwardMoving(float x)
    {
        zTranslation = speed * x * Time.deltaTime;
    }
    public void SideMoving(float x)
    {
        xTranslation = speed * x * Time.deltaTime;
    }
    public void HeightMoving(float x)
    {
        yTranslation = speed * x * Time.deltaTime;
    }
    public void HorizontalRotate(float x)
    {
        xRotation = rotSpeed * x * Time.deltaTime;
    }
    public void VerticalRotate(float x)
    {
        yRotatiom = rotSpeed * x * Time.deltaTime;
    }
    public void DefaultRotation()
    {
        cam.transform.localEulerAngles = new Vector3(45f, 0, 0);
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    public void TopView()
    {
        cam.transform.localEulerAngles = new Vector3(90f, 0, 0);
    }
    void Update() 
    {
        this.transform.Translate(xTranslation, yTranslation, zTranslation);
        cam.transform.Rotate(xRotation, 0, 0);
        this.transform.Rotate(0, yRotatiom, 0);
    }
}
