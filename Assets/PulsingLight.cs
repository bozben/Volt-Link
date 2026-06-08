using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PulsingLight : MonoBehaviour
{
    [SerializeField] private float minIntensity = 1.2f;
    [SerializeField] private float maxIntensity = 2.0f;
    [SerializeField] private float pulseSpeed = 1.5f;

    private Light2D light2D;

    private void Awake() => light2D = GetComponent<Light2D>();

    private void Update()
    {
        light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
    }
}