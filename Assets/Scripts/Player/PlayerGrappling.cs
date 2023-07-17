using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrappling : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private Rigidbody2D rb;
    private ControllerManager controllerManager;

    public GameObject hookPrefab;

    public float launchPowerMin = 10f;
    public float launchPowerMax = 25f;
    public float launchPowerIncrement = 0.14f;
    private float launchPower = 0f;

    public ChargeBar chargeBar;

    private Vector2 aim;
    private Vector2 stickSave;

    public bool hookAboveLaunchPosition;
    private Vector2 launchPosition;

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controllerManager = FindObjectOfType<ControllerManager>();

        launchPower = launchPowerMin;
    }

    public void GrapplingUpdate()
    {
        MouseRelease();
    }

    public void GrapplingFixedUpdate()
    {
        aim = controllerManager.aimInputVector;
        ChargeGrapplingHook();
    }

    private void MouseRelease()
    {
        if (player.mouseUp || (player.stickAim && aim == Vector2.zero))
        {
            if (player.currentHook == null && hookPrefab)
            {
                LaunchGrapplingHook(Mathf.Max(launchPower, launchPowerMin));
                launchPosition = player.transform.position;
            }
            else
            {
                RecallLogic();
            }
            launchPower = launchPowerMin;
            player.stickAim = false;
        }
    }

    private void ChargeGrapplingHook() 
    {
        if (player.currentHook == null)
        {
            animator.SetBool("Charging", player.mouseDown || aim != Vector2.zero);

            if (player.mouseDown || aim != Vector2.zero)
            {
                if (!player.displayChargeUI) // set mouse down instance to trigger event that can be read by UIHandler
                {
                    SoundManager.Instance.PlaySound(Sound.Charge);
                    player.displayChargeUI = true;
                }

                if (launchPower < launchPowerMax)
                {
                    launchPower += launchPowerIncrement;

                    player.chargePercent = (launchPower - launchPowerMin) / (launchPowerMax - launchPowerMin);
                    chargeBar.SetValue(player.chargePercent);

                }
                if (aim != Vector2.zero)
                {
                    player.stickAim = true;
                    if (aim.magnitude > .95)
                    {
                        stickSave = aim;
                    }
                }
            }
        }
    }

    private void LaunchGrapplingHook(float power)
    {
        SoundManager.Instance.PlaySound(Sound.Throw);

        animator.SetBool("Charging", false);
        player.displayChargeUI = false;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPosition = new Vector2(worldPosition.x, worldPosition.y);

        Vector2 difference = targetPosition - (Vector2) transform.position;
        float angle = Vector2.Angle(Vector2.up, difference);

        angle = (worldPosition.x > transform.position.x) ? -angle : angle;

        Vector2 unitVector = Vector2.up;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Vector2 fireVector = rotation * unitVector;

        if (player.stickAim)
        {
            fireVector = stickSave;
        }

        if (player.invertDirection)
        {
            fireVector *= -1f; // Invert the fireVector in the Y direction if invertDirection is true
        }

        GameObject hookObject = Instantiate(hookPrefab, transform.position, Quaternion.identity);

        Hook hook = hookObject.GetComponent<Hook>();
        hook.Player = player;
        hook.Launch(fireVector, power);

        player.currentHook = hookObject;
        GameManager.Instance.ChangeCameraTarget(hookObject.transform);
    }

    public void DestroyGrapplingHook()
    {
        GameManager.Instance.ChangeCameraTarget(transform);

        player.springJoint.connectedBody = rb;
        player.movingToHook = false;

        Destroy(player.currentHook);
        player.currentHook = null;

        player.touchingHook = false;
        player.softlocked = false;
        player.softlockCheckCoroutineRunning = false;
        hookAboveLaunchPosition = false;
    }

    private void RecallLogic()
    {
        if (!player.movingToHook)
        {
            Hook hook = player.currentHook.GetComponent<Hook>();
            if (hook.CanRecall())
                DestroyGrapplingHook();
        }
        else if (
               player.touchingHook 
            || player.currentHook.transform.position.y >= transform.position.y 
            || player.softlocked
            || hookAboveLaunchPosition)
        {
            DestroyGrapplingHook();
        }
    }

    public float ReturnLaunchHeight()
    {
        return launchPosition.y;
    }
}
