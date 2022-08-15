using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollower : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 0.09f;
    [SerializeField] private GameObject targetToFollow;
    [SerializeField] private GridMap layoutMap;
    
    private AiFieldView fieldView;
    private PathFinder pathFinder;
    private Transform myTransform;
    private Rigidbody myRigidbody;

    private int currentIndex;
    private Vector3 directionGoal;

    private bool outOfDirection;

    private List<Vector3> pathPoints;

    //additional variables
    Vector3 directionLocalRight;
    Vector3 localScale;
    float marginScale;
    float maxLength ;
    Vector3 originOffSet ;


    // Start is called before the first frame update
    
    void Start()
    {
        pathFinder = new PathFinder();
        fieldView = GetComponent<AiFieldView>();
        myTransform = this.transform;
        outOfDirection = false;
        localScale = myTransform.localScale;
        layoutMap.CreateGridCoordinates(this.transform);

        //set new Path points
        GenerateNewPath();

        //look at the next point
        LookAtPath();

        // get right direction and movement to follow the point
        marginScale = 0.1f;
        maxLength =  localScale.x/2+1.5f;       
    }



    void Update(){
        fieldView.CastFieldView();
        if(fieldView.InFieldView){
            //Debug.Log("Field of View Activated");
            fieldView.ChangeMeshColorMaterial(Color.red);
            FollowTargetPlayer(fieldView.targetPos);
            currentIndex = pathPoints.Count;
        }
        else{
            // move ai
            //Debug.Log("Ai Activated");
            if(!fieldView.InSphereView){
                fieldView.ChangeMeshColorMaterial(Color.yellow);
            }
            AiMove();
          
        }
    }
    void AiMove(){

        //error checking
        if(pathPoints.Count <= 0 || currentIndex>=pathPoints.Count){
            GenerateNewPath();
        }

        // get new points to follow
        Vector3 currentPosition = this.transform.position;

        //generate new path
        bool insideRect = InBound(pathPoints[pathPoints.Count - 1], targetToFollow.transform.position, layoutMap.GetScaleBox()*10f, true);
        if((!insideRect&&IsInOppositeDirection(currentPosition, targetToFollow.transform.position, 90f))){
            
            //just rotate
            if(currentIndex>=pathPoints.Count&&currentIndex == 1){
                myTransform.LookAt(targetToFollow.transform);
            }
           //follow player 
           else{
                //modify path points and do an A* algorithmn
               GenerateNewPath();
            }
          
          return;
        }
        // if its out of direction to the current point it needs to take
        else if(currentIndex<pathPoints.Count && IsInOppositeDirection(currentPosition, pathPoints[currentIndex],10f)){
            LookAtPath();
        }

        //update path
        UpdatePathPoints(currentPosition);
     
        //move position
        if(currentIndex<pathPoints.Count){
            Vector3 directionToTheTarget = (pathPoints[currentIndex]-currentPosition).normalized  - myTransform.forward;
            if((int)directionToTheTarget.x == 0 && (int) directionToTheTarget.z == 0){
                Move(myTransform.TransformDirection(Vector3.forward));
            }
            else{
                //rotate if out of direction
                LookAtPath();
            }
        }
       
    }

    private void UpdatePathPoints(Vector3 currentPosition){
        // if it does not have any 
        if(currentIndex>=pathPoints.Count){
            GenerateNewPath();
        }
        // if it founds the new point
        else if(InBound(pathPoints[currentIndex],currentPosition, layoutMap.GetScaleBox()/2.5f, true)){
            //change new point
            currentIndex++;
            LookAtPath();
        }
    }
    
    void LookAtPath(){
        
        if(currentIndex<pathPoints.Count)
        {
            Vector3 newPoint = pathPoints[currentIndex];
            newPoint.y = myTransform.position.y;
            LookAtNextPos(newPoint);
        }
    }

    private void GenerateNewPath(){
         FindTarget(targetToFollow.transform, myTransform.position);
    }
    
    private void LookAtNextPos(Vector3 targetPos){
        Vector3 targetDirection = (targetPos - myTransform.position).normalized;
        Quaternion currentRot = myTransform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(targetDirection);
        myTransform.rotation = Quaternion.Slerp(currentRot, targetRot, rotationSpeed);
    }
    private void FindTarget(Transform playerTransform, Vector3 currentPosition){
         Vector2Int startGridPos = layoutMap.WorldPosToGridPos(myTransform.position);
         Vector2Int endGridPos = layoutMap.WorldPosToGridPos(targetToFollow.transform.position);
         pathPoints = pathFinder.FindPath(layoutMap.GetGridMapLayout(), startGridPos, endGridPos);
         currentIndex = 1;
         LookAtPath();
    }
   


    Vector3 GetOffSetDirection(Vector3 currentPosition){
        Vector3 direction = Vector3.zero;
        //calculations
        directionLocalRight = myTransform.right;
        Vector3 originOffSetRight = myTransform.TransformDirection(Vector3.right*(localScale.x/2+0.5f));
        Vector3 originOffSetForward = (myTransform.forward*(localScale.z/2));
        originOffSet = transform.TransformDirection(Vector3.right*(localScale.x/2+marginScale));
        //raycasts
        Vector3 origin = (currentPosition+originOffSetForward);
        
        // bool forwardRightRayCast = BuildRayCast(currentPosition+originOffSetRight+originOffSetForward, myTransform.forward, maxLength);

        // bool forwardLeftRayCast = BuildRayCast(currentPosition-originOffSetRight+originOffSetForward, myTransform.forward, maxLength);
       
         
        if(Physics.Raycast(origin+originOffSetRight, myTransform.forward, out RaycastHit info1,maxLength)){
            if(info1.collider.gameObject.layer==layoutMap.obstacleLayerMask){
                direction = Vector3.left;
            }
            outOfDirection = true;
        }        
        else if(Physics.Raycast(origin-originOffSetRight, myTransform.forward, out RaycastHit info2,maxLength)){
            if(info2.collider.gameObject.layer==layoutMap.obstacleLayerMask){
                direction = Vector3.right;
            }
           outOfDirection = true;
        }
        else if(outOfDirection && currentIndex<pathPoints.Count){
    
           Vector3 newPoint = pathPoints[currentIndex];
           newPoint.y = currentPosition.y;
           myTransform.LookAt(newPoint);
           outOfDirection = false;
        }
        return direction;
    }

    // if its inbound on the points
    bool InBound(Vector3 point, Vector3 position, float margin=1f, bool cast=false){
        if(cast){
            Vector3 A = new Vector3(point.x-margin, point.y, point.z+margin);
            Vector3 B = new Vector3(point.x+margin, point.y, point.z+margin);
            Vector3 C = new Vector3(point.x-margin, point.y, point.z-margin);
            Vector3 D = new Vector3(point.x+margin, point.y, point.z-margin);
            //cast a box
            Debug.DrawLine(A,B, Color.black);
            Debug.DrawLine(B,D,Color.black);
            Debug.DrawLine(D,C,Color.black);
            Debug.DrawLine(C,A,Color.black);
        }
        if(point.x+margin>=position.x && position.x>=point.x-margin){
            return (point.z+margin>=position.z && point.z-margin<=position.z);
        }
        return false;
    }

    bool InBound(Vector3 point, Vector3 position, Vector3 marginVector, bool cast=false){
        if(cast){
            Vector3 A = new Vector3(point.x-marginVector.x, point.y, point.z+marginVector.z);
            Vector3 B = new Vector3(point.x+marginVector.x, point.y, point.z+marginVector.z);
            Vector3 C = new Vector3(point.x-marginVector.x, point.y, point.z-marginVector.z);
            Vector3 D = new Vector3(point.x+marginVector.x, point.y, point.z-marginVector.z);
            //cast a box
            Debug.DrawLine(A,B, Color.red);
            Debug.DrawLine(B,D,Color.red);
            Debug.DrawLine(D,C,Color.red);
            Debug.DrawLine(C,A,Color.red);
        }
        if(point.x-marginVector.x<=position.x && position.x<=point.x+marginVector.x){
            return (point.z-marginVector.z<=position.z && position.z<=point.z+marginVector.z);
        }
        return false;
    }

    bool InBound(float targetAxis, float currentAxis, float margin=1f){
        return (targetAxis-margin<=currentAxis) &&  (currentAxis<=targetAxis+margin);
    }



    void Move(Vector3 direction, float boostSpeed=0f){
        
        myTransform.position += (direction*(speed+boostSpeed)*Time.deltaTime);
    }

    void FollowTargetPlayer(Vector3 playerPos){
        //look at rotation
        transform.LookAt(playerPos);

        //direction
        Move(transform.forward, 0.4f);
    }
    bool IsInOppositeDirection(Vector3 currentPosition, Vector3 targetPos, float angle){
        Vector3 direction = (targetPos - currentPosition).normalized;
        return Vector3.Angle(myTransform.forward, direction)>angle;
    }


    // for debugging
    void DebugPoints(List<Vector3> points){
        foreach(Vector3 point in points){
            Debug.Log(point);
        }
    }

   
    private void ColorTile(Vector3 origin){
        if(Physics.Raycast(origin,Vector3.down, out RaycastHit info,5f)){
            GameObject tile = info.collider.gameObject;
            Material material = tile.GetComponent<Renderer>().material;
            material.color = Color.green;
        }
    }

    private Vector2Int GetTilePosition(Vector3 origin){
        if(Physics.Raycast(origin,Vector3.down, out RaycastHit info,5f)){
            GameObject tile = info.collider.gameObject;
            if(tile.tag=="Tile"){
                Tile tileScript = tile.GetComponent<Tile>();
                return tileScript.GetPositionInGrid();
            }
            
        }
        return Vector2Int.zero;
    }

    private void OnCollisionEnter(Collision other) {
        GameObject otherGameObject = other.gameObject;
        if(otherGameObject.tag == "Finish" || otherGameObject.tag == "Player"){
            //set up c// cameraSetUp.SetUpCameraUIBack();

            //destroy player gameObject
            if(otherGameObject.tag=="Player"){
                Destroy(otherGameObject);
            }
        
            //restrict movment of character
            this.gameObject.GetComponent<AIFollower>().enabled = false;
        }
    }

    private void OnDrawGizmos() {
        if(pathPoints!=null){
            Gizmos.color = Color.green;
            for(int i=currentIndex; i<pathPoints.Count; i++){
                Vector3 point = pathPoints[i];
                point.y = myTransform.position.y;
                Gizmos.DrawSphere(point, 15f);
            }
        }
    }

}
