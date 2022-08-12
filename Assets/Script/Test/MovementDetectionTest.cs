using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDetectionTest : MonoBehaviour
{
    [SerializeField] private float speed;
    Transform myTransform;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = myTransform.position;
        Vector3 directionLocalRight = (myTransform.right).normalized;
        Vector3 localScale = myTransform.localScale;
        float marginScale = 0.1f;
        float maxLength =  localScale.x/2 + 0.5f;
        Vector3 originOffSet = transform.TransformDirection(new Vector3(localScale.x/2+marginScale,0,0));

        //get direction
        Vector3 direction =  Vector3.forward;
        Vector3 originOffSetForward = myTransform.TransformDirection(Vector3.forward*((localScale.z/2)+marginScale));
        bool forwardRayCast = BuildRayCast(currentPosition+originOffSetForward, myTransform.forward, maxLength);
        bool rightRayCast = BuildRayCast(currentPosition+originOffSet, directionLocalRight, maxLength+1f);
        bool leftRayCast = BuildRayCast(currentPosition-originOffSet, -directionLocalRight, maxLength+1f);
        if(forwardRayCast || (rightRayCast || leftRayCast)){    
            if(rightRayCast){
               
                direction = myTransform.TransformDirection(Vector3.left);
               
            }
            else if(leftRayCast){
     
                direction = myTransform.TransformDirection(Vector3.right);
            }
        }
     
        //move
       Move(myTransform.forward+direction);
    }

    private bool BuildRayCast(Vector3 origin, Vector3 direction, float length){
        //for debugging
        Debug.DrawRay(origin, direction*length, Color.blue);
        return Physics.Raycast(origin, direction,out hit ,length);
    }
    // mvoe position
    void Move(Vector3 direction){
        Debug.Log(direction);
        //myTransform.Translate(direction*speed*Time.deltaTime);
        myTransform.position = myTransform.position + (direction*speed*Time.deltaTime);
    }

}
