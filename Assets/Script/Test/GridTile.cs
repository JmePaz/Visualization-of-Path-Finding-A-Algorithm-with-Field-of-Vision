using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GridTile : MonoBehaviour
{
    // variables
    [SerializeField] World world;
    [SerializeField] GameObject tile;
 
    [SerializeField] int tileCols;
    [SerializeField] int tileRows;

    private GameObject ground;
    private Vector3 tileScale;


    protected internal Vector2Int start,end;

    // Start is called before the first frame update
    void Awake()
    {
        //initialize all values
        ground = this.gameObject;
        world.GridMap = new GameObject[tileRows, tileCols];


        //Calculate number of tiles per ground
        float tileSizeRow = (ground.transform.localScale.x)/tileCols;
        float tileSizeCol = (ground.transform.localScale.z)/tileRows;
        tileScale = new Vector3(tileSizeRow, 4,tileSizeCol);
        //set scale of tiles
        tile.transform.localScale = tileScale;
        //generate map of tiles

        GenerateMapTiles(world.GridMap,tileSizeRow, tileSizeCol, tileScale.y);


    }

    void Update() {
        
    }

    // this will generate map tiles
    void GenerateMapTiles(GameObject[,] gridMap,float tileSizeRow, float tileSizeCol, float positionY = 3){

        // get upper corner position left of the object
        MeshRenderer renderer = GetComponent<MeshRenderer>();
     
        //get upperleft position
        Vector3 upperLeft = renderer.bounds.min ;
        // vector of last position for reference
        Vector3 lastPosition = default(Vector3);

        // starting width and height for postion of every tile
        float height = upperLeft.x+(tileSizeRow/2);
        float width  = upperLeft.z+(tileSizeCol/2);
        positionY = renderer.bounds.max.y + (positionY/2);
        // loop
        for( int i=0; i<tileRows; i++){
            for(int j=0; j<tileCols; j++){ 
                Vector3 position = new Vector3(height,positionY,tileSizeCol);
                
                if(j!=0){
                    position.z += lastPosition.z;
                }
                else{
                    position.z = width;
                }
                // create a grid tile
                GameObject gridTile = Instantiate(tile, position, Quaternion.identity);
                gridTile.GetComponent<Tile>().SetPosition(i,j);
                gridTile.transform.parent = this.transform;
                
                lastPosition = position;

                //assign it in the grid map
                gridMap[i,j] = gridTile;

            }
            height += tileSizeRow;
        }
    }

    protected internal void GenerateChanges(){
        float distance = 4f;
        for( int i=0; i<tileRows; i++){
            for(int j=0; j<tileCols; j++){ 
                // get all components
                GameObject tile = world.GridMap[i,j];
                Material tileMaterial = tile.GetComponent<Renderer>().material;
                TextMeshPro textTile = tile.GetComponentInChildren<TextMeshPro>();
                RectTransform rectTransform = tile.GetComponent<RectTransform>();

                //  if its a wall
                if(textTile.text == "1"){
                    //change scale
                    tile.transform.localScale  = new Vector3(tile.transform.localScale.x,tile.transform.localScale.y*distance, tile.transform.localScale.z);
                    
                    //change position
                    tile.transform.Translate(Vector3.up*distance*1.5f);
                }
                // an available/walkable cell
                else if(textTile.text == "0"){
                    tileMaterial.color = Color.white;
                }            
                // a starting point
                else if(textTile.text == "S"){
                    // we teleport the player to that position
                    Vector3 instancePos = tile.transform.position;
                    instancePos.y = 3.12f;
                    GameObject enemyInstance = Instantiate(world.EnemyModel,instancePos, Quaternion.identity);
                    world.Enemy = enemyInstance;
                    world.StartPosCell = new Vector2Int(j,i);

                }
                else if(textTile.text == "F"){
                    GameObject flagshipInstance = Instantiate(world.GoalPoleModel, tile.transform.position+(Vector3.up*2f), Quaternion.identity);
                    world.GoalPole = flagshipInstance;
                    world.EndPosCell = new Vector2Int(j,i);
                }
                // a finish point
                else if(textTile.text == "P"){
                    // GameObject flagshipInstance = Instantiate(world.GoalPoleModel, tile.transform.position+(Vector3.up*2f), Quaternion.identity);
                    // world.GoalPole = flagshipInstance;
                    Vector3 position =  tile.transform.position;
                    position.y = 3f;
                    GameObject playerInstance = Instantiate(world.PlayerModel, position, Quaternion.identity);
                    world.Player = playerInstance;
                    world.EndPosCell = new Vector2Int(j,i);
                }

                // remove display of text of every cell unless finish nor end
                if(textTile.text=="1" || textTile.text=="0"){
                    textTile.text = "";
                }
                
            }
        }
    }


}
