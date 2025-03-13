using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private Cinemachine3rdPersonFollow camBody;
    public GameObject Target;

    public Vector2 look; // input value
    public float zoom; // input value
    public float scrollSpeed = 1f; // zoom scroll speed
    public bool camLock; // cam rotation lock check value

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    
    //Camera rotation or zoom setting value
    public float minCamDist = 8f;
    public float maxCamDist = 14f;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;

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

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    public void OnZoom(InputValue value)
    {
        zoom = value.Get<float>();
        float val = zoom * scrollSpeed + camBody.CameraDistance;
        if(val < maxCamDist && val > minCamDist)
        {
            camBody.CameraDistance += zoom * scrollSpeed;
        }
    }

    public void OnCamLock(InputValue value)
    {
        if (value.isPressed)
        {
            PlayerController.instance.camState = CamState.Unlock;
            camLock = false;
        }
        else
        {
            PlayerController.instance.camState = CamState.Lock;
            camLock = true;
        }
    }

    private void CameraRotation()
    {
        if (camLock) return;
        //Debug.Log("CamRotation start");
        // if there is an input and camera position is not fixed
        if (look.sqrMagnitude >= _threshold)
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
