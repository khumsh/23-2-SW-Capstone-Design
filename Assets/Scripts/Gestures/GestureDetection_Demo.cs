using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public interface ILeftGesture
{
    public string targetName { get; }
    public GameObject targetGO { get; }
}

public enum LeftHandTargets
{
    SmartPhone,
    Ball,
    HumanAvatar
}

public enum LeftGestureMode
{
    Normal, // 일반 모드 (왼손 제스처에 따라 타겟 변경)
    TargetSetting, // 타겟 세팅 모드 (왼손 제스처를 오래 지속하면 해당 제스처를 타겟으로 TargetHold 모드로 변경)
    TargetHold, // 타겟 세팅 모드를 통해 타겟이 홀드된 모드 (취소 제스처를 통해 Normal 모드로 변경)
}

public class GestureDetection_Demo : MonoBehaviour
{
    public float threshold = 0.02f;// too small, will find nothing (too strict)
    public OVRSkeleton skeletonLeft;
    public OVRSkeleton skeletonRight;
    public List<Gesture> gesturesLeft;
    public List<Gesture> gesturesRight;
    public Gesture currentGesture_stable_Left;
    public Gesture currentGesture_stable_Right;
    public bool debugMode = true;
    public TextMeshPro ModeLogger;
    public bool writeModeLogger = true;
    public TextMeshPro GestureLoggerLeft;
    public TextMeshPro GestureLoggerRight;
    public List<OVRBone> fingerBonesLeft { get; private set; }
    public List<OVRBone> fingerBonesRight { get; private set; }
    private Gesture previousGestureLeft;
    private Gesture previousGestureRight;
    public bool thereAreBonesLeft { get; private set; } = false;
    public bool thereAreBonesRight { get; private set; } = false;
    private GameObject lefthand;
    private GameObject righthand;
    //Hand Interface
    [Header("Left Hand Targets")]
    public GameObject[] leftHandTargets;
    public Dictionary<string, ILeftGesture> leftHandTargetsDic;

    //Left
    LeftGestureMode leftGestureMode = LeftGestureMode.Normal;

    private GameObject joystick;
    private GameObject thumbpiano;
    private GameObject scissors;

    private GameObject smartphone;
    private GameObject ball;
    private GameObject humanAvatar;

    //Right

    private float startTime = 0f;
    private float timer = 0f;
    public float holdTime = 0.2f;

    private float changeModeTime = 2.5f;
    private float modeTimer;

    public GameObject MyTargetGO { get; set; }
    public GameObject targetGO;
    public string targetName;
   
    private Dictionary<string, GameObject> gesturedictLeft;
    private Dictionary<string, GameObject> gesturedictRight;
    // private GameObject[] assets;
    private string currentInterface;

    // Start is called before the first frame update
    void Start()
    {
        fingerBonesLeft = new List<OVRBone>(skeletonLeft.Bones);
        fingerBonesRight = new List<OVRBone>(skeletonRight.Bones);
        previousGestureLeft = new Gesture();  
        previousGestureRight = new Gesture();
        currentGesture_stable_Left = new Gesture();   
        currentGesture_stable_Right = new Gesture();
        lefthand = GameObject.FindGameObjectsWithTag("lefthand")[0];
        righthand = GameObject.FindGameObjectsWithTag("righthand")[0];

        //Hand Interface
        //Left
        // 타겟 넣기
        foreach(GameObject t in leftHandTargets)
        {
            ILeftGesture iLeft = t.GetComponent<ILeftGesture>();

            if (iLeft != null)
            {
                string targetName = iLeft.targetName;
                if (!leftHandTargetsDic.ContainsKey(targetName))
                {
                    leftHandTargetsDic.Add(targetName, iLeft);
                }
            }
            
        }

        // temp
        MyTargetGO = leftHandTargetsDic[LeftHandTargets.HumanAvatar.ToString()].targetGO;

        joystick = GameObject.FindGameObjectsWithTag("joystick")[0];
        thumbpiano = GameObject.FindGameObjectsWithTag("thumbpiano")[0];
        scissors = GameObject.FindGameObjectsWithTag("scissors")[0];
        // assets = GameObject.FindGameObjectsWithTag("assets");

        modeTimer = changeModeTime;

        gesturedictLeft = new Dictionary<string, GameObject>()
        {
            {"Joystick", joystick},
            {"ThumbPiano", thumbpiano},
            {"Scissors", scissors},
            {"Smartphone", smartphone},
            {"Ball", ball}
        };


        foreach(var ges in gesturedictLeft)
            //ges.Value.SetActive(false);
            ChildrenRendering(ges.Value, false);
        lefthand.GetComponent<Renderer>().enabled = true;
        righthand.GetComponent<Renderer>().enabled = true;
        // RelatedAssetsRendering();
    }

    // Update is called once per frame
    void Update()
    {
        if(!thereAreBonesLeft)
        {
            FindBonesLeft();
        }

        if (!thereAreBonesRight)
        {
            FindBonesRight();
        }


        if (thereAreBonesLeft)
        {
            //should not put it in save, 
            //or fingerBones will have nth when you don't press space
            fingerBonesLeft= new List<OVRBone>(skeletonLeft.Bones); //added
            if(debugMode && Input.GetKeyDown(KeyCode.F1))
            {
                Save();
            }

            Gesture currentGesture = Recognize();

            if (currentGesture.name != null)
            {
                if (currentGesture.name != previousGestureLeft.name){
                    startTime = Time.time;
                    timer = startTime;
                    // Debug.Log("Start to count for this gesture: "+currentGesture.name);
                } else if (currentGesture.name == previousGestureLeft.name){
                    timer += Time.deltaTime;
                    Debug.Log("Time: "+timer);
                    if (timer > (startTime + holdTime))
                    {
                        currentGesture_stable_Left = currentGesture;
                        // Debug.Log("get stable gesture: "+currentGesture_stable.name);
                    }
                }
                previousGestureLeft = currentGesture;


                switch(leftGestureMode)
                {
                    case LeftGestureMode.Normal:
                        if (currentGesture.name == "SetTarget")
                        {
                            if (modeTimer > 0)
                                modeTimer -= Time.deltaTime;
                            else if (modeTimer <= 0)
                            {
                                modeTimer = changeModeTime;
                                leftGestureMode = LeftGestureMode.TargetSetting;
                                break;
                            }
                        }
                        else modeTimer = changeModeTime;

                        if (leftHandTargetsDic.ContainsKey(currentGesture.name))
                        {
                            targetName = leftHandTargetsDic[currentGesture.name].targetName;
                            targetGO = leftHandTargetsDic[currentGesture.name].targetGO;
                        }
                        break;
                    case LeftGestureMode.TargetSetting:
                        if (leftHandTargetsDic.ContainsKey(currentGesture.name))
                        {
                            if (modeTimer > 0)
                                modeTimer -= Time.deltaTime;
                            else if (modeTimer <= 0)
                            {
                                modeTimer = changeModeTime;
                                targetName = leftHandTargetsDic[currentGesture.name].targetName;
                                targetGO = leftHandTargetsDic[currentGesture.name].targetGO;
                                leftGestureMode = LeftGestureMode.TargetHold;
                                break;
                            }
                                
                        }
                        else modeTimer = changeModeTime;

                        break;
                    case LeftGestureMode.TargetHold:
                        if (currentGesture.name == "CancelTarget")
                        {
                            if (modeTimer > 0)
                                modeTimer -= Time.deltaTime;
                            else if (modeTimer <= 0)
                            {
                                modeTimer = changeModeTime;
                                leftGestureMode = LeftGestureMode.Normal;
                                break;
                            }
                        }
                        else modeTimer = changeModeTime;

                        break;
                }


                //switch(currentGesture.name) 
                //{
                //    case "Scissors":
                //        targetGO = scissors.GetComponent<ILeftGesture>().targetGO;
                //        break;
                //    case "SmartPhone":
                //        targetGO = smartphone.GetComponent<ILeftGesture>().targetGO;
                //        break;
                //    case "Ball":
                //        targetGO = ball.GetComponent<ILeftGesture>().targetGO;
                //        break;
                //    case "HumanAvatar":
                //        targetGO = humanAvatar.GetComponent<ILeftGesture>().targetGO;
                //        break;
                //}
            }
            else
            {
                targetGO = null;
            }

            // bool hasRecognized = !currentGesture.Equals(new Gesture());
            // //check if gesture changes
            // if (hasRecognized && !currentGesture.Equals(previousGesture))
            // {
            //     previousGesture = currentGesture;
            //     currentGesture.onRecognized.Invoke();
            // }

            currentInterface = currentGesture.name;
            
            GestureLoggerLeft.text="LeftHand Current Gesture:"+currentGesture.name;
            HandInterfaceRendering(currentGesture);
            // RelatedAssetsRendering();
            if (writeModeLogger)
            {
                ModeLogger.text="SW Capstone Test";
            }
        }

        if (thereAreBonesRight)
        {
            //sh
            //ould not put it in save, 
            //or fingerBones will have nth when you don't press space
            fingerBonesRight = new List<OVRBone>(skeletonRight.Bones);//added
            if (debugMode && Input.GetKeyDown(KeyCode.F2))
            {
                SaveRight();
            }

            Gesture currentGestureRight = RecognizeRight();

            if (currentGestureRight.name != null)
            {
                if (currentGestureRight.name != previousGestureRight.name)
                {
                    startTime = Time.time;
                    timer = startTime;
                    // Debug.Log("Start to count for this gesture: "+currentGesture.name);
                }
                else if (currentGestureRight.name == previousGestureRight.name)
                {
                    timer += Time.deltaTime;
                    Debug.Log("Time: " + timer);
                    if (timer > (startTime + holdTime))
                    {
                        currentGesture_stable_Right = currentGestureRight;
                        // Debug.Log("get stable gesture: "+currentGesture_stable.name);
                    }
                }
                previousGestureRight = currentGestureRight;
            }

            // bool hasRecognized = !currentGesture.Equals(new Gesture());
            // //check if gesture changes
            // if (hasRecognized && !currentGesture.Equals(previousGesture))
            // {
            //     previousGesture = currentGesture;
            //     currentGesture.onRecognized.Invoke();
            // }

            currentInterface = currentGestureRight.name;

            GestureLoggerRight.text = "RightHand Current Gesture:" + currentGestureRight.name;
            //HandInterfaceRendering(currentGestureRight);
            // RelatedAssetsRendering();
            if (writeModeLogger)
            {
                ModeLogger.text = "SW Capstone Test";
            }
        }
    }

    void FindBonesLeft()
    {
        //if (new List<OVRBone>(skeleton.Bones).Count > 0)
        if (skeletonLeft.Bones.Count > 0)
        {
            //fingerBones= new List<OVRBone>(skeleton.Bones);//added
            thereAreBonesLeft = true;
        }
    }

    void FindBonesRight()
    {
        if (skeletonRight.Bones.Count > 0)
        {
            //fingerBones= new List<OVRBone>(skeleton.Bones);//added
            thereAreBonesRight = true;
        }
    }

    void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBonesLeft)
        {
            //finger position relative to root
            data.Add(skeletonLeft.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerDatas = data;
        gesturesLeft.Add(g);

    }

    void SaveRight()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBonesRight)
        {
            //finger position relative to root
            data.Add(skeletonRight.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerDatas = data;
        gesturesRight.Add(g);

    }

    public Gesture Recognize()
    {
        Gesture currentgesture = new Gesture();
        float currentMin = Mathf.Infinity;
        int testk = 0;
        
        foreach (var gesture in gesturesLeft)
        {
            //Debug.Log("debug mode is "+debugMode);
            
            float sumDistance = 0;
            bool isDiscarded = false;
            float adaptivethreshold;
            if (gesture.name == "Joystick"){
                adaptivethreshold = 0.06f;
                // 조이스틱은 손가락 잡고 움직일 때도 인식해야 하니까 임계값 좀 더 느슨하게
            }
            else {
                adaptivethreshold = threshold;
            } 
            
            for (int i = 0; i < fingerBonesLeft.Count; i++)
            {
                
                Vector3 currentData = skeletonLeft.transform.InverseTransformPoint(fingerBonesLeft[i].Transform.position);
                float distance = Vector3.Distance(currentData,gesture.fingerDatas[i]);
                if (distance > adaptivethreshold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentgesture = gesture;
            }
        }
        //Debug.Log("Current Gesture:"+currentgesture.name);
        return currentgesture;
    }

    public Gesture RecognizeRight()
    {
        Gesture currentgesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gesturesRight)
        {
            //Debug.Log("debug mode is "+debugMode);

            float sumDistance = 0;
            bool isDiscarded = false;
            float adaptivethreshold;
            if (gesture.name == "Run")
            {
                adaptivethreshold = 0.1f;
            }
            else
            {
                adaptivethreshold = threshold;
            }

            for (int i = 0; i < fingerBonesRight.Count; i++)
            {

                Vector3 currentData = skeletonRight.transform.InverseTransformPoint(fingerBonesRight[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if (distance > adaptivethreshold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentgesture = gesture;
            }
        }
        //Debug.Log("Current Gesture:"+currentgesture.name);
        return currentgesture;
    }

    void FindByNameRendering(string name, bool isEnabled)
    {
        GameObject Gobj = GameObject.Find(name);
        Gobj.GetComponent<Renderer>().enabled = isEnabled;
        ChildrenRendering(Gobj,isEnabled);
    }
    void RelatedAssetsRendering()
    {
        // foreach(var asset in assets)
        // {
        //     //disable themselves
        //     asset.GetComponent<Renderer>().enabled = false;
        //     //disable their children as well
        //     ChildrenRendering(asset, false);
        // }
        FindByLayerRendering(3,false);//cuttable layers

        if (currentInterface == "Joystick")
        {
            FindByNameRendering("JoystickControlledCube", true);
            FindByNameRendering("JoystickResetButton", true);
        }
        else if (currentInterface=="Scissors"){
            FindByLayerRendering(3,true);
        }
    }
    void FindByLayerRendering(int layNum,bool isEnabled)
    {
        GameObject[] cuttables = FindGameObjectsInLayer(layNum);
        foreach (GameObject cuttable in cuttables)
        {
            cuttable.GetComponent<Renderer>().enabled = isEnabled;
            //ChildrenRendering(Gobj,isEnabled);
        }       
    }
    GameObject[] FindGameObjectsInLayer(int layer)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == 3)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

    void HandInterfaceRendering(Gesture currentgesture)
    {
        string caseSwitch = currentgesture.name;

        foreach(var ges in gesturedictLeft)
            ChildrenRendering(ges.Value, false);
        if(caseSwitch!=null)
        {
            if (gesturedictLeft.ContainsKey(caseSwitch)){
                ChildrenRendering(gesturedictLeft[caseSwitch], true);
            }
        }
        
    }

    void ChildrenRendering(GameObject parent, bool isEnabled){
        foreach (Renderer r in parent.GetComponentsInChildren<Renderer>())
            r.enabled = isEnabled;
    }
 
}