using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPaint : MonoBehaviour 
{
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collision!!" + col);
        if (col.gameObject.name == "prop_powerCube")
        {
            Destroy(col.gameObject);
        }
    }
}
