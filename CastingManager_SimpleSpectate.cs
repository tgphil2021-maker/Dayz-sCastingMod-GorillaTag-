CastingManager_SimpleSpectate.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CastingManager_SimpleSpectate : MonoBehaviour
{
    public enum ViewMode { FirstPerson, ThirdPerson }

    [Header("Camera")]
    public Camera mainCamera;
    public Camera anchorCamera;
    public Transform spectateTarget; // Assign your player or character here
    public Vector3 thirdPersonOffset = new Vector3(0, 2, -4);
    public float smoothing = 0.2f;
    public float targetFOV = 70f;
    public ViewMode currentView = ViewMode.ThirdPerson;

    [Header("UI")]
    public Button toggleViewButton;
    public Slider smoothingSlider;
    public Slider fovSlider;
    public Button fullscreenButton;
    public Button scoreboardButton;
    public Button stopwatchButton;

    [Header("Scoreboard UI")]
    public GameObject scoreboardPanel;
    public TMP_Text team1Name;
    public TMP_Text team2Name;
    public TMP_Text team1Score;
    public TMP_Text team2Score;
    public TMP_InputField team1Input;
    public TMP_InputField team2Input;
    public Button addScoreTeam1;
    public Button addScoreTeam2;
    public TMP_Text stopwatchText;

    private bool scoreboardEnabled = false;
    private bool stopwatchRunning = false;
    private float stopwatchTime = 0f;

    void Start()
    {
        // Hook up UI events
        toggleViewButton.onClick.AddListener(ToggleView);
        smoothingSlider.onValueChanged.AddListener(v => smoothing = v);
        fovSlider.onValueChanged.AddListener(v => mainCamera.fieldOfView = v);
        fullscreenButton.onClick.AddListener(() => Screen.fullScreen = !Screen.fullScreen);
        scoreboardButton.onClick.AddListener(ToggleScoreboard);
        stopwatchButton.onClick.AddListener(EnableStopwatch);
        addScoreTeam1.onClick.AddListener(() => AddPoint(team1Score));
        addScoreTeam2.onClick.AddListener(() => AddPoint(team2Score));

        // Setup anchor camera
        anchorCamera.rect = new Rect(0.02f, 0.75f, 0.2f, 0.2f);
        anchorCamera.depth = 1;
        anchorCamera.enabled = true;

        scoreboardPanel.SetActive(false);
    }

    void LateUpdate()
    {
        UpdateCameraFollow();
        UpdateStopwatch();
    }

    // ----- CAMERA FOLLOW LOGIC -----
    void UpdateCameraFollow()
    {
        if (!spectateTarget || !mainCamera) return;

        if (currentView == ViewMode.FirstPerson)
        {
            Vector3 targetPos = spectateTarget.position + Vector3.up * 1.7f;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, smoothing);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, spectateTarget.rotation, smoothing);
        }
        else if (currentView == ViewMode.ThirdPerson)
        {
            Vector3 desiredPos = spectateTarget.position + spectateTarget.TransformDirection(thirdPersonOffset);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPos, smoothing);
            mainCamera.transform.LookAt(spectateTarget.position + Vector3.up * 1.5f);
        }
    }

    void ToggleView()
    {
        currentView = currentView == ViewMode.FirstPerson ? ViewMode.ThirdPerson : ViewMode.FirstPerson;
        Debug.Log($"View mode: {currentView}");
    }

    // ----- SCOREBOARD -----
    void ToggleScoreboard()
    {
        scoreboardEnabled = !scoreboardEnabled;
        scoreboardPanel.SetActive(scoreboardEnabled);
    }

    void AddPoint(TMP_Text scoreText)
    {
        int score = int.Parse(scoreText.text);
        scoreText.text = (score + 1).ToString();
    }

    // ----- STOPWATCH -----
    void EnableStopwatch()
    {
        stopwatchRunning = true;
        stopwatchTime = 0f;
    }

    void UpdateStopwatch()
    {
        if (!stopwatchRunning) return;

        stopwatchTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);
        stopwatchText.text = $"{minutes:00}:{seconds:00}";

        if (stopwatchTime >= 180f)
            stopwatchRunning = false;
    }
}
