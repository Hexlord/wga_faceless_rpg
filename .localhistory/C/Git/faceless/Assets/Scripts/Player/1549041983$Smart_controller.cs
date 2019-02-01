﻿using System.Collections;
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

    GameObject player;
    Transform player_transform;

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

    private Vector2 look_camera = new Vector2();
    private Vector2 look_aim = new Vector2();

    private float look_afk_timer = 0.0f;

    // Cache

    private CharacterController controller;
    private Vector3 move = new Vector3();

    // Single alloc

    private Vector2 look_delta = new Vector2();

    void Start()
    {
        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();

        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        // Time.fixedDeltaTime
        
        look_delta.Set(
            Look_speed.x * Input.GetAxis("Mouse X"),
            Look_speed.y * Input.GetAxis("Mouse Y"));
        look_aim += look_delta;



        player_transform.Rotate(0, delta_x, 0);

        float angle = (player_transform.rotation.eulerAngles.y / 360.0f * 2.0f) * Mathf.PI;

        float cosf = Mathf.Cos(angle);
        float sinf = Mathf.Sin(angle);

        move.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float x = move.x * cosf + move.z * sinf;
        float z = move.z * cosf + -move.x * sinf;
        move.Set(x, 0.0f, z);
        if (x != 0.0f || z != 0.0f) move.Normalize();
        move.y = -1.0f;

        controller.Move(move * Time.deltaTime * Move_speed);
    }

}
