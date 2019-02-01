using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Basic character rotation algorithm is:
 *  
 *  1. After timeout of not moving mouse rotate character to match aim
 *  2. After X angle becoming 90+ degree rotate character to match aim continuously (aim can still move during character rotation) until reaching the aim
 *  3. After any movement rotate character to match aim
 *  
 *  Camera rotation is respecting configurable speed limit (protection from dizziness)
 * 
 */
public class Smart_controller : MonoBehaviour
{

    public float Move_speed = 1.0f;
    public Vector2 Look_speed = new Vector2(1.0f, 1.0f);

    /*
     * Limit camera rotation speed in radians per second
     */
    public Vector2 Look_speed_limit = new Vector2(
        Mathf.PI / 32.0f,
        Mathf.PI / 32.0f);

    /*
    * Limit aim movement speed in radians per second
    */
    public Vector2 Look_aim_speed_limit = new Vector2(
        Mathf.PI / 2.0f,
        Mathf.PI / 2.0f);

    /*
     * Time in seconds of inactive mouse to trigger character rotation
     */
    public float Look_afk_timeout = 10.0f;

    /*
     * Minimum angle difference in radians to allow character rotation due to timeout
     */
    public float Look_afk_threshold = Mathf.PI / 32.0f;

    /*
     * Minimum angle difference in radians to trigger force character rotation
     */
    public float Look_away_threshold = Mathf.PI / 2.0f;

    // Private

    private Vector2 input_previous = new Vector2();
    private Vector2 look_camera = new Vector2();
    private Vector2 look_aim = new Vector2();

    private float look_afk_timer = 0.0f;
    private bool is_player_rotating = false;

    // Cache

    private GameObject player;
    private Transform player_transform;
    private CharacterController player_controller;


    private GameObject camera;
    private Transform camera_transform;
    private Camera camera_component;

    // Single alloc

    private Vector2 input_current = new Vector2();
    private Vector2 input_delta = new Vector2();
    private Vector3 player_movement = new Vector3();

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();
        player_controller = GetComponent<CharacterController>();

        camera_transform = player.GetComponent<Transform>();
        player_controller = GetComponent<CharacterController>();

        // Private

        input_previous.Set(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"));
    }

    void FixedUpdate()
    {
        // Time.fixedDeltaTime

        input_current.Set(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"));

        input_delta.Set(
            input_current.x - input_previous.x, 
            input_current.y - input_previous.y);

        input_previous.Set(
            input_current.x, 
            input_current.y);

        // Move aim limited by parameter

        float aim_limit_x = Look_aim_speed_limit.x * Time.fixedDeltaTime;
        float aim_limit_y = Look_aim_speed_limit.y * Time.fixedDeltaTime;

        look_aim.Set(
            look_aim.x + Mathf.Clamp(input_delta.x, -aim_limit_x, aim_limit_x),
            look_aim.y + Mathf.Clamp(input_delta.y, -aim_limit_y, aim_limit_y));

        // Move camera



        // Move character


        player_transform.Rotate(0, delta_x, 0);

        float angle = (player_transform.rotation.eulerAngles.y / 360.0f * 2.0f) * Mathf.PI;

        float cosf = Mathf.Cos(angle);
        float sinf = Mathf.Sin(angle);

        player_movement.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float x = player_movement.x * cosf + player_movement.z * sinf;
        float z = player_movement.z * cosf + -player_movement.x * sinf;
        player_movement.Set(x, 0.0f, z);
        if (x != 0.0f || z != 0.0f) player_movement.Normalize();
        player_movement.y = -1.0f;

        player_controller.Move(player_movement * Time.deltaTime * Move_speed);
    }

}
