using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class test2 : MonoBehaviour
{
    public GameObject testFollow;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setTarget()
    {
        NavMeshAgent nav = testFollow.GetComponent<NavMeshAgent>();
        nav.SetDestination(transform.position);
    }
}
