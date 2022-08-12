using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float rayDist;


    Transform myTransform;
    Rigidbody myRigidbody;

    RaycastHit hit;

    Vector3[] directions;

    
    // start one frame
    void Start()
    {
      myTransform = this.transform;
      myRigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    //backtracking
    void Update()
    { 

      //possible directions
      directions = new Vector3[]{myTransform.forward, myTransform.right, -myTransform.right, -myTransform.forward};

      //iterate all possible directions
      foreach (Vector3 direction in directions){

        //validate direction! if it found a possible direction (no wall)
        if(!DrawRay(direction))
        {
          //rotate around the direction until the forward does not detect the wall
          float rotationRound = 0;
          while(DrawRay(myTransform.forward) && rotationRound<360){
            Rotate(direction,30);
            rotationRound += 30*Time.deltaTime;
          }

          // for error handling
          if(rotationRound>=360){
            if(rayDist <= 1.5){
              this.enabled = false;
              return;
            }
            rayDist -= 1;
          }

          // move forward
          Move(myTransform.forward);
          break;
        }
      }

    }

    bool DrawRay(Vector3 direction){
      //for debug
      Debug.DrawRay(myTransform.position, direction*rayDist, Color.green);

      //drawing a ray
      return Physics.Raycast(myTransform.position, direction, out hit, rayDist);
    }


    void Rotate(Vector3 direction, float angle){
       Quaternion rotation = Quaternion.LookRotation(direction*angle);
       myTransform.rotation = Quaternion.Lerp(myTransform.rotation,rotation,Time.deltaTime*(speed/2));
    }

    //movement
    void Move(Vector3 direction){
      Vector3 newPosition = myRigidbody.position+direction*speed*Time.deltaTime;

      //move 
      myRigidbody.MovePosition(newPosition);
      
    }

    // get distance of two vectors
    float GetDistanceOfVectors(Vector3 from, Vector3 to){
      double a = System.Math.Pow((from.x - to.x),2);
      double b = System.Math.Pow((from.y - to.y),2);
      double c = System.Math.Pow((from.z - to.z),2);
      return (float)System.Math.Sqrt(a+b+c);
    }
}
