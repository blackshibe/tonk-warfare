using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using TMPro;

// https://www.youtube.com/watch?v=x0LUiE0dxP0
// https://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html

// todo: cleanup
public class ReplicatedVehiclePhysics : MonoBehaviour
{

    [Header("Main")]
    public Rigidbody tank;
    public Transform turret; 
    public Transform elevator; 
    public Transform cannon; 
    public TMP_Text banner; 

    [Header("Audio")]
    public AudioSource fire_sound;
    public AudioSource treads_sound;
    public AudioSource active_engine_sound;
    public AudioSource idle_engine_sound;

    // implementing a transmission sounds hard so this is the alternative
    public float treads_max_vol_speed = 5f;
    public float idle_stfu_speed = 3f;
    public float idle_engine_sound_volume = 0.5f;
    public float active_engine_sound_volume = 0.5f;

    [Header("Other")]
    public float camera_rotation = 0;
    public float camera_pitch = 0;

    // Start is called before the first frame update
    void Start() {}

    void Update() {
        turret.transform.localRotation = Quaternion.Euler(0, camera_rotation, 0);
        elevator.localRotation = Quaternion.Euler(camera_pitch, 0, 0);
    }

    void FixedUpdate() {}
}
