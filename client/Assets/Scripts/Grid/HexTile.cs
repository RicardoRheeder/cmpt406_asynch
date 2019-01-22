using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile: MonoBehaviour {
    Elevation elevation;
    TileAttribute attribute;
    Vector3Int localPosition;
    Vector3Int worldPosition;
    

    public Elevation Elevation { get; set; }

    public TileAttribute Attribute { get; set; }

    public Vector3Int LocalPosition { get; set; }

    public Vector3Int WorldPosition { get; set; }

}
