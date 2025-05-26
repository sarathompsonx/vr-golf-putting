using UnityEngine;

public class GolfBall : MonoBehaviour
{
    [Header("Ball Physics")]
    [SerializeField] private float ballMass = 0.0459f; // Standard golf ball mass (45.9g)
    [SerializeField] private float ballRadius = 0.02135f; // Standard golf ball radius (21.35mm)
    [SerializeField] private float rollingFriction = 0.3f;
    [SerializeField] private float bounciness = 0.1f;
    [SerializeField] private float linearDrag = 1.5f;
    [SerializeField] private float angularDrag = 3f;
    
    [Header("Movement Settings")]
    [SerializeField] private float puttingForceMultiplier = 1.0f; // Adjust putting strength
    [SerializeField] private float velocityThreshold = 0.05f; // Speed below which ball is considered stopped
    [SerializeField] private float targetTolerance = 0.1f; // Distance from target to consider "in hole" (10cm)
    
    // Internal variables
    Vector3 startPosition;
    private Vector3 lastReportedPosition;
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private bool isMoving = false;
    private int currentTrialId = 0;

    void Awake()
    {
        // Setup physics components
        SetupPhysicsComponents();
        
        // Store initial position
        startPosition = transform.position;
        lastReportedPosition = startPosition;
        
        Debug.Log($"[GolfBall] Initialized at position: {startPosition}");
    }

    void OnEnable()
    {
        // Subscribe to all events from the broadcaster
        GolfBallEvents.OnSetStartPosition += SetStartPosition;
        GolfBallEvents.OnMoveToPosition += MoveToPosition;
        GolfBallEvents.OnResetToStartPosition += ResetToStartPosition;
        GolfBallEvents.OnApplyForce += ApplyForce;
        GolfBallEvents.OnStopPhysics += StopPhysics;
        
        Debug.Log("[GolfBall] Event subscriptions activated");
    }

    void OnDisable()
    {
        // Unsubscribe from all events
        GolfBallEvents.OnSetStartPosition -= SetStartPosition;
        GolfBallEvents.OnMoveToPosition -= MoveToPosition;
        GolfBallEvents.OnResetToStartPosition -= ResetToStartPosition;
        GolfBallEvents.OnApplyForce -= ApplyForce;
        GolfBallEvents.OnStopPhysics -= StopPhysics;
        
        Debug.Log("[GolfBall] Event subscriptions deactivated");
    }

    void Update()
    {
        // Check for position changes and report them
        CheckAndReportPositionChange();
        
        // Check if ball has stopped moving
        CheckIfBallStopped();

    }

    /// <summary>
    /// Setup Rigidbody and Collider components with realistic golf ball physics
    /// </summary>
    private void SetupPhysicsComponents()
    {
        // Setup Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.mass = ballMass;
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;
        rb.useGravity = true;
        
        // Setup Collider
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        
        sphereCollider.radius = ballRadius;
        
        // Create physics material for realistic ball behavior
        PhysicsMaterial ballMaterial = new PhysicsMaterial("GolfBallMaterial");
        ballMaterial.dynamicFriction = rollingFriction;
        ballMaterial.staticFriction = rollingFriction;
        ballMaterial.bounciness = bounciness;
        ballMaterial.frictionCombine = PhysicsMaterialCombine.Average;
        ballMaterial.bounceCombine = PhysicsMaterialCombine.Average;
        
        sphereCollider.material = ballMaterial;
        
        Debug.Log("[GolfBall] Physics components configured");
    }

    /// <summary>
    /// Set the ball's start position (responds to broadcaster event)
    /// </summary>
    private void SetStartPosition(float x, float z)
    {
        Vector3 currPos = transform.position;
        Vector3 position = new Vector3(x, currPos.y, z);
        transform.position = position;
        
        // Update stored start position and stop physics
        startPosition = position;
        lastReportedPosition = position;
        StopBallPhysics();
        
        Debug.Log("Golf ball position set to: " + position);
        GolfBallEvents.BallPositionChanged(position, Vector3.zero);
    }
    
    /// <summary>
    /// Move ball to specific position (responds to broadcaster event)
    /// </summary>
    private void MoveToPosition(Vector3 position)
    {
        transform.position = position;
        lastReportedPosition = position;
        StopBallPhysics();
        
        Debug.Log($"[GolfBall] Moved to position: {position}");
        GolfBallEvents.BallPositionChanged(position, Vector3.zero);
    }
    
    /// <summary>
    /// Reset ball to start position (responds to broadcaster event)
    /// </summary>
    private void ResetToStartPosition()
    {
        transform.position = startPosition;
        lastReportedPosition = startPosition;
        StopBallPhysics();
        isMoving = false;
        
        Debug.Log($"[GolfBall] Reset to start position: {startPosition}");
        GolfBallEvents.BallPositionChanged(startPosition, Vector3.zero);
    }
    
    /// <summary>
    /// Apply force to ball for putting (responds to broadcaster event)
    /// </summary>
    private void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        Vector3 adjustedForce = force * puttingForceMultiplier;
        rb.AddForce(adjustedForce, forceMode);
        isMoving = true;
        
        Debug.Log($"[GolfBall] Force applied: {adjustedForce} (Mode: {forceMode})");
        Debug.Log($"[GolfBall] Resulting velocity: {rb.linearVelocity}");
    }
    
    /// <summary>
    /// Stop all physics motion (responds to broadcaster event)
    /// </summary>
    private void StopPhysics()
    {
        StopBallPhysics();
        Debug.Log("[GolfBall] Physics stopped via broadcaster");
    }
    
    /// <summary>
    /// Internal method to stop ball physics
    /// </summary>
    private void StopBallPhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isMoving = false;
    }
    
    /// <summary>
    /// Check for position changes and report them
    /// </summary>
    private void CheckAndReportPositionChange()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastReportedPosition);
        
        if (distanceMoved > 0.001f) // 1mm threshold
        {
            lastReportedPosition = currentPosition;
            GolfBallEvents.BallPositionChanged(currentPosition, rb.linearVelocity);
        }
    }
    
    /// <summary>
    /// Check if ball has stopped moving
    /// </summary>
    private void CheckIfBallStopped()
    {
        if (isMoving && rb.linearVelocity.magnitude < velocityThreshold)
        {
            isMoving = false;
            Debug.Log($"[GolfBall] Ball stopped at position: {transform.position}");
            
        }
    }
    
    /// <summary>
    /// Handle collision with target or other objects
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            Debug.Log("[GolfBall] Ball entered target hole!");
            StopBallPhysics();
            GolfBallEvents.TrialEnd(currentTrialId, true, 0f);
        }
    }
    
    /// <summary>
    /// Public method to start a new trial
    /// </summary>
    public void StartNewTrial()
    {
        currentTrialId++;
        ResetToStartPosition();
        GolfBallEvents.TrialStarted(currentTrialId);
        Debug.Log($"[GolfBall] Started trial {currentTrialId}");
    }
}