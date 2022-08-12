using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMovement : MonoBehaviour
{
    // Start is called before the first frame update\
    private Transform myTransform;
    private Rigidbody myRigidbody;
    private World world;
    private CameraSetUp cameraSetUp;

    private List<Vector3> pathPoints;

    private int currentIndex;
    [SerializeField] private float speed;


    private AiFieldView fieldView;

    private float rotateDegree =7f;

    private Vector3 directionGoal;

    private bool outOfDirection = false;

    private RaycastHit hit;



    void Awake(){
        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
        cameraSetUp = GameObject.FindGameObjectWithTag("Cameras").GetComponent<CameraSetUp>();
    }


    
    //debug
    
    Vector3 directionLocalRight;
    Vector3 localScale;
    float marginScale;
    float maxLength ;
    Vector3 originOffSet ;

    //debug
    void Start()
    {
     
        currentIndex = 1;
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody>();
        fieldView = GetComponent<AiFieldView>();
        
        // show path
        pathPoints = world.FindPath();


        //look at the next point
        if(currentIndex<pathPoints.Count)
        {
            Vector3 newPoint = pathPoints[currentIndex];
            newPoint.y = myTransform.position.y;
            this.transform.LookAt(pathPoints[currentIndex]);
            directionGoal = (newPoint-myTransform.position).normalized;
            

        }

        // get right direction and movement to follow the point

        localScale = myTransform.localScale;
        marginScale = 0.1f;
        maxLength =  localScale.x/2+1.5f;       
        
    }

    void Update(){
        fieldView.CastFieldView();
        if(fieldView.InSphereView && fieldView.InFieldView){
            Debug.Log("Field of View Activated");
            FollowTargetPlayer(fieldView.targetPos);
            currentIndex = pathPoints.Count;
        }
        else{
            // move ai
            Debug.Log("Ai Activated");
            AiMove();
          
        }
    }

    void FollowTargetPlayer(Vector3 playerPos){
        //look at rotation
        transform.LookAt(playerPos);

        //direction
        Move(transform.forward, 0.4f);
    }
    bool IsInRightDirection(Vector3 currentPosition){
        Vector3 direction = (world.Player.transform.position - currentPosition).normalized;
        return Vector3.Angle(myTransform.forward, direction)<=90;
    }
    void AiMove(){

        // get new points to follow
        Vector3 currentPosition = this.transform.position;

        // if there are no moves  follow the player
        if(currentIndex>=pathPoints.Count || ((!InBound(pathPoints[pathPoints.Count-1], world.Player.transform.position, 50, true))&&IsInRightDirection(currentPosition))){
        
           //follow player 
            if(world.Player!=null){
                Transform playerTransform = world.Player.transform;

                //modify path points and do an A* algorithmn
                FindTarget(playerTransform, currentPosition);
            }
          
          return;
        }

        //update path
        UpdatePathPoints(currentPosition);
        //get offset direction
        //move position
        Move(myTransform.TransformDirection(Vector3.forward));
    }

    void Move(Vector3 direction, float boostSpeed=0f){
        
        myTransform.position += (direction*(speed+boostSpeed)*Time.deltaTime);
    }


    private void FindTarget(Transform playerTransform, Vector3 currentPosition){

         Debug.Log("Fixing points");
          // start Pos
          Vector2Int startPos = GetTilePosition(currentPosition+myTransform.TransformDirection(Vector3.down*(localScale.y/2)));
    
          //end Pos
          Vector2Int endPos = GetTilePosition(playerTransform.position+ playerTransform.TransformDirection(Vector3.down*
                                                (playerTransform.localScale.y/2)));
          //reset points list and currentIndex, use path finding algorithmn
          pathPoints = world.FindPath(startPos, endPos);
          currentIndex = 1;

          // look at next tile position
          if(currentIndex<pathPoints.Count)
          {
              Vector3 newPoint = pathPoints[currentIndex];
              newPoint.y = currentPosition.y;
              myTransform.LookAt(newPoint);
          }

    }
    private void UpdatePathPoints(Vector3 currentPosition){
                // if it founds the new point
        if(InBound(pathPoints[currentIndex],currentPosition, 0.5f)){
            //change new point
            currentIndex++;
            //look at the new point
            if(currentIndex<pathPoints.Count)
            {
                Vector3 newPoint = pathPoints[currentIndex];
                newPoint.y = currentPosition.y;
                myTransform.LookAt(newPoint);
                outOfDirection = false;
                directionGoal = (newPoint-currentPosition).normalized;

                //color tile 
                //ColorTile(currentPosition+myTransform.TransformDirection(Vector3.down*(localScale.y/2)));
            }
        }
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
            if(info1.collider.gameObject.tag=="Player"){
                direction = Vector3.right;
            }
            else{
                direction = Vector3.left;
            }
            outOfDirection = true;
        }        
        else if(Physics.Raycast(origin-originOffSetRight, myTransform.forward, out RaycastHit info2,maxLength)){
            if(info2.collider.gameObject.tag == "Player"){
                direction = Vector3.left;
            }
            else{
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
            Debug.DrawLine(A,B, Color.green);
            Debug.DrawLine(B,D,Color.green);
            Debug.DrawLine(D,C,Color.green);
            Debug.DrawLine(C,A,Color.green);
        }
        if(point.x+margin>=position.x && position.x>=point.x-margin){
            return (point.z+margin>=position.z && point.z-margin<=position.z);
        }
        return false;
    }

    bool InBound(float targetAxis, float currentAxis, float margin=1f){
        return (targetAxis-margin<=currentAxis) &&  (currentAxis<=targetAxis+margin);
    }


    //build a raycast
    private bool BuildRayCast(Vector3 origin, Vector3 direction, float length){
        //for debugging
        Debug.DrawRay(origin, direction*length, Color.blue);
        return Physics.Raycast(origin, direction,out hit ,length);
    }


    // move position


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
            this.gameObject.GetComponent<AiMovement>().enabled = false;
        }
    }
}
