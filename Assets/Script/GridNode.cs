using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode 
{
    protected internal Vector3 worldPosition;
    protected internal bool hasObstacle;

    protected internal Vector3 scale;

    public GridNode(Vector3 worldPosition, bool hasObstacle, Vector3 scale){
        this.worldPosition = worldPosition;
        this.hasObstacle = hasObstacle;
        this.scale = scale;
    }
}
