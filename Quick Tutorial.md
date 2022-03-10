# Quick tutorial for RGB-D Unity

​	

1. Install Unity (2020.3.20f1)
2. Clone the repository with https://github.com/Makichan520/RGBD_unity.git
3. If you want to make changes to the code, please configure your development environment with Unity.
4. Find Scene directory in <Project> window of Unity, open the “Office Scene”
5. Configure the Randomizer in Scenario(you can find it in Hierarchy window below), please refer the guideline in README, part Ⅰ.
6. Configure the perception camera and depth camera, please refer the guideline in README, part Ⅲ.
7. If you want to change the target objects (in the sample case are Cube_roundedge_01 and Duck), please add the gameObject into the Scene, add component on it refer to components of Duck. Also modify the labeling config according to the target labels.
8. Click the play button ▷ right above the center of the window![image-20220308162924369](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220308162924369.png)

​		You can check the data in Lastest Output Folder in component Perception Camera:

![image-20220308163112500](https://gitee.com/Makichan520/pics/raw/master/Picgo+Gitee图床/image-20220308163112500.png)