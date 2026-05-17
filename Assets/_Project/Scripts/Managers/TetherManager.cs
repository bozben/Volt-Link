using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class TetherManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Rigidbody2D coreRb;

    [Header("Chain Settings")]
    [SerializeField] private int segmentCount = 8;        // Number of links in the chain
    [SerializeField] private float targetRopeLength = 6f; 
    [SerializeField] private float segmentMass = 0.1f;     
    [SerializeField] private float segmentDamping = 2f;    
    [SerializeField] private float maxTension = 1500f;     // Force threshold for snapping
    [SerializeField] private float wallTensionMultiplier = 0.5f; // Lower the value easier to snap

    private List<Rigidbody2D> segments = new List<Rigidbody2D>();
    private List<DistanceJoint2D> joints = new List<DistanceJoint2D>();
    private LineRenderer lineRenderer;
    private bool isConnected = false;
    private bool isTouchingWall = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
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
            Debug.Log($"Force: {force:F2} | Limit: {currentMaxTension:F2} | Wall: {isTouchingWall}");
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

        // Update LineRenderer to follow the chain segments
        lineRenderer.positionCount = segments.Count + 2;
        lineRenderer.SetPosition(0, playerRb.position);

        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i + 1, segments[i].position);
        }

        lineRenderer.SetPosition(segments.Count + 1, coreRb.position);
    }


    private void CreateChain()
    {
        DestroyChain();

        float segmentLength = targetRopeLength / segmentCount;
        Vector2 direction = (coreRb.position - playerRb.position).normalized;

        float currentActualDistance = Vector2.Distance(playerRb.position, coreRb.position);
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
    }
    private void DestroyChain()
    {
        foreach (var seg in segments)
        {
            if (seg != null) Destroy(seg.gameObject);
        }
        segments.Clear();
        joints.Clear();
    }

    public void Reconnect()
    {
        CreateChain();
    }
    public bool IsConnected()
    {
        return isConnected;
    }
    public void SetWallContact(bool contact)
    {
        isTouchingWall = contact;
    }
}
