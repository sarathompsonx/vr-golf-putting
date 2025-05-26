using System;
using UnityEngine;

public class GolfHole : MonoBehaviour
{
    void OnEnable()
    {
        GolfHoleEvents.OnSetPosition += SetPosition;
    }

    private void OnDisable()
    {
        GolfHoleEvents.OnSetPosition -= SetPosition;
    }
    
    private void SetPosition(float posX, float posZ)
    {   
        Debug.Log($"pos X = {posX}, Z = {posZ}");
        Vector3 currPos = transform.position;
        Vector3 updatedPos = new Vector3(posX, currPos.y, posZ);
        transform.position = updatedPos;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
