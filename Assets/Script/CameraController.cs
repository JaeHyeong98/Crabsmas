using Cinemachine;
using Main;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private Cinemachine3rdPersonFollow camBody;
    public GameObject Target;

    //cam
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    
    //Camera rotation or zoom setting value
    public float minCamDist = 8f;
    public float maxCamDist = 20f;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public float scrollSpeed = 1f; // zoom scroll speed

    public CamState camState;

    private void Awake()
    {
        GSC.cameraController = this;
    }

    void Start()
    {
        _cinemachineTargetYaw = Target.transform.rotation.eulerAngles.y;

        Vector3 angles = transform.eulerAngles;
        camBody = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    public void ZoomFn(float zoom)
    {
        float val = zoom * scrollSpeed + camBody.CameraDistance;
        if (val < maxCamDist && val > minCamDist)
        {
            camBody.CameraDistance += zoom * scrollSpeed;
        }
    }

    public void CamLockUp(bool value)
    {
        if (value)
        {
            camState = CamState.Unlock;
        }
        else
        {
            camState = CamState.Lock;
        }
    }

    public void SetTarget(Transform target)
    {
        virtualCamera.Follow = target;
    }

    private void CameraRotation()
    {
        if (camState == CamState.Lock)
            return;
        
        //Debug.Log("CamRotation start");
        // if there is an input and camera position is not fixed
        if (GSC.inputController.look.sqrMagnitude >= _threshold)
        {
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += Input.GetAxis("Mouse X") * deltaTimeMultiplier;
            _cinemachineTargetPitch += Input.GetAxis("Mouse Y") * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        Target.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}

public enum CamState
{
    Lock,
    Unlock
}
