using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    static public int count = 0;
    public float hp = 100;

    private void Start() {
        count++;
    }

    public void OnDamage(int damageAmount)
    {
        hp -= damageAmount;

        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
