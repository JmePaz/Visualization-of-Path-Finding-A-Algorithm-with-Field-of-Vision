using System;
using System.Collections.Generic;
using UnityEngine;

class PathFinder{


    // set all possible directions
    Vector2Int[] directions =  new Vector2Int[]{
        new Vector2Int(0, 1), new Vector2Int(0,-1), 
        new Vector2Int(1, 0), new Vector2Int(-1, 0)
        , new Vector2Int(-1, 1), new Vector2Int(1,-1),
        new Vector2Int(1,1), new Vector2Int(-1,-1)
    };

    protected internal List<Vector3> FindPath(GameObject[,] gridTile, Vector2Int startPosition, Vector2Int targetPosition){
        return this.AstarAlgorithm(gridTile, startPosition, targetPosition);
    }
    // AstarAlgorithmn
    private List<Vector3> AstarAlgorithm(GameObject[,] gridTile, Vector2Int startPosition,Vector2Int targetPosition){
        // defined a closed and open list
        PriorityQueue<Node> queue = new PriorityQueue<Node>();
        Dictionary<Vector2Int, Node> visited = new Dictionary<Vector2Int, Node>();

    
        // initialize a starting node
        Node start =  new Node(startPosition);
        queue.Enqueue(start);

        // while the queue is not empty        
        while(!queue.IsEmpty()){    
            Node currNode = queue.Dequeue();

            // found the goal terminate the loop
            if(currNode.GetPosition() == targetPosition){             
                return  ShowPath(gridTile, currNode);;
            }
            // if it found an equal and more step of visited node then we ignored it
            else if(visited.ContainsKey(currNode.GetPosition()) &&  currNode.CompareTo(visited[currNode.GetPosition()])>=0){
                continue;
            }

            // add to the visited Node
            visited.Add(currNode.GetPosition(), currNode);

            // move all possible directions
            foreach(Vector2Int direction in directions){
                Vector2Int newPosition = currNode.GetPosition() + direction;
                if(!ValidatePosition(gridTile, newPosition, direction)){
                    continue;
                }
                //new NODE
                Node newNode = new Node(newPosition, targetPosition, currNode);
                if (visited.ContainsKey(newPosition) && visited[newPosition].CompareTo(newNode) > 0)
                {
                    visited[newPosition] = newNode;
                    queue.Enqueue(newNode);
                }
                else if (!visited.ContainsKey(newPosition))
                {
                    queue.Enqueue(newNode);
                }
            }
        }

        // not found
        Debug.Log("Not found");
        return default(List<Vector3>);
    }


    
    

    // for validation
    private bool ValidatePosition(GameObject[,] matrix,Vector2Int position, Vector2Int sign)
    {
            // means out of bounds
            if(OutBoundsPosition(matrix, position))
            {
                return false;
            }
            // check if it is a wall
            else if (matrix[position.y, position.x].tag == "Wall")
            {
                return false;
            }
            //check extra wall if it is traversable to walk diagonal
            if(Math.Abs(sign.x)==1 && Math.Abs(sign.y)==1){
                Vector2Int tilePos1 = new Vector2Int(position.x+GetOppositeSign(sign.x),  position.y);
                Vector2Int tilePos2 = new Vector2Int(position.x, position.y+GetOppositeSign(sign.y));

                if(!OutBoundsPosition(matrix,tilePos1) && !OutBoundsPosition(matrix,tilePos2)){
                    return !(matrix[tilePos1.y, tilePos1.x].tag=="Wall" && matrix[tilePos2.y, tilePos2.x].tag=="Wall");
                }
            }
            
            // if it is traversable/ walkable cell
            return true;
    }

    private bool OutBoundsPosition(GameObject[,]matrix, Vector2Int position){

        return position.y<0 || position.y>=matrix.GetLength(0) || position.x<0 || position.x >= matrix.GetLength(1);
    }

    private int GetOppositeSign(int num){
        if(num<0){
            return 1;
        }
        else if(num>0){
            return -1;
        }
        else{
            return 0;
        }
    }

    protected internal List<Vector3> ShowPath(GameObject[,] gridTile, Node currNode){
        List<Vector3> points = new List<Vector3>();

        while(currNode != default(Node)){
            //get the tile game object
            Vector2Int position = currNode.GetPosition();
            GameObject tile = gridTile[position.y, position.x];

            // change appearance
         


            //store in pointsPath
            Vector3 tilePosition = tile.transform.position;
            points.Add(tilePosition);

            // move to next tile
            currNode = currNode.GetParent();
        }
        //reverse
        points.Reverse();
        return points;

    }


    //gridNodes
    
    protected internal List<Vector3> FindPath(GridNode[,] gridTile, Vector2Int startPosition, Vector2Int targetPosition){
        return this.AstarAlgorithm(gridTile, startPosition, targetPosition);
    }
    // AstarAlgorithmn
    private List<Vector3> AstarAlgorithm(GridNode[,] matrix, Vector2Int startPosition,Vector2Int targetPosition){
        // defined a closed and open list
        PriorityQueue<Node> queue = new PriorityQueue<Node>();
        Dictionary<Vector2Int, Node> visited = new Dictionary<Vector2Int, Node>();

    
        // initialize a starting node
        Node start =  new Node(startPosition);
        queue.Enqueue(start);

        // while the queue is not empty        
        while(!queue.IsEmpty()){    
            Node currNode = queue.Dequeue();

            // found the goal terminate the loop
            if(currNode.GetPosition() == targetPosition){             
                return ShowPath(matrix, currNode);;
            }
            // if it found an equal and more step of visited node then we ignored it
            else if(visited.ContainsKey(currNode.GetPosition()) &&  currNode.CompareTo(visited[currNode.GetPosition()])>=0){
                continue;
            }

            // add to the visited Node
            visited.Add(currNode.GetPosition(), currNode);

            // move all possible directions
            foreach(Vector2Int direction in directions){
                Vector2Int newPosition = currNode.GetPosition() + direction;
                if(!ValidatePosition(matrix, newPosition, direction)){
                    continue;
                }
                //new NODE
                Node newNode = new Node(newPosition, targetPosition, currNode);
                if (visited.ContainsKey(newPosition) && visited[newPosition].CompareTo(newNode) > 0)
                {
                    visited[newPosition] = newNode;
                    queue.Enqueue(newNode);
                }
                else if (!visited.ContainsKey(newPosition))
                {
                    queue.Enqueue(newNode);
                }
            }
        }

        // not found
        Debug.Log("Not found");
        return default(List<Vector3>);
    }


    // for validation
    private bool ValidatePosition(GridNode[,] matrix,Vector2Int position, Vector2Int sign)
    {
            // means out of bounds
            if(OutBoundsPosition(matrix, position))
            {
                
                return false;
            }
            // check if it is a wall
            else if (matrix[position.y, position.x].hasObstacle)
            {
                return false;
            }
            //check extra wall if it is traversable to walk diagonal
            if(Math.Abs(sign.x)==1 && Math.Abs(sign.y)==1){
                Vector2Int tilePos1 = new Vector2Int(position.x+GetOppositeSign(sign.x),  position.y);
                Vector2Int tilePos2 = new Vector2Int(position.x, position.y+GetOppositeSign(sign.y));

                if(!OutBoundsPosition(matrix,tilePos1) && !OutBoundsPosition(matrix,tilePos2)){
                    return !(matrix[tilePos1.y, tilePos1.x].hasObstacle || matrix[tilePos2.y, tilePos2.x].hasObstacle);
                }
           
            }
            
            // if it is traversable/ walkable cell
            return true;
    }

    private bool OutBoundsPosition(GridNode[,]matrix, Vector2Int position){

        return position.y<0 || position.y>=matrix.GetLength(0) || position.x<0 || position.x >= matrix.GetLength(1);
    }

    // optimize grid tiles
    protected internal List<Vector3> ShowPath(GridNode[,] matrix, Node currNode){
        List<Vector3> points = new List<Vector3>();
        Vector3 origin  = default(Vector3);
        Vector3 pastDirection = default(Vector3);
        GridNode prev = null;
        while(currNode != default(Node)){
            //get the grid node
            Vector2Int position = currNode.GetPosition();
            Debug.Log("Path to take: "+position);
            GridNode tile = matrix[position.y, position.x];

            //store in pointsPath
            Vector3 tilePosition = tile.worldPosition;

            if(origin == default(Vector3)){
                origin = tilePosition;
                points.Add(tilePosition);
            }
            else{
                Vector3 currDirection = (tile.worldPosition - origin).normalized;
                if(pastDirection==default(Vector3)){
                    pastDirection = currDirection;
                }
                else if(pastDirection!=currDirection){
                    origin = prev.worldPosition;
                    pastDirection = (tilePosition - origin).normalized;
                    points.Add(origin);
                }
            }
            
            //draw

            // move to next tile
            prev = tile;
            currNode = currNode.GetParent();
        }
        if(prev!=null && points.Count>=0&&prev.worldPosition!=points[points.Count-1]){
            points.Add(prev.worldPosition);
        }


        //reverse
        points.Reverse();
        return points;

    }
}