using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotDropDown : MonoBehaviour
{
    public enum DropDownState
    {
        Empty,
        Player,
        AI,
    }
    Dropdown dropdown;
    DropDownState dropDownState;
    public Scrollbar scroll;
    public GameObject SlotImage;
    public Image iconImage;
    public Sprite[] icon;
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        scroll.value = 1f;
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = "Move to this slot" }); //0
        dropdown.options.Add(new Dropdown.OptionData() { text = "Add a AI" });                  //1
        dropdown.options.Add(new Dropdown.OptionData() { text = "RemoveAI" });              //2
        dropdown.options.Add(new Dropdown.OptionData() { text = "Change AI Tank" });    //3
        dropdown.options.Add(new Dropdown.OptionData() { text = "Change Tank" });       //4
        ChangeToEmptySlot();
    }
    public void DropDownSelection(int value)
    {
        switch (value)
        {
            case 0:
                ChangeToPlayerSlot();
                break;
            case 1:
                ChangeToAISlot();
                break;
            case 2:
                ChangeToEmptySlot();
                break;
            case 3: 
                break;
            case 4:
                break;
            default:
                break;
        }
    }
    void ChangeToEmptySlot()
    {
        dropDownState = DropDownState.Empty;
        SlotImage.SetActive(false);
    }
    void ChangeToPlayerSlot()
    {
        dropDownState = DropDownState.Player;
        iconImage.sprite = icon[0];
        SlotImage.SetActive(true);
    }
    void ChangeToAISlot()
    {
        dropDownState = DropDownState.AI;
        iconImage.sprite = icon[1];
        SlotImage.SetActive(true);
    }

}
