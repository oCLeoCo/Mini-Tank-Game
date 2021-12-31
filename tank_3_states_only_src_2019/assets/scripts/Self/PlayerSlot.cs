using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour
{
    public GameObject blockPrefab;
    Canvas ca, ra;
    void Start()
    {
        ca = GetComponentInParent<Canvas>();
        ra = ca.rootCanvas;
    }
    public void CreateBlock()
    {
        GameObject block = Instantiate(blockPrefab, ra.transform);
    }
}

