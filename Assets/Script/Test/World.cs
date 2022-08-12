using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
  protected internal GameObject[,] GridMap {get; set;}
  protected internal Vector2Int StartPosCell {get; set;}

  protected internal Vector2Int EndPosCell {get; set;}

  protected internal GameObject Player {get; set;}
 
  protected internal GameObject Enemy {get; set;}
  protected internal GameObject GoalPole{get; set;}

  [SerializeField] protected internal GameObject PlayerModel;

  [SerializeField] protected internal GameObject GoalPoleModel;

  [SerializeField] protected internal GameObject EnemyModel;
  
  protected internal List<Vector3> FindPath(){
        
        // generate path
        PathFinder path = new PathFinder();
        return  path.FindPath(this.GridMap, this.StartPosCell, this.EndPosCell);
  }

  protected internal List<Vector3> FindPath(Vector2Int startPos, Vector2Int endPos){
        
        // generate path
        PathFinder path = new PathFinder();
        return  path.FindPath(this.GridMap, startPos, endPos);
  }
}
