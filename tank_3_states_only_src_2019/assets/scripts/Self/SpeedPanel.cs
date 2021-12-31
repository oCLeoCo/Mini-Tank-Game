using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedPanel : MonoBehaviour
{
    Text text;
    private void Start()
    {
        text = transform.GetChild(0).GetComponent<Text>();
    }
    public void SignUp(TankRole tR)
    {
        tR.OnSpeedChange += OnSpeedChange;
    }
    void OnSpeedChange(float speed)
    {
        text.text = speed.ToString("0");
    }
}
