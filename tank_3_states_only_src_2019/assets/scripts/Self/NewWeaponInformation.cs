using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewWeaponInformation : MonoBehaviour
{
    public TankRole player { get; set; }
    public int barNum { get; set; } 
    Image reloadBar;
    Text weaponName, coolDownText, ammoLeft;
    float reloadSecond;
    private void Start()
    {
        reloadBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        weaponName = transform.GetChild(1).GetComponent<Text>();
        coolDownText = transform.GetChild(2).GetComponent<Text>();
        ammoLeft = transform.GetChild(3).GetComponent<Text>();
    }
    public void SignUp(TankRole tankCs, BuffAndUpgrade playerBU )
    {
        player = tankCs;
        SignUp();
        player.OnWeaponChange += SignUp;
    }
    public void SignUp()
    {
        player.weapon[barNum].GetReloadElapsedTime += ChangeCoolDown;
        player.weapon[barNum].GetNewWeaponState += GetNewWeaponState;
        player.weapon[barNum].GetAmmoState += GetAmmoState;
    }
    void GetNewWeaponState(string name, float time, int maxAmmo)
    {
        weaponName.text = name;
        reloadSecond = time;
    }
    void ChangeCoolDown(float time)
    {
        reloadBar.fillAmount = (time / reloadSecond);
        time = reloadSecond - time;
        coolDownText.text = time.ToString("0.00");
    }
    void GetAmmoState(int ammo, int maxAmmo)
    {
        ammoLeft.text = ammo + " / " + maxAmmo;
    }
}
