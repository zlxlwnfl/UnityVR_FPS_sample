using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public Animator m_Animator;
    public Transform m_FireTransform;
    public ParticleSystem m_MuzzleFlashEffect;

    public AudioSource m_GunAudioPlayer;
    public AudioClip m_ShotClip;
    public AudioClip m_ReloadClip;

    public LineRenderer m_BulletLineRenderer;

    public GameObject m_ImpactPrefab;

    public Text m_AmmoText;

    public int m_MaxAmmo = 13;
    public float m_TimeBetFire = 0.3f;
    public int m_Damage = 25;
    public float m_ReloadTime = 2.0f;
    public float m_FireDistance = 100f;

    private enum State { Ready, Empty, Reloading };

    private State m_CurrentState = State.Empty;

    private float m_LastFireTime;
    private int m_CurrentAmmo = 0;

    private Target target;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentState = State.Empty;
        m_LastFireTime = 0;

        m_BulletLineRenderer.positionCount = 2;
        m_BulletLineRenderer.enabled = false;

        UpdateUI();
    }

    public void Fire()
    {
        if(m_CurrentState == State.Ready && Time.time >= m_LastFireTime + m_TimeBetFire)
        {
            m_LastFireTime = Time.time;

            Shot();
            UpdateUI();
        }
    }

    private void Shot()
    {
        RaycastHit hit;

        Vector3 hitPosition = m_FireTransform.position + m_FireTransform.forward * m_FireDistance;

        if(Physics.Raycast(m_FireTransform.position, m_FireTransform.forward, out hit, m_FireDistance))
        {
            target = hit.collider.GetComponent<Target>();

            if(target != null)
            {
                target.OnDamage(m_Damage);
            }

            hitPosition = hit.point;

            GameObject decal = Instantiate(m_ImpactPrefab, hitPosition, Quaternion.LookRotation(hit.normal));
            decal.transform.SetParent(hit.collider.transform);    
        }

        StartCoroutine(ShotEffect(hitPosition));

        m_CurrentAmmo--;

        if(m_CurrentAmmo <= 0)
        {
            m_CurrentState = State.Empty;
        }
    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        m_Animator.SetTrigger("Fire");

        m_BulletLineRenderer.enabled = true;
        m_BulletLineRenderer.SetPosition(0, m_FireTransform.position);
        m_BulletLineRenderer.SetPosition(1, hitPosition);

        m_MuzzleFlashEffect.Play();

        if(m_GunAudioPlayer.clip != m_ShotClip)
        {
            m_GunAudioPlayer.clip = m_ShotClip;
        }

        m_GunAudioPlayer.Play();

        yield return new WaitForSeconds(0.07f);

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

    private IEnumerator ReloadRoutin()
    {
        m_Animator.SetTrigger("Reload");
        m_CurrentState = State.Reloading;

        m_GunAudioPlayer.clip = m_ReloadClip;
        m_GunAudioPlayer.Play();

        UpdateUI();

        yield return new WaitForSeconds(m_ReloadTime);

        m_CurrentAmmo = m_MaxAmmo;
        m_CurrentState = State.Ready;

        UpdateUI();
    }
}
