using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class Spawning_Manager : MonoBehaviour
{
    public GameObject AvatarPrefab;
    [HideInInspector]
    public GameObject avatar;
    private ARRaycastManager aRRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector3 ten_meters_from_Device;
    [HideInInspector]
    public int location = 0;
    [HideInInspector]
    public bool spawner_configuration_done = false;
    private Animation_Manager animation_Manager;
    private Vector3 temp, avatarBasePosition;
    public ARPlaneManager ARPlaneManager;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        ten_meters_from_Device = new Vector3(0.0f, 0.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon) && spawner_configuration_done == false)
        {
            var hitPose = hits[0].pose;

            if (avatar == null)
                avatar = Instantiate(AvatarPrefab, hitPose.position, Quaternion.identity);
            else
            {
                temp = ten_meters_from_Device + new Vector3(-hitPose.position.x, 0.0f, -hitPose.position.z);
                avatar.transform.position += temp;
                avatarBasePosition = avatar.transform.position;
                avatar.AddComponent<Animation_Manager>();
                animation_Manager = avatar.GetComponent<Animation_Manager>();
                avatar.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                ARPlaneManager.SetTrackablesActive(false);
                Destroy(ARPlaneManager);
                spawner_configuration_done = true;
            }
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && spawner_configuration_done == true)
        {
            if (location == 0)
            {
                int[] choices = new int[] { 1, 2 };
                Next_Location_Calculation(choices);
            }
            else if (location == 1)
            {
                int[] choices = new int[] { 0, 2 };
                Next_Location_Calculation(choices);
            }
            else if (location == 2)
            {
                int[] choices = new int[] { 0, 1 };
                Next_Location_Calculation(choices);
            }
        }
    }

    void Next_Location_Calculation(int[] choices_array)
    {
        int[] choices = choices_array;
        int howManyChoices = choices.Length;
        int RandomIndex = Random.Range(0, howManyChoices);
        int next_location = choices[RandomIndex];
        location = next_location;
        animation_Manager.animator.SetBool("IsWalking", false);
        animation_Manager.configuration_done = false;

        avatar.transform.position = avatarBasePosition;
        if (location == 1)
            avatar.transform.position = avatarBasePosition + new Vector3(0.0f, 0.0f, 15.0f);
        else if (location == 2)
            avatar.transform.position = avatarBasePosition + new Vector3(0.0f, 0.0f, 30.0f);
        else if (location == 0)
            avatar.transform.position = avatarBasePosition;
    }
}
