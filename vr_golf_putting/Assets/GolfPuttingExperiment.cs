using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.UI;
// using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
// using ClosedXML.Excel;
using TMPro;

public class GolfPuttingExperiment : MonoBehaviour
{
    // References to game objects
    [Header("Game Objects")]
    public GameObject ball;
    public GameObject hole;
    public GameObject putter;
    public Transform ballStartPosition;
    public Canvas instructionCanvas;
   public TextMeshProUGUI instructionText;
public TextMeshProUGUI trialCounterText;
public TextMeshProUGUI forceText;
    
    // Experiment parameters
    [Header("Experiment Settings")]
    public int totalTrials = 40;
    public float holeDistance = 200f; // Distance in cm
    public string participantID = "P001";
    public float maxPuttForce = 500f;
    public float forceBuildRate = 100f; // How quickly force builds when holding trigger
    
    // Meta Quest 2 controller references
    [Header("VR Controller")]
public InputActionReference triggerAction;
public InputActionReference nextTrialAction;
    
    // Physics parameters
    [Header("Physics")]
    public float ballFriction = 0.05f;
    public float surfaceFriction = 0.3f;
    
    // Private variables
    private int currentTrial = 0;
    private float trialStartTime;
    private float currentPuttForce = 0f;
    private bool isBuildingForce = false;
    private bool ballInMotion = false;
    private bool trialComplete = false;
    private Vector3 initialBallPosition;
    private Vector3 holePosition;
    private Rigidbody ballRigidbody;
    
    // Data collection
    private List<TrialData> allTrialData = new List<TrialData>();
    private List<Vector2> trialPositions = new List<Vector2>();
    
    // Structure to store data for each trial
    private class TrialData
    {
        public string ParticipantID;
        public string Date;
        public int TrialNumber;
        public float TrialTime;
        public float BallPositionX;
        public float BallPositionZ;
        public float DistanceToHole;
        public float RadialError;
        public float ConstantError;
        public float VariableErrorX;
        public float VariableErrorZ;
    }
    
    void Start()
    {
        // Setup initial positions and references
        initialBallPosition = ballStartPosition.position;
        holePosition = hole.transform.position;
        ballRigidbody = ball.GetComponent<Rigidbody>();
        
        if (ballRigidbody == null)
        {
            Debug.LogError("Ball must have a Rigidbody component!");
            return;
        }
        
        // Set physical properties
        ballRigidbody.linearDamping = ballFriction;
        
        // Reset experiment
        ResetBall();
        UpdateTrialCounter();
        
        instructionText.text = "Hold trigger to build force, release to putt. Press A to start the next trial.";
    }
    
    void Update()
    {
        // Check for controller input
        CheckControllerInput();
        
        // Handle force building when trigger is held down
        if (isBuildingForce)
        {
            currentPuttForce += forceBuildRate * Time.deltaTime;
            currentPuttForce = Mathf.Clamp(currentPuttForce, 0f, maxPuttForce);
            forceText.text = $"Force: {Mathf.RoundToInt(currentPuttForce)}";
        }
        
        // Check if the ball has stopped moving to end the trial
        if (ballInMotion && ballRigidbody.linearVelocity.magnitude < 0.01f)
        {
            EndTrial();
        }
    }
    
    private void CheckControllerInput()
    {
        bool isTriggerPressed = triggerAction.action.IsPressed();
       bool isNextTrialButtonPressed = nextTrialAction.action.IsPressed();

        
        // Start building force when trigger is pressed
        if (isTriggerPressed && !isBuildingForce && !ballInMotion && !trialComplete)
        {
            isBuildingForce = true;
            currentPuttForce = 0f;
        }
        // Release putt when trigger is released
        else if (!isTriggerPressed && isBuildingForce)
        {
            isBuildingForce = false;
            PuttBall();
        }
        
        // Check for next trial button (A button) press
            
            if (isNextTrialButtonPressed && trialComplete)
        {
            StartNextTrial();
        }
    }
    
    private void PuttBall()
    {
        // Apply force to the ball in the direction of the hole
        Vector3 direction = (hole.transform.position - ball.transform.position).normalized;
        ballRigidbody.AddForce(direction * currentPuttForce, ForceMode.Impulse);
        
        // Start the trial timer
        trialStartTime = Time.time;
        ballInMotion = true;
        
        // Hide force text during ball motion
        forceText.text = "";
        
        // Give user feedback
        instructionText.text = "Ball in motion...";
    }
    
    private void EndTrial()
    {
        ballInMotion = false;
        trialComplete = true;
        
        // Calculate trial time
        float trialTime = Time.time - trialStartTime;
        
        // Record final ball position
        Vector3 finalBallPosition = ball.transform.position;
        Vector2 ballPos2D = new Vector2(finalBallPosition.x, finalBallPosition.z);
        Vector2 holePos2D = new Vector2(holePosition.x, holePosition.z);
        
        // Add to positions list for calculating variable error
        trialPositions.Add(ballPos2D);
        
        // Calculate errors
        float distanceToHole = Vector2.Distance(ballPos2D, holePos2D);
        float radialError = distanceToHole;
        
        // Calculate constant error (signed error along the distance axis)
        // Positive means overshot, negative means undershot
        Vector2 directionToHole = (holePos2D - new Vector2(initialBallPosition.x, initialBallPosition.z)).normalized;
        float constantError = Vector2.Dot(ballPos2D - holePos2D, directionToHole);
        
        // Calculate variable error (standard deviation of endpoints)
        float variableErrorX = 0f;
        float variableErrorZ = 0f;
        
        if (trialPositions.Count > 1)
        {
            // Calculate mean position
            Vector2 meanPos = Vector2.zero;
            foreach (Vector2 pos in trialPositions)
            {
                meanPos += pos;
            }
            meanPos /= trialPositions.Count;
            
            // Calculate standard deviation
            float sumSqX = 0f;
            float sumSqZ = 0f;
            foreach (Vector2 pos in trialPositions)
            {
                sumSqX += Mathf.Pow(pos.x - meanPos.x, 2);
                sumSqZ += Mathf.Pow(pos.y - meanPos.y, 2);
            }
            
            variableErrorX = Mathf.Sqrt(sumSqX / trialPositions.Count);
            variableErrorZ = Mathf.Sqrt(sumSqZ / trialPositions.Count);
        }
        
        // Create trial data object
        TrialData data = new TrialData
        {
            ParticipantID = participantID,
            Date = DateTime.Now.ToString("yyyy-MM-dd"),
            TrialNumber = currentTrial + 1,
            TrialTime = trialTime,
            BallPositionX = finalBallPosition.x,
            BallPositionZ = finalBallPosition.z,
            DistanceToHole = distanceToHole,
            RadialError = radialError,
            ConstantError = constantError,
            VariableErrorX = variableErrorX,
            VariableErrorZ = variableErrorZ
        };
        
        // Add to data collection
        allTrialData.Add(data);
        
        // Update instruction text
        instructionText.text = $"Trial complete! Distance to hole: {distanceToHole:F2} cm\nPress A button to start next trial.";
    }
    
    private void StartNextTrial()
    {
        // Increment trial counter
        currentTrial++;
        
        // Check if experiment is complete
        if (currentTrial >= totalTrials)
        {
            EndExperiment();
            return;
        }
        
        // Reset for next trial
        ResetBall();
        trialComplete = false;
        UpdateTrialCounter();
        
        instructionText.text = "Hold trigger to build force, release to putt.";
    }
    
    private void ResetBall()
    {
        // Reset ball position and physics
        ball.transform.position = initialBallPosition;
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        
        // Reset force
        currentPuttForce = 0f;
        forceText.text = "Force: 0";
    }
    
    private void UpdateTrialCounter()
    {
        trialCounterText.text = $"Trial: {currentTrial + 1} / {totalTrials}";
    }
    
    private void EndExperiment()
    {
                // Save all data to Excel file
        // SaveDataToExcel();
        
        // Show completion message
        instructionText.text = "Experiment complete! Thank you for participating.";
        trialCounterText.text = "All trials completed";
        forceText.text = "";
    }
    /*
    // private void SaveDataToExcel()
    {
        try
        {
            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet
                var worksheet = workbook.Worksheets.Add("Results");
                
                // Add headers
                worksheet.Cell(1, 1).Value = "Participant ID";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Trial Number";
                worksheet.Cell(1, 4).Value = "Trial Time (s)";
                worksheet.Cell(1, 5).Value = "Ball Position X (cm)";
                worksheet.Cell(1, 6).Value = "Ball Position Z (cm)";
                worksheet.Cell(1, 7).Value = "Distance to Hole (cm)";
                worksheet.Cell(1, 8).Value = "Radial Error (cm)";
                worksheet.Cell(1, 9).Value = "Constant Error (cm)";
                worksheet.Cell(1, 10).Value = "Variable Error X (cm)";
                worksheet.Cell(1, 11).Value = "Variable Error Z (cm)";
                
                // Add data rows
                for (int i = 0; i < allTrialData.Count; i++)
                {
                    var data = allTrialData[i];
                    int row = i + 2; // Start from row 2 (after headers)
                    
                    worksheet.Cell(row, 1).Value = data.ParticipantID;
                    worksheet.Cell(row, 2).Value = data.Date;
                    worksheet.Cell(row, 3).Value = data.TrialNumber;
                    worksheet.Cell(row, 4).Value = data.TrialTime;
                    worksheet.Cell(row, 5).Value = data.BallPositionX;
                    worksheet.Cell(row, 6).Value = data.BallPositionZ;
                    worksheet.Cell(row, 7).Value = data.DistanceToHole;
                    worksheet.Cell(row, 8).Value = data.RadialError;
                    worksheet.Cell(row, 9).Value = data.ConstantError;
                    worksheet.Cell(row, 10).Value = data.VariableErrorX;
                    worksheet.Cell(row, 11).Value = data.VariableErrorZ;
                }
                
                // Format the worksheet
                worksheet.Columns().AdjustToContents();
                
                // Create directory if it doesn't exist
                string directory = Application.persistentDataPath + "/Results";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Save the file
                string fileName = $"{directory}/GolfPutting_{participantID}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                workbook.SaveAs(fileName);
                
                Debug.Log($"Data saved to: {fileName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving data to Excel: {e.Message}");
        }
    }
    */
}
