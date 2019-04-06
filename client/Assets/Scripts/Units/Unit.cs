using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The actual script on the unit that handles animations
public class Unit : MonoBehaviour {
    
    public SkinnedMeshRenderer rend;
    public float moveSpeed = 0.5f;
    Animator anim;
    public bool isWalking;

    Vector2Int currTilePosition;
    FogViewer fogViewer;
    UnitOutline unitOutline;

    private int currDirection = 0;
    public float currRotation = 0f;

    private static readonly float Y_ROTATION_CONST = 90f;
    private static readonly float Z_ROTATION_CONST = -90f;
    
    void Awake() {
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();
        unitOutline = GetComponent<UnitOutline>();
        fogViewer = new FogViewer();
        transform.rotation = Quaternion.Euler(-90,0,0); 

        if(unitOutline != null) {
            unitOutline.enabled = false;
        }
    }

    //Method used to handle the attack animation
    public void Attack(Vector3 targetWorldPos, UnitType type, AudioManager manager = null) {
        if (manager != null) {
            manager.Play(type, SoundType.Attack); //plays the attack sound
        }
        //TurnToDirection(dir);
        if(this.anim != null) {
            anim.SetTrigger("attack");
        }
    }

    public void GetHit() {
        if(this.anim != null) {
            anim.SetTrigger("gothit");
        }
    }

    public void SetMoveSpeed(float speed) {
        moveSpeed = speed;
    }

    //Method used to handle the movement animation
    public void MoveAlongPath(List<Tuple<Vector2Int,int>> path, ref BoardController board, AudioManager manager = null) {
        isWalking = true;
        StartCoroutine(PathMovement(path,board));
    }

    public void PlaceAt(Vector2Int position, ref BoardController board) {
        currTilePosition = position;
        transform.position = board.CellToWorld(position);
        fogViewer.SetPosition(position);
    }

    public void TurnToDirection(int dir) {
        int clampedDir = Mathf.Clamp(dir,0,5);
        StartCoroutine(RotateToDirection(dir));
    }

    // Turns unit to direction without animation. Should be used during game building
    public void SnapToDirection(int dir) {
        int prevDir = currDirection;
        currDirection = dir;
        int angle = HexUtility.DirectionToAngle(currDirection) - HexUtility.DirectionToAngle(prevDir);
        transform.rotation = Quaternion.AngleAxis(angle,transform.up)*transform.rotation;
    }

    public void SnapToAngle(int angle) {
        currDirection = angle % 60;
        currRotation = angle;
        transform.rotation = Quaternion.Euler((float)angle, Y_ROTATION_CONST, Z_ROTATION_CONST);
    }

    public void OutlineUnit() {
        if(unitOutline != null) {
            unitOutline.enabled = true;
        }
    }

    public void HideUnitOutline() {
        if(unitOutline != null) {
            unitOutline.enabled = false;
        }
    }

    IEnumerator PathMovement(List<Tuple<Vector2Int,int>> path, BoardController board) {
        if(this.anim != null) {
            anim.SetBool("walking",true);
        }
        float step = moveSpeed * Time.fixedDeltaTime;
        float t = 0;
        Vector3 prevPos = transform.position;
        Vector2Int prevTilePos = currTilePosition;
        int prevDir = currDirection;
        Quaternion prevRotation = transform.rotation;
        for(int i = 0; i < path.Count; i++) {
            currTilePosition = path[i].First;
            currDirection = path[i].Second;
            fogViewer.SetPosition(currTilePosition);
            Vector3 worldPos = board.CellToWorld(currTilePosition);
            int angle = HexUtility.DirectionToAngle(currDirection) - HexUtility.DirectionToAngle(prevDir);
            Quaternion rot = Quaternion.AngleAxis(angle,transform.up)*transform.rotation;
            t = 0;
            while (t <= 1.0f) {
                t += step;
                transform.position = Vector3.Lerp(prevPos, worldPos, t);
                transform.rotation = Quaternion.Lerp(prevRotation, rot, t);
                yield return new WaitForFixedUpdate();
            }
            prevPos = worldPos;
            prevTilePos = currTilePosition;
            prevDir = currDirection;
            prevRotation = transform.rotation;
            transform.position = worldPos;
            if(this.anim != null) {
                anim.SetBool("walking",false);
            }
        }
        isWalking = false;
    }

    IEnumerator RotateToDirection(int dir) {
        if(this.anim != null) {
            anim.SetBool("walking",true);
        }
        float step = moveSpeed * Time.fixedDeltaTime;
        float t = 0;
        int prevDir = currDirection;
        currDirection = dir;
        Quaternion prevRotation = transform.rotation;
        int angle = HexUtility.DirectionToAngle(currDirection) - HexUtility.DirectionToAngle(prevDir);
        Quaternion rot = Quaternion.AngleAxis(angle,transform.up)*transform.rotation;
        while (t <= 1.0f) {
            t += step;
            transform.rotation = Quaternion.Lerp(prevRotation, rot, t);
            yield return new WaitForFixedUpdate();
        }
        if(this.anim != null) {
            anim.SetBool("walking",false);
        }
    }

    IEnumerator RotateToDirection(Vector3 worldPos) {
        if (this.anim != null) {
            anim.SetBool("walking", true);
        }
        float step = moveSpeed * Time.fixedDeltaTime;
        float t = 0;
        Quaternion prevRotation = transform.rotation;
        Vector3 direction = worldPos - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.back);
        while (t <= 1.0f) {
            t += step;
            transform.rotation = Quaternion.Lerp(prevRotation, toRotation, t);
            yield return new WaitForFixedUpdate();
        }
        if (this.anim != null) {
            anim.SetBool("walking", false);
        }
    }

    public void Kill(UnitType type, AudioManager manager = null) {
        if(this.anim != null) {
            anim.SetTrigger("death");
        }
        if (manager != null) {
            manager.Play(type, SoundType.Death, isVoice: true);
            manager.Play(type, SoundType.Death);
        }
        Destroy(this.gameObject);
    }

    public FogViewer GetFogViewer() {
        return fogViewer;
    }

    public void FaceDirection(Vector3 worldPos) {
        worldPos.z = transform.position.z;
        StartCoroutine(RotateToDirection(worldPos));
        this.currRotation = transform.rotation.x % 360;
    }
}
