using Fusion;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    private Vector2 viewInput;

    // Rotation
    float cameraRotationX = 0;
    float cameraRotationY = 0;

    //Other components
    NetworkCharacterControllerCustom networkCharacterControllerCustom;
    Camera localCamera;

    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerCustom = GetComponentInParent<NetworkCharacterControllerCustom>();
    }

    private void Start()
    {
        if (localCamera.enabled)
        {
            localCamera.transform.parent = null;
        }
    }

    private void LateUpdate()
    {
        if (cameraAnchorPoint == null)
        {
            return;
        }

        if (!localCamera.enabled)
        {
            return;
        }

        localCamera.transform.position = cameraAnchorPoint.position;

        // Calculate rotation
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterControllerCustom.rotationSpeed;

        // Apply rotation
        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }

    public void SetViewInputVector(Vector2 viewInout)
    {
        this.viewInput = viewInout;
    }
}
