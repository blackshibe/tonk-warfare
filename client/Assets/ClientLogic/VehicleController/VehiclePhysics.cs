using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

// https://www.youtube.com/watch?v=x0LUiE0dxP0
// https://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html

// todo: cleanup
public class VehiclePhysics : MonoBehaviour
{

    public Transform[] wheels = new Transform[5]; 
    float[] spring_list = new float[] {0,0,0,0}; 

    [Header("Main")]
    public Client client;
    public Rigidbody tank;
    public Transform turret; 
    public Transform elevator; 
    public Transform cannon; 
    public VolumeProfile volume_profile;

    [Header("Particles")]
    public ParticleSystem[] particles = new ParticleSystem[3];
    public ParticleSystem smoke;

    [Header("UI")]
    public Canvas first_person_ui;
    public TMP_Text ammo_text;
    public TMP_Text username_text;
    public TMP_Text health_text;
    public TMP_Text speed_text;
    public GameObject tank_image;
    public GameObject tank_overlay;
    public ClientChat chat;

    [Header("Audio")]
    public AudioSource pov_switch_sound;
    public AudioSource aim_switch_sound;
    public AudioSource fire_sound;
    public AudioSource treads_sound;
    public AudioSource active_engine_sound;
    public AudioSource idle_engine_sound;
    public AudioSource reload_sound;
    public AudioSource throttle_sound;
    public AudioSource interior_sound;
    public AudioSource interior_fire_sound;
    public AudioSource impact_sound;
    public AudioSource low_hp_sound;

    // implementing a transmission sounds hard so this is the alternative
    public float treads_max_vol_speed = 5f;
    public float idle_stfu_speed = 3f;
    public float idle_engine_sound_volume = 0.5f;
    public float active_engine_sound_volume = 0.5f;

    [Header("Suspension")]
    public float suspension_length = 0.67f;
    public float suspension_stiffness = 50000;
    public float rot_velocity = 130000f;
    public float sensitivity = 2f;
    public float damper_stiffness = 4000;
    public float max_force = 10000;
    public float rolling_resistance = 0.045f;

    [Header("Camera")]
    public Vector3 fp_offset = new Vector3(0.02f, 0.02f, 0.01f);
    public Vector3 offset = new Vector3(0, 0.04f, 0.25f);
    public float camera_rotation = 0;
    public float camera_pitch = 0;

    [Header("Stats")]
    public int current_ammo = 5;
    public int fire_delay_timeout = 100;
    public int magazine_ammo = 5;
    public int health = 500;
    public int max_health = 500;
    
    Delay reload_delay = new Delay(3500);
    Delay fire_delay = new Delay(0);
    Spring camera_spring = new Spring();

    bool first_person = false;
    bool reloading = false;
    bool aiming = false;

    Vector3 camera_position;
    Quaternion camera_quaternion;

    Vector3 tank_last_position = new Vector3();
    Quaternion tank_last_quaternion = new Quaternion();

    public ProjectileEmitter emitter;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        username_text.text = client.local_username;
        camera_spring.speed = 3;
        camera_spring.mass *= 2f;

        camera_position = Camera.main.transform.position;
        camera_quaternion = Camera.main.transform.rotation;
        first_person_ui.transform.SetParent(null);
    }

    public void Impact(int damage) {
        health -= damage;
    }

    void Update() {
        bool view_change = Input.GetKeyDown(KeyCode.V);
        bool throttle = Input.GetKeyDown(KeyCode.W);
        bool fire = Input.GetKeyDown(KeyCode.Mouse0);
        bool aim = Input.GetKeyDown(KeyCode.Mouse1);

        // todo: clean
        if (chat.textbox.isFocused) {
            view_change = false;
            throttle = false;
            fire = false;
            aim = false;
        }

        Camera world_camera = Camera.main;

        if (view_change) {
            pov_switch_sound.Play();
            first_person = !first_person;
             // transition shouldn't lerp
            if (!first_person) {
                camera_position = turret.transform.TransformPoint(offset);
                camera_quaternion = turret.transform.rotation * Quaternion.Euler(0, 180, 0);
            }
        }

        if (first_person && aim) { 
            aiming = !aiming; 
            aim_switch_sound.Play();
        }
        world_camera.fieldOfView = first_person ? (aiming ? 40 : 92) : 60;

        camera_rotation += Input.GetAxis("Mouse X") * sensitivity;
        camera_pitch += Input.GetAxis("Mouse Y") * sensitivity;
        camera_pitch = Mathf.Clamp(camera_pitch, -20, 35);

        turret.transform.localRotation = Quaternion.Euler(0, camera_rotation, 0);
        elevator.localRotation = Quaternion.Euler(camera_pitch, 0, 0);

        if (first_person) {
            // chromatic_aberration.active = true;
            // dof.active = true;
            // film_grain.active = true;
            // https://docs.unity3d.com/Packages/com.unity.render-pipelines.core@13.0/api/UnityEngine.Rendering.VolumeComponent.html
            volume_profile.components.ForEach(element => {
                element.active = true;
            });
            interior_sound.volume = 0.1f;
            reload_sound.volume = 0.5f;
            first_person_ui.enabled = true;
            camera_quaternion = elevator.transform.rotation * Quaternion.Euler(0, 180, 0);
            camera_position = elevator.transform.TransformPoint(fp_offset);
        } else {
            // updated in fixedupdate to prevent jittering
            // chromatic_aberration.active = false;
            // dof.active = false;
            // film_grain.active = false;
            // https://stackoverflow.com/questions/60513915/how-to-access-volume-post-processing-effects-by-script-in-unity
            volume_profile.components.ForEach(element => {
                if (element.name != "Bloom") element.active = false;
            });

            first_person_ui.enabled = false;
            interior_sound.volume = 0;
            reload_sound.volume = 0;
        }

        // cannon debug raycast
        RaycastHit cannon_hit;
        Vector3 from = cannon.position;
        Vector3 to = -cannon.transform.forward;
        if (Physics.Raycast(from, to, out cannon_hit, 200f)) {
            Debug.DrawLine(from, cannon_hit.point, Color.blue);
            Debug.DrawLine(cannon_hit.point, cannon_hit.point + cannon_hit.normal, Color.blue);
        }

        // fire sound
        fire_delay.timeout = fire_delay_timeout;
        if (fire && !reloading && fire_delay.expired()) {
            if (current_ammo > 0) {
                fire_sound.Play();
                if (first_person) {
                    interior_fire_sound.Play();
                }
                fire_delay.reset();
                current_ammo -= 1;
                emitter.Fire(cannon.position, -cannon.forward);            
                camera_spring.Shove(new Vector3(90, -10, 60));

                for (int i = 0; i < particles.Length; i++) {
                    ParticleSystem particle = particles[i];
                    if (!particle.isPlaying) {
                        particle.Play();
                    }
                    particle.Emit(30);
                }
            } else {
                reload_delay.reset();
                reload_sound.Play();
                reloading = true;
                reload_sound.Play();
            }

        }

        if (health < max_health * 0.2) {
            low_hp_sound.volume = 1;
        } else {
            low_hp_sound.volume = 0;
        }

        if (reloading) {
            ammo_text.text = "RELOADING";
        } else {
            ammo_text.text = $"{current_ammo}/{magazine_ammo}";
        }

        if (reload_delay.expired() && reloading) {
            reloading = false;
            current_ammo = magazine_ammo;
        }

        if (throttle) {
            throttle_sound.Play();
        }

        // speed display
        float velocity = Mathf.Floor((tank.velocity.magnitude * 60 * 60) / 1000);
        speed_text.text = $"{velocity} kph";

        // spring
        Vector3 position = camera_spring.Update(Time.deltaTime);
        Vector3 local_change_position = tank.transform.InverseTransformDirection(tank.position - tank_last_position) * 50;
        camera_spring.Shove(new Vector3(-local_change_position.z, local_change_position.x, 0));

        world_camera.transform.position = camera_position;
        world_camera.transform.rotation = camera_quaternion * Quaternion.Euler(position);

        tank_last_position = tank.position;
        tank_last_quaternion = tank.rotation;
    }

    // todo: clean
    void FixedUpdate() {

        Camera world_camera = Camera.main;
        float forwards_input = Input.GetAxis("Vertical");
        float side_input = Input.GetAxis("Horizontal");

        if (!first_person) {
            camera_quaternion = Quaternion.Lerp(camera_quaternion, turret.transform.rotation * Quaternion.Euler(-camera_pitch/2, 180, 0), Time.deltaTime * 10);
            camera_position = Vector3.Lerp(camera_position, turret.transform.TransformPoint(offset + new Vector3(0, -camera_pitch / 1000, 0)), Time.deltaTime * 10);
        }

        if (chat.textbox.isFocused) {
            forwards_input = 0;
            side_input = 0;
        }

        float speed = tank.velocity.magnitude + tank.angularVelocity.magnitude;
        float treads_volume = Mathf.Lerp(0, 1, speed/treads_max_vol_speed);
        float idle_volume = Mathf.Lerp(1, 0, speed/idle_stfu_speed);

        if (first_person) treads_volume *= 2;
        treads_sound.volume = treads_volume * active_engine_sound_volume;
        active_engine_sound.volume = treads_volume * active_engine_sound_volume;
        idle_engine_sound.volume = idle_volume * idle_engine_sound_volume;

        ParticleSystem.MinMaxCurve curve = smoke.main.startSpeed;
        curve.constant = treads_volume * 10f;
        
        // At low speeds the rolling resistance is the main resistance force, at high speeds the drag takes over in magnitude. 
        tank.AddForce(-rolling_resistance * tank.velocity, ForceMode.VelocityChange);
        
        for (int i = 0; i <= 3; i++) {
            Transform wheel = wheels[i];
            RaycastHit hit;
            Vector3 from = wheel.position;
            Vector3 to = wheel.position - wheel.transform.up * suspension_length;

            if (Physics.Raycast(from, -wheel.transform.up * suspension_length, out hit)) {
                Debug.DrawLine(from, to, Color.red);

                Vector3 local_wheel_velocity = transform.InverseTransformDirection(tank.GetPointVelocity(hit.point));

                float compression_distance = hit.distance;
                float force = suspension_stiffness * Mathf.Clamp((suspension_length - compression_distance), 0f, suspension_length);
                float velocity = (spring_list[i] - compression_distance) / Time.fixedDeltaTime;
                float damper_force = velocity * damper_stiffness;

                float force_forward = forwards_input * force;
                float force_side = -local_wheel_velocity.x * force;
                if (i <= 1) force_forward += side_input * rot_velocity * Time.fixedDeltaTime;
                else force_forward -= side_input * rot_velocity * Time.fixedDeltaTime;
                
                tank.AddForceAtPosition(
                    transform.up * Mathf.Clamp(force + damper_force, -max_force, max_force) 
                      + (transform.forward * force_forward) 
                      + (transform.right * force_side), 
                    hit.point);

                spring_list[i] = compression_distance;
            } else {
                Debug.DrawLine(from, to, Color.green);
            }
        }

        tank_overlay.transform.LookAt(cannon.transform.position + -cannon.transform.forward * 200f, cannon.transform.up);
        tank_overlay.transform.position = world_camera.transform.position + tank_overlay.transform.forward * 0.2f;
        tank_image.transform.rotation = world_camera.transform.rotation * Quaternion.Euler(0, 0, camera_rotation);
        first_person_ui.worldCamera = world_camera;
        first_person_ui.planeDistance = 0.2f;
    }
}
