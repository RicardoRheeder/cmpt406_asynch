using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

//This script is partly from the RTSCamera script in the package "Virtual Cameras for Unity Lite"
public class CameraMovement : MonoBehaviour
{

    [Header("Camera")]
    [Tooltip("Zone that will receive on-screen cursor position.")]
    [Range(0f, 100f)]
    public float scrollZone = 100f;

    [Tooltip("Multiplier for camera movement sensitivity.")]
    [Range(0f, 2f)]
    public float sensitivity = 1f;

    [Tooltip("Multiplier for camera zoom sensitivity.")]
    [Range(0f, 20f)]
    public float zoomSensitivity = 10f;

    [Tooltip("Smoothing factor.")]
    [Range(0f, 10f)]
    public float smoothFactor = 0.2f;

    //For left/right movement
    [Tooltip("Movement limits on the X-axis. X represents the lowest and Y the highest value.")]
    public Vector2 moveLimitsX;

    //For up/down movement
    [Tooltip("Movement limits on the Y-axis. X represents the lowest and Y the highest value.")]
    public Vector2 moveLimitsY;

    //for zoom In/Out
    [Tooltip("Zoom limits . X represents the lowest and Y the highest value.")]
    public Vector2 zoomLimits;

    [Tooltip("Whether the position changes using the cursor.")]
    public bool useCursor = true;

    [Tooltip("Maximum speed the camera can rotate")]
    public float maxRotationSpeed = 20f;

    private Vector3 desiredPosition;
    private Vector3 currentPosition;
    private float zoom;
    private Vector2 rotationAnchorPoint;

    public Tilemap tilemap;

    private void Awake() {
        zoom = Camera.main.orthographicSize;
    }

    // Use this for initialization
    private void Start() {
        desiredPosition = transform.position;
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    void LateUpdate() {
        if(Input.GetMouseButtonDown(1)) {
            rotationAnchorPoint = Input.mousePosition;
        }

        if(Input.GetMouseButton(1)) {
            HandleRotation();
        } else {
            HandlePan();
            HandleZoom();
        }
    }

    void HandlePan() {
        Vector3 tilemapMin = (transform.rotation * Quaternion.Euler(-90,0,0)) * new Vector3(tilemap.localBounds.min.x,0,tilemap.localBounds.min.y);
        Vector3 tilemapMax = (transform.rotation * Quaternion.Euler(-90,0,0)) * new Vector3(tilemap.localBounds.max.x,0,tilemap.localBounds.max.y);

        if (useCursor) {
            currentPosition = Vector3.zero;
            float percentOutsideScrollZone = 0f;

            if (Input.mousePosition.x < scrollZone) {   // pan left
                percentOutsideScrollZone = scrollZone - Input.mousePosition.x;
                transform.Translate(-transform.right*Time.deltaTime*sensitivity*percentOutsideScrollZone,Space.World);
            } else if (Input.mousePosition.x > Screen.width - scrollZone && transform.position.x < tilemapMax.x) { // pan right
                percentOutsideScrollZone = Input.mousePosition.x - (Screen.width - scrollZone);
                transform.Translate(transform.right*Time.deltaTime*sensitivity*percentOutsideScrollZone,Space.World);
            }

            if (Input.mousePosition.y < scrollZone) {   // pan down
                percentOutsideScrollZone = scrollZone - Input.mousePosition.y;
                transform.Translate(-transform.up*Time.deltaTime*sensitivity*percentOutsideScrollZone,Space.World);
            } else if (Input.mousePosition.y > Screen.height - scrollZone) {    // pan up
                percentOutsideScrollZone = Input.mousePosition.y - (Screen.height - scrollZone);
                transform.Translate(transform.up*Time.deltaTime*sensitivity*percentOutsideScrollZone,Space.World);
            }
        } else {
            currentPosition.x = Input.GetAxis("Horizontal") * sensitivity * Time.deltaTime;
            currentPosition.y = Input.GetAxis("Vertical") * sensitivity * Time.deltaTime;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x,tilemap.localBounds.min.x,tilemap.localBounds.max.x),transform.position.y,Mathf.Clamp(transform.position.z,tilemap.localBounds.min.y,tilemap.localBounds.max.y));

        Vector3 move = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z) + desiredPosition;
        move.x = Mathf.Clamp(move.x, tilemap.localBounds.min.x, tilemap.localBounds.max.x);
        move.y = Mathf.Clamp(move.y, tilemap.localBounds.min.y, tilemap.localBounds.max.y);

        // desiredPosition = move;
        // transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothFactor);
    }

    void HandleRotation() {
        float rotationSpeed = Math.Abs(Input.mousePosition.x - rotationAnchorPoint.x);
        if(rotationSpeed > maxRotationSpeed) {
            rotationSpeed = maxRotationSpeed;
        }

        if (Input.mousePosition.x < rotationAnchorPoint.x) {
            transform.Rotate(new Vector3(0,0,-rotationSpeed*Time.deltaTime*sensitivity));
        } else if (Input.mousePosition.x > rotationAnchorPoint.x) {
            transform.Rotate(new Vector3(0,0,rotationSpeed*Time.deltaTime*sensitivity));
        }
    }

    void HandleZoom() {
        if (Input.mouseScrollDelta.y < 0) {
            zoom += zoomSensitivity * Time.deltaTime * Math.Abs(Input.mouseScrollDelta.y);
        }

        if (Input.mouseScrollDelta.y > 0) {
            zoom -= zoomSensitivity * Time.deltaTime * Math.Abs(Input.mouseScrollDelta.y);
        }

        zoom = Mathf.Lerp(zoom,Camera.main.orthographicSize,smoothFactor);
        Camera.main.orthographicSize = Mathf.Clamp(zoom, zoomLimits.x, zoomLimits.y);
        zoom = Camera.main.orthographicSize;
    }
}