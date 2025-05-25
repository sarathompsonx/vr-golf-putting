using UnityEngine;

public class GolfBallEvents : MonoBehaviour
{
    public delegate void SetStartPositionDelegate(float x, float z);
    public static event SetStartPositionDelegate OnSetStartPosition;

    public delegate void MoveToPositionDelegate(Vector3 position);
    public delegate void ResetToStartPositionDelegate();
    public delegate void ApplyForceDelegate(Vector3 force, ForceMode forceMode);
    public delegate void StopPhysicsDelegate();
    public delegate void BallPositionChangedDelegate(Vector3 position, Vector3 velocity);

    public delegate void TrialStartedDelegate(int trialId);
    public delegate void TrialEndedDelegate(int trialId, bool success, float distanceFromTarget);
    
    public static event MoveToPositionDelegate OnMoveToPosition;
    public static event ResetToStartPositionDelegate OnResetToStartPosition;
    public static event ApplyForceDelegate OnApplyForce;
    public static event StopPhysicsDelegate OnStopPhysics;
    public static event BallPositionChangedDelegate OnBallPositionChanged;
    public static event TrialStartedDelegate OnTrialStarted;
    public static event TrialEndedDelegate OnTrialEnded;

    public static void SetStartPosition(float x, float z)
    {
        OnSetStartPosition?.Invoke(x, z);
    }

    public static void MoveToPosition(Vector3 position)
    {
        OnMoveToPosition?.Invoke(position);
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
        OnApplyForce?.Invoke(force, forceMode);
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