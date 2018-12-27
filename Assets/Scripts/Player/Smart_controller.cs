﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smart_controller : MonoBehaviour {

    GameObject player;
    Transform player_transform;

    private CharacterController controller;
    public float Speed = 1.0f;

    public Vector2 Look_speed = new Vector2(1.0f, 1.0f);

    Quaternion quaternion;
    Vector3 view = new Vector3();
    Vector3 move = new Vector3();

    void Start()
    {
        player = GameObject.Find("Player");
        player_transform = player.GetComponent<Transform>();

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        float h = Look_speed.x * Input.GetAxis("Mouse X");
        float v = Look_speed.y * Input.GetAxis("Mouse Y");

        player_transform.Rotate(0, h, 0);

        float angle = (player_transform.rotation.eulerAngles.y / 360.0f * 2.0f) * Mathf.PI;

        float cosf = Mathf.Cos(angle);
        float sinf = Mathf.Sin(angle);

        move.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float x = move.x * cosf + move.z * sinf;
        float z = move.z * cosf + -move.x * sinf;
        move.Set(x, 0.0f, z);
        if (x != 0.0f || z != 0.0f) move.Normalize();
        move.y = -1.0f;

        controller.Move(move * Time.deltaTime * Speed);
    }

}