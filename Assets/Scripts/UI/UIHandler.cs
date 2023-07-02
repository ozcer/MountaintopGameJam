using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    //bullet time
    [SerializeField] private Image bulletTime;
    public float bulletTimeAmount = 100f, drainSpeed = 1f, refillSpeed = 0.5f;

    //timer
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer, hours, minutes, seconds;

    //distance travelled

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //timer 
        setTimerText();

        //bullet time
        if (Input.GetMouseButton(1)) useBulletTime();
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
}
