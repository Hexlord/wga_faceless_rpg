using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

[RequireComponent(typeof(AudioSource))]
public class Familiar : MonoBehaviour
{
    [Header("Basic Settings")]

    [Tooltip("Fly height")]
    public float flyHeight = 4.5f;

    [Tooltip("Fly speed")]
    public float flySpeed = 5.0f;

    [Tooltip("Raise speed")]
    public float raiseSpeed = 5.0f;

    [Tooltip("Land speed")]
    public float landSpeed = 1.5f;

    [Tooltip("Time before landing")]
    public float landTimeout = 2.0f;

    [Tooltip("Time before following")]
    public float followTimeout = 0.2f;

    [Header("Advanced Settings")]

    [Tooltip("Player rest place")]
    public Vector3 playerOffset = new Vector3(-0.7f, 1.5f, 0.0f);


    [Header("Audio Settings")]

    [Tooltip("Fly sound")]
    public AudioSource audioFly;
    [Tooltip("Rest sound")]
    public AudioSource audioRest;
    [Tooltip("Rest sound timeout baseline")]
    public float audioRestTimeout = 10.0f;
    [Tooltip("Rest sound timeout random amount")]
    public float audioRestTimeoutRandom = 10.0f;

    private float audioRestTimer = 0.0f;

    // Private

    public enum State
    {
        Rest,
        Raise,
        Follow,
        Land
    };

    public State state;
    public Vector3 prevPlayerPosition = new Vector3();
    public Vector3 prevPlayerScale = new Vector3();
    public Quaternion prevPlayerRotation = new Quaternion();

    public Vector3 target;
    private float landTimer;
    private float followTimer;
    private System.Random random = new System.Random();

    // Cache

    private GameObject player;
    private Transform playerTransform;

    // Single alloc


    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();

        audioFly = gameObject.transform.Find("Wings").gameObject.GetComponent<AudioSource>();
        audioRest = gameObject.transform.Find("Mouth").gameObject.GetComponent<AudioSource>();

        // Private
        landTimer = landTimeout;
        followTimer = followTimeout;
        state = State.Raise;
        audioFly.Play();

        prevPlayerPosition = playerTransform.position;
        prevPlayerRotation = playerTransform.rotation;
        prevPlayerScale = playerTransform.localScale;

    }
    void FixedUpdate()
    {
        target = playerTransform.position + Quaternion.Euler(0, playerTransform.rotation.eulerAngles.y, 0) * playerOffset;

        float height = playerTransform.position.y + flyHeight;
        if(state == State.Rest)
        {
            audioFly.Stop();
            audioRestTimer -= Time.fixedDeltaTime;
            if (audioRestTimer <= 0.0f)
            {
                if (!audioRest.isPlaying) audioRest.Play();
                audioRestTimer = audioRestTimeout + (float)random.NextDouble() * audioRestTimeoutRandom;
            }
        }
        else
        {
            audioRestTimer = 0.0f;
            audioRest.Stop();
            
            if (!audioFly.isPlaying) audioFly.Play();
        }

        switch (state)
        {
            case State.Rest:

                if (!playerTransform.position.Equals(prevPlayerPosition) ||
                    !playerTransform.localScale.Equals(prevPlayerScale))
                {
                    state = State.Raise;
                }
                else
                {
                    transform.position = target;
                    transform.rotation = playerTransform.rotation;
                }
                break;
            case State.Raise:
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Min(height, transform.position.y + raiseSpeed * Time.fixedDeltaTime), transform.position.z);
                    if (transform.position.y >= height)
                    {
                        landTimer = landTimeout;
                        state = State.Follow;
                    }
                }
                break;
            case State.Follow:
                {
                    Vector2 direction = new Vector2(target.x - transform.position.x, target.z - transform.position.z);
                    float distance = direction.magnitude;
                    float speed = flySpeed * Time.fixedDeltaTime;
                    if (distance > Mathf.Epsilon)
                    {
                        if (followTimer <= 0)
                        {
                            direction.Normalize();
                            transform.position += new Vector3(direction.x, 0.0f, direction.y) * Mathf.Min(speed, distance);
                            transform.rotation = Quaternion.Euler(0.0f, -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90, 0.0f);

                        }
                        followTimer -= Time.fixedDeltaTime;
                    }
                    if (distance <= speed)
                    {
                        transform.rotation = playerTransform.rotation;
                        followTimer = followTimeout;
                        landTimer -= Time.fixedDeltaTime;
                        if (landTimer <= 0)
                        {
                            state = State.Land;
                        }
                    }
                    else landTimer = landTimeout;
                }
                break;
            case State.Land:
                if (!playerTransform.position.Equals(prevPlayerPosition) ||
                    !playerTransform.localScale.Equals(prevPlayerScale))
                {
                    state = State.Raise;
                }
                else
                {
                    transform.position = new Vector3(target.x, Mathf.Max(target.y, transform.position.y - landSpeed * Time.fixedDeltaTime), target.z);
                    transform.rotation = playerTransform.rotation;
                    if (transform.position.y <= target.y)
                    {
                        state = State.Rest;
                    }
                }
                break;
        }
        prevPlayerPosition = playerTransform.position;
        prevPlayerRotation = playerTransform.rotation;
        prevPlayerScale = playerTransform.localScale;
    }
}
