using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Report the status of target object, estimate if it is static
/// </summary>
public class StaticController : MonoBehaviour
{
    private bool status = false;
    private Vector3 lastPosition;
    private float lastTime = 0;

    private float last_render = 0;

    private float curr_render = 0;

    private bool is_Rendering = false;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        lastTime = Time.time;
        ObjectMeasure();
    }

    // Update is called once per frame
    void Update()
    {
        if(lastTime != Time.time && lastPosition == transform.position){
            status = true;
        }else{
            status = false;
        }
        //LogInfos();
        lastPosition = transform.position;
        lastTime = Time.time;

        is_Rendering = curr_render != last_render ? true:false;
        last_render = curr_render;
    }

    public bool isStatic(){
        return status;
    }
    public bool isRendering(Camera cam) {
        Transform camTransform = cam.transform;
        Vector2 viewPos = cam.WorldToViewportPoint(this.transform.position);
        if(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1){
            is_Rendering = true;
        }else{
            is_Rendering = false;
        }
        return is_Rendering; 
        }
 


    private void OnWillRenderObject() {
        curr_render = Time.time;
    }

    private void ObjectMeasure(){
        if(GetComponent<MeshFilter>() == null){
            return;
        }
        Vector3 length = GetComponent<MeshFilter>().mesh.bounds.min;
        float x = length.x;
        float y = length.y;
        float z = length.z;
        Debug.Log("Target object min scale:\nX: " + x + " Y: " + y + " Z: " + z);
    }

    private void LogInfos(){
        Debug.Log(name + "in " + Time.frameCount + ". frame: pos = x:" + transform.position.x + "y: " + transform.position.y
        + "z: " + transform.position.z +  "\n rot = " + transform.rotation);
    }
}
