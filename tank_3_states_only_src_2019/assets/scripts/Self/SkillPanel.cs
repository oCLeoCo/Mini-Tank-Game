using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public GameObject skillButtonPrefab;
    BuffAndUpgrade buffAndUpgrade;
    Transform tankSkillLayout;
    Text lv, sP, exp;
    Image expBar;
    private void Start()
    {
        tankSkillLayout = transform.GetChild(0);
        expBar = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        exp = transform.GetChild(1).GetChild(1).GetComponent<Text>();
        lv = transform.GetChild(2).GetChild(0).GetComponent<Text>();
        sP = transform.GetChild(3).GetChild(0).GetComponent<Text>();
    }
    public void SignUp(GameManager gm)
    {
        gm.OnPlayerExpChange += OnPlayerExpChange;
        gm.OnPlayerLevelChange += OnPlayerLevelChange;
        gm.OnPlayerSkillPointChange += OnPlayerSkillPointChange;
    }
    void OnPlayerExpChange(int v, int u)
    {
        exp.text = v + " / " + u;
        expBar.fillAmount = (float)v / (float)u;
    }
    void OnPlayerLevelChange(int v)
    {
        lv.text = "" + v;
    }
    void OnPlayerSkillPointChange(int v)
    {
        sP.text = "" + v;
    }
    public void InstantiateButton(BuffAndUpgrade bu)
    {
        buffAndUpgrade = bu;
        int sSN = 0;
        foreach (Skill x in buffAndUpgrade.skills)
        {
            if (x.skillType != SkillType.Passive)
            {
                GameObject sb = Instantiate(skillButtonPrefab, tankSkillLayout);
                SkillButton sB = sb.GetComponent<SkillButton>();
                sB.InstantiateButton(buffAndUpgrade.skills[sSN], sSN, buffAndUpgrade);
                sSN += 1;
            }
        }
    }
}
