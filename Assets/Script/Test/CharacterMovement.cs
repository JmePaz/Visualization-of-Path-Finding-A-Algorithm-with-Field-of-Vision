using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{   
    [SerializeField] private float speed = 7f;
    Transform myTransform;
    

    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
    
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(inputX, 0, inputZ).normalized;
        Vector3 movementPos = direction*speed*Time.deltaTime;
        myTransform.LookAt(myTransform.position+movementPos);
        myTransform.position += movementPos;

    }
}
