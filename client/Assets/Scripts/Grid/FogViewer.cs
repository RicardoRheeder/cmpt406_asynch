using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void FogUpdateDelegate(FogViewer viewer);

public class FogViewer {
    string id;
    Vector2Int position;
    int radius;
    List<Vector2Int> affectedTiles;
    FogUpdateDelegate fogUpdateMethod;

    public void SetFogUpdateMethod(FogUpdateDelegate method) {
        fogUpdateMethod = method;
    }

    public Vector2Int GetPosition() {
        return position;
    }

    public void SetPosition(Vector2Int pos) {
        position = pos;
        fogUpdateMethod(this);
    }

    public int GetRadius() {
        return radius;
    }

    public void SetRadius(int r) {
        radius = r;
        fogUpdateMethod(this);
    }

    public List<Vector2Int> GetAffectedTiles() {
        return affectedTiles;
    }

    public void SetAffectedTiles(List<Vector2Int> tiles) {
        affectedTiles = tiles;
    }

}
