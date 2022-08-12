using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AiFieldView))]
public class AiFieldViewEditor: Editor
{

    private AiFieldView fieldOfView;
    private void OnSceneGUI() {
        AiFieldView fieldOfView = (AiFieldView) target;
        Handles.color = Color.green;
        Vector3 currentPosition = fieldOfView.transform.position;

        // for circle
        Handles.DrawWireArc(currentPosition,Vector3.up,Vector3.forward, 360, fieldOfView.Radius);

        //for angle
        Vector3 viewAngleA = fieldOfView.GetDirectionalAngle(fieldOfView.Angle/2);
        Vector3 viewAngleB = fieldOfView.GetDirectionalAngle(-fieldOfView.Angle/2);
        Handles.DrawLine(currentPosition,currentPosition +viewAngleA*fieldOfView.Radius);
        Handles.DrawLine(currentPosition,currentPosition+viewAngleB*fieldOfView.Radius);

        // for player
        if(fieldOfView.InFieldView){
            Handles.DrawLine(currentPosition, fieldOfView.targetPos);
        }

  
    }
}
