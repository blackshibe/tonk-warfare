using UnityEngine;

public class BillboardAlign : MonoBehaviour {
    void Start() {
        
    }

    void Update() {
        
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(new Vector3(0, 0, -Camera.main.transform.rotation.z));
        // transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
    }
}
