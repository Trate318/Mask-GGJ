using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanningCamera : MonoBehaviour
{
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float lookSens;
    [SerializeField] private Transform camTransform;
    [SerializeField] private CameraPoint[] points; //nothing0 //camera1 // phone2
    private LookingAt currentPoint = LookingAt.Nothing;
    private float camMovement;

    private bool changingView;

    private void Start()
    {
        camTransform.position = points[0].camPos.position;
        camTransform.rotation = points[0].camPos.rotation;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (changingView || !ctx.started) return;

        Vector2 moveDir = ctx.ReadValue<Vector2>();
        
        switch (currentPoint)
        {
            case LookingAt.Computer:
                if (moveDir.x < -.5)
                {
                    StartCoroutine(LerpToCameraPoint(points[2], .5f));
                    currentPoint = LookingAt.Phone;
                }

                if (moveDir.y < -.5)
                {
                    StartCoroutine(LerpToCameraPoint(points[0], .5f));
                    currentPoint = LookingAt.Nothing;
                }
                break;
            case LookingAt.Phone:
                if (moveDir.x > .5)
                {
                    StartCoroutine(LerpToCameraPoint(points[1], .5f));
                    currentPoint = LookingAt.Computer;
                }

                if (moveDir.y < -.5)
                {
                    StartCoroutine(LerpToCameraPoint(points[0], .5f));
                    currentPoint = LookingAt.Nothing;
                }
                break;
            case LookingAt.Nothing:
                if (moveDir.x < -.5)
                {
                    StartCoroutine(LerpToCameraPoint(points[2], .5f));
                    currentPoint = LookingAt.Phone;
                }

                if (moveDir.y > .5 || moveDir.x > .5)
                {
                    StartCoroutine(LerpToCameraPoint(points[1], .5f));
                    currentPoint = LookingAt.Computer;
                }
                break;
        }
    }

    public void OnSpacePressed(InputAction.CallbackContext ctx) 
    {
        if (ctx.started)
        {
            StartCoroutine(LerpToCameraPoint(points[0], .5f));
        }
    }

    private void LateUpdate()
    {
        MoveCamera();
    }

    IEnumerator LerpToCameraPoint(CameraPoint point, float duration)
    {
        changingView = true;
        Camera cam = camTransform.GetComponent<Camera>();
        Vector3 pos = camTransform.position;
        Quaternion rot = camTransform.rotation;
        float fov = cam.fieldOfView;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            camTransform.position = Vector3.Lerp(pos, point.camPos.position, t);
            camTransform.rotation = Quaternion.Slerp(rot, point.camPos.rotation, t);
            cam.fieldOfView = Mathf.Lerp(fov, point.fov, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camTransform.position = point.camPos.position;
        camTransform.rotation = point.camPos.rotation;
        cam.fieldOfView = point.fov;
        changingView = false;
    }

    private void MoveCamera()
    {
        
    }
    
    [Serializable]
    private struct CameraPoint
    {
        public Transform camPos;
        public float fov;
    }

    private enum LookingAt
    {
        Computer,
        Nothing,
        Phone,
    }
    
    
}