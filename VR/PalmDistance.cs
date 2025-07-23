using System;
using System.Collections.Generic;
#if OPENXR_AVAILABLE
using UnityEngine.XR.OpenXR;
#endif
using UnityEngine.XR.Hands;
using UnityEngine;

/// <summary>
/// This script detects the distance between the palms of both hands using XR Hands and
/// fires an event when they are close. Subscribe to the `OnPalmsNear` event.
/// </summary>

public class PalmDistance : MonoBehaviour
{
    public event Action<bool> OnPalmsNear;
    [SerializeField] float _palmDistanceThreshold = 0.25f;
    XRHandSubsystem _handSubsystem;
    bool _previousPalmsNearState = false;

    
    void Start()
    {
        List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);
        if (handSubsystems.Count > 0)
            _handSubsystem = handSubsystems[0];
    }
    
    void Update()
    {
        Vector3 leftPalmPosition = Vector3.zero;
        Vector3 rightPalmPosition = Vector3.zero;
        
        if (_handSubsystem != null && _handSubsystem.running)
        {
            XRHand leftHand = _handSubsystem.leftHand;
            if (leftHand.isTracked)
            {
                XRHandJoint palmJoint = leftHand.GetJoint(XRHandJointID.Palm);
                if (palmJoint.TryGetPose(out Pose palmPose))
                {
                    leftPalmPosition = palmPose.position;
                }
            }
            
            XRHand rightHand = _handSubsystem.rightHand;
            if (rightHand.isTracked)
            {
                XRHandJoint palmJoint = rightHand.GetJoint(XRHandJointID.Palm);
                if (palmJoint.TryGetPose(out Pose palmPose))
                {
                    rightPalmPosition = palmPose.position;
                }
            }

            if (leftHand.isTracked && rightHand.isTracked)
            {
                bool currentPalmsNearState = Vector3.Distance(leftPalmPosition, rightPalmPosition) < _palmDistanceThreshold;
                
                if (_previousPalmsNearState != currentPalmsNearState)
                {
                    OnPalmsNear?.Invoke(currentPalmsNearState);
                    _previousPalmsNearState = currentPalmsNearState;
                }
            }
        }
    }
}
