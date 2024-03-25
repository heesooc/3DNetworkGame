using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotateAbility : CharacterAbility
{
    // 목표: 마우스 이동에 따라 카메라와 플레이어를 회전하고 싶다.
    public Transform CameraRoot;

    private float _mx; // 각도
    private float _my;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        // 순서:
        // 1. 마우스 입력 값을 받는다.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. 회전 값을 마우스 입력에 따라 미리 누적한다.
        _mx += mouseX * Owner.Stat.RotationSpeed * Time.deltaTime;
        _my += mouseY * Owner.Stat.RotationSpeed * Time.deltaTime;
        _my = Mathf.Clamp(_my, -90f, 90f);

        // 3. 카메라(3인칭)와 캐릭터를 회전 방향으로 회전시킨다. 
        transform.eulerAngles = new Vector3(0, _mx, 0f); // X와 Z 축은 0으로 고정, 회전은 Y축을 중심으로만 
        CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0f); // "로컬"은 부모 오브젝트의 회전을 기준으로 한다
                                                                // -_my: 마우스를 위로 움직일 때 카메라가 아래를 향하게 

        // 4. 시네머신- virtual 카메라

    }
}
