using System;
using UnityEngine;


class Node:IComparable{

    Node parent;
    Vector2Int position;
    protected internal float  g, h;

    public Node(Vector2Int position){
        this.position = position;
        this.parent = default(Node);
        this.g = 0;
        this.h = 0;
    }

    public Node(Vector2Int position, Vector2Int targetPos, Node parent){
        this.position = position;
        this.parent = parent;
        this.g = parent.g + 1;
        this.h = GetEuclideanDistance(position, targetPos);
    }

    protected internal Node GetParent(){
        return this.parent;
    }
    protected internal Vector2Int GetPosition(){
        return this.position;
    }
    protected internal float GetF(){
        return this.g + this.h;
    }

    // calculate H
    private float CalculateH(float f, float g){
        return 0;
    }


    private float GetEuclideanDistance(Vector2 pos1, Vector2 pos2)
    {
        return (float)Math.Sqrt(Math.Pow((pos1.x - pos2.x), 2)
                                    + Math.Pow((pos1.y - pos2.y), 2));
    }

    // override/ functions for utlizing the object istances of Node
        
    public override bool Equals(object obj)
    {
        return obj is Node node &&
               position.Equals(node.position);
    }
    // for hashset or dictionary
    public override int GetHashCode()
    {
        return 1206833562 + position.GetHashCode();
    }


    // required for priority queue
    public int CompareTo(object obj)
    {
         Node nodeB = obj as Node;
        return this.GetF().CompareTo(nodeB.GetF());
    }
    


}