using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraPivotTransform;

    private Transform playerTransform;
    private Vector3 cameraTransformPosition;
    private LayerMask ignoredLayers;

    public static CameraHandler Singltone;

    [SerializeField] private float lookSpeed = 0.1f;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private float pivotSpeed = 0.01f;

    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;

    [SerializeField] private float minPivot = -40f;
    [SerializeField] private float maxPivot = 40f;

    private void Awake()
    {
        Singltone = this;
        playerTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void FollowTarget(float delta)
    {
        Vector3 target = Vector3.SmoothDamp(playerTransform.position, targetTransform.position, ref cameraTransformPosition, delta / followSpeed);
        playerTransform.position = target;
    }

    public void HandleCamera(float delta, float mouseInputX, float mouseInputY)
    {
        lookAngle += mouseInputX * lookSpeed / delta;
        pivotAngle -= mouseInputY * pivotSpeed / delta;
        pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;

        Quaternion targetRotation = Quaternion.Euler(rotation);
        playerTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }
}
