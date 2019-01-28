using System;
using UnityEngine;
public class PriorityTuple : IComparable<PriorityTuple>{
    private Tuple<int, Vector3Int> priorityTuple;

    public PriorityTuple(int priority, Vector3Int hex){
        priorityTuple = new Tuple<int,Vector3Int>(priority, hex);
    }

    public Tuple<int, Vector3Int> GetPriorityTuple(){
        return priorityTuple;
    }

    public int GetPriority(){
        return priorityTuple.First;
    }
    public Vector3Int GetHex(){
        return priorityTuple.Second;
    }
    public int CompareTo(PriorityTuple other){
        if(priorityTuple.First < other.GetPriorityTuple().First){
            return -1;
        }
        else if(priorityTuple.First > other.GetPriorityTuple().First){
            return 1;
        }
        else{
            return 0;
        }
    }
}