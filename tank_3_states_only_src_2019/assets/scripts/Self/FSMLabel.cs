using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSMLabel : MonoBehaviour
{
    public TankAI tank;
    public Text fsmText;
    public bool selfSetting;
    void Awake()
    {
        if (selfSetting)
        {
            SignUp();
        }
    }
    public void SignUp()
    {
        tank.OnFSMStateChange += ChangeText;
    }
    void ChangeText(string state)
    {
        fsmText.text = "" + state;
    }
}
