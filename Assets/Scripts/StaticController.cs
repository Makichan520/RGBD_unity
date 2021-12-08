using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticController : MonoBehaviour
{
    private bool status = false;
    private bool is_Rendering = false;
    private float renderTime = 0;
    private Vector3 lastPosition;
    private float lastTime = 0;
    
    private float lastRenderTime = 0;
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
        if(lastTime != Time.time && renderTime != lastRenderTime){
            is_Rendering = true;
        }else{
            is_Rendering = false;
        }
        //LogInfos();
        lastPosition = transform.position;
        lastTime = Time.time;
        lastRenderTime = renderTime;
    }

    public bool isStatic(){
        return status;
    }

    public bool isRendering(){
        return is_Rendering;
    }

    private void OnWillRenderObject() {
        renderTime = Time.time;
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
