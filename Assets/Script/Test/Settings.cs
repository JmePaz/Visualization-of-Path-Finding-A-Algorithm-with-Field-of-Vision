using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 class Settings : MonoBehaviour
{
    [SerializeField] World world;
    [SerializeField] private CameraSetUp cameraSetUp;
    private GridTile scriptGridTile;

    private void Start() {
        GameObject ground = GameObject.Find("Ground");
        if(ground != null){
            scriptGridTile = ground.GetComponent<GridTile>();
        }
        
    }


    private void Update() {
        if(TileSetting.status == "Set Walls" && Input.GetMouseButton(0)){
            TileSetting.isPlacingWalls = true;
        }
        else{
            TileSetting.isPlacingWalls = false;
        }
    }
    public void SetStartPoint(){
        TileSetting.status = "Set Start Point";     

    }

    public void SetEndPoint(){
        TileSetting.status = "Set End Point";

    }

    public void SetMovingTargetPoint(){
        TileSetting.status = "Set Moving Target Point";
    }

    public void SetWalls(){
        
       TileSetting.status = "Set Walls";
    }   

    public void SaveChanges(){
        if(scriptGridTile==null){
            return;
        }
        //set status
        TileSetting.status = "Saved";

        //generate changes
        scriptGridTile.GenerateChanges();
        

        // set up camera
        // if(world.Player!=null){
        //     cameraSetUp.SetUpPlayerCamera(world.Enemy, world.Player);
        // }
        // else if(world.GoalPole!=null){
        //     cameraSetUp.SetUpPlayerCamera(world.Enemy, world.Enemy);
        // }
        
     

        //hide
        this.gameObject.SetActive(false);
    }

    private void SetOff(ref bool switchPoint){
        if(switchPoint == true){
            switchPoint = false;
        }
    }
}
