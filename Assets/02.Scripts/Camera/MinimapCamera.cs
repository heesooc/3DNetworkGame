using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private Transform _target;
    public float YDistance = 20f;

    //private Vector3 _initialEulerAngles; // 처음의, 초기

    /*private void Start()
    {
        _initialEulerAngles = transform.eulerAngles;
    }*/

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 targetPosition = _target.position;
        targetPosition.y += YDistance;

        transform.position = targetPosition;

        /* Vector3 targetEulerAngles = _target.eulerAngles;
         targetEulerAngles.x = _initialEulerAngles.x;
         targetEulerAngles.z = _initialEulerAngles.z;

         transform.eulerAngles = targetEulerAngles;*/

        Vector3 targetEulerAngles = new Vector3(90f, _target.eulerAngles.y, 0f);
        transform.eulerAngles = targetEulerAngles;
    }
}
