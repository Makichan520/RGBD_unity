using System;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Perception.Randomization.Randomizers.Utilities;

/// <summary>
/// Supprot class for Prefeb used in Scenario, report the information of Prefeb Lower(Rotation,Lenght,Width,Height), also modify the collide behaviour of lower objects
/// </summary>
public class LowerInfos : MonoBehaviour
{
    [Tooltip("If this environment object like table(desk, table,etc.), set ture. Only when isTop = ture, other parameters are relevant")]
    public bool isTop = false;
    private Vector3 lastPosition;
    private float lastTime = 0;

    [Tooltip("If already obtained the length,width,height and rotation of the object, and want to set it manual, set ture")]
    public bool areaSetManual = false;

    [Tooltip("Manual area setting")]
    public Vector3Parameter area = new Vector3Parameter
    {
        x = new UniformSampler(0, 1),
        y = new UniformSampler(0, 1),
        z = new UniformSampler(0, 1)
    };

    [Tooltip("Manual rotation setting(euler angle)")]
    public Vector3 rotate_euler= new Vector3(0,0,0);

    private float x_min = 0,x_max = 1,z_min = 0,z_max = 1,y_max = 1;

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

    // Calculate 3d pose of object automatic
    private void ObjectMeasure(){
        if(GetComponent<MeshFilter>() == null){
            return;
        }
        Vector3 length_min = GetComponent<MeshFilter>().mesh.bounds.min * (transform.localScale.x * 0.8f);
        Vector3 length_max = GetComponent<MeshFilter>().mesh.bounds.max * (transform.localScale.x * 0.8f);
        x_min = length_min.x;
        x_max = length_max.x;
        z_min = length_min.z;
        z_max = length_max.z;
        y_max = length_max.y/0.8f;
    }

    public float getHeight(){
        return y_max;
    }

    public float getX(){
        return x_max-x_min;
    }

    public float getZ(){
        return z_max-z_min;
    }

    public void LogInfos(){
        Debug.Log(name + "in " + Time.frameCount + ". frame: pos = x:" + transform.position.x + "y: " + transform.position.y
        + "z: " + transform.position.z +  "\n rot = " + transform.rotation);
        Debug.Log(name + "infos: x -- " + x_min+ " " + x_max + "\nz--" + z_min + " " + z_max);
    }

    // Generate sample with deperate distance
    public NativeList<Unity.Mathematics.float2> generateSample(float distance,uint seed){
        var areaSample = PoissonDiskSampling.GenerateSamples(
        x_max-x_min, z_max-z_min, distance, seed);
        return  areaSample;
    }


}
