using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform coreTransform;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.2f; 
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    private Vector3 currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (playerTransform == null || coreTransform == null) return;
        Vector3 midpoint = (playerTransform.position + coreTransform.position) / 2f;

        Vector3 targetPosition = midpoint + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
            );
    }
}
