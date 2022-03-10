using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for debug, measure the object to validate correctness of model
/// </summary>
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
