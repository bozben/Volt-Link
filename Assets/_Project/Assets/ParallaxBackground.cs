using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxFactorX = 0.05f;
    [SerializeField] private float parallaxFactorY = 0.05f;

    private Camera cam;
    private Vector3 lastCameraPos;

    private void Start()
    {
        cam = Camera.main;
        lastCameraPos = cam.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = cam.transform.position - lastCameraPos;
        transform.position += new Vector3(delta.x * parallaxFactorX, delta.y * parallaxFactorY, 0);
        lastCameraPos = cam.transform.position;
    }
}