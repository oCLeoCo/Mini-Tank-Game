using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TankLabel : MonoBehaviour
{
    public bool selfSetting;
    public TankRole tank ;
    public Text nameText, healthText;
    public Image healthBar;
    void Awake()
    {
        if(selfSetting)
        {
            SignUp();
        }
    }
    public void SignUp()
    {
        tank.OnHealthChange += HealthChange;
        nameText.text = tank.userName + "\n" + tank.tankName;
    }
    void HealthChange(float health, float maxHealth, bool inDamage)
    {
        healthText.text = "HP: " + (int)health + " / " + (int)maxHealth;
        healthBar.fillAmount = (health/maxHealth);
    }
}
