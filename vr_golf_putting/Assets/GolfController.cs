using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GolfController : MonoBehaviour
{
    private InputDevice rightHandDevice;
    private bool triggerHeld = false;

    public Rigidbody golfBall; // Assign your GolfBall Rigidbody here in Inspector
    public float shotPowerMultiplier = 2.0f; // Adjust how powerful your putt is

    void Start()
    {
        // Find the right-hand controller
        List<InputDevice> rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

        if (rightHandDevices.Count > 0)
        {
            rightHandDevice = rightHandDevices[0];
            Debug.Log("Right hand controller detected.");
        }
        else
        {
            Debug.LogWarning("No right hand controller found!");
        }
    }

    void Update()
    {
        if (!rightHandDevice.isValid)
            return;

        bool triggerButtonPressed;
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButtonPressed))
        {
            if (triggerButtonPressed && !triggerHeld)
            {
                // Just started holding trigger
                triggerHeld = true;
                Debug.Log("Trigger Held: Preparing Swing");
            }
            else if (!triggerButtonPressed && triggerHeld)
            {
                // Just released trigger
                triggerHeld = false;
                Debug.Log("Trigger Released: Swing!");

                Vector3 controllerVelocity;
                if (rightHandDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out controllerVelocity))
                {
                    Debug.Log("Controller velocity at release: " + controllerVelocity);

                    if (golfBall != null)
                    {
                        // Apply force to the ball
                        golfBall.linearVelocity = controllerVelocity * shotPowerMultiplier;
                    }
                }
            }
        }
    }
}