using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMess : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<MeshFilter>() == null){
            return;
        }
        Vector3 length = GetComponent<MeshFilter>().mesh.bounds.size;
        float x = length.x;
        float y = length.y;
        float z = length.z;
        Debug.Log("Target object scale:\nX: " + x + "Y: " + y + "Z: " + z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
