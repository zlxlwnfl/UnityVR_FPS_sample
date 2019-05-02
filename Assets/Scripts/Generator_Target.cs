using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator_Target : MonoBehaviour
{
    public GameObject target; // 생성될 타겟

    private float before_x = 0; // 마지막으로 생성된 타겟의 x좌표 값
    private float before_y = 0; // 마지막으로 생성된 타겟의 y좌표 값

    // Update is called once per frame
    void Update()
    {
        float x,y; // 타겟 생성을 위한 x,y좌표 값 선언

        do{
            x = Random.Range(36, 46); // 랜덤값 x 삽입
            y = Random.Range(0.8f, 2.8f); // 랜덤값 y 삽입
        }while(before_x.Equals(x) || before_y.Equals(y));
        // 마지막으로 생성된 타겟의 x 좌표값과 방금 생성된 x 좌표값이 같다면 x 좌표값 재생성
        // 마지막으로 생성된 타겟의 y 좌표값과 방금 생성된 y 좌표값이 같다면 y 좌표값 재생성
        // 타겟이 생성되는 위치가 겹치지 않게 하기위한 조건

        if(Target.count < 5) // 타겟의 최대 생성수: 5
        {
            Instantiate(target, new Vector3(x, y, 14), Quaternion.identity);
            // 타겟 생성 함수
        }

        before_x = x;
        before_y = y;
    }
}