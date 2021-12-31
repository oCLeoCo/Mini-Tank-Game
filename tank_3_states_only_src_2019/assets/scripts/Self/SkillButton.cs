using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    Skill skill;
    BuffAndUpgrade bU;
    int sSN;
    Image sI;
    Text sT, slv;
    float cD;
    public void InstantiateButton(Skill s, int ssn, BuffAndUpgrade bu)
    {
        skill = s;
        sSN = ssn;
        bU = bu;
        sI = transform.GetChild(0).GetComponent<Image>();
        sT = transform.GetChild(1).GetComponent<Text>();
        slv = transform.GetChild(2).GetComponent<Text>();
        if (s.skillIcon != null) sI.sprite = s.skillIcon;
        SignUp();
        OnSkillLevelChange(0);
        ChangeCoolDown(0);
    }
    void SignUp()
    {
        skill.OnSkillLevelChange += OnSkillLevelChange;
        skill.OnSkillStateChange += GetSkillState;
        skill.OnSkillElapsedTimeChange += ChangeCoolDown;
    }
    void OnSkillLevelChange(int lv)
    {
        slv.text = lv.ToString("0");
        if (lv == 0) slv.text = "";
    }
    void GetSkillState(float cd)
    {
        cD = cd;
    }
    void ChangeCoolDown(float seT)
    {
        sT.text = seT.ToString("0.00");
        if (seT == 0) sT.text = "";
    }
    public void LaunchSkill()
    {
        bU.LaunchSkill(sSN);
    }
}
