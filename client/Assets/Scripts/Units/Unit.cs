using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The actual script on the unit that handles animations
public class Unit : MonoBehaviour {
    
    public Renderer rend;
    public float moveSpeed = 5f;

    Vector2Int currTilePosition;
    FogViewer fogViewer;

    public int direction = 0;

    
	void Awake() {
		rend = this.GetComponent<Renderer>();
        fogViewer = new FogViewer();
        transform.rotation = Quaternion.Euler(-90,0,0); 
	}

    //Method used to handle the attack animation
    public void Attack() {

    }

    public void SetMoveSpeed(float speed) {
        moveSpeed = speed;
    }

    //Method used to handle the movement animation
    public void MoveTo(Vector2Int endPosition, ref BoardController board) {
        List<Vector2Int> path = HexUtility.Pathfinding(currTilePosition,endPosition,board.GetTilemap(),false);
        StartCoroutine(PathMovement(path, board));
    }

    public void PlaceAt(Vector2Int position, ref BoardController board) {
        currTilePosition = position;
        transform.position = board.CellToWorld(position);
        fogViewer.SetPosition(position);
    }

    public void RotateToDirection(int dir) {
        int clampedDir = Mathf.Clamp(dir,0,5);
    }

    IEnumerator PathMovement(List<Vector2Int> path, BoardController board) {
         float step = moveSpeed * Time.fixedDeltaTime;
         float t = 0;
         Vector3 prevPos = transform.position;
         Vector2Int prevTilePos = currTilePosition;
         int prevDir = direction;
         Quaternion prevRotation = transform.rotation;
         for(int i = 0; i < path.Count; i++) {
            currTilePosition = path[i];
            fogViewer.SetPosition(currTilePosition);
            Vector3 worldPos = board.CellToWorld(currTilePosition);
            direction = HexUtility.FindDirection(prevTilePos,currTilePosition);
            Quaternion rot = Quaternion.AngleAxis(60*(prevDir - direction),transform.up)*transform.rotation;
            t = 0;
            while (t <= 1.0f) {
                t += step;
                transform.position = Vector3.Lerp(prevPos, worldPos, t);
                transform.rotation = Quaternion.Lerp(prevRotation, rot, t);
                yield return new WaitForFixedUpdate();
            }
            prevPos = worldPos;
            prevTilePos = currTilePosition;
            prevDir = direction;
            prevRotation = transform.rotation;
            transform.position = worldPos;
        }
    }

    public void Kill() {
        Destroy(this.gameObject);
    }

    public FogViewer GetFogViewer() {
        return fogViewer;
    }
}
