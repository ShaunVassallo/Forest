﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyFly : MonoBehaviour
{
    public float noise = 10f;
    

    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Random.insideUnitCircle * noise;
        transform.localPosition = Vector2.Lerp(transform.localPosition, pos, Time.deltaTime);
    }
}
