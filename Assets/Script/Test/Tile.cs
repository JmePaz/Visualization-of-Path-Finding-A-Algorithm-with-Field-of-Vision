using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    private Renderer renderer;
    [SerializeField]private TextMeshPro textDisplay;
    private int weightCount;
    private Color past;
    // Start is called before the first frame update
    private int row, col;



    void Start()
    {
     renderer = GetComponent<Renderer>();  
     past = renderer.material.color;
    }

    private void OnMouseDown(){
        if(TileSetting.status == "Set Start Point"){
            TileSetting.startPoint = this.transform.position;
            SetTileProperty(Color.yellow,"S");
        }
        else if(TileSetting.status == "Set End Point"){
            TileSetting.endPoint = this.transform.position;
            SetTileProperty(Color.yellow,"F");
        }
        else if(TileSetting.status == "Set Moving Target Point"){
            TileSetting.endPoint = this.transform.position;
            SetTileProperty(Color.yellow,"P");
        }
        
    }
    private void OnMouseEnter() {
         if(TileSetting.status == "Set Walls" && TileSetting.isPlacingWalls){
             Gizmos.color = Color.red;
           SetTileProperty(new Color(179,0,0), "1");
           

           //set tag a wall
           this.gameObject.tag = "Wall";
        }
        
    }

    private void SetTileProperty(Color color, string text){
        renderer.material.color = color;
        textDisplay.text = text;
    }

    public void SetPosition(int row, int col){
        this.row = row;
        this.col = col;
    }
    
    public Vector2Int GetPositionInGrid(){
        return new Vector2Int(this.col, this.row);
    }

    // this draw gizmos

    void OnDrawGizmos() {
        Gizmos.color = Color.black;
        
        Vector3 pos = this.transform.position;
        Vector3 scale = this.transform.lossyScale;

        //change pos y
        pos.y = renderer.bounds.max.y;
        //draw gizmos
        Gizmos.DrawWireCube(pos, new Vector3(scale.x, 0, scale.z));    
    }

}
