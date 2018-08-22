using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovementAndPlacement : MonoBehaviour {
    public float move_speed = 0.3f;
    public float rotate_speed = 1;

    public Camera Camera_Prefab;

    private float input_forward;
    private float input_horizontal;
    private float input_vertical;
    private float input_pitch;
    private float input_yaw;
    private float input_roll;

    private Scene scene;
    private string cameraFilePath = null;
    private Camera mainCamera;
    private Camera activeCamera;
    private int activeCameraNum = 0;
    private List<Camera> cameras = new List<Camera>();

    private string Vector3String(Vector3 v)
    {
        var arr = new string[] { v.x.ToString(), v.y.ToString(), v.z.ToString() };
        return string.Join(",", arr);
    }

    private Vector3 Vector3FromString(string line)
    {
        Vector3 result = new Vector3();
        var data = line.Split(',');
        result.x = float.Parse(data[0]);
        result.y = float.Parse(data[1]);
        result.z = float.Parse(data[2]);
        return result;
    }

    private Camera makeCamera(Vector3 pos, Vector3 rotation)
    {
        Camera result = Instantiate(Camera_Prefab);
        result.CopyFrom(mainCamera);
        result.transform.position = pos;
        result.transform.eulerAngles = rotation;
        result.enabled = false;
        result.GetComponent<AudioListener>().enabled = false;
        return result;
    }

    private void writeCameraFile()
    {
        StreamWriter writer = new StreamWriter(cameraFilePath, false);
        int n = 1;
        foreach(Camera c in cameras)
        {
            if(c != mainCamera)
            {
                writer.WriteLine("Camera" + n.ToString());
                writer.WriteLine(Vector3String(c.transform.position));
                writer.WriteLine(Vector3String(c.transform.eulerAngles));
                n++;
            }
        }
        writer.Close();
    }

    private List<Camera> readCameraFile()
    {
        List<Camera> result = new List<Camera>();
        if (File.Exists(cameraFilePath))
        {
            StreamReader r = new StreamReader(cameraFilePath);
            string line;
            while ((line = r.ReadLine()) != null)
            {
                if (line.Contains("Camera"))
                {
                    Vector3 pos = Vector3FromString(r.ReadLine());
                    Vector3 rot = Vector3FromString(r.ReadLine());
                    //print("Loading Camera at " + pos + " " + rot);
                    result.Add(makeCamera(pos, rot));
                }
                else
                {
                    r.Close();
                    break;
                }
            }
            r.Close();
        }
        return result;
    }

    private void saveCamera()
    {
        Vector3 pos = mainCamera.transform.position;
        Vector3 rot = mainCamera.transform.eulerAngles;
        if (File.Exists(cameraFilePath))
        {
            foreach(Camera c in readCameraFile())
            {
                if ((pos - c.transform.position).magnitude < .1f)
                {
                    if ((rot - c.transform.eulerAngles).magnitude < .1f)
                    {
                        print("That camera already exists");
                        print("At " + pos + " " + rot);
                        return;
                    }
                }
            }
        }
        print("Adding Camera at " + pos + " " + rot);
        Camera cam = makeCamera(pos, rot);
        cam.name = "Camera" + cameras.Count;
        cameras.Add(cam);
        writeCameraFile();
    }

    private void removeCamera()
    {
        activeCamera.enabled = false;
        print("Removing Camera" + activeCameraNum);
        cameras.Remove(activeCamera);
        Destroy(activeCamera);
        activeCamera = mainCamera;
        activeCameraNum = 0;
        activeCamera.enabled = true;
        writeCameraFile();
    }

    private void changeCamera()
    {
        activeCamera.enabled = false;
        activeCameraNum += 1;
        if(activeCameraNum >= cameras.Count)
        {
            activeCamera = cameras[0];
            activeCameraNum = 0;
        }
        else
        {
            activeCamera = cameras[activeCameraNum];
        }
        activeCamera.enabled = true;
    }

    private void moveCamera(Camera c)
    {
        
        getInputs();
        Vector3 translate = new Vector3(input_horizontal, input_vertical, input_forward) * move_speed;
        Vector3 rotate = new Vector3(input_pitch, input_yaw, input_roll) * rotate_speed;
        c.transform.Translate(translate);
        c.transform.Rotate(rotate);
    }

    private void getInputs()
    {
        input_forward = Input.GetAxis("Forward");
        input_horizontal = Input.GetAxis("Horizontal");
        input_vertical = Input.GetAxis("Vertical");
        input_pitch = Input.GetAxis("Pitch");
        input_yaw = Input.GetAxis("Yaw");
        input_roll = Input.GetAxis("Roll");
    }

	// Use this for initialization
	void Start () {
        if (Camera.main)
        {
            mainCamera = Camera.main;
            activeCamera = Camera.main;
        }
        else
        {
            mainCamera = new Camera();
            activeCamera = mainCamera;
        }
        scene = SceneManager.GetActiveScene();
        cameraFilePath = scene.path;
        cameraFilePath = cameraFilePath.Replace(scene.name, scene.name + "_Camera.txt").Replace(".unity", "");
        cameras.Add(mainCamera);
        int i = 1;
        foreach(Camera c in readCameraFile())
        {
            c.name = "Camera" + i.ToString();
            print("Loaded " + c.name + " at " + c.transform.position + " " + c.transform.eulerAngles);
            cameras.Add(c);
            i++;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(activeCamera == mainCamera)
        {
            moveCamera(activeCamera);
            if (Input.GetKeyDown("return"))
            {
                saveCamera();
            }
        }
        if(activeCamera != mainCamera)
        {
            if (Input.GetKeyDown("delete") || Input.GetKeyDown("backspace"))
            { 
                removeCamera();
            }
            if (Input.GetKeyDown("home"))
            {
                while (activeCamera != mainCamera)
                {
                    changeCamera();
                }
            }
        }
        if (Input.GetKeyDown("tab"))
        {
            changeCamera();
        }
        
	}
}
