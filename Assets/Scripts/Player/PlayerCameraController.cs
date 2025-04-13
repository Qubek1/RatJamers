using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] public Transform target; // Player to follow
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);


    private float _defaultOrthoSize;
    private Vector3 _defaultOffset;

    private Camera _camera;

   // private Vector2 _targetPos;

    private void Awake()
    {
        _camera=GetComponent<Camera>();
        _defaultOrthoSize=_camera.orthographicSize;
        _defaultOffset=offset;
    }

    void Update()
    {
        //if (target != null)
        //    _targetPos=target.position;

        Vector3 desiredPosition = target.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        //transform.position = smoothedPosition;
        transform.position = desiredPosition;
    }
    /*
    public void SetupForPlayer(PlayerController player)
    {
        if (player == PlayerController.Player1)
        {
            _camera.rect = new Rect(0, 0, 0.5f, 1); // Left half of the screen
        }
        else //player2
        {
            _camera.rect = new Rect(0.5f, 0, 0.5f, 1); // Right half of the screen
        }
    }
    */
    public void SetTarget(Transform newTarget)
    {
        //Debug.Log($"Setting {gameObject.name} camera target to {newTarget}");
        target = newTarget;
        transform.position= newTarget.position + offset;
    }

    public void SetConfig(MinigameCameraConfig config)
    {
        SetTarget(config.CameraTarget);
        offset = config.CameraOffset;
        _camera.orthographicSize = config.CameraSize;
    }

    /// <summary>
    /// restore the camera state to default follow player state
    /// </summary>
    public void ResetCamera()
    {
        _camera.orthographicSize= _defaultOrthoSize;
        offset = _defaultOffset;
    }
/*
    public void SetTarget(Vector2 pos)
    {
        target = null;
        _targetPos = pos;
    }
*/
}