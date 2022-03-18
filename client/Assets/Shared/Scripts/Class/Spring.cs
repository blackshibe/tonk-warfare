
using UnityEngine;

public class Spring {
    const int ITERATIONS = 5;

	public Vector3 target;
	public Vector3 position;
	public Vector3 velocity;
	public float mass = 1f;
	public float force = 40f;
	public float damping = 6f;
	public float speed = 1f;

    public Spring() {
		target = new Vector3();
		position = new Vector3();
		velocity = new Vector3();
    }

	public void Shove(Vector3 shove_force) {
		this.velocity = this.velocity + shove_force;
	}

	public Vector3 Update(float delta_time) {
		float scaled_delta_time = (delta_time * this.speed) / ITERATIONS;

		for (int i = 1; i <= ITERATIONS; i++) {
			Vector3 force = this.target - this.position;
			Vector3 acceleration = force * this.force / this.mass;

			acceleration -= (this.velocity * (this.damping));
			this.velocity = this.velocity + (acceleration * (scaled_delta_time));
			this.position = this.position + (this.velocity * (scaled_delta_time));
		}

		return this.position;
	}
}
