using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator_Target : MonoBehaviour
{
    public GameObject target;

    // Update is called once per frame
    void Update()
    {
        if(Target.count < 5)
        {
            Instantiate(target, new Vector3(Random.Range(36, 46), Random.Range(0.8f, 2.8f), 14), Quaternion.identity);
        }
    }
}
