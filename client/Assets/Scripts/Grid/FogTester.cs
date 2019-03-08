using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTester : MonoBehaviour {

    BoardController boardController;
    FogOfWarController fogOfWarController;

    public enum FogTestMode { MouseClear, UpdatePosition, PlaceMultiple};
    public FogTestMode fogTestMode = FogTestMode.MouseClear;

    List<FogViewer> viewers = new List<FogViewer>();

    FogViewer singleViewer;

    void Awake(){
        fogOfWarController = new FogOfWarController();
        boardController = new BoardController();
    }

    void Start() {
        boardController.Initialize();
        fogOfWarController.InitializeFogOfWar(boardController.GetTilemap());
    }

    void Update() {
        if(Input.GetMouseButton(0)) {
            switch(fogTestMode) {
                case FogTestMode.MouseClear:
                    MouseClear();
                    break;
                case FogTestMode.PlaceMultiple:
                    PlaceMultiple();
                    break;
                case FogTestMode.UpdatePosition:
                    UpdatePosition();
                    break;
                default:
                    break;
            }
        } 
    }

    void MouseClear() {
        fogOfWarController.ClearFogAtPosition(boardController.MousePosToCell());
    }

    void UpdatePosition() {
        Vector2Int tilePos = boardController.MousePosToCell();
        if(singleViewer == null) {
            singleViewer = new FogViewer();
            singleViewer.SetPosition(tilePos);
            singleViewer.SetRadius(2);
            fogOfWarController.AddFogViewer(singleViewer);
        }
        singleViewer.SetPosition(tilePos);
    }

    void PlaceMultiple() {
        Vector2Int tilePos = boardController.MousePosToCell();
        FogViewer viewer = new FogViewer();
        viewer.SetRadius(2);
        viewer.SetPosition(tilePos);
        fogOfWarController.AddFogViewer(viewer);
    }
}
