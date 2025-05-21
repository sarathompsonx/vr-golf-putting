using System.Collections;
using System.Data;
using bmlTUX;
using UnityEngine;

/// <summary>
/// Classes that inherit from Trial define custom behaviour for your experiment's trials.
/// Most experiments will need to edit this file to describe what happens in a trial.
///
/// This template shows how to set up a custom trial script using the toolkit's built-in functions.
///
/// You can delete any unused methods and unwanted comments. The only required parts are the constructor and the MainCoroutine.
/// </summary>
public class GolfPuttingBodyImageTrial : Trial {


    // // You usually want to store a reference to your experiment runner
    GolfPuttingBodyImageRunner myRunner;

    Vector2 startPos;
    
    // Required Constructor. Good place to set up references to objects in the unity scene
    public GolfPuttingBodyImageTrial(ExperimentRunner runner, DataRow data) : base(runner, data)
    {
        myRunner = (GolfPuttingBodyImageRunner)runner;  //cast the generic runner to your custom type.
        // GameObject myGameObject = myRunner.MyGameObject;  // get reference to gameObject stored in your custom runner

    }


    // Optional Pre-Trial code. Useful for setting unity scene for trials. Executes in one frame at the start of each trial
    protected override void PreMethod()
    {
        startPos = (Vector2)Data["StartPosition"]; // Read values of independent variables
    
        // float thisTrialsDistanceValue = (float)Data["MyDistanceFloatVariableName"]; // Read values of independent variables
        // myGameObject.transform.position = new Vector3(thisTrialsDistanceValue, 0, 0); // set up scene based on value
    }


    // Optional Pre-Trial code. Useful for waiting for the participant to
    // do something before each trial (multiple frames). Also might be useful for fixation points etc.
    protected override IEnumerator PreCoroutine() {

        GolfBallEvents.SetStartPosition(startPos.x, startPos.y); // set the start position of the golf ball

        InstructionEvents.ShowInstructions("Press the spacebar to start the trial.");

        yield return null; //required for coroutine
    }


    // Main Trial Execution Code.
    protected override IEnumerator RunMainCoroutine() {
    
        // You might want to do a while-loop to wait for participant response: 
        bool waitingForParticipantResponse = true;
        Debug.Log("Press the spacebar to end this trial.");
        while (waitingForParticipantResponse) {   // keep check each frame until waitingForParticipantResponse set to false.
            if (Input.GetKeyDown(KeyCode.Space)) { // check return key pressed
                waitingForParticipantResponse = false;  // escape from while loop
            }
        
            yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
        }
    
    }


    // Optional Post-Trial code. Useful for waiting for the participant to do something after each trial (multiple frames)
    protected override IEnumerator PostCoroutine() {
        yield return null;
    }


    // Optional Post-Trial code. useful for writing data to dependent variables and for resetting everything.
    // Executes in a single frame at the end of each trial
    protected override void PostMethod() {
        // How to write results to dependent variables: 
        // Data["MyDependentFloatVariable"] = someFloatVariable;
    }
}

