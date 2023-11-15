using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalCompanyModV2.Component;

public class ControlManager : MonoBehaviour
{
    private bool _isMenuOpen;
    private GUIManager _guiManager;

    public void Awake()
    {
        Debug.LogWarning("Control Manager Started");
        _guiManager = new GUIManager();
    }

    public void Update()
    {
        if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
        {
            Debug.Log("[ key pressed - Triggering Method2");
            Debug.Log("Days until deadline value = " + TimeOfDay.Instance.daysUntilDeadline);
            Debug.Log("Time until deadline value = " + TimeOfDay.Instance.timeUntilDeadline);
            Debug.Log("Total time = " + TimeOfDay.Instance.totalTime);
            Debug.Log("Length of hours = " + TimeOfDay.Instance.lengthOfHours);
        }
        else if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            Debug.Log("m key pressed - Triggering Method2");
            ToggleMenu();
        }
        else if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            Debug.Log("i key pressed - Triggering Method2");
            TimeOfDay.Instance.SetNewProfitQuota();
        }
    }

    private void ToggleMenu()
    {
        _isMenuOpen = !_isMenuOpen;

        if (_isMenuOpen)
        {
            Debug.Log("Opening Menu...");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("Closing Menu...");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnGUI()
    {
        if (_isMenuOpen)
        {
            _guiManager.OnGUI();
        }
    }
}