using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AiFieldView : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float angle;

    public LayerMask targetMask;

    public Vector3 targetPos;
    public bool InSphereView;
    public bool InAngleView;

    public bool InFieldView;

    protected internal float Radius {get{return radius;} set{radius = value;} }
    protected internal float Angle {get{return angle;} set{angle = value;}}
    
    [SerializeField][Range(0f, 1f)]private float meshResolution;

    public MeshFilter meshFilter;
    public Material materialFilter;
    Mesh mesh;

   void Start() {
        InSphereView = false;
        InAngleView = false;
        InFieldView = false;

        mesh = new Mesh();
        mesh.name = "Mesh View Cast";
        meshFilter.mesh = mesh;

        materialFilter.color = Color.yellow;

    }

     void LateUpdate() {
       DisplayFieldView();   
    }
    
    
    protected internal void CastFieldView(){
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.radius, targetMask);
        Vector3 currentPosition = this.transform.position;

         InSphereView = colliders.Length>=1;
        foreach(Collider objectInField in colliders){
            Transform objectTransform = objectInField.transform;
            Vector3 objectDirection = (objectTransform.position-currentPosition).normalized;
            float angleInBetween = Vector3.Angle(transform.forward, objectDirection);
            //object is in the field of view

            InAngleView = Mathf.Abs(angleInBetween)<=(this.angle/2);
            if(InAngleView){
                
                InFieldView = !Physics.Raycast(currentPosition,objectDirection,this.radius, ~targetMask);
                //cast a ray
                if(InFieldView){
                    //found the player
                   targetPos = objectInField.transform.position;
                }      
            }
        }
        
    }

   public void DisplayFieldView(){
       int raysCount = Mathf.RoundToInt(angle*meshResolution);
       float raySize = this.Angle/raysCount;
       List<ViewRayHitInfo> viewRayHitInfos = new List<ViewRayHitInfo>();
       for(int i = 0; i <= raysCount; i++){
            float currAngle = this.transform.eulerAngles.y - this.angle/2 + raySize *i;
            Vector3 direction = GetDirectionalAngle(currAngle, true);
            Debug.DrawLine(this.transform.position, this.transform.position+direction*this.radius, Color.red);
            ViewRayHitInfo viewRayHitInfo = GetViewRayHitInfo(direction, radius,currAngle);
            viewRayHitInfos.Add(viewRayHitInfo);
        
       }

       // get renderer points
        Vector3[] vertexPoints = new Vector3[viewRayHitInfos.Count+1];
        int[] triangles = new int[(vertexPoints.Length-2)*3];
        vertexPoints[0] = Vector3.zero;
        for (int i = 1; i<viewRayHitInfos.Count; i++){
             vertexPoints[i] =  transform.InverseTransformPoint(viewRayHitInfos[i].hitPoint);
            
             if(i*3+2<triangles.Length){
                triangles[i*3] = 0;
                triangles[i*3+1]  = i;
                triangles[i*3+2] = i+1;
             }
        }

        mesh.Clear();
        mesh.vertices = vertexPoints;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

   }

   private ViewRayHitInfo GetViewRayHitInfo( Vector3 direction, float radius,float angle){
       RaycastHit rayInfo;
       bool isHit= Physics.Raycast(this.transform.position, direction, out rayInfo,radius,~targetMask);
       ViewRayHitInfo currRayHitInfo;
       if(isHit){
           Vector3 hitPoint = rayInfo.point;
           currRayHitInfo = new ViewRayHitInfo(true,hitPoint ,direction,radius, angle);
       }
       else{
           Vector3 maxPoint = this.transform.position + direction*radius;
           currRayHitInfo = new ViewRayHitInfo(false,maxPoint, direction, radius, angle);
       }

       return currRayHitInfo;
   }
    protected internal Vector3 GetDirectionalAngle(float angleDegree, bool isEuler=false) {
        if(!isEuler)
            angleDegree += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleDegree*Mathf.Deg2Rad),0, Mathf.Cos(angleDegree*Mathf.Deg2Rad));
    }

    protected internal void ChangeMeshColorMaterial(Color color){
        materialFilter.color = color;
    }

    struct ViewRayHitInfo {
        public bool isHit;
        public Vector3 hitPoint;
        public Vector3 direction;

        public float dist;
        public float angle;
        public ViewRayHitInfo (bool hit, Vector3 hitPoint, Vector3 direction, float dist,float angle){
            this.isHit = hit;
            this.hitPoint = hitPoint;
            this.direction = direction;
            this.dist = dist;
            this.angle = angle;
        }
     
    }
}
