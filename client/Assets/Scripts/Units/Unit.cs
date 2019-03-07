using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The actual script on the unit that handles animations
public class Unit : MonoBehaviour {
	
	public Renderer rend;
    public float moveSpeed = 5f;

    Vector2Int currTilePosition;

    
	void Awake() {
		rend = this.GetComponent<Renderer>();
	}
	
	// Start is called before the first frame update
    void Start() {
       transform.rotation = Quaternion.Euler(-90,0,0); 
    }

    // Update is called once per frame
    void Update() {
        
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
        StartCoroutine(PathMovement(path, board, moveSpeed));
    }

    public void PlaceAt(Vector2Int position, ref BoardController board) {
        currTilePosition = position;
        transform.position = board.CellToWorld(position);
    }

    IEnumerator PathMovement(List<Vector2Int> path, BoardController board, float speed) {
         float step = speed * Time.fixedDeltaTime;
         float t = 0;
         Vector3 prevPos = transform.position;
         for(int i = 0; i < path.Count; i++) {
            currTilePosition = path[i];
            Vector3 worldPos = board.CellToWorld(currTilePosition);
            t = 0;
            while (t <= 1.0f) {
                t += step;
                transform.position = Vector3.Lerp(prevPos, worldPos, t);
                yield return new WaitForFixedUpdate();
            }
            prevPos = worldPos;
            transform.position = worldPos;
        }
    }

    public void Kill() {
        Destroy(this.gameObject);
    }
}
