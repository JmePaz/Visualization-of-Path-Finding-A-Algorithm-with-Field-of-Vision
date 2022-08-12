using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
   [SerializeField] private float speed = 8f;
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 direction = (new Vector3(x,0,z)).normalized;

        this.transform.Translate(direction*speed*Time.deltaTime);

    }
}
