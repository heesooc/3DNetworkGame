using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance {  get; private set; }

    public CinemachineVirtualCamera followCamera;
    public CinemachineVirtualCamera rotationCamera;

    public Transform lookAtTarget;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        // 현재 활성화된 카메라의 우선순위를 변경하여 전환
        if (followCamera.Priority > rotationCamera.Priority)
        {
           
            followCamera.Priority = 0;
            rotationCamera.Priority = 10;
            //rotationCamera.LookAt = lookAtTarget;
            rotationCamera.Follow = lookAtTarget;
        }
        else
        {
            followCamera.Priority = 10;
            rotationCamera.Priority = 0;
            //followCamera.LookAt = lookAtTarget;
            followCamera.Follow = lookAtTarget;
        }
    }
}
