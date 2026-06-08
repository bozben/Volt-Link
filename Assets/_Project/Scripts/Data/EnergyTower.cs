using System;
using System.Runtime.InteropServices;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class EnergyTower : MonoBehaviour
{
    [Header("Charging Settings")]
    [SerializeField] private float chargeTimeRequired = 3f;
    [SerializeField] private Color idleColor = Color.red;
    [SerializeField] private Color chargedColor = Color.cyan;
    [SerializeField] private AudioClip chargeLoopClip;
    [SerializeField] private AudioClip activateClip;
    [Header("Zone Settings")]
    [SerializeField] private float energyRadius = 1.2f;

    private float currentChargeTime = 0f;
    private bool isCharged = false;
    private SpriteRenderer sr;
    private AudioSource chargeSource;

    private int ropeSegmentsTouching = 0;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = idleColor;

        SetupColliders();
        chargeSource = gameObject.AddComponent<AudioSource>();
        chargeSource.outputAudioMixerGroup = AudioManager.Instance.GetAmbienceLoudGroup();
        chargeSource.clip = chargeLoopClip;
        chargeSource.loop = true;
        chargeSource.spatialBlend = 1.0f;

    }

    private void SetupColliders()
    {
        Collider2D bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider != null) bodyCollider.isTrigger = false;

        GameObject energyObj = new GameObject("EnergyZone");
        energyObj.transform.SetParent(this.transform);
        energyObj.transform.localPosition = Vector3.zero;

        CircleCollider2D energyCollider = energyObj.AddComponent<CircleCollider2D>();
        energyCollider.isTrigger = true;
        energyCollider.radius = energyRadius;


        EnergySensor sensor = energyObj.AddComponent<EnergySensor>();
        sensor.tower = this;
    }

    private void Update()
    {
        if (isCharged) return;

        if (ropeSegmentsTouching > 0)
        {
            if (!chargeSource.isPlaying) chargeSource.Play();
            currentChargeTime += Time.deltaTime;
            float progress = currentChargeTime / chargeTimeRequired;
            sr.color = Color.Lerp(idleColor, chargedColor, progress);

            if (currentChargeTime >= chargeTimeRequired)
            {
                CompleteCharge();
            }
        }
        else
        {
            if (chargeSource.isPlaying) chargeSource.Stop();
        }
    }

    public void AddContact() { ropeSegmentsTouching++; }
    public void RemoveContact() { ropeSegmentsTouching--; if (ropeSegmentsTouching < 0) ropeSegmentsTouching = 0; }

    private void CompleteCharge()
    {
        isCharged = true;
        sr.color = chargedColor;
        if (chargeSource.isPlaying) chargeSource.Stop();
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(activateClip);
        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.TowerActivated(this);
            
        }
    }
    public bool IsCharged() { return isCharged; }
}
