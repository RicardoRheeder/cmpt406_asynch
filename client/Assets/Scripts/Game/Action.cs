using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Action {

    [DataMember]
    private ActionType type;

    [DataMember]
    private Vector2Int targetTile;
}
