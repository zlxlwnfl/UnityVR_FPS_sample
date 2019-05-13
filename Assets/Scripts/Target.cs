using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Target : MonoBehaviour
{
    static public int count = 0; // 생성된 모든 타겟 수를 나타내는 변수
    public float hp = 100;

    public GameObject Gun;

    private Gun gun;
    private float m_LastFireTime = 0;
    public float m_TimeBetFire = 2.0f;
    public float m_FireDistance = 100f;
    public int m_Damage = 25;

  //  private TargetAttribute attri;
   // public Slider hpbar;
   // public GameObject HeadUpPosition;



    private void Start() {
        count++; // 타겟 생성시마다 count 값 1 증가
        //attri = gameObject.GetComponent<TargetAttribute> ();
    }

    public void OnDamage(int m_Damage)
    {
        hp -= m_Damage; // 타겟이 데미지 입음

        if(hp <= 0)
        {
            Destroy(gameObject); // 타겟 삭제
            Debug.Log("타겟 객체 수 : " + count);

        }
    }

    private void Update()
    {

        // 총이 준비된 상태이고 현재 시간 >= 마지막 발사 시점 + 연사 간격
        if (Time.time >= m_LastFireTime + m_TimeBetFire)
        {
            m_LastFireTime = Time.time; // 마지막으로 총을 쏜 시점이 현재 시점으로 갱신

            Shot();
        }

       // hpbar.value = (float)attri.hp / (float)attri.hpMax;
       // hpbar.transform.position = HeadUpPosition.transform.position;
    }

    private void Shot()
    {
        RaycastHit hit; // 레이캐스트 정보를 저장하는 충돌 정보 컨테이너

        // 레이캐스트(시작지점, 방향, 충돌 정보 컨테이너, 사정거리)
        if (Physics.Raycast(gameObject.transform.position, Gun.transform.position, out hit, m_FireDistance))
        {
            if (hit.collider.tag == "Player")
                gun = hit.collider.GetComponent<Gun>();
            //맞힌 상대의 태그가 Enemy라면 Target 컴포넌트 불러옴

            if (gun != null) // Target 컴포넌트가 존재한다면
            {
                gun.OnDamage(m_Damage); // 타겟에게 데미지 가함
            }
        }
    }
}
