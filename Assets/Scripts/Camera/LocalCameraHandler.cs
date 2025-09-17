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
    NetworkCharacterController networkCharacterController;
    public Camera localCamera;

    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterController = GetComponentInParent<NetworkCharacterController>();
    }

    private void Start()
    {
        cameraRotationX = GameManager.instance.cameraViewRotation.x;
        cameraRotationY = GameManager.instance.cameraViewRotation.y;
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
        cameraRotationX += viewInput.y * Time.deltaTime * 40;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterController.rotationSpeed;

        // Apply rotation
        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }

    public void SetViewInputVector(Vector2 viewInout)
    {
        this.viewInput = viewInout;
    }

    private void OnDestroy()
    {
        if (cameraRotationX != 0 && cameraRotationY != 0)
        {
            GameManager.instance.cameraViewRotation.x = cameraRotationX;
            GameManager.instance.cameraViewRotation.y = cameraRotationY;           
        }
    }
}
