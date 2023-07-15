using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private Player player;

    [Header("Charge")]
    [SerializeField] private CanvasGroup chargeCG;

    [Header("Glide Time")]
    [SerializeField] private CanvasGroup glideCG;
    [SerializeField] private Image bulletTime;
    public float bulletTimeAmount = 100f, drainSpeed = 1f, refillSpeed = 0.5f;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer, hours, minutes, seconds;

    [Header("Distance Traveled")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private GameObject playerObject;
    private float distance, startHeight;

    // Start is called before the first frame update
    void Start()
    {
        startHeight = playerObject.transform.position.y;
        player = playerObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.displayGlideUI) StartCoroutine(FadeCanvasCoroutine(glideCG, 0.1f, true));
        else StartCoroutine(FadeCanvasCoroutine(glideCG, 0.1f, false));

        if (player.displayChargeUI) StartCoroutine(FadeCanvasCoroutine(chargeCG, 0.1f, true));
        else StartCoroutine(FadeCanvasCoroutine(chargeCG, 0.1f, false));

        //timer 
        setTimerText();

        //distance
        setDistanceText();

        //bullet time
        bulletTime.fillAmount = player.glideFramesRemaining / player.maxGlideFrames;
    }

    private void setTimerText()
    {
        timer += Time.deltaTime;
        hours = Mathf.Floor(timer / 3600f);
        minutes = Mathf.Floor(timer / 60f);
        seconds = Mathf.Floor(timer % 60f);

        if (hours <= 0)
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        else 
            timerText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void setDistanceText()
    {
        distance = playerObject.transform.position.y - startHeight;
        if (distance < 0f) distance = 0f;
        distanceText.text = distance.ToString("F0") + "m";
    }

    private IEnumerator FadeCanvasCoroutine(CanvasGroup cg, float duration, bool fadeIn)
    {
        float t = 0;
        while (t <= 1)
        {
            cg.alpha = fadeIn ? t : 1 - t;
            t += Time.deltaTime / duration;
            yield return null;
        }
        cg.alpha = fadeIn ? 1 : 0;
    }
}
