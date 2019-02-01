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

    public float Move_speed = 2.0f;
    public Vector2 Aim_speed = new Vector2(3.7f, 3.7f);

    /*
     * Limit character rotation speed in degree per second
     */
    public Vector2 Rotation_speed_limit = new Vector2(
        180.0f,
        180.0f);

    /*
     * Limit camera rotation speed in degree per second
     */
    public Vector2 Look_speed_limit = new Vector2(
        220.0f,
        220.0f);

    /*
    * Limit aim movement speed in degree per second
    */
    public Vector2 Look_aim_speed_limit = new Vector2(
        360.0f,
       360.0f);

    /*
     * Time in seconds of inactive mouse to trigger character rotation
     */
    public float Look_afk_timeout = 1.0f;

    /*
     * Minimum angle difference in degree to allow character rotation due to timeout
     */
    public float Look_afk_threshold = 30.0f;

    /*
     * Minimum angle difference in degree to trigger force character rotation
     */
    public float Look_away_threshold = 80.0f;

    /*
     * Planar XZ distance between camera and player
     */
    public float Camera_distance = 1.2f;

    /*
     * Vertical distance between camera and player
     */
    public float Camera_height = 0.7f;

    /*
     * Z distance between camera target and player
     */
    public float Camera_target_depth = 0.5f;

    /*
     * Y distance between camera target and player
     */
    public float Camera_target_height = 0.5f;

    // Private

    public Vector2 look_aim = new Vector2();

    public float look_afk_timer;
    public bool is_player_rotating = false;

    // Cache

    private GameObject player;
    private Transform player_transform;
    private CharacterController player_controller;

    private new GameObject camera;
    private Transform camera_transform;
    private Camera camera_component;

    // Single alloc

    public Vector2 camera_look_current = new Vector2();
    public Vector2 camera_look_delta = new Vector2();
    public Vector2 camera_look_next = new Vector2();

    public Vector2 player_current = new Vector2();
    public Vector2 player_delta = new Vector2();
    public Vector2 player_next = new Vector2();

    private Vector2 input_delta = new Vector2();

    private Vector3 player_movement = new Vector3();
    private Vector3 camera_position = new Vector3();
    private Vector3 camera_target = new Vector3();

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();
        player_controller = GetComponent<CharacterController>();

        camera = GameObject.Find("Main Camera");
        camera_transform = camera.GetComponent<Transform>();
        camera_component = camera.GetComponent<Camera>();

        // Private

    }

    void FixedUpdate()
    {
        // Time.fixedDeltaTime

        input_delta.Set(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"));

        // Move aim with speed limited by parameter

        float aim_limit_x = Look_aim_speed_limit.x * Time.fixedDeltaTime;
        float aim_limit_y = Look_aim_speed_limit.y * Time.fixedDeltaTime;

        look_aim.Set(
            look_aim.x + Mathf.Clamp(input_delta.x * Aim_speed.x, -aim_limit_x, aim_limit_x),
            look_aim.y + Mathf.Clamp(input_delta.y * Aim_speed.y, -aim_limit_y, aim_limit_y));

        while (look_aim.x < 0) look_aim.x += 360.0f;
        while (look_aim.y < 0) look_aim.y += 360.0f;
        while (look_aim.x > 360) look_aim.x -= 360.0f;
        while (look_aim.y > 360) look_aim.y -= 360.0f;

        // Move camera towards aim with speed limited by parameter

        float look_limit_x = Look_speed_limit.x * Time.fixedDeltaTime;
        float look_limit_y = Look_speed_limit.y * Time.fixedDeltaTime;

        camera_look_current.Set(
            camera_transform.eulerAngles.y,
            camera_transform.eulerAngles.z);

        camera_look_delta.Set(
            Mathf.DeltaAngle(camera_look_current.x, look_aim.x),
            Mathf.DeltaAngle(camera_look_current.y, look_aim.y));

        camera_look_next.Set(
            camera_look_current.x + Mathf.Clamp(camera_look_delta.x, -look_limit_x, look_limit_x),
            camera_look_current.y + Mathf.Clamp(camera_look_delta.y, -look_limit_y, look_limit_y));

        // TODO: use Y component to look vertically respecting terrain and solid objects

        float camera_angle_horizontal = (camera_look_next.x / 180.0f + 0.5f) * Mathf.PI;

        float angle_cos = Mathf.Cos(camera_angle_horizontal);
        float angle_sin = Mathf.Sin(camera_angle_horizontal);

        camera_position.Set(
            player_transform.position.x + angle_cos * Camera_distance,
            player_transform.position.y + Camera_height,
            player_transform.position.z - angle_sin * Camera_distance);

        camera.transform.SetPositionAndRotation(camera_position, Quaternion.identity);

        camera_target.Set(
            player_transform.position.x,
            player_transform.position.y + Camera_target_height,
            player_transform.position.z + Camera_target_depth);
        camera.transform.LookAt(camera_target);

        // Move character towards aim

        float player_rotation_limit_x = Rotation_speed_limit.x * Time.fixedDeltaTime;
        float player_rotation_limit_y = Rotation_speed_limit.y * Time.fixedDeltaTime;

        // TODO: consider using Y component to affect character' head vertical rotation

        player_current.Set(
            player_transform.rotation.eulerAngles.y,
            player_transform.rotation.eulerAngles.z);

        player_delta.Set(
            Mathf.DeltaAngle(player_current.x, look_aim.x),
            Mathf.DeltaAngle(player_current.y, look_aim.y) * 0.0f);

        bool is_moving = Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon ||
            Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Epsilon;

        if (!is_player_rotating)
        {
            if(is_moving || Mathf.Abs(player_delta.x) > Look_away_threshold)
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
                    look_afk_timer -= Time.fixedDeltaTime;
                    if (look_afk_timer <= 0.0f &&
                        Mathf.Abs(Mathf.DeltaAngle(player_current.x, look_aim.x)) > Look_afk_threshold)
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
            }
            else
            {
                is_player_rotating = false;
            }
        }

        // Renew after rotation, not a scope of rotation itself
        player_current.Set(
            player_transform.rotation.eulerAngles.y,
            player_transform.rotation.eulerAngles.z);

        float angle = (player_current.x / 180.0f) * Mathf.PI;

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
