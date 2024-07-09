using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlightController : MonoBehaviour
{
    public bool isAlive { get; private set; } = true;
    public GameObject mapCenter;
    public Camera mainCamera;
    public ProgressBar progressBar;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI recordText;
    public Button restartButton;
    public Button menuButton;
    public AudioSource crashSound;
    public GameObject propeller;
    public SoundController soundController;

    private float cameraDistance = 15; 
    private float cameraForward = 8f;
    private float cameraUp = 1;

    private Vector3 centerPosition;

    public float flightRadius { get; private set; } = 400;
    public float circleLen { get; private set; }
    private float speed = 13;
    private float height = 20;

    private float cameraRadius;

    public float aroundMapAngle { get; private set; } = 0;
    private float angularSpeed;

    private float verticalInput;
    private float verticalInputSensivity = 2;
    private float pitch = 0; 
    private float maxPitch = 89;
    private float pitchDecrementSpeed = 10; 
    private float pitchIncrementSpeed = 35; 
    private float fuelConsumption = 7;    
    private float maxFuel = 100;
    public float fuelAmount { get; private set; }         
    private float fuelItemVolume = 50;    

    public int score { get; private set; } = 0;

    private float almostZero = 0.001f;

    public void collectFuel()
    {
        fuelAmount += fuelItemVolume;
        if (fuelAmount > maxFuel)
        {
            fuelAmount = maxFuel;
        }
    }

    public Vector3 calculatePosition(float height, float angle, float radius)
    {
        return new Vector3(
            centerPosition.x + radius * Mathf.Cos(angle),
            height,
            centerPosition.z + radius * Mathf.Sin(angle));
    }

    void Start()
    {
        circleLen = flightRadius * 2 * Mathf.PI;
        angularSpeed = metersToRadians(speed);
        centerPosition = mapCenter.transform.position;
        cameraRadius = flightRadius + cameraDistance;
        fuelAmount = maxFuel;
        restartButton.gameObject.SetActive(false);
        recordText.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        soundController.setPlaneAlive(true);

    }

    void Update()
    {
        soundController.checkState();
        if (!isAlive) { return; }
        verticalInput = Input.GetAxis("Vertical") * verticalInputSensivity;
        progressBar.BarValue = Mathf.RoundToInt(fuelAmount);
        scoreText.text = "Очки: " + score;
        propeller.transform.eulerAngles = new Vector3(Time.time * 1000, 0, 0);

    }

    private void FixedUpdate()
    {
        if (!isAlive) { return; }
        UpdatePitch();                                          

        aroundMapAngle += angularSpeed * Time.fixedDeltaTime * Mathf.Cos(pitch * Mathf.Deg2Rad);  
        height += Mathf.Sin(pitch * Mathf.Deg2Rad);     
        centerPosition.y = height;                      

        transform.position = calculatePosition(height, aroundMapAngle, flightRadius);
        transform.LookAt(centerPosition);
        transform.Rotate(0, 0, pitch);

        mainCamera.transform.position = calculatePosition(transform.position.y, aroundMapAngle, cameraRadius);
        mainCamera.transform.LookAt(transform);
        mainCamera.transform.position += transform.right * cameraForward;
        Vector3 cameraTarget = mainCamera.transform.position;
        cameraTarget.y = transform.position.y + cameraUp;
        mainCamera.transform.position = cameraTarget;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Obstacle":
                crashSound.Play();
                isAlive = false;
                restartButton.gameObject.SetActive(true);
                menuButton.gameObject.SetActive(true);
                CheckAndSaveScore();
                soundController.setPlaneAlive(false);
                break;
            case "Star":
                score++;
                break;
            case "SuperStar":
                score += 5;
                break;
            case "Fuel":
                collectFuel();
                break;
            default:
                Debug.Log(collision.gameObject);
                break;
        }
    }

    private void UpdatePitch()
    {
        if (verticalInput > almostZero)
        {
            if (fuelAmount <= 0)
            {
                fuelAmount = 0;
                pitch -= pitchDecrementSpeed * Time.fixedDeltaTime;
            }
            else
            {
                fuelAmount -= fuelConsumption * Time.fixedDeltaTime;
                pitch += pitchIncrementSpeed * Time.fixedDeltaTime;
            }
        }
        else if (verticalInput < -almostZero)
        {
            pitch -= pitchIncrementSpeed * Time.fixedDeltaTime;
        }
        else
        {
            pitch -= pitchDecrementSpeed * Time.fixedDeltaTime;
        }
        normalizePitch();
    }

    private void normalizePitch()
    {
        if (pitch > maxPitch)
        {
            pitch = maxPitch;
        }
        else if (pitch < -maxPitch)
        {
            pitch = -maxPitch;
        }
    }

    public float metersToRadians(float distance)
    {
        return (distance / circleLen) * 2 * Mathf.PI;
    }

    public float radiansToMeters(float angle)
    {
        return (angle * circleLen) / (2 * Mathf.PI);
    }

    public void CheckAndSaveScore()
    {
        int[] scores = LoadScores();

        for (int i = 0; i < scores.Length; i++)
        {
            if (score == scores[i]) {
                recordText.gameObject.SetActive(true);
                return; 
            }
            else if (score > scores[i])
            {
                recordText.gameObject.SetActive(true);
                for (int j = scores.Length - 1; j > i; j--)
                {
                    scores[j] = scores[j - 1];
                }
                scores[i] = score;
                break;
            }
        }

        SaveScores(scores[0], scores[1], scores[2]);
    }

    private void SaveScores(int score1, int score2, int score3)
    {
        PlayerPrefs.SetInt("HighScore1", score1);
        PlayerPrefs.SetInt("HighScore2", score2);
        PlayerPrefs.SetInt("HighScore3", score3);
        PlayerPrefs.Save();
    }

    private int[] LoadScores()
    {
        int[] scores = new int[3];
        scores[0] = PlayerPrefs.GetInt("HighScore1", 0);
        scores[1] = PlayerPrefs.GetInt("HighScore2", 0);
        scores[2] = PlayerPrefs.GetInt("HighScore3", 0);
        return scores;
    }
}