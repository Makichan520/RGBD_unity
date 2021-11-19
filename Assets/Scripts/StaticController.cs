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
}
