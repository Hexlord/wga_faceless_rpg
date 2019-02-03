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
 *  Camera is ray casting from character to determine safe distance (protection from clipping)
 * 
 */
public class Smart_controller : MonoBehaviour
{
    [Header("Basic Settings")]

    [Tooltip("Player movement speed in meters per second")]
    public float Move_speed = 7.0f;

    [Tooltip("Mouse sensitivity")]
    public Vector2 Mouse_sensitivity = new Vector2(5.0f, 5.0f);

    [Header("Advanced Settings")]

    [Tooltip("Limit character rotation speed in degree per second")]
    public Vector2 Rotation_speed_limit = new Vector2(
        180.0f,
        180.0f);
    
    [Tooltip("Limit camera rotation speed in degree per second")]
    public Vector2 Look_speed_limit = new Vector2(
        260.0f,
        260.0f);

    [Tooltip("Limit aim movement speed in degree per second")]
    public Vector2 Look_aim_speed_limit = new Vector2(
        390.0f,
       390.0f);

    [Tooltip("Limit aim vertical in degree")]
    public float Look_aim_limit = 60.0f;

    [Tooltip("Time in seconds of inactive mouse to trigger character rotation")]
    public float Look_afk_timeout = 2.0f;

    [Tooltip("Minimum angle difference in degree to allow character rotation due to timeout")]
    public float Look_afk_threshold = 0.0f;

    [Tooltip("Minimum angle difference in degree to trigger force character rotation")]
    public float Look_away_threshold = 90.0f;

    [Tooltip("Distance between camera and player")]
    public float Camera_distance = 4.8f;

    [Tooltip("Vertical offset for camera")]
    public float Camera_height = 2.2f;

    [Tooltip("Affects how safe distance for camera is calculated")]
    public float Camera_avoidance_factor = 1.5f;

    [Tooltip("Affects how fast camera safe distance grows in meter per second")]
    public float Camera_avoidance_speed = 4.0f;



    // Private

    public float safe_camera_distance;
    private float look_afk_timer;
    public bool is_player_rotating = false;

    // Cache

    private GameObject player;
    private Transform player_transform;
    private CharacterController player_controller;

    private new GameObject camera;
    private Transform camera_transform;

    // Single alloc

    public Vector2 look_aim = new Vector2();

    private Vector2 camera_look_current = new Vector2();
    private Vector2 camera_look_delta = new Vector2();
    private Vector2 camera_look_next = new Vector2();

    private Vector2 player_current = new Vector2();
    private Vector2 player_delta = new Vector2();
    private Vector2 player_next = new Vector2();

    private Vector2 input_delta = new Vector2();

    private Vector3 player_movement = new Vector3();
    private Vector3 camera_position = new Vector3();
    private Vector3 camera_target = new Vector3();

    public float camera_distance_debug = 0.0f;
    public float camera_ray_distance_debug = 0.0f;

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();
        player_controller = GetComponent<CharacterController>();

        camera = GameObject.Find("Main Camera");
        camera_transform = camera.GetComponent<Transform>();

        // Private

        safe_camera_distance = Camera_distance;
    }

    void process_input()
    {
        input_delta.Set(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"));
    }

    void move_aim()
    {
        // Move aim with speed limited by parameter

        float aim_limit_x = Look_aim_speed_limit.x * Time.deltaTime;
        float aim_limit_y = Look_aim_speed_limit.y * Time.deltaTime;

        look_aim.Set(
            look_aim.x + Mathf.Clamp(input_delta.x * Mouse_sensitivity.x, -aim_limit_x, aim_limit_x),
            look_aim.y + Mathf.Clamp(-input_delta.y * Mouse_sensitivity.y, -aim_limit_y, aim_limit_y));

        while (look_aim.x < 0) look_aim.x += 360.0f;
        while (look_aim.x > 360) look_aim.x -= 360.0f;

        while (look_aim.y < -Look_aim_limit + Mathf.Epsilon) look_aim.y = -Look_aim_limit + Mathf.Epsilon;
        while (look_aim.y > Look_aim_limit - Mathf.Epsilon) look_aim.y = Look_aim_limit - Mathf.Epsilon;
    }

    float filter_distance(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Camera_distance * (1.0f + Camera_avoidance_factor), ~LayerMask.NameToLayer("Environment")))
        {
            float angle = Vector3.Angle(hit.normal, camera.transform.forward);
            if (angle > 90.0f) angle = 90.0f;
            float factor = 1.0f - angle / 90.0f;
            float factor_sqrt = Mathf.Pow(factor, 1.0f / (1.0f + Camera_avoidance_factor));

            float d = hit.distance * factor_sqrt;
            camera_ray_distance_debug = Mathf.Min(camera_ray_distance_debug, hit.distance);

            return d;
        }

        return float.MaxValue;
    }

    void move_camera()
    {
        // Move camera towards aim with speed limited by parameter

        float look_limit_x = Look_speed_limit.x * Time.deltaTime;
        float look_limit_y = Look_speed_limit.y * Time.deltaTime;
        
        camera_look_delta.Set(
            Mathf.DeltaAngle(camera_look_current.x, look_aim.x),
            Mathf.DeltaAngle(camera_look_current.y, look_aim.y));

        camera_look_next.Set(
            camera_look_current.x + Mathf.Clamp(camera_look_delta.x, -look_limit_x, look_limit_x),
            camera_look_current.y + Mathf.Clamp(camera_look_delta.y, -look_limit_y, look_limit_y));

        camera_look_current.Set(camera_look_next.x, camera_look_next.y);
        
        while (camera_look_current.x < 0) camera_look_current.x += 360.0f;
        while (camera_look_current.y < 0) camera_look_current.y += 360.0f;
        while (camera_look_current.x > 360) camera_look_current.x -= 360.0f;
        while (camera_look_current.y > 360) camera_look_current.y -= 360.0f;

        // TODO: make camera_distance movement due to clipping protection smoother
        float distance = Camera_distance;
        camera_ray_distance_debug = float.MaxValue;

        // character-widening
        // works bad due to points going inside geometry
        // leave here as comment to consider alternatives
        /*
        {
            Vector3 origin1 = player_transform.position + camera_transform.right + camera_transform.up;
            Vector3 origin2 = player_transform.position - camera_transform.right + camera_transform.up;
            Vector3 origin3 = player_transform.position - camera_transform.right - camera_transform.up;
            Vector3 origin4 = player_transform.position + camera_transform.right - camera_transform.up;

            distance = Mathf.Min(distance, filter_distance(new Ray(origin1, -camera_transform.forward)));
            distance = Mathf.Min(distance, filter_distance(new Ray(origin2, -camera_transform.forward)));
            distance = Mathf.Min(distance, filter_distance(new Ray(origin3, -camera_transform.forward)));
            distance = Mathf.Min(distance, filter_distance(new Ray(origin4, -camera_transform.forward)));
        }
        */

        // direction-based
        {
            Vector3 dir1 = Quaternion.Euler(5.0f, 0.0f, 0.0f) * -camera_transform.forward;
            Vector3 dir2 = Quaternion.Euler(-5.0f, 0.0f, 0.0f) * -camera_transform.forward;
            Vector3 dir3 = Quaternion.Euler(0.0f, 5.0f, 0.0f) * -camera_transform.forward;
            Vector3 dir4 = Quaternion.Euler(0.0f, -5.0f, 0.0f) * -camera_transform.forward;

            distance = Mathf.Min(distance, filter_distance(new Ray(player_transform.position, dir1)));
            distance = Mathf.Min(distance, filter_distance(new Ray(player_transform.position, dir2)));
            distance = Mathf.Min(distance, filter_distance(new Ray(player_transform.position, dir3)));
            distance = Mathf.Min(distance, filter_distance(new Ray(player_transform.position, dir4)));
        }

        camera_distance_debug = distance;

        if(safe_camera_distance > distance)
        {
            safe_camera_distance = distance;
        }
        else
        {
            float growth_speed = Camera_avoidance_speed * Time.deltaTime;

            safe_camera_distance = safe_camera_distance + Mathf.Clamp(distance - safe_camera_distance, -growth_speed, growth_speed);
        }

        Quaternion rotation = Quaternion.Euler(camera_look_current.y, camera_look_current.x, 0);

        camera_target.Set(player_transform.position.x, player_transform.position.y + Camera_height, player_transform.position.z);
        camera_position.Set(0, 0, safe_camera_distance);
        camera_position = camera_target - rotation * camera_position;

        camera_transform.position = camera_position;
        camera_transform.LookAt(camera_target);
        
    }

    void move_character()
    {
        // Move character towards aim

        float player_rotation_limit_x = Rotation_speed_limit.x * Time.deltaTime;
        float player_rotation_limit_y = Rotation_speed_limit.y * Time.deltaTime;

        // TODO: consider using Y component to affect character' head vertical rotation

        player_current.Set(
            player_transform.rotation.eulerAngles.y,
            player_transform.rotation.eulerAngles.x);

        player_delta.Set(
            Mathf.DeltaAngle(player_current.x, camera_look_next.x),
            Mathf.DeltaAngle(player_current.y, camera_look_next.y) * 0.0f);

        bool is_moving = Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon ||
            Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Epsilon;

        if (!is_player_rotating)
        {
            if (is_moving || Mathf.Abs(player_delta.x) > Look_away_threshold)
            {
                is_player_rotating = true;
            }
            else
            {
                if (Mathf.Abs(input_delta.x) > Mathf.Epsilon ||
                    Mathf.Abs(input_delta.y) > Mathf.Epsilon)
                {
                    look_afk_timer = Look_afk_timeout;
                }
                else
                {
                    look_afk_timer -= Time.deltaTime;
                    if (look_afk_timer <= 0.0f &&
                        Mathf.Abs(Mathf.DeltaAngle(player_current.x, camera_look_next.x)) > Look_afk_threshold)
                    {
                        is_player_rotating = true;
                    }
                }
            }
        }

        if (is_player_rotating)
        {
            if (Mathf.Abs(player_delta.x) > Mathf.Epsilon ||
                Mathf.Abs(player_delta.y) > Mathf.Epsilon)
            {
                player_next.Set(
                    player_current.x + Mathf.Clamp(player_delta.x, -player_rotation_limit_x, player_rotation_limit_x),
                    player_current.y + Mathf.Clamp(player_delta.y, -player_rotation_limit_y, player_rotation_limit_y));

                Quaternion q = Quaternion.Euler(0.0f, player_next.x, 0.0f);

                player_transform.rotation = q;

                if (Mathf.Abs(player_delta.x) <= player_rotation_limit_x &&
                    Mathf.Abs(player_delta.y) <= player_rotation_limit_y)
                {
                    // Done in this step
                    is_player_rotating = false;
                }
            }
            else
            {
                is_player_rotating = false;
            }
        }

    }

    void movement()
    {
        // Renew after rotation, not a scope of rotation itself
        player_current.Set(
            player_transform.rotation.eulerAngles.y,
            player_transform.rotation.eulerAngles.x);

        float angle = (player_current.x / 180.0f) * Mathf.PI;

        float cosf = Mathf.Cos(angle);
        float sinf = Mathf.Sin(angle);

        player_movement.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float x = player_movement.x * cosf + player_movement.z * sinf;
        float z = player_movement.z * cosf + -player_movement.x * sinf;
        player_movement.Set(x, 0.0f, z);
        if (x != 0.0f || z != 0.0f) player_movement.Normalize();
        player_movement.y = -1.0f;

        player_controller.Move(player_movement * Time.fixedDeltaTime * Move_speed);
    }

    void Update()
    {
        process_input();

        move_aim();

        move_camera();

        move_character();
    }

    void FixedUpdate()
    {
        movement();
    }

}
