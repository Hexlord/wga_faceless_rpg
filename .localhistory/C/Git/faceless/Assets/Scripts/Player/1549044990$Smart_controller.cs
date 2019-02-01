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
 *  Camera rotation is smoothed via parabolic interpolation respecting configurable speed limit (protection from dizziness)
 * 
 */
public class Smart_controller : MonoBehaviour {
    
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

    private Vector2 look_previous = new Vector2();
    private Vector2 look_camera = new Vector2();
    private Vector2 look_aim = new Vector2();

    private float look_afk_timer = 0.0f;

    // Cache

    private GameObject player;
    private Transform player_transform;
    private CharacterController controller;

    // Single alloc

    private Vector2 look_current = new Vector2();
    private Vector2 look_delta = new Vector2();
    private Vector3 player_movement = new Vector3();

    void Start()
    {
        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();

        controller = GetComponent<CharacterController>();

        look_previous.Set(
            Look_speed.x * Input.GetAxis("Mouse X"),
            Look_speed.y * Input.GetAxis("Mouse Y"));
    }

    void FixedUpdate()
    {
        // Time.fixedDeltaTime
        
        look_delta.Set(
            Input.GetAxis("Mouse X"),
            Look_speed.y * Input.GetAxis("Mouse Y"));
        look_aim += look_delta;



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

        controller.Move(player_movement * Time.deltaTime * Move_speed);
    }

}
