# Guide for generating RGB-D dataset with Unity



​	The purpose of this guide is to briefly explain how to automate the generation of RGBD datasets using this Unity program. Since this project makes extensive use of the [Perception package](https://github.com/Unity-Technologies/com.unity.perception), users can refer to the README in the [Perception](https://github.com/Unity-Technologies/com.unity.perception) to work with it.

If you just want to do data generation, you can refer to the easy tutorial in the perception package first, and then follow the guidelines in section Ⅰ to proceed.



##   Preliminary preparation

1. Install Unity (2020.3.20f1)
2. Clone the repository with https://github.com/Makichan520/RGBD_unity.git
3. If you want to make changes to the code, please configure your development environment with Unity.

______________



## Ⅰ. Randomization configuration

​	Open the Assets folder in Unity, find a suitable theme in the Scenes folder (office, industrial area, bedroom, etc.), open the Scene. <Only available for office scenarios so far>

<img src="https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/QQ图片20220306133826.jpg" alt="QQ图片20220306133826" style="zoom:80%;" /> 

A space for generating items has been added to the pre-defined office scene (a simple office room), we can find all GameObjects already added in Hierarchy window:

![image-20220306134423485](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306134423485.png) 



___

- **Scenario configuration**

The main part for randomization is Scenario, Scenario is container for all randomizers, and is responsible for regulating the runtime of the entire program, random cycles. By default, the scenario is Fixlength, this Scenario runs each Iteration for a fixed number of frames.

![image-20220306140111416](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306140111416.png) 

(Hover the cursor over the corresponding item from Inspector to see the tips)

Normally, we only need to modify Random Seed, Total Iterations and Frames Per Iteration, we take one image per iteration in this project.

——more details on [[Scenarios.md](https://github.com/Unity-Technologies/com.unity.perception/blob/main/com.unity.perception/Documentation~/Randomization/Scenarios.md)]

------------------------

- **Randomizer configuration**

Randomizers encapsulate specific randomization activities to perform during the execution of a randomized simulation. Different randomizers are able to randomize for various specific environments, for example **RotationRandomizer**, which randomizes the rotation of objects tagged with a **RotationRandomizerTag**.

![image-20220306142039897](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306142039897.png) 

Randomizer Tag are components used to allow randomizers to identify targets that need to be randomized, and can be added to any game object.



In this project, we already added 4 randomizer: SunAngleRandomizer, BrightnessRandomizer, CameraRandomizer and BackgroundRandomizer. The first two are related to the control of lighting conditions, i.e., the orientation of the directional light and the intensity of any light source. The other two are important randomizers related to the scene.

The main process of scene randomization is: randomly generate a scene at the beginning of a large period, randomly select a desktop in the scene, randomly generate target objects and other small objects within the desktop, after generation and scene stabilization, the camera starts to shoot around the desktop, reset the scene after a certain number of shots, and carry out the next cycle of random.

1. BackgroundRandomizer

![image-20220306160059788](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306160059788.png) 

**Capture Frames**: Number of image in each big period.

**Wait Frames**: After object generating, the camera must wait for all object to become static. In one period, scenario should generate 2 times, so : Overall Frame Number = Wait Frames * 2 + Capture Frames

**Depth**: Initial height for generating of large objects, the objects will fall onto the ground and wait for capture.

**Layer Count**: Objects are spawned at random locations on a Layer, Layer count show the number of layer include the Prefabs lower and Prefabs upper(Layer Count must be larger than 0)

**Max Top Object**: Maximum number of small objects to be created on the desktop.

**Separation Distance**: The minimum distance between the centers of placed objects on table(target objects)

**Separation Distance_lower**: The minimum distance between the centers of placed objects below(prefabs lower and option)

**Separation Distance_upper**: The minimum distance between the centers of placed objects in the upper(prefabs upper)

**Placement Area**: Specify the area where the object will be generated, area center is (0,0,0) by default.

**Prefabs Lower**: GameObject list for large objects, which should be table-like and first generated then fall onto the ground.

**Prefabs Upper**: GameObject list for small objects, which should be generated above the Prefabs Lower.

**Prefabs Option**: GameObject list for large objects, which should be first generated then fall onto the ground but not table-like.

**Top number**: Indicate how many table-like object should be generated in a period.

**Targets**: Define the targets should be captured. (should be added in scene before randomize)



However, the environment object and target object should have some components, if you want to add new object or target, you can refer the component setting of existing gameObjects:

![image-20220306161818014](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306161818014.png) (target object)

![image-20220306163700524](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306163700524.png)(environment object)



2. Camera Randomizer

![image-20220306163915852](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306163915852.png) 

**Start At Frame**: Specify which frame in the cycle to start the capture, generally equal to 2*waitFrames

**Capture Frames**: Same as in BackgroundRandomizer.

**Rotation_z**: Range of random rotation in axis z

**Distance**: Range of random distance between camera and targets.

**Camera**: Perception camera(also as Depth Camera)

**Targets**: Define the targets should be captured. (should be added in scene before randomize)



_____________

## Ⅱ. Labeling Configuration

General labelers are already added to Perception Camera, they can generate specific information about ground truth, and output into JSON files at ending of generation.

As a user, you just need to add a **Labeling component** to the target when you add a new object (either a target or an environment object), write the label of the corresponding item in it, and finally add that label to the corresponding Labeling config.

![image-20220306172448929](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306172448929.png)(Example for target object: Duck)

The **Label Config** acts as a mapping between string labels and object classes (currently colors or integers), deciding which labels in the Scene (and thus which objects) should be tracked by the Labeler, and what color (or integer id) they should have in the captured frames.

![image-20220306172809703](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306172809703.png) 

(ID label config, semantic label config is similar to this)

These config will eventually be used in the perception camera settings.

____



## Ⅲ. Camera configuration

​	There is a detailed description of the perception camera on [guide page from perception package](https://github.com/Unity-Technologies/com.unity.perception/blob/main/com.unity.perception/Documentation~/PerceptionCamera.md), so here we will only explain the other components of the camera

​	![image-20220306181906248](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220306181906248.png)

​	Component **Depth Camera** should be also added in Camera object, recommended settings as above, user should just  modify the **Base Directory**(output path for depth graphs)

​	Component **Custom Reporter** can report the information about target(position,rotation etc.) and write it into JSON files. You can insert new target object that should be focused into the list.

------------

## 

## Ⅳ. Generate dataset

After adjusting all the above settings, click the "Play" button **▷** at the top to start the simulation. RGB files can eventually be found in the Perception Camera viewer, and Depth Graph is generated at the location you specify.

![img](https://github.com/Unity-Technologies/com.unity.perception/raw/main/com.unity.perception/Documentation~/Tutorial/Images/play.png)

If you have any questions, please refer to the [quick tutorial](https://github.com/Unity-Technologies/com.unity.perception/blob/main/com.unity.perception/Documentation~/Tutorial/Phase1.md) in the perception package