using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Player player;
    private ControllerManager controllerManager;

    public float airSpeed = 30f;
    public float airControl = 10f;
    public bool extraControl = false;
    public float speed = 12f;

    private bool wallClimbL = false;
    private bool wallClimbR = false;

    private float currentClamp = 20f;
    public float minClamp = 16f;
    public float maxClamp = 40f;
    public float clampInterval = 0.3f;


    [Header("Gliding")]
    public float maxGlideFrames = 1200f;
    public float glideFramesRemaining;
    public bool glideButton;
    public float glideFallSpeed = -3f;

    public bool glideDepleted = false;
    public bool useGlideOverride = false;
    public bool glideOverride = false;

    public FeatherParticles featherParticles;
    public GroundCheck groundScript;

    public GameObject smokeParticlesPrefab;
    private int smokeFramesInterval = 10;
    private int smokeFramesRemaining = 10;

    private float bouncyHitCooldown = 0.14f;
    private float lastBouncyHitTime = 0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        controllerManager = FindObjectOfType<ControllerManager>();
        glideFramesRemaining = maxGlideFrames;
    }

    public void MovementUpdate()
    {
        GlideLogic();
    }

    public void MovementFixedUpdate()
    {
        ClampPlayerMovement();
        MovePlayer();
    }

    private void ClampPlayerMovement()
    {
        //Reduce maximum clamp after a bounce
        if (currentClamp > minClamp)
        {
            currentClamp -= clampInterval;
        }

        // Clamp speed
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -player.maxSpeed, player.maxSpeed), Mathf.Clamp(rb.velocity.y, -currentClamp, currentClamp));

        // If touching hook
        if (player.movingToHook && Vector2.Distance((Vector2)player.currentHook.transform.position, (Vector2)transform.position) < player.retriveHookDistance)
        {
            player.touchingHook = true;
        }
    }

    private void MovePlayer()
    {
        Vector2 movement = new Vector2(player.moveHorizontal, 0);

        // If hook is below player or wall climbing, disable movement
        if (((wallClimbL && rb.velocity.x < 4) && player.moveHorizontal < 0.1)
          || ((wallClimbR && rb.velocity.x > -4) && player.moveHorizontal > 0.1)
          || player.softlockCheckCoroutineRunning)
        {
            return;
        }

        // Move the player
        rb.AddForce(movement * airSpeed, ForceMode2D.Force);

        if (player.groundScript.playerIsGrounded)
        {
            // Overwrite player velocity, IE snap turning
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
            SmokeEffect();
        }

        // Add more air control
        else
        {
            // Add more control while speed is less than 10
            if (player.moveHorizontal > 0 && rb.velocity.x < 10)
            {
                rb.AddForce(movement * airControl, ForceMode2D.Force);
            }
            else if (player.moveHorizontal < 0 && rb.velocity.x > -10)
            {
                rb.AddForce(movement * airControl, ForceMode2D.Force);
            }
        }
    }

    private void GlideLogic()
    {
        if (player.movingToHook)
            return;

        player.displayGlideUI = (glideFramesRemaining < maxGlideFrames);

        if (glideButton && !glideDepleted && !groundScript.playerIsGrounded) //set glide instance to trigger event that can be read by UIHandler
        {
            featherParticles?.StartParticleSystem();
            animator.SetBool("Gliding", true);

            if (rb.velocity.y < -1)
            {
                float newVelocityY = Mathf.MoveTowards(rb.velocity.y, glideFallSpeed, Time.deltaTime * 50f);
                rb.velocity = new Vector2(rb.velocity.x, newVelocityY);
            }

            if (glideFramesRemaining % 200 == 0)
            {
                SoundManager.Instance.PlaySound(Sound.Flap, 2f);
            }

            glideFramesRemaining -= 1;
            glideDepleted = (glideFramesRemaining <= 0); 
        }
        else
        {
            featherParticles?.StopParticleSystem();
            animator.SetBool("Gliding", false);

            if (glideFramesRemaining < maxGlideFrames)
            {
                glideFramesRemaining += 1;
                glideDepleted = (glideFramesRemaining >= maxGlideFrames) ? false : glideDepleted;
            }
        }

        if (useGlideOverride)
        {
            animator.SetBool("Gliding", glideOverride);
        }
    }

    private void SmokeEffect()
    {
        if (player.moveHorizontal != 0)
        {
            smokeFramesRemaining -= 1;

            if (smokeFramesRemaining <= 0 && smokeParticlesPrefab != null)
            {
                Instantiate(
                    smokeParticlesPrefab,
                    new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z),
                    Quaternion.identity);

                smokeFramesRemaining = smokeFramesInterval;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        wallClimbL = collision.gameObject.CompareTag("WallL");
        wallClimbR = collision.gameObject.CompareTag("WallR");

        if (collision.gameObject.layer == LayerMask.NameToLayer("BouncyWall"))
        {
            currentClamp = maxClamp;

            if (Time.time - lastBouncyHitTime >= bouncyHitCooldown)
            {
                SoundManager.Instance.PlaySound(Sound.BouncyHit);
                lastBouncyHitTime = Time.time;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        wallClimbL = (collision.gameObject.CompareTag("WallL")) ? false : wallClimbL;
        wallClimbR = (collision.gameObject.CompareTag("WallR")) ? false : wallClimbR;
    }
}
