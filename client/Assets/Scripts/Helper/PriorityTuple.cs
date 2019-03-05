using System;
using UnityEngine;
public class PriorityTuple : IComparable<PriorityTuple>{
    private Tuple<int, Vector2Int> priorityTuple;

    public PriorityTuple(int priority, Vector2Int hex){
        priorityTuple = new Tuple<int,Vector2Int>(priority, hex);
    }

    public Tuple<int, Vector2Int> GetPriorityTuple(){
        return priorityTuple;
    }

    public int GetPriority(){
        return priorityTuple.First;
    }
    public Vector2Int GetHex(){
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