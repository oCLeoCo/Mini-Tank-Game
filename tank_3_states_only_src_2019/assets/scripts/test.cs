using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Transform o;
    int x, x2;
    float f;

    // Update is called once per frame
    void Update()
    {
        va();
    }
    void va()
    {
        Vector3 v = o.position - transform.position;
        v.y = 0f;
        float angle = Vector3.Angle(v, transform.forward);
        int y = (int)(angle);
        if (x != y)
        {
            x = y;
            print(x + ", angle= " + angle);
        }
    }
    void va2()
    {
        float angle = Vector3.Angle(o.position - transform.position, transform.up);
        int y = (int)(angle);
        if (x2 != y)
        {
            x2 = y;
            print(x2 + ", angle= " + angle);
        }
    }
    void qlr()
    {
        Vector3 relativePos = o.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.forward);
        print(rotation);
    }
    void qa()
    {
        float angle = Quaternion.Angle(transform.rotation, o.rotation);
        if(f != angle)
        {
            f = angle;
            print(angle);
        }
    }
    void cal()
    {
        float angle = o.transform.localEulerAngles.y - transform.localEulerAngles.y;
        if (f != angle)
        {
            f = angle;
            print(angle);
        }
    }
    void cal12()
    {
        float angle =  transform.localEulerAngles.y + o.transform.localEulerAngles.y ;
        int y = (int)(angle / 30) +6;
        while (y > 12) y -= 12;
        if (x != y)
        {
            x = y;
            print(x + ", angle= " + angle);
        }
    }
    void cal12c()
    {
        float angle = o.transform.localEulerAngles.y - transform.localEulerAngles.y;
        int y = (int)(angle / 30)+18;
        while (y > 12) y -= 12;
        if (x != y)
        {
            x = y;
            print(x + ", angle= " + angle);
        }
    }
}
