using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    static public int count = 0; // 생성된 모든 타겟 수를 나타내는 변수
    public float hp = 100;

    private void Start() {
        count++; // 타겟 생성시마다 count 값 1 증가
    }

    public void OnDamage(int damageAmount)
    {
        hp -= damageAmount; // 타겟이 데미지 입음

        if(hp <= 0)
        {
            Destroy(gameObject); // 타겟 삭제
        }
    }
}
