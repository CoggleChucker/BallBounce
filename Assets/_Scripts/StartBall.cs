using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartBall : MonoBehaviour
{
    public Slider timingSlider;
    public float sliderFillSpeed = 5f;
    public float sliderMaxValue = 2f;
    public bool canBowl = true;

    public float timeBetweenBowling = 1f;

    private float sliderCurrentValue = 0f;

    CricketBowlingController cricketBowlingController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cricketBowlingController = GetComponent<CricketBowlingController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canBowl)
        {
            return;
        }

        sliderCurrentValue += sliderFillSpeed * Time.deltaTime;

        if (sliderCurrentValue <= 0f || sliderCurrentValue >= sliderMaxValue)
        {
            SwitchSliderDirection();
        }

        timingSlider.value = sliderCurrentValue;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartBowling();
        }
    }

    public void StartBowling()
    {
        if (canBowl)
        {
            canBowl = false;
            cricketBowlingController.SetMultiplier(CalculateMultiplier(sliderCurrentValue));
            cricketBowlingController.Bowl();
            StartCoroutine(NextBall());
        }
    }

    private void SwitchSliderDirection()
    {
        sliderFillSpeed = -sliderFillSpeed;
    }

    private IEnumerator NextBall()
    {
        yield return new WaitForSeconds(timeBetweenBowling);
        canBowl = true;
    }

    private float CalculateMultiplier(float sliderValue)
    {
        float multiplier = 0f;
        float accuracyValue = sliderValue > 1 ? sliderValue - 1 : 1 - sliderValue;

        if (accuracyValue < 0.1)
        {
            multiplier = 1f;
        }
        else if (accuracyValue < 0.5f)
        {
            multiplier = 0.7f;
        }
        else if (accuracyValue < 0.7f)
        {
            multiplier = 0.4f;
        }
        else
        {
            multiplier = 0f;
        }

        return multiplier;
    }

}
