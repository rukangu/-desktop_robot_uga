using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RuntimeGizmos;
using System.Threading.Tasks;
using System.IO;
using AGXUnity;
using unityutilities;
using UnityEngine.SceneManagement;
using System.IO;
public class PracticeSceneUI : MonoBehaviour
{
    public List<Transform> blocks;
    public List<Transform> targetLocations;
    string movementLogPath;
    float prev;
    //public Transform player;
    public float logInterval = 1.0f;
    public bool IK;
    public string folder;// = "D:/Andrew/git_repo/desktopRobot/study_data/practice_scene";
    string directory;
    string logDirectory;
    string eventLogPath;
    public string extension = ".txt";
    public FileHandler fileHandler;
    public TransformGizmo transformGizmo;
    public Transform target;
    targetmovement m_target; // this allows us to reset the target (align it with the hand before we do IK)
    public Transform orbitCam;
    public CanvasGroup TrajectoryControlUI, StopButtonCanvasGroup, RobotControlCanvasGroup;
    Vector3 originalPosition;
    Quaternion originalRotation;
    public Trajectory trajectory;
    public Transform TCP; // reference point for trajectories
    public PositionResetter PositionResetter;
    public string BlockTagName, LinkTagName;
    public JointControllerForAGXRobot RobotController;
    public float GripTime;
    //prefabs 
    public GameObject WayPointPrefab;
    public GameObject pointDisplayPrefab;
    public GameObject actionDisplayPrefab;
    public List<GameObject> displayObjects;
    int pointCount, actionCount;
    public GameObject Menu;
    // Start is called before the first frame update
    void Start()
    {
        prev = Time.time;
        m_target = target.GetComponent<targetmovement>();
        directory = Application.persistentDataPath + "/study_data/" + folder + "/Trajectories";
        logDirectory = Application.persistentDataPath + "/study_data/" + folder + "/Logs";
        string userIDPath = Application.persistentDataPath + "/study_data/userIDS";
        eventLogPath = logDirectory + "/" + DataManager.Instance.userID + "_actions";
        movementLogPath = logDirectory + "/" + DataManager.Instance.userID + "_movement";
        //Debug.Log(directory);
        //BlockTagName = "block"; // maybe use tags to independently reset items
        // LinkTagName = "RobotLink";
        pointCount = 0;
        actionCount = 0;
        originalPosition = orbitCam.position;
        originalRotation = orbitCam.rotation;
        //make the trajectory controls invisible and uniteractable
        HideCanvasGroup(TrajectoryControlUI);
        HideCanvasGroup(StopButtonCanvasGroup);
        // trajectory = new Trajectory();
        displayObjects = new List<GameObject>();

        // store transforms using agx method
        PositionResetter.StoreTransforms(BlockTagName,CheckPoint.Blocks);
        PositionResetter.StoreTransforms(LinkTagName, CheckPoint.RobotLink);
        PositionResetter.StoreTCPPosition(target);

        HandleLoadTrajectory();

        // start the logger
        //DataManager.Instance.myLogger.InitializeLogging(orbitCam, logInterval, DataManager.Instance.userID, logDirectory, userIDPath);
        CreateFolder(logDirectory);
        CreateFolder(directory);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Time.time - prev > logInterval)
        {   prev = Time.time;
            List<string> data = new List<string>();
            data.Add("blocks");
            foreach (Transform t in blocks)
            {
                data.Add(t.gameObject.name.ToString());
                data.Add(t.position.x.ToString());
                data.Add(t.position.y.ToString());
                data.Add(t.position.z.ToString());
                data.Add(t.rotation.w.ToString());
                data.Add(t.rotation.x.ToString());
                data.Add(t.rotation.y.ToString());
                data.Add(t.rotation.z.ToString());

            }
            data.Add("targetLocations");
            //Debug.Log("1:" + data.ToString());
            foreach (Transform t in targetLocations)
            {
                data.Add(t.gameObject.name.ToString());
                data.Add(t.position.x.ToString());
                data.Add(t.position.y.ToString());
                data.Add(t.position.z.ToString());
                data.Add(t.rotation.w.ToString());
                data.Add(t.rotation.x.ToString());
                data.Add(t.rotation.y.ToString());
                data.Add(t.rotation.z.ToString());

            }
            int i = 0;
            //find the distance between the blocks and the position
            float distance;
            float angle;
            foreach(Transform t in targetLocations)
            {
                distance = Vector3.Distance(t.position, blocks[i].position);
                angle = Quaternion.Angle(t.rotation, blocks[i].rotation);
                data.Add("distance"); data.Add(distance.ToString());
                data.Add("angle"); data.Add(angle.ToString());
                i++;
            }
            //Debug.Log("2:" + data.ToString());
            data.Add("player");
            data.Add(orbitCam.position.x.ToString());
            data.Add(orbitCam.position.y.ToString());
            data.Add(orbitCam.position.z.ToString());
            data.Add(orbitCam.rotation.w.ToString());
            data.Add(orbitCam.rotation.x.ToString());
            data.Add(orbitCam.rotation.y.ToString());
            data.Add(orbitCam.rotation.z.ToString());
            data.Add(SceneManager.GetActiveScene().name);
            Vector3 mousePos = Input.mousePosition;
            data.Add("mouse");
            data.Add(mousePos.x.ToString());
            data.Add(mousePos.y.ToString());
            //Debug.Log("3:" + data.ToString());
            unityutilities.Logger.LogRow(movementLogPath, data);
        }

        // log all mouse click events
        if (Input.GetMouseButtonDown(0))
        {
            Log("mouse_0_down", eventLogPath);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Log("mouse_1_down", eventLogPath);
        }
        if (Input.GetMouseButtonDown(2))
        {
            Log("mouse_2_down", eventLogPath);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Log("spacebar", eventLogPath);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Log("shiftkey", eventLogPath);
        }
    }
    // Update is called once per frame
    //void Update()
    //{
    //    if (Time.time - prev > logInterval)
    //    {
    //        prev = Time.time;
    //        List<string> data = new List<string>()
    //        {
    //            orbitCam.position.x.ToString(),
    //            orbitCam.position.y.ToString(),
    //            orbitCam.position.z.ToString(),
    //            orbitCam.rotation.w.ToString(),
    //            orbitCam.rotation.x.ToString(),
    //            orbitCam.rotation.y.ToString(),
    //            orbitCam.rotation.z.ToString(),
    //            SceneManager.GetActiveScene().name
    //            };
    //        unityutilities.Logger.LogRow(movementLogPath, data);
    //    }
    //}

    public void CreateFolder(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
    public void Log(string action, string path)
    {
        List<string> data = new List<string>()
        {action, SceneManager.GetActiveScene().name
        };
        unityutilities.Logger.LogRow(eventLogPath, data);
    }
    public void ResetCameraView()
    {
        orbitCam.position = originalPosition;
        orbitCam.rotation = originalRotation;
        Log("ResetCamera", eventLogPath);
    }
    public void StartTrajectoryCreation()
    {
        ShowCanvasGroup(TrajectoryControlUI);
        Log("startTrajectoryCreation", eventLogPath);

        // when button is clicked, start recording points

        //context switch : hide create trajectory button
        
        //show button to add points. display points as they are added
        
        // show button to play trajectory up to current point ( generalize it so that tou can send the trajectory to the robot and have it run)

    }
    public void StopTrajectoryCreation()
    {
        HideCanvasGroup(TrajectoryControlUI);
    }
    void HideCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 0.0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        // among other things
    }
    void ShowCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 1.0f;
        cg.blocksRaycasts = true;
        cg.interactable = true;
        // among other things
    }
    // UI Buttons clicked
    #region HANDLE UI BUTTON CLICKS
    public void HandleAddPoint()
    {
        // store the positions of the lock controllers of the constraints in the trajectory pose

        float[] angles = new float[RobotController.constraintArray.Length];
        int i = 0;
        foreach(Constraint constraint in RobotController.constraintArray)
        {
            // store value
            angles[i] = constraint.GetCurrentAngle();
            i++;
        }

        trajectory.pose.Add(angles);

        pointCount ++;
        // create the visual button to be added to the menu
        var b = Instantiate(pointDisplayPrefab, Vector3.zero, Quaternion.identity);
        string name = "point " + pointCount.ToString();
        b.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = name;
        WayPoint waypoint = b.gameObject.GetComponent<WayPoint>();
        displayObjects.Add(b);
        // create the waypoint sphere and place it at the current TCP location
        GameObject o = Instantiate(WayPointPrefab, TCP.position, TCP.rotation);
        Transform t = o.transform;
        waypoint.marker = o;
        waypoint.TCPTarget = target;
        waypoint.myName = name;
        waypoint.UI = this;
        waypoint.finger1Pos = RobotController.finger1Constraint.GetCurrentAngle();
        waypoint.finger2Pos = RobotController.finger2Constraint.GetCurrentAngle();
        PositionResetter.StoreTransforms(waypoint.myName, CheckPoint.All);
        trajectory.points.Add(t);
        trajectory.actions.Add(TrajectoryAction.MoveTCP);
        trajectory.timeDeltas.Add(trajectory.speed);
        UpdateTrajectoryVisuals();
        ShowGizmo();
        Log("AddPoint", eventLogPath);
    }

    public void HandleMoveRobotToPosition(WayPoint wayPoint)
    {
        PositionResetter.RestoreTransforms(wayPoint.myName);

        //reset finger position:
        RobotController.SetFingerPositions(wayPoint.finger1Pos, wayPoint.finger2Pos);
        //Debug.Log("moving to waypoint");
        ShowGizmo();
        Log("moveToWaypoint" + wayPoint.myName, eventLogPath);
    }

    public void HandleUndo()
    {
        int lastIdx = trajectory.points.Count - 1;
        if (lastIdx >= 0)
        {
            if(trajectory.actions[lastIdx] == TrajectoryAction.MoveTCP)
            {
                pointCount -= 1;
            }
            else
            {
                actionCount -= 1;
            }
            trajectory.UndoLastPoint(lastIdx);
        }

        // removing the item from the menu
        int idx = displayObjects.Count - 1;
        if(idx >= 0)
        {
            GameObject o = displayObjects[idx];
            displayObjects.RemoveAt(idx);
            GameObject.Destroy(o);
            UpdateTrajectoryVisuals();
        }
        ShowGizmo();
        Log("undo", eventLogPath);
    }
    public async void HandlePlay()
    {
       // Debug.Log("play");
        // Hide all the Interactable UI buttons except navigation panel
        HideCanvasGroup(RobotControlCanvasGroup);
        HideCanvasGroup(TrajectoryControlUI);
        //Show the Stop Play Button
        ShowCanvasGroup(StopButtonCanvasGroup);

        if (IK) //normal (using the tcp)
        {
            UseIK(true);
            await RobotController.ExecuteTrajectory(trajectory);
            // once trajectory returns Handle stop
            HandleStop();
        }
        else
        {
            //switch to FK
            UseIK(false);
            await RobotController.ExecuteTrajectory(trajectory);
        }


        Log("play", eventLogPath);

    }
    public async void HandleStop()
    {
        await RobotController.StopTrajectoryExecution();
        //UseIK(true); //return to using IK so the user can move the robot; make sure that you're resetting the followtarget
        HideCanvasGroup(StopButtonCanvasGroup);
        ShowCanvasGroup(TrajectoryControlUI);
        ShowCanvasGroup(RobotControlCanvasGroup);
        ShowGizmo();
        Log("stop", eventLogPath);
    }
    public void HandleCloseGripper()
    {
        // store the positions of the lock controllers of the constraints in the trajectory pose
        float[] angles = new float[RobotController.constraintArray.Length];
        int i = 0;
        foreach (Constraint constraint in RobotController.constraintArray)
        {
            // store value
            angles[i] = constraint.GetCurrentAngle();
            i++;
        }
        trajectory.pose.Add(angles); // placeholder
        actionCount++;
        trajectory.points.Add(TCP); // placeholder
        trajectory.actions.Add(TrajectoryAction.GripperClose);
        trajectory.timeDeltas.Add(trajectory.speed);
        var b = Instantiate(actionDisplayPrefab, Vector3.zero, Quaternion.identity);
        b.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "close gripper";
        displayObjects.Add(b);
        UpdateTrajectoryVisuals();
        ShowGizmo();
        Log("addCloseGripper", eventLogPath);
    }

    public void HandleOpenGripper()
    {   
        // store the positions of the lock controllers of the constraints in the trajectory pose
        float[] angles = new float[RobotController.constraintArray.Length];
        int i = 0;
        foreach (Constraint constraint in RobotController.constraintArray)
        {
            // store value
            angles[i] = constraint.GetCurrentAngle();
            i++;
        }

        trajectory.pose.Add(angles);// placeholder


        actionCount++;
        trajectory.points.Add(TCP); // placeholder
        trajectory.actions.Add(TrajectoryAction.GripperOpen);
        trajectory.timeDeltas.Add(trajectory.speed);
        var b = Instantiate(actionDisplayPrefab, Vector3.zero, Quaternion.identity);
        b.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "open gripper";
        displayObjects.Add(b);
        UpdateTrajectoryVisuals();
        ShowGizmo();
        Log("addOpenGripper", eventLogPath);
    }
    public void HandleResetPieces()
    { //also reset the follow target (or disable the constraint) second option is too convoluted


        PositionResetter.RestoreTransforms(LinkTagName);
        PositionResetter.RestoreTransforms(BlockTagName);
        PositionResetter.RestoreTCPPosition(target);
        ShowGizmo();
        Log("resetPieces", eventLogPath);
    }
    public void HandleShowGizmo()
    {
        ShowGizmo();
        Log("showGizmo", eventLogPath);
    }

    public void HandleSaveTrajectory()
    {
        //Debug.Log("json " + fileHandler.ConvertToJSON(trajectory));
        string json = fileHandler.ConvertToJSON(trajectory);
        fileHandler.WriteFile(directory, DataManager.Instance.userID+extension, json, FileMode.OpenOrCreate);
        ShowGizmo();
        Log("saveTrajectory", eventLogPath);
    }

    public void HandleLoadTrajectory()
    {
        string path;
        if (DataManager.Instance == null)
        {
            Debug.LogError("scene loaded directly and Datamanager not assigned yet. Loading test trajectory");
            //return;
            path =  directory + "/" + "test" + extension;
        }
        else
        {
            path = directory + "/" + DataManager.Instance.userID + extension;
        }
        //check if file exists in the directory

       // Debug.Log(path);
        if (System.IO.File.Exists(path))
        {
            //Debug.Log("user data available. loading it");
            string json = fileHandler.ReadFile(path);
            fileHandler.LoadFromJSON(json, target, ref trajectory, ref displayObjects, ref pointCount);
            UpdateTrajectoryVisuals();
            StartTrajectoryCreation();
        }
    }
    public void HandleLoadExercise()
    {
        MyUI.Instance.LoadScene2();
    }

    public void HandleLoadPractice()
    {
        MyUI.Instance.LoadScene1();
    }
    #endregion
    public void UpdateTrajectoryVisuals()
    {
        // update the UI with the trajectory points and actions
        foreach(GameObject o in displayObjects)
        {
            o.transform.SetParent(Menu.transform);
        }
    }

    public void ShowGizmo()
    {
        transformGizmo.HighlightTarget(target);
    }
    public void GripperUICommand(bool action)
    {
        StartCoroutine(GripperCoroutine(action));
    }
    public IEnumerator GripperCoroutine(bool action)
    {
        float endTime = GripTime + Time.time;
        while (Time.time < endTime)
        {
            RobotController.Grip(action);
            yield return null;
        }
        ShowGizmo();
    }

    public void UseIK(bool use)
    {
        if (use)
        {
            //robot follows target

            //first disable all the other lock controllers
            foreach(Constraint constraint in RobotController.constraintArray)
            {
                var lockController = constraint.GetComponent<LockController>();
                lockController.Enable = false;
            }

            // make sure the target gameObject is at the current robot location
            m_target.MoveToOrigin();

            //enable the target lock controller
            RobotController.targetConstraint.gameObject.SetActive(true);

        }
        else
        {
            //robot gets to joint positions


            //disable the follow target constraint to prevent the robot from following it
            RobotController.targetConstraint.gameObject.SetActive(false);
            //Then enable all the other lock controllers
            foreach (Constraint constraint in RobotController.constraintArray)
            {
                var lockController = constraint.GetComponent<LockController>();
                lockController.Enable = true;
            }

        }
    }
}
