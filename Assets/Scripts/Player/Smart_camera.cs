using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smart_camera : MonoBehaviour {

    GameObject player;
    Transform player_transform;
    new Camera camera;
    new Transform transform;

    public float angle_debug = 0.0f;

    public float Radius_minimum = 1.0f;
    public float Radius_maximum = 10.0f;
    public float Scroll_speed = 0.2f;
    float radius = 8.0f;

    private Vector3 camera_position = new Vector3();

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();

        camera = GetComponent<Camera>();
        transform = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        float v = player.GetComponent<Smart_controller>().Look_speed.y * Input.GetAxis("Mouse Y");

        radius = Mathf.Clamp(radius + -Input.mouseScrollDelta.y * Scroll_speed, Radius_minimum, Radius_maximum);

        float angle = (player_transform.rotation.eulerAngles.y / 360.0f * 2.0f + 0.5f) * Mathf.PI;
        angle_debug = angle;
        float acos = Mathf.Cos(angle);
        float asin = Mathf.Sin(angle);


        camera_position.x = player_transform.position.x + acos * radius;
        camera_position.z = player_transform.position.z + -asin * radius;
        camera_position.y = player_transform.position.y + radius * 0.2f;
        
        camera.transform.SetPositionAndRotation(camera_position, Quaternion.identity);
        camera.transform.LookAt(player_transform);
    }
}
