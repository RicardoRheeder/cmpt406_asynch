using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

//This script is partly from the RTSCamera script in the package "Virtual Cameras for Unity Lite"
public class CameraMovement : MonoBehaviour {

    [Tooltip("Zone that will receive on-screen cursor position.")]
    [Range(0f, 100f)]
    public float scrollZone = 65f;

    [Tooltip("Multiplier for camera movement sensitivity when using mouse.")]
    [Range(0f, 5f)]
    public float mouseSensitivity = 2f;

    [Tooltip("Multiplier for camera movement sensitivity when using input.")]
    [Range(1f, 100f)]
    public float inputSensitivity = 50f;

    [Tooltip("Multiplier for camera zoom sensitivity.")]
    [Range(0f, 300f)]
    public float zoomSensitivity = 40f;

    [Tooltip("Smoothing factor.")]
    [Range(0f, 10f)]
    public float smoothFactor = 0.2f;

    [Tooltip("Whether the camera scrolls using the cursor.")]
    public bool useCursor = true;

    [Tooltip("Maximum speed the camera can rotate")]
    public float maxRotationSpeed = 20f;
    
    [Space]
    [Header("--- Orthographic Mode ---")]

    //for zoom In/Out
    [Tooltip("Zoom limits in orthographic mode. X represents the lowest and Y the highest value.")]
    public Vector2 zoomLimits;

    [Space]
    [Header("--- Perspective Mode ---")]

    [Tooltip("Enable camera pitch rotation with zoom")]
    public bool enableCameraPitch;

    [Tooltip("Max pitch when zoomed all the way in.")]
    public float maxPitch = 60f;

    [Tooltip("Zoom limits in perspective mode. X represents the lowest and Y the highest value.")]
    public Vector2 perspectiveZoomLimits;

    [Tooltip("A target object for the camera to look at.")]
    public GameObject lookTarget;

    private Tilemap tilemap; // used to determine camera bounds
    private float zoom;
    private Vector2 rotationAnchorPoint;
    private Vector3 maxBound;
    private Vector3 minBound;

    private void Awake() {
        if (Camera.main.orthographic) {
            zoom = Camera.main.orthographicSize;
        }
        else {
            zoom = -15;
        }

        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        if(tilemap != null) {
            maxBound = tilemap.CellToWorld(tilemap.cellBounds.max);
            minBound = tilemap.CellToWorld(tilemap.cellBounds.min);
        }
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    void LateUpdate() {
        if(Input.GetMouseButtonDown(1)) {
            rotationAnchorPoint = Input.mousePosition;
        }
        HandleRotation();
        HandlePan();
        HandleZoom();
    }

    public void SnapToPosition(Vector2 position) {
        transform.position = position;
    }

    public void MoveToPosition(Vector2 position) {
        StartCoroutine(TransitionPosition(position));
    }

    IEnumerator TransitionPosition(Vector2 position) {
        float step = Time.fixedDeltaTime;
        float t = 0;
        Vector3 prevPos = transform.position;
        while (t <= 1.0f) {
            t += step;
            transform.position = Vector3.Lerp(prevPos, position, t);
            yield return new WaitForFixedUpdate();
        }
    }

    void HandlePan() {
        if (useCursor) {
            float percentOutsideScrollZone = 0f;

            if (Input.mousePosition.x < scrollZone) {   // pan left
                float mouseX = Input.mousePosition.x;
                if (mouseX < 0f) { mouseX = 0f; }
                percentOutsideScrollZone = scrollZone - mouseX;
                transform.Translate(-transform.right*Time.deltaTime*mouseSensitivity*percentOutsideScrollZone,Space.World);
            }
            else if (Input.mousePosition.x > Screen.width - scrollZone) { // pan right
                float mouseX = Input.mousePosition.x;
                if (mouseX > Screen.width) { mouseX = Screen.width; }
                percentOutsideScrollZone = mouseX - (Screen.width - scrollZone);
                transform.Translate(transform.right*Time.deltaTime*mouseSensitivity*percentOutsideScrollZone,Space.World);
            }

            if (Input.mousePosition.y < scrollZone) {   // pan down
                float mouseY = Input.mousePosition.y;
                if (mouseY < 0) { mouseY = 0f; }
                percentOutsideScrollZone = scrollZone - mouseY;
                transform.Translate(-transform.up*Time.deltaTime*mouseSensitivity*percentOutsideScrollZone,Space.World);
            }
            else if (Input.mousePosition.y > Screen.height - scrollZone) {    // pan up
                float mouseY = Input.mousePosition.y;
                if (mouseY > Screen.height) { mouseY = Screen.height; }
                percentOutsideScrollZone = mouseY - (Screen.height - scrollZone);
                transform.Translate(transform.up*Time.deltaTime*mouseSensitivity*percentOutsideScrollZone,Space.World);
            }
        }
        
        transform.Translate(transform.right*Input.GetAxis("Horizontal")*inputSensitivity*Time.deltaTime,Space.World);
        transform.Translate(transform.up*Input.GetAxis("Vertical")*inputSensitivity*Time.deltaTime,Space.World);
        
        if(tilemap != null) {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x,minBound.x,maxBound.x),Mathf.Clamp(transform.position.y,minBound.y,maxBound.y),transform.position.z);
        }
    }

    void HandleRotation() {
        if(useCursor && Input.GetMouseButton(1)) {
            float rotationSpeed = Math.Abs(Input.mousePosition.x - rotationAnchorPoint.x);
            if(rotationSpeed > maxRotationSpeed) {
                rotationSpeed = maxRotationSpeed;
            }
            if (Input.mousePosition.x < rotationAnchorPoint.x) {
                transform.Rotate(new Vector3(0,0,-rotationSpeed*Time.deltaTime*mouseSensitivity));
            }
            else if (Input.mousePosition.x > rotationAnchorPoint.x) {
                transform.Rotate(new Vector3(0,0,rotationSpeed*Time.deltaTime*mouseSensitivity));
            }
        }
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + Input.GetAxis("Rotate") * Time.deltaTime * 100f);
    }

    void HandleZoom() {
        // Orthographic mode:
        if (Camera.main.orthographic) {
            if (Input.mouseScrollDelta.y < 0) {
                zoom += zoomSensitivity * Time.deltaTime * Math.Abs(Input.mouseScrollDelta.y);
            }
            if (Input.mouseScrollDelta.y > 0)  {
                zoom -= zoomSensitivity * Time.deltaTime * Math.Abs(Input.mouseScrollDelta.y);
            }

            zoom = Mathf.Lerp(zoom, Camera.main.orthographicSize, smoothFactor);
            Camera.main.orthographicSize = Mathf.Clamp(zoom, zoomLimits.x, zoomLimits.y);
            zoom = Camera.main.orthographicSize;
        }
        // Perspective mode:
        else {
            if (Input.mouseScrollDelta.y > 0) {
                zoom += zoomSensitivity * Time.deltaTime * Math.Abs(Input.mouseScrollDelta.y);
            }
            if (Input.mouseScrollDelta.y < 0) {
                zoom -= zoomSensitivity * Time.deltaTime * Math.Abs(Input.mouseScrollDelta.y);
            }
            if (Time.timeSinceLevelLoad < 1f) {
                zoom -= zoomSensitivity * Time.deltaTime * 0.2f;
            }
            // Altitude
            zoom = Mathf.Lerp(zoom, Camera.main.transform.position.z, smoothFactor);
            Camera.main.transform.SetPositionAndRotation(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Mathf.Clamp(zoom, perspectiveZoomLimits.y, perspectiveZoomLimits.x)), Camera.main.transform.rotation);
            zoom = Camera.main.transform.position.z;
            // Pitch Rotation
            if (enableCameraPitch){
                Camera.main.transform.LookAt(lookTarget.transform, Vector3.back);
            }
        }
    }
}