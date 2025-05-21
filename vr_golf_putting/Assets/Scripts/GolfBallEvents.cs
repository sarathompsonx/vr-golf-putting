using UnityEngine;

public class GolfBallEvents : MonoBehaviour
{
    public delegate void SetStartPositionDelegate(float x, float z);
    public static event SetStartPositionDelegate OnSetStartPosition;

    public static void SetStartPosition(float x, float z)
    {
        OnSetStartPosition?.Invoke(x, z);
    }
}
