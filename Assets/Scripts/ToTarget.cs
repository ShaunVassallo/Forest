﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTarget : MonoBehaviour
{
    public Transform target { get; private set; }

    public EnemyBehavior behavior;

    public bool followPlayer = true;

    void Start()
    {
        if (followPlayer)
        {
            target = GameObject.FindWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            float step = behavior.worldSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }
}