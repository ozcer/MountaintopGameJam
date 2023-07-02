using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("Bullet Time")]
    [SerializeField] private Image bulletTime;
    public float bulletTimeAmount = 100f, drainSpeed = 1f, refillSpeed = 0.5f;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer, hours, minutes, seconds;

    [Header("Distance Traveled")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private GameObject player;
    private float distance, startHeight;

    // Start is called before the first frame update
    void Start()
    {
        startHeight = player.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //timer 
        setTimerText();

        //distance
        setDistanceText();

        //bullet time
        if (Input.GetKeyDown(KeyCode.Space)) useBulletTime();
        else refillBulletTime();
    }

    private void useBulletTime()
    {
        bulletTimeAmount -= Time.deltaTime * drainSpeed;
        bulletTime.fillAmount = bulletTimeAmount / 100f;
    }

    private void refillBulletTime()
    {
        bulletTimeAmount += Time.deltaTime * refillSpeed;
        bulletTimeAmount = Mathf.Clamp(bulletTimeAmount, 0f, 100f);
        bulletTime.fillAmount = bulletTimeAmount / 100f;
    }

    private void setTimerText()
    {
        timer += Time.deltaTime;
        hours = Mathf.Floor(timer / 3600f);
        minutes = Mathf.Floor(timer / 60f);
        seconds = Mathf.Floor(timer % 60f);

        if (hours <= 0)
            timerText.text = "Time Elapsed: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        else 
            timerText.text = "Time Elapsed: " + hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void setDistanceText()
    {
        distance = player.transform.position.y - startHeight;
        if (distance < 0f) distance = 0f;
        distanceText.text = "Current Height: " + distance.ToString("F2") + "m";
    }
}
