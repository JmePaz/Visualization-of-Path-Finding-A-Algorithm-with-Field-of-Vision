using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    
    private GridNode[,] gridMap;
    private Vector3 size;
    [SerializeField][Range(30, 100)]private int numOfRows ;
    [SerializeField][Range(30, 100)]private int numOfColumns;
    public LayerMask obstacleLayerMask;
    private Transform myTransform;
    private Vector3 scaleBox;
    
    Vector2Int[] directions =  new Vector2Int[]{
        new Vector2Int(0, 1), new Vector2Int(0,-1), 
        new Vector2Int(1, 0), new Vector2Int(-1, 0)
        , new Vector2Int(-1, 1), new Vector2Int(1,-1),
        new Vector2Int(1,1), new Vector2Int(-1,-1)
    };

    
    void Awake() {
        myTransform = this.transform;
        size = myTransform.localScale;
        scaleBox = new Vector3(size.x/numOfColumns, 2, size.z/numOfRows);

    }

    protected internal void CreateGridCoordinates(Transform aiTransform){
        gridMap = new GridNode[numOfRows, numOfColumns];
        Vector3 topLeftPos = this.transform.position + (Vector3.left*(size.x/2)) + (Vector3.forward*size.z/2);
        Vector3 halfScale = scaleBox/2;
        Vector3[] offSetPosScales = new Vector3[]{
            halfScale.x*Vector3.right, halfScale.x*Vector3.left,
            halfScale.z*Vector3.forward, halfScale.z*Vector3.back
        };


        for(int row = 0; row < numOfRows; row++){
            for(int col = 0; col < numOfColumns; col++){
                Vector3 pos = topLeftPos + (col*Vector3.right*scaleBox.x) -(row*Vector3.forward*scaleBox.z);
                pos += (Vector3.right*scaleBox.x/2) - (Vector3.forward*scaleBox.z/2);
                pos += (Vector3.up*(size.y));
                bool hasObstacle = Physics.CheckBox(pos, halfScale, Quaternion.identity,obstacleLayerMask);
                // if(hasObstacle){
                //     // foreach(Vector3 offsetScale in  offSetPosScales){
                //     //     Vector3 offsetPos = pos + offsetScale;
                //     //     if(!Physics.CheckBox(pos, halfScale,Quaternion.identity,obstacleLayerMask)){
                //     // //         pos = offsetPos;
                //     // //         hasObstacle = false;
                //     //         break;
                //     //     }
                //     // }
                // ;
                // }
                gridMap[row, col] = new GridNode(pos, hasObstacle, scaleBox);
                
            }
        }
    }


    protected internal Vector2Int WorldPosToGridPos(Vector3 pos){
         Vector3 topLeftPos = this.transform.position + (Vector3.left*(size.x/2)) + (Vector3.forward*size.z/2);
         Vector3 startPoint = topLeftPos += (Vector3.right*scaleBox.x/2) - (Vector3.forward*scaleBox.z/2);
         startPoint += Vector3.up*(size.y/2+1f);

         Vector3 offSet = (pos-startPoint);
         float percentAxisCol = offSet.x/size.x;
         float percentAxisRow = -(offSet.z/size.z);

         int indexRow = Mathf.RoundToInt(percentAxisRow*numOfRows);
         int indexCol = Mathf.RoundToInt(percentAxisCol*numOfColumns);

         if(indexRow < 0 || indexCol < 0 || indexRow >= gridMap.GetLength(0) || indexCol >= gridMap.GetLength(1)){
             // out of bounds
             return new Vector2Int(-1,-1);
         }
         return OffSetGridPosition(new Vector2Int(indexCol, indexRow));
    }


    private Vector2Int OffSetGridPosition(Vector2Int gridPos){
        if(gridMap[gridPos.y, gridPos.x].hasObstacle){
            foreach(Vector2Int direction in directions){
                Vector2Int newGridPos = direction+gridPos;
                if(!OutBoundsPosition(newGridPos)&&!gridMap[newGridPos.y, newGridPos.x].hasObstacle){
                    return newGridPos;
                }
            }
        }

        return gridPos;
    }
   private bool OutBoundsPosition(Vector2Int position){

        return position.y<0 || position.y>=gridMap.GetLength(0) || position.x<0 || position.x >=gridMap.GetLength(1);
    }



    public GridNode[,] GetGridMapLayout(){
        return this.gridMap;
    }

    void OnDrawGizmos() {
        if(this!=null){
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(myTransform.position,new Vector3(size.x,1,size.z));
           
        }
        if(gridMap!=null){
            for(int row=0;row<numOfRows; row++){
                for(int col=0;col<numOfColumns; col++){
                    GridNode node = gridMap[row,col];
                    
                    // means target
                    if(node.hasObstacle){
                        Gizmos.color = new Color(239, 48, 58, 0.8f);
                         Gizmos.DrawCube(node.worldPosition, node.scale);
                    }
                    else{
                         Gizmos.color = Color.blue;        
                          Gizmos.DrawWireCube(node.worldPosition, node.scale);            
                    }                  
                    
                }
            }
        }
    }

    public Vector3 GetScaleBox(){
        return scaleBox;
    }
}
