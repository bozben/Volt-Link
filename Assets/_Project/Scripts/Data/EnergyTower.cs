using System;
using UnityEngine;

public class EnergyTower : MonoBehaviour
{
    [Header("Charging Settings")]
    [SerializeField] private float chargeTimeRequired = 3f;
    [SerializeField] private Color idleColor = Color.red;
    [SerializeField] private Color chargedColor = Color.cyan; 
    [Header("Zone Settings")]
    [SerializeField] private float energyRadius = 1.2f;

    private float currentChargeTime = 0f;
    private bool isCharged = false;
    private SpriteRenderer sr;

    private int ropeSegmentsTouching = 0;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = idleColor;

        SetupColliders();
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
            currentChargeTime += Time.deltaTime;
            float progress = currentChargeTime / chargeTimeRequired;
            sr.color = Color.Lerp(idleColor, chargedColor, progress);

            if (currentChargeTime >= chargeTimeRequired)
            {
                CompleteCharge();
            }
        }
    }

    public void AddContact() { ropeSegmentsTouching++; }
    public void RemoveContact() { ropeSegmentsTouching--; if (ropeSegmentsTouching < 0) ropeSegmentsTouching = 0; }

    private void CompleteCharge()
    {
        isCharged = true;
        sr.color = chargedColor;
        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.TowerActivated(this);
        }
    }
    public bool IsCharged() { return isCharged; }
}
