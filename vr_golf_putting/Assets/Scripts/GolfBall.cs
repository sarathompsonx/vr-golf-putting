using UnityEngine;

public class GolfBall : MonoBehaviour
{

    Vector3 startPosition;

    void OnEnable()
    {
        GolfBallEvents.OnSetStartPosition += SetStartPosition;
    }

    void OnDisable()
    {
        GolfBallEvents.OnSetStartPosition -= SetStartPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetStartPosition(float x, float z)
    {
        Vector3 currPos = transform.position;
        Vector3 position = new Vector3(x, currPos.y, z);
        transform.position = position;
        
        Debug.Log("Golf ball position set to: " + position);
    }

}
