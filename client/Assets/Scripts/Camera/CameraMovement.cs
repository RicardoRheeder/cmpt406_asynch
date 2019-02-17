﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

//This script is partly from the RTSCamera script in the package "Virtual Cameras for Unity Lite"
public class CameraMovement : MonoBehaviour {

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

    //for zoom In/Out
    [Tooltip("Zoom limits . X represents the lowest and Y the highest value.")]
    public Vector2 zoomLimits;

    [Tooltip("Whether the position changes using the cursor.")]
    public bool useCursor = true;

    [Tooltip("Maximum speed the camera can rotate")]
    public float maxRotationSpeed = 20f;

    [Tooltip("The tilemap used to determine camera bounds")]
    public Tilemap tilemap;

    private float zoom;
    private Vector2 rotationAnchorPoint;

    private void Awake() {
        zoom = Camera.main.orthographicSize;
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
        if (useCursor) {
            float percentOutsideScrollZone = 0f;

            if (Input.mousePosition.x < scrollZone) {   // pan left
                percentOutsideScrollZone = scrollZone - Input.mousePosition.x;
                transform.Translate(-transform.right*Time.deltaTime*sensitivity*percentOutsideScrollZone,Space.World);
            } else if (Input.mousePosition.x > Screen.width - scrollZone) { // pan right
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
            transform.Translate(transform.right*Input.GetAxis("Horizontal")*sensitivity*Time.deltaTime,Space.World);
            transform.Translate(transform.up*Input.GetAxis("Vertical")*sensitivity*Time.deltaTime,Space.World);
        }

        if(tilemap != null) {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x,tilemap.localBounds.min.x,tilemap.localBounds.max.x),transform.position.y,Mathf.Clamp(transform.position.z,tilemap.localBounds.min.y,tilemap.localBounds.max.y));
        }
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