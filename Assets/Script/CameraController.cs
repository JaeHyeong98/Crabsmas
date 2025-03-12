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
    public float scrollSpeed = 1f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    private float minCamDist = 8f;
    private float maxCamDist = 14f;


    public bool LockCameraPosition = false;

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
        if(val < 14f && val > 8f)
        {
            camBody.CameraDistance += zoom * scrollSpeed;
        }
    }

    private void CameraRotation()
    {
        //Debug.Log("CamRotation start");
        // if there is an input and camera position is not fixed
        if (look.sqrMagnitude >= _threshold && !LockCameraPosition)
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
