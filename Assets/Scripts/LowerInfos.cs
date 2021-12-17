using System;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Utilities;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine;

public class LowerInfos : MonoBehaviour
{
    public bool isTop = false;
    private Vector3 lastPosition;
    private float lastTime = 0;

    public bool areaSetManual = false;
    public Vector3Parameter area = new Vector3Parameter
    {
        x = new UniformSampler(0, 1),
        y = new UniformSampler(0, 1),
        z = new UniformSampler(0, 1)
    };

    public Vector3 rotate_euler= new Vector3(0,0,0);

    private float x_min = 0,x_max = 1,z_min = 0,z_max = 1;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        lastTime = Time.time;
        if(isTop && !areaSetManual){
            ObjectMeasure();
            area.x = new UniformSampler(x_min,x_max);
            area.z = new UniformSampler(z_min,z_max);
        }

    }


    private void ObjectMeasure(){
        if(GetComponent<MeshFilter>() == null){
            return;
        }
        Vector3 length_min = GetComponent<MeshFilter>().mesh.bounds.min * (transform.localScale.x * 0.9f);
        Vector3 length_max = GetComponent<MeshFilter>().mesh.bounds.max * (transform.localScale.x * 0.9f);
        x_min = length_min.x;
        x_max = length_max.x;
        z_min = length_min.z;
        z_max = length_max.z;
    }

    public void LogInfos(){
        Debug.Log(name + "in " + Time.frameCount + ". frame: pos = x:" + transform.position.x + "y: " + transform.position.y
        + "z: " + transform.position.z +  "\n rot = " + transform.rotation);
        Debug.Log(name + "infos: x -- " + x_min+ " " + x_max + "\nz--" + z_min + " " + z_max);
    }

    private void OnCollisionEnter(Collision other) {
        transform.rotation = Quaternion.Euler(rotate_euler.x,transform.eulerAngles.y,rotate_euler.z);

    }
}
