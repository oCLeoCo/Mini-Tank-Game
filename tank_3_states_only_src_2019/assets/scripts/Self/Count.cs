using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Count : MonoBehaviour
{
    public int countNUM { get; set; }
    [SerializeField] int countUpLimit;
    public Text countText;
    //public InputField input;
    void Start()
    {
        countNUM = 0;
        countText.text = "" + countNUM;
    }
    public void ChangeValue(int value)
    {
        countNUM = Mathf.Clamp(
            countNUM + value,
            0, countUpLimit);
        countText.text = "" + countNUM;
    }
}
