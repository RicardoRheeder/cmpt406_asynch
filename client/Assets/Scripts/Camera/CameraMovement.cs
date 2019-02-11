using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is mostly from the RTSCamera script in the package "Virtual Cameras for Unity Lite"

//Attach this script to the Camera object
public class CameraMovement : MonoBehaviour
{

    [Header("Camera")]
    [Tooltip("Zone that will receive on-screen cursor position.")]
    [Range(0f, 100f)]
    public float scrollZone = 100f;

    [Tooltip("Multiplier for camera sensitivity.")]
    [Range(0f, 100)]
    public float sensitivity = 50f;

    [Tooltip("Smoothing factor.")]
    [Range(0f, 10f)]
    public float smoothFactor = 0.2f;


    //Camera Bounds begin

    //For left/right movement
    [Tooltip("Movement limits on the X-axis. X represents the lowest and Y the highest value.")]
    public Vector2 moveLimitsX;

    //For up/down movement
    [Tooltip("Movement limits on the Y-axis. X represents the lowest and Y the highest value.")]
    public Vector2 moveLimitsY;

    //for zoom In/Out
    [Tooltip("Zoom limits . X represents the lowest and Y the highest value.")]
    public Vector2 zoomLimits;



    //Since we dont need Z movement for orthographic camera
    //[Tooltip("Movement limits on the Z-axis. X represents the lowest and Y the highest value.")]
    //public Vector2 moveLimitsZ;

    //Camera Bounds end


    [Tooltip("Whether the position changes using the cursor.")]
    public bool useCursor = true;

    private Vector3 desiredPosition;
    private Vector3 currentPosition;

    private float zoom = 50f;

    // public float mouseRotationSpeed = 100f;

    private void Awake()
    {
        zoom = Camera.main.orthographicSize;
    }


    // Use this for initialization
    private void Start()
    {
        

        desiredPosition = transform.position;
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    void LateUpdate()
    {

        //***DO Not Know how to rotate with the orthgraphic camera*****
        //This is for roation with prespective camera
        //When right mouse button is pressed, Rotate camera left/right if mouse is moved left/right
        /*   if (Input.GetKey(KeyCode.Mouse1)) {
               transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * mouseRotationSpeed, Space.World); // -Input.GetAxis("Mouse X"): remove minus to invert rotation
           }

           else


           //**********************
           */


        if (useCursor)
        {
            currentPosition = Vector3.zero;

            if (Input.mousePosition.x < scrollZone)
            {
                currentPosition.x -= sensitivity * Time.deltaTime;
            }
            else if (Input.mousePosition.x > Screen.width - scrollZone)
            {
                currentPosition.x = sensitivity * Time.deltaTime;
            }

            if (Input.mousePosition.y < scrollZone)
            {
                currentPosition.y -= sensitivity * Time.deltaTime;
            }
            else if (Input.mousePosition.y > Screen.height - scrollZone)
            {
                currentPosition.y = sensitivity * Time.deltaTime;
            }
        }
        else
        {
            currentPosition.x = Input.GetAxis("Horizontal") * sensitivity * Time.deltaTime;
            currentPosition.y = Input.GetAxis("Vertical") * sensitivity * Time.deltaTime;
        }
        //-Input.GetAxis("Mouse ScrollWheel")
        //Zoom In/Out based on mouse wheel scroll by changing camera's Y position
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += sensitivity * Time.deltaTime;
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            zoom -= sensitivity * Time.deltaTime;
        }

        // Zoom In/ Out based on mouse wheel scroll by changing camera's orthographic size

        Camera.main.orthographicSize = Mathf.Clamp(zoom, zoomLimits.x, zoomLimits.y); ;

        //zoom for prepective camera
        //Zoom In/Out based on mouse wheel scroll by changing camera's Y position
        // currentPosition.y = -Input.mouseScrollDelta.y * sensitivity * Time.deltaTime;


        Vector3 move = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z) + desiredPosition;
        move.x = Mathf.Clamp(move.x, moveLimitsX.x, moveLimitsX.y);
        move.y = Mathf.Clamp(move.y, moveLimitsY.x, moveLimitsY.y);
        // move.z = Mathf.Clamp(move.z, moveLimitsX.x, moveLimitsX.y);
        desiredPosition = move;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothFactor);
    }
}