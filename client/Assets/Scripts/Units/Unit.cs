using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The actual script on the unit that handles animations
public class Unit : MonoBehaviour {
    
    public Renderer rend;
    public float moveSpeed = 5f;

    Vector2Int currTilePosition;
    FogViewer fogViewer;

    private int currDirection = 0;

    
    void Awake() {
        rend = this.GetComponent<Renderer>();
        fogViewer = new FogViewer();
        transform.rotation = Quaternion.Euler(-90,0,0); 
    }

    //Method used to handle the attack animation
    public void Attack(int dir) {
        TurnToDirection(dir);
    }

    public void SetMoveSpeed(float speed) {
        moveSpeed = speed;
    }

    //Method used to handle the movement animation
    public void MoveAlongPath(List<Tuple<Vector2Int,int>> path, ref BoardController board) {
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

    IEnumerator PathMovement(List<Tuple<Vector2Int,int>> path, BoardController board) {
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
        }
    }

    IEnumerator RotateToDirection(int dir) {
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
    }

    public void Kill() {
        Destroy(this.gameObject);
    }

    public FogViewer GetFogViewer() {
        return fogViewer;
    }
}
