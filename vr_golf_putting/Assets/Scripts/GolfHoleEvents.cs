using UnityEngine;

public class GolfHoleEvents : MonoBehaviour
{
    public delegate void SetPosition(float posX, float posZ);
    public static event SetPosition OnSetPosition;

    public static void SetTargetPosition(float posX, float posZ)
    {
        OnSetPosition?.Invoke(posX, posZ);
    }
}
