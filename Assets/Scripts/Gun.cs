using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public Animator m_Animator;
    public Transform m_FireTransform; // 총구 위치
    public ParticleSystem m_MuzzleFlashEffect; // 터지는 이펙트 애니메이션

    public AudioSource m_GunAudioPlayer;
    public AudioClip m_ShotClip; // 총 쏘는 소리
    public AudioClip m_ReloadClip; // 리로드 하는 소리

    public LineRenderer m_BulletLineRenderer; // 총이 발사하는 궤적을 그리기 위한 라인 렌더러

    public GameObject m_ImpactPrefab; // 총 맞은 장소에 생성할 이펙트 & 데칼 원본

    public Text m_AmmoText; // 총 옆 UI에 나타낼 텍스트

    public int m_MaxAmmo = 10; // 탄창의 최대 탄약 갯수
    public float m_TimeBetFire = 0.3f; // 발사와 발사 사이의 시간 간격
    public int m_Damage = 25;
    public float m_ReloadTime = 2.0f; // 리로드할때 걸리는 시간
    public float m_FireDistance = 100f; // 총이 맞힐 수 있는 사정거리

    private enum State { Ready, Empty, Reloading }; // 총의 상태를 나타낼 변수

    private State m_CurrentState = State.Empty; // 총의 현재 상태

    private float m_LastFireTime; // 마지막으로 발사한 시간
    private int m_CurrentAmmo = 0; // 현재 탄약 갯수

    private Target target;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentState = State.Empty; // 탄약이 빈 상태로 시작
        m_LastFireTime = 0; // 마지막으로 총을 쏜 시점을 초기화

        m_BulletLineRenderer.positionCount = 2; // 라인 렌더러가 사용할 정점을 두개로 지정
        m_BulletLineRenderer.enabled = false; // 라인 렌더러를 끔

        UpdateUI(); // UI를 갱신
    }

    public void Fire()
    {
        // 총이 준비된 상태이고 현재 시간 >= 마지막 발사 시점 + 연사 간격
        if(m_CurrentState == State.Ready && Time.time >= m_LastFireTime + m_TimeBetFire)
        {
            m_LastFireTime = Time.time; // 마지막으로 총을 쏜 시점이 현재 시점으로 갱신

            Shot();
            UpdateUI();
        }
    }

    private void Shot()
    {
        RaycastHit hit; // 레이캐스트 정보를 저장하는 충돌 정보 컨테이너

        Vector3 hitPosition = m_FireTransform.position + m_FireTransform.forward * m_FireDistance;
        // 총을 쏴서 총알이 맞은 곳 : 처음 값으로는 총구 위치 + 총구 위치로 앞 쪽 방향 * 사정거리

        // 레이캐스트(시작지점, 방향, 충돌 정보 컨테이너, 사정거리)
        if(Physics.Raycast(m_FireTransform.position, m_FireTransform.forward, out hit, m_FireDistance))
        {
            target = hit.collider.GetComponent<Target>();
            //맞힌 상대에게 Target 컴포넌트가 있다면 불러옴

            if(target != null) // Target 컴포넌트가 존재한다면
            {
                target.OnDamage(m_Damage); // 타겟에게 데미지 가함
            }

            hitPosition = hit.point; // 충돌 위치 가져오기

            GameObject decal = Instantiate(m_ImpactPrefab, hitPosition, Quaternion.LookRotation(hit.normal));
            // 피탄 효과 게임 오브젝트를 복제 생성, 충돌 지점에, 충돌한 표면의 방향으로 생성
            decal.transform.SetParent(hit.collider.transform);    
        }

        StartCoroutine(ShotEffect(hitPosition));
        //발사 이펙트 재생 시작

        m_CurrentAmmo--; // 남의 탄환의 수 1 감소

        if(m_CurrentAmmo <= 0)
        {
            m_CurrentState = State.Empty; // 탄환이 빈 상태로 설정
        }
    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        m_Animator.SetTrigger("Fire"); // Fire 트리거를 당김

        m_BulletLineRenderer.enabled = true;
        // 총알 궤적 랜더러를 켬
        m_BulletLineRenderer.SetPosition(0, m_FireTransform.position);
        // 선분의 첫번째 점은 총구의 위치로 셋팅
        m_BulletLineRenderer.SetPosition(1, hitPosition);
        // 선분의 두번째 점은 입력으로 들어온 피탄 위치로 셋팅

        m_MuzzleFlashEffect.Play(); // 총구 화염 효과 재생

        if(m_GunAudioPlayer.clip != m_ShotClip)
        {
            m_GunAudioPlayer.clip = m_ShotClip; // 총 발사 소리 장전
        }

        m_GunAudioPlayer.Play(); // 총격 소리 재생

        yield return new WaitForSeconds(0.07f); // 처리를 잠시 쉼

        m_BulletLineRenderer.enabled = false;
    }

    private void UpdateUI()
    {
        switch(m_CurrentState)
        {
        case State.Empty:
            break;
        case State.Reloading:
            break;
        default:
            break;
        }
    }

    public void Reload()
    {
        if(m_CurrentState != State.Reloading)
        {
            StartCoroutine(ReloadRoutin());
        }
    }

    private IEnumerator ReloadRoutin() // 실제 재장전 처리가 진행되는 곳
    {
        m_Animator.SetTrigger("Reload"); // Reload 파라미터 트리거를 당김
        m_CurrentState = State.Reloading; // 현재 상태를 재장전 상태로 전환

        m_GunAudioPlayer.clip = m_ReloadClip; // 오디오 소스의 클립을 재장전 소리로 교체 
        m_GunAudioPlayer.Play(); // 재장전 소리 재생

        UpdateUI();

        yield return new WaitForSeconds(m_ReloadTime); // 지정된 시간만큼 처리를 쉼

        m_CurrentAmmo = m_MaxAmmo; // 탄약 갯수 최대 충전
        m_CurrentState = State.Ready;

        UpdateUI();
    }
}
