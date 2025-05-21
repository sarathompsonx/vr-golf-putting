using UnityEngine;
using bmlTUX.Extras.Instructions;

public class InstructionControl : MonoBehaviour
{

    public InstructionsDisplay instructionDisplay;

    private void OnEnable()
    {
        InstructionEvents.OnShowInstructions += instructionDisplay.ShowInstructions;
        InstructionEvents.OnShowInstructionsForDuration += instructionDisplay.ShowInstructionsForDuration;
        InstructionEvents.OnHideInstructions += instructionDisplay.HideInstructions;
    }

    private void OnDisable()
    {
        InstructionEvents.OnShowInstructions -= instructionDisplay.ShowInstructions;
        InstructionEvents.OnShowInstructionsForDuration -= instructionDisplay.ShowInstructionsForDuration;
        InstructionEvents.OnHideInstructions -= instructionDisplay.HideInstructions;
    }
}
