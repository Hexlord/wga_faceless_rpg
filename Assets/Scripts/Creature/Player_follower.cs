using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 
 */
public class Player_follower : MonoBehaviour
{
    [Header("Basic Settings")]

    [Tooltip("Fly height")]
    public float Fly_height = 4.5f;

    [Tooltip("Fly speed")]
    public float Fly_speed = 5.0f;

    [Tooltip("Raise speed")]
    public float Raise_speed = 10.0f;

    [Tooltip("Time before landing")]
    public float Land_timeout = 2.0f;

    [Tooltip("Time before following")]
    public float Follow_timeout = 0.33f;

    [Header("Advanced Settings")]

    [Tooltip("Player rest place")]
    public Vector3 player_offset = new Vector3(-0.7f, 1.0f, 0.0f);

    // Private

    public enum State
    {
        Rest,
        Raise,
        Follow,
        Land
    };

    public State state;
    public Vector3 previous_player_position = new Vector3();
    public Vector3 previous_player_scale = new Vector3();
    public Quaternion previous_player_rotation = new Quaternion();

    public Vector3 target;
    private float land_timer;
    private float follow_timer;

    // Cache

    private GameObject player;
    private Transform player_transform;

    // Single alloc


    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();

        // Private
        land_timer = Land_timeout;
        follow_timer = Follow_timeout;
        state = State.Raise;

        previous_player_position = player_transform.position;
        previous_player_rotation = player_transform.rotation;
        previous_player_scale = player_transform.localScale;
          
    }
    void FixedUpdate()
    {
        target = player_transform.position + Quaternion.Euler(0, player_transform.rotation.eulerAngles.y, 0) * player_offset;
        
        switch (state)
        {
            case State.Rest:
                if (!player_transform.position.Equals(previous_player_position) ||
                    !player_transform.localScale.Equals(previous_player_scale))
                {
                    state = State.Raise;
                }
                else
                {
                    transform.position = target;
                    transform.rotation = player_transform.rotation;
                }
                break;
            case State.Raise:
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Min(Fly_height, transform.position.y + Raise_speed * Time.fixedDeltaTime), transform.position.z);
                    if(transform.position.y >= Fly_height)
                    {
                        land_timer = Land_timeout;
                        state = State.Follow;
                    }
                }
                break;
            case State.Follow:
               {
                    Vector2 direction = new Vector2(target.x - transform.position.x, target.z - transform.position.z);
                    float distance = direction.magnitude;
                    float speed = Fly_speed * Time.fixedDeltaTime;
                    if (distance > Mathf.Epsilon)
                    {
                        if (follow_timer <= 0)
                        {
                            direction.Normalize();
                            transform.position += new Vector3(direction.x, 0.0f, direction.y) * Mathf.Min(speed, distance);
                            transform.rotation = Quaternion.Euler(0.0f, -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90, 0.0f);
                            
                        }
                        follow_timer -= Time.fixedDeltaTime;
                    }
                    if (distance <= speed)
                    {
                        transform.rotation = player_transform.rotation;
                        follow_timer = Follow_timeout;
                        land_timer -= Time.fixedDeltaTime;
                        if(land_timer <= 0)
                        {
                            state = State.Land;
                        }
                    } else land_timer = Land_timeout;
                }
                break;
            case State.Land:
                if (!player_transform.position.Equals(previous_player_position) ||
                    !player_transform.localScale.Equals(previous_player_scale))
                {
                    state = State.Raise;
                }
                else
                {
                    transform.position = new Vector3(target.x, Mathf.Max(target.y, transform.position.y - Raise_speed * Time.fixedDeltaTime), target.z);
                    transform.rotation = player_transform.rotation;
                    if (transform.position.y <= target.y)
                    {
                        state = State.Rest;
                    }
                }
                break;
        }
        previous_player_position = player_transform.position;
        previous_player_rotation = player_transform.rotation;
        previous_player_scale = player_transform.localScale;
    }

}
