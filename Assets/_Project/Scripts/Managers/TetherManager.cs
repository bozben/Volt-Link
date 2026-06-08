using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class TetherManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Rigidbody2D coreRb;
    [SerializeField] private AudioClip ropeSnapClip;

    [Header("Visual Offsets")]
    [SerializeField] private Vector3 playerOffset = new Vector3(0, -0.5f, 0);
    [SerializeField] private Vector3 coreOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private float ropeWidth = 0.2f;

    [Header("Chain Settings")]
    [SerializeField] private int segmentCount = 8;
    [SerializeField] private float targetRopeLength = 6f;
    [SerializeField] private float segmentMass = 0.1f;
    [SerializeField] private float segmentDamping = 2f;
    [SerializeField] private float maxTension = 1500f;
    [SerializeField] private float wallTensionMultiplier = 0.5f;

    [Header("Tether Visuals")]
    [SerializeField] private float flowSpeed = 0.5f;
    private float offset = 0f;
    private Material lineMaterial;



    private List<Rigidbody2D> segments = new List<Rigidbody2D>();
    private List<DistanceJoint2D> joints = new List<DistanceJoint2D>();
    private LineRenderer lineRenderer;
    private bool isConnected = false;
    private int wallContactCount = 0;
    bool isTouchingWall => wallContactCount > 0;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineMaterial = new Material(lineRenderer.sharedMaterial);
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
    }
    private void Start()
    {
        CreateChain();
    }

    private void FixedUpdate()
    {

        if (!isConnected || joints == null || joints.Count == 0) return;

        float currentMaxTension = isTouchingWall ? (maxTension * wallTensionMultiplier) : maxTension;

        if (joints[0] != null)
        {
            float force = joints[0].reactionForce.magnitude;
            // Debug.Log($"Force: {force:F2} | Limit: {currentMaxTension:F2} | Wall: {isTouchingWall}");
        }
        // Check tension on EVERY joint in the chain
        foreach (var joint in joints)
        {
            if (joint != null && joint.reactionForce.magnitude > currentMaxTension)
            {
                SnapTether();
                break;
            }
        }
    }
    private void LateUpdate()
    {
        if (!isConnected)
        {
            lineRenderer.positionCount = 0;
            return;
        }
        Vector3 startPos = playerRb.transform.TransformPoint(playerOffset);
        Vector3 endPos = coreRb.transform.TransformPoint(coreOffset);
        // Update LineRenderer to follow the chain segments
        lineRenderer.positionCount = segments.Count + 2;
        lineRenderer.SetPosition(0, startPos);

        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i + 1, segments[i].position);
        }

        lineRenderer.SetPosition(segments.Count + 1, endPos);
    }
    private void Update()
    {
        if (!isConnected) return;

        offset += Time.deltaTime * flowSpeed;
        if (offset > 1f) offset -= 1f;

        lineMaterial.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }

    private void CreateChain()
    {
        DestroyChain();
        Vector2 startPoint = playerRb.transform.TransformPoint(playerOffset);
        Vector2 endPoint = coreRb.transform.TransformPoint(coreOffset);

        float segmentLength = targetRopeLength / segmentCount;
        Vector2 direction = (endPoint - startPoint).normalized;

        float currentActualDistance = Vector2.Distance(startPoint, endPoint);
        float spawnSpacing = currentActualDistance / segmentCount;

        Rigidbody2D previousBody = playerRb;

        for (int i = 0; i < segmentCount; i++)
        {
            // SegmentObject
            GameObject segmentObject = new GameObject($"RopeSegment_{i}");

            segmentObject.layer = LayerMask.NameToLayer("Rope");
            segmentObject.transform.position = playerRb.position + (direction * spawnSpacing * (i + 1));

            // Physics Components
            Rigidbody2D rb = segmentObject.AddComponent<Rigidbody2D>();
            rb.mass = segmentMass;
            rb.linearDamping = segmentDamping;
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Tether segment 
            TetherSegment segScript = segmentObject.AddComponent<TetherSegment>();
            segScript.Initialize(this);

            // Collider for segments
            CircleCollider2D circleCollider = segmentObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = .1f;



            // connection
            DistanceJoint2D joint = segmentObject.AddComponent<DistanceJoint2D>();
            joint.connectedBody = previousBody;
            joint.distance = segmentLength;
            joint.autoConfigureDistance = false;

            segments.Add(rb);
            joints.Add(joint);
            previousBody = rb;

        }

        // add final segment
        DistanceJoint2D finalJoint = segments[segments.Count - 1].gameObject.AddComponent<DistanceJoint2D>();
        finalJoint.connectedBody = coreRb;
        finalJoint.connectedAnchor = coreOffset;
        finalJoint.distance = segmentLength;
        finalJoint.autoConfigureDistance = false;
        joints.Add(finalJoint);

        isConnected = true;
    }
    private void SnapTether()
    {
        Debug.Log("CHAIN SNAPPED!");
        isConnected = false;
        DestroyChain();

        if (GameManager.Instance != null)
            GameManager.Instance.StartRecovery();
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(ropeSnapClip);
    }
    private void DestroyChain()
    {
        foreach (var seg in segments)
        {
            if (seg != null) Destroy(seg.gameObject);
        }
        segments.Clear();
        joints.Clear();
        wallContactCount = 0;
    }

    public void Reconnect()
    {
        CreateChain();
    }
    public bool IsConnected()
    {
        return isConnected;
    }
    public void AddWallContact() { wallContactCount++; }
    public void RemoveWallContact() { wallContactCount = Mathf.Max(0, wallContactCount - 1); }


}