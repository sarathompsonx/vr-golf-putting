using UnityEngine;

public class InstructionEvents : MonoBehaviour
{
    public delegate void ShowInstructionsDelegate(string instructions);
    public static event ShowInstructionsDelegate OnShowInstructions;

    public delegate void ShowInstructionsForDurationDelegate(string instructions, float duration);
    public static event ShowInstructionsForDurationDelegate OnShowInstructionsForDuration;

    public delegate void HideInstructionsDelegate();
    public static event HideInstructionsDelegate OnHideInstructions;

    public static void ShowInstructions(string instructions)
    {
        OnShowInstructions?.Invoke(instructions);
    }

    public static void ShowInstructionsForDuration(string instructions, float duration)
    {
        OnShowInstructionsForDuration?.Invoke(instructions, duration);
    }

    public static void HideInstructions()
    {
        OnHideInstructions?.Invoke();
    }
}
