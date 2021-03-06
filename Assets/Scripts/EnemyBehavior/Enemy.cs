﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Enemy : MonoBehaviour
{
    //Input of behavior and stats
    public EnemyBehavior behavior;

    //public GameObject attackArea;

    //If rangeAttack = True
    public GameObject projectile;

    [SerializeField]
    private Transform enemyGraphic;

    private Transform player;

    private Animator anim;

    public Rigidbody2D rb2d;

    public float health;

    private float timeAttack;

    public GameObject[] drop;
    //private GameObject border;

    public SpriteRenderer _renderer;

    private MenuManager menuManager;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        menuManager = GameObject.FindObjectOfType<MenuManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        health = behavior.health;
        //border = GameObject.FindWithTag("Border");
        anim = GetComponentInChildren<Animator>();

        // if (menuManager != null) menuManager.Register(this)
        menuManager?.Register(this);
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else if (player.position.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        //_renderer.flipX = player.position.x < 0.1f;

        float move = Input.GetAxis("Horizontal");
        float distFromPlayer = Vector2.Distance(transform.position, player.position);

        if (timeAttack > 0) timeAttack -= Time.deltaTime;

        if (distFromPlayer < behavior.detectDist || health < behavior.health)
        {
            if (distFromPlayer > behavior.stoppingDist) //|| health < behavior.health if taken damage
            {
                anim.SetInteger("Speed", 1);
                transform.position = Vector2.MoveTowards(transform.position, player.position, behavior.speed * Time.deltaTime);            
            }
            else if (distFromPlayer < behavior.retreatDist)
            {
                anim.SetInteger("Speed", 1);
                transform.position = Vector2.MoveTowards(transform.position, player.position, -behavior.speed * Time.deltaTime);
            }
            else
            {
                anim.SetInteger("Speed", 0);
                Attack();
            }
        }
        else
        {
            anim.SetInteger("Speed", 0);
        }

        if (behavior.noise)
        {
            Vector2 pos = Random.insideUnitCircle * behavior.noiseAmount;
            enemyGraphic.localPosition = Vector2.Lerp(enemyGraphic.localPosition, pos, Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Attack()
    {

        if (timeAttack <= 0)
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
            anim.SetBool("Attack", true);

            /** 
            if (behavior.rangeAttack && behavior.meleeAttack)
            {
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    Debug.Log("Range ATTACK");
                    Instantiate(projectile, transform.position, Quaternion.identity);
                }
                if (rand == 2)
                {
                    Debug.Log("start ATTACK");
                    anim.SetBool("Melee Attack", true);
                }
            }

            if (behavior.rangeAttack && !behavior.meleeAttack)
            {
                Debug.Log("Range ATTACK");
                Instantiate(projectile, transform.position, Quaternion.identity);
            }

            if (behavior.meleeAttack && !behavior.rangeAttack)
            {
                //Debug.Log("start ATTACK");
                attackArea.SetActive(true);
                anim.SetBool("Melee Attack", true);
            }
            */

            timeAttack = behavior.startTimeBtwAttack;
        }
        else
        {
            // if attackArea is not null, it will do the function.
            //if (attackArea != null) attackArea.SetActive(false);
            anim.SetInteger("Speed", 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Blade")
        {
            anim.SetTrigger("Hit");
            //Debug.Log("Hit!!");

        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        anim.SetTrigger("Hit");

        if (timeAttack <= 0.3f)
        {
            timeAttack += 0.3f;
        }

        if (health <= 0)
        {
            menuManager?.Unregister(this);
            Destroy(gameObject);
            anim.SetTrigger("Die");

            if(Random.value > 0.5)
            {
            int dropRand = Random.Range(0, drop.Length);
            Instantiate(drop[dropRand], transform.position, transform.rotation);
            }

            //border.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // if no behavior on script, stop here.
        if (behavior == null) return;

        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.forward, behavior.stoppingDist);
        
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, behavior.retreatDist);

        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.forward, behavior.detectDist);
    }
#endif

}
