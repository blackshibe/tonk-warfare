using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=x0LUiE0dxP0
public class VehiclePhysics : MonoBehaviour
{
    public Transform[] wheels = new Transform[5]; 
    float[] spring_list = new float[] {0,0,0,0}; 

    public Rigidbody car;
    public Camera world_camera;
    public Transform turret; 

    public float suspension_length;
    public float suspension_stiffness;
    public float rot_velocity = 200f;
    public float damper_stiffness;
    public float speed;
    public float wheel_radius = 0.33f;
    public float max_force;
    public float max_speed;

    public Vector3 offset;

    float camera_rotation = 0;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        
    }

    Vector3 mul_vec3_vec3(Vector3 a, Vector3 b) {
        return new Vector3(a.x *  b.x, a.y * b.y, a.z * b.z);
    }

    void FixedUpdate() {

        world_camera = Camera.main;
        
        bool forwards = Input.GetKey(KeyCode.W);
        bool backwards = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        float forwards_input = Input.GetAxis("Vertical");

        camera_rotation += Input.GetAxis("Mouse X");
        turret.transform.localRotation = Quaternion.Euler(0, camera_rotation, 0);
        world_camera.transform.rotation = Quaternion.Lerp(world_camera.transform.rotation, turret.transform.rotation * Quaternion.Euler(0, 180, 0), Time.deltaTime * 10);
        world_camera.transform.position = Vector3.Lerp(world_camera.transform.position, turret.transform.TransformPoint(offset), Time.deltaTime * 10);

        for (int i = 0; i <= 3; i++) {
            Transform wheel = wheels[i];
            RaycastHit hit;
            Vector3 from = wheel.position;
            Vector3 to = wheel.position - wheel.transform.up * suspension_length;

            if (Physics.Raycast(from, -wheel.transform.up * suspension_length, out hit)) {
                Debug.DrawLine(from, to, Color.red);

                float compression_distance = hit.distance;
                float force = suspension_stiffness * Mathf.Clamp((suspension_length - compression_distance), 0f, suspension_length);
                float velocity = (spring_list[i] - compression_distance) / Time.fixedDeltaTime;
                float damper_force = velocity * damper_stiffness;

                Vector3 local_wheel_velocity = transform.InverseTransformDirection(car.GetPointVelocity(hit.point));
                float factored_speed = -local_wheel_velocity.z;
                float fZ = factored_speed * ((1 - Mathf.Abs(forwards_input)) * 1000) + forwards_input * speed * force;
                float fX = local_wheel_velocity.x * force;
                if (i <= 1) fZ += Input.GetAxis("Horizontal") * rot_velocity * Time.fixedDeltaTime;
                else fZ -= Input.GetAxis("Horizontal") * rot_velocity * Time.fixedDeltaTime;
                
                car.AddForceAtPosition(transform.up * Mathf.Clamp(force + damper_force, -max_force, max_force) + (transform.forward * fZ) + (-transform.right * fX), hit.point);
                spring_list[i] = compression_distance;
            } else {
                Debug.DrawLine(from, to, Color.green);
            }
        }
    }
}
