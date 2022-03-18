using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

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
    public Canvas first_person_ui;

    public VolumeProfile volume_profile;

    [Header("Audio")]
    public AudioSource pov_switch_sound;
    public AudioSource fire_sound;
    public AudioSource treads_sound;
    public AudioSource active_engine_sound;
    public AudioSource idle_engine_sound;
    public AudioSource reload_sound;

    // implementing a transmission sounds hard so this is the alternative
    public float treads_max_vol_speed = 5f;
    public float idle_stfu_speed = 3f;
    public float idle_engine_sound_volume = 0.5f;
    public float active_engine_sound_volume = 0.5f;

    float camera_rotation = 0;
    float camera_pitch = 0;

    Vector3 camera_position;
    Quaternion camera_quaternion;

    Vector3 tank_last_position = new Vector3();
    Quaternion tank_last_quaternion = new Quaternion();


    // Start is called before the first frame update
    void Start() {
        // Cursor.lockState = CursorLockMode.Locked;
        // camera_position = Camera.main.transform.position;
        // camera_quaternion = Camera.main.transform.rotation;
    }

    void Update() {
        // bool view_change = Input.GetKeyDown(KeyCode.V);
        // bool throttle = Input.GetKeyDown(KeyCode.W);
        // bool fire = Input.GetKeyDown(KeyCode.Mouse0);
        // bool aim = Input.GetKeyDown(KeyCode.Mouse1);

        // Camera world_camera = Camera.main;

        // if (view_change) {
        //     pov_switch_sound.Play();
        //     first_person = !first_person;
        //      // transition shouldn't lerp
        //     if (!first_person) {
        //         camera_position = turret.transform.TransformPoint(offset);
        //         camera_quaternion = turret.transform.rotation * Quaternion.Euler(0, 180, 0);
        //     }
        // }

        // if (first_person && aim) { 
        //     aiming = !aiming; 
        //     pov_switch_sound.Play();
        // }
        // world_camera.fieldOfView = first_person ? (aiming ? 40 : 92) : 60;

        // camera_rotation += Input.GetAxis("Mouse X") * sensitivity;
        // camera_pitch += Input.GetAxis("Mouse Y") * sensitivity;
        // camera_pitch = Mathf.Clamp(camera_pitch, -20, 35);

        // turret.transform.localRotation = Quaternion.Euler(0, camera_rotation, 0);
        // elevator.localRotation = Quaternion.Euler(camera_pitch, 0, 0);

        // // cannon debug raycast
        // RaycastHit cannon_hit;
        // Vector3 from = cannon.position;
        // Vector3 to = cannon.position - cannon.transform.forward * 200f;
        // if (Physics.Raycast(from, to, out cannon_hit)) {
        //     Debug.DrawLine(from, cannon_hit.point, Color.blue);
        // } else {
        //     Debug.DrawLine(from, to, Color.red);
        // }

        // // spring
        // Vector3 position = camera_spring.Update(Time.deltaTime);
        // Vector3 local_change_position = tank.transform.InverseTransformDirection(tank.position - tank_last_position) * 50;
        // camera_spring.Shove(new Vector3(-local_change_position.z, local_change_position.x, 0));

        // world_camera.transform.position = camera_position;
        // world_camera.transform.rotation = camera_quaternion * Quaternion.Euler(position);

        // tank_last_position = tank.position;
        // tank_last_quaternion = tank.rotation;
    }

    void FixedUpdate() {

        // Camera world_camera = Camera.main;

        // if (!first_person) {
        //     camera_quaternion = Quaternion.Lerp(camera_quaternion, turret.transform.rotation * Quaternion.Euler(-camera_pitch/2, 180, 0), Time.deltaTime * 10);
        //     camera_position = Vector3.Lerp(camera_position, turret.transform.TransformPoint(offset), Time.deltaTime * 10);
        // }
        
        // bool forwards = Input.GetKey(KeyCode.W);
        // bool backwards = Input.GetKey(KeyCode.S);
        // bool left = Input.GetKey(KeyCode.A);
        // bool right = Input.GetKey(KeyCode.D);
        // float forwards_input = Input.GetAxis("Vertical");

        // float speed = tank.velocity.magnitude + tank.angularVelocity.magnitude;
        // float treads_volume = Mathf.Lerp(0, 1, speed/treads_max_vol_speed);
        // float idle_volume = Mathf.Lerp(1, 0, speed/idle_stfu_speed);

        // if (first_person) treads_volume *= 2;
        // treads_sound.volume = treads_volume * active_engine_sound_volume;
        // active_engine_sound.volume = treads_volume * active_engine_sound_volume;
        // idle_engine_sound.volume = idle_volume * idle_engine_sound_volume;

        // // At low speeds the rolling resistance is the main resistance force, at high speeds the drag takes over in magnitude. 
        // tank.AddForce(-rolling_resistance * tank.velocity, ForceMode.VelocityChange);
        
        // for (int i = 0; i <= 3; i++) {
        //     Transform wheel = wheels[i];
        //     RaycastHit hit;
        //     Vector3 from = wheel.position;
        //     Vector3 to = wheel.position - wheel.transform.up * suspension_length;

        //     if (Physics.Raycast(from, -wheel.transform.up * suspension_length, out hit)) {
        //         Debug.DrawLine(from, to, Color.red);

        //         Vector3 local_wheel_velocity = transform.InverseTransformDirection(tank.GetPointVelocity(hit.point));

        //         float compression_distance = hit.distance;
        //         float force = suspension_stiffness * Mathf.Clamp((suspension_length - compression_distance), 0f, suspension_length);
        //         float velocity = (spring_list[i] - compression_distance) / Time.fixedDeltaTime;
        //         float damper_force = velocity * damper_stiffness;

        //         float force_forward = forwards_input * force;
        //         float force_side = -local_wheel_velocity.x * force;
        //         if (i <= 1) force_forward += Input.GetAxis("Horizontal") * rot_velocity * Time.fixedDeltaTime;
        //         else force_forward -= Input.GetAxis("Horizontal") * rot_velocity * Time.fixedDeltaTime;
                
        //         tank.AddForceAtPosition(
        //             transform.up * Mathf.Clamp(force + damper_force, -max_force, max_force) 
        //               + (transform.forward * force_forward) 
        //               + (transform.right * force_side), 
        //             hit.point);

        //         spring_list[i] = compression_distance;
        //     } else {
        //         Debug.DrawLine(from, to, Color.green);
        //     }
        // }
    }
}
