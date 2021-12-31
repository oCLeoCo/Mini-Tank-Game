using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundPanel : MonoBehaviour
{
    Text fundText;
    private void Start()
    {
        fundText = transform.GetChild(0).GetComponent<Text>();
    }
    public void SignUp(GameManager gm)
    {
        gm.OnPlayerFundChange += ChangeFund;
    }
    void ChangeFund(int fund)
    {
        fundText.text = "" + fund;
    }
    
}
