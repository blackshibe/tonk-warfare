using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

// todo: clean (probably)
public class ProjectileEmitter : MonoBehaviour {

    public struct ProjectileData {
        public TrailRenderer projectile;
        public Vector3 velocity;
    }

    public TrailRenderer projectile_trail;
    public ParticleSystem hit_particle;
    public List<ProjectileData> projectiles = new List<ProjectileData>();

    public void Fire(Vector3 origin, Vector3 direction) {
        Debug.DrawLine(origin, origin + direction, Color.white, 5f);

        ProjectileData projectile_data = new ProjectileData();
        projectile_data.projectile = Instantiate(projectile_trail, origin, new Quaternion());
        projectile_data.projectile.GetComponent<TrailRenderer>().emitting = true;
        projectile_data.velocity = direction.normalized;

        projectiles.Add(projectile_data);
    }

    public void Update() {
        for (int i = 0; i < projectiles.Count; i++) {
            ProjectileData data = projectiles[i];
            RaycastHit hit;

            // this is probably not at all how physics works but uhhh sure yeah
            Vector3 frame_velocity = data.velocity * 10f * Time.deltaTime * 60f;
            frame_velocity += new Vector3(0, -9.8f, 0) * Time.deltaTime; 
            
            if (Physics.Raycast(data.projectile.transform.position, frame_velocity, out hit)) {
                Destroy(data.projectile);

                Vector3 reflected_normal = Vector3.Reflect(data.velocity, hit.normal);

                // tfw variable names
                ParticleSystem hit_fx = Instantiate(hit_particle);
                hit_fx.transform.position = hit.point;
                hit_fx.transform.rotation = Quaternion.LookRotation(reflected_normal);

                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red, 5f);
                Debug.DrawLine(hit.point, hit.point + reflected_normal, Color.white, 5f);

                Destroy(hit_fx, 10f);

                Destroy(data.projectile);
                projectiles.RemoveAt(i);
            } else {
                data.projectile.transform.position += frame_velocity;
                projectiles[i] = data;
            }
        }
    }
}