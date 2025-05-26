using UnityEngine;

public class GolfBallEvents : MonoBehaviour
{
    public delegate void SetStartPositionDelegate(float x, float z);// delegate declaring a function/blueprint for a function
    public static event SetStartPositionDelegate OnSetStartPosition;// static event others can subscribe to 

    public delegate void MoveToPositionDelegate(Vector3 position);// full setting full position using Vector3
    public delegate void ResetToStartPositionDelegate();// no params needed, resets to saved position 
    public delegate void ApplyForceDelegate(Vector3 force, ForceMode forceMode);// applies force using direction and force mode
    public delegate void StopPhysicsDelegate();// freezes physics on ball
    public delegate void BallPositionChangedDelegate(Vector3 position, Vector3 velocity);// reports ball position and velocity 

    public delegate void TrialStartedDelegate(int trialId);// called when new trial starts
    public delegate void TrialEndedDelegate(int trialId, bool success, float distanceFromTarget);// called when new trial ends
    
    public static event MoveToPositionDelegate OnMoveToPosition;// belongs to class itself, not an instance
    public static event ResetToStartPositionDelegate OnResetToStartPosition;
    public static event ApplyForceDelegate OnApplyForce;
    public static event StopPhysicsDelegate OnStopPhysics;
    public static event BallPositionChangedDelegate OnBallPositionChanged;
    public static event TrialStartedDelegate OnTrialStarted;
    public static event TrialEndedDelegate OnTrialEnded;

    public static void SetStartPosition(float x, float z)
    {
        OnSetStartPosition?.Invoke(x, z);// notifies subscribers to move ball to this posit
    }

    public static void MoveToPosition(Vector3 position)// reset ball to saved starting point
    {
        OnMoveToPosition?.Invoke(position);// tells listeners to reset abll
        Debug.Log($"[GolfBallEvents] Broadcasting MoveToPosition: {position}");
    }
    
    // reset ball to stored start position 
    public static void ResetToStartPosition()
    {
        OnResetToStartPosition?.Invoke();
        Debug.Log("[GolfBallEvents] Broadcasting ResetToStartPosition");
    }

    // physics for force but will need to take out for motion tracking to come in
    public static void ApplyForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse)
    {
        OnApplyForce?.Invoke(force, forceMode);// tells listeners when to apply force
        Debug.Log($"[GolfBallEvents] Broadcasting ApplyForce: {force} with mode {forceMode}");
    }

    //freeze all ball physics 
    public static void StopPhysics()
    {
        OnStopPhysics?.Invoke();
        Debug.Log("[GolfBallEvents] Broadcasting StopPhysics");
    }

    // reporting ball position changes 
    public static void BallPositionChanged(Vector3 position, Vector3 velocity)
    {
        OnBallPositionChanged?.Invoke(position, velocity);
    }

    // starting new putting trial 
    public static void TrialStarted(int trialId)
    {
        OnTrialStarted?.Invoke(trialId);
        Debug.Log($"[GolfBallEvents] Broadcasting TrialStarted: {trialId}");
    }

    // ending putting trial with results 
    public static void TrialEnd(int trialId, bool success, float distanceFromTarget)
    {
        OnTrialEnded?.Invoke(trialId, success, distanceFromTarget);
        Debug.Log($"[GolfBallEvents] Broadcasting TrialEnded: Trial {trialId}, Success: {success}, Distance: {distanceFromTarget}");
    }
}