using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float maxHealth = 5;
    public float health = 5;
    public Rigidbody2D rb;
    public Animator animator;
    public Weapon weapon;
    public Transform firePoint;
    UnityEngine.Vector2 moveDirection;
    UnityEngine.Vector2 mousePosition;
    private SpriteRenderer spriteRenderer;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 30f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 2f;
    [SerializeField] private TrailRenderer tr;

    public Slider DashBar;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        health = maxHealth;
        animator.SetFloat("hp", health);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DashBar.value < 2)
        {
            DashBar.value += Time.deltaTime;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new UnityEngine.Vector2(moveX, moveY).normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (moveDirection.x <= 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            weapon.Shoot();
            Debug.Log(mousePosition);
        }

        if (isDashing)
        {
            DashBar.value = 0;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }


    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.velocity = new UnityEngine.Vector2(moveDirection.x * dashingPower, moveDirection.y * dashingPower);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rb.velocity = new UnityEngine.Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        animator.SetFloat("speed", rb.velocity.magnitude);
    }

    public void ChangeHp(int val)
    {
        health += val;
        animator.SetFloat("hp", health);
        if (health <= 0)
        {

        }
    }
}
