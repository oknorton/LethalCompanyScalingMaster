using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalCompanyModV2.Component;

public class ControlManager : MonoBehaviour
{
    private bool _isMenuOpen;
    private GUIManager _guiManager;
    private bool isInitialized;

    public void Awake()
    {
        Debug.LogWarning("Control Manager Started");
        _guiManager = new GUIManager();
    }

    public void Update()
    {
        if (!isInitialized)
        {
            if (StartOfRound.Instance != null)
            {
                Debug.Log("Playercount: " + StartOfRound.Instance.connectedPlayersAmount);
            }
            if (!isInitialized && StartOfRound.Instance != null && (StartOfRound.Instance.connectedPlayersAmount + 1 == 1))
            {
                InitializeFunctions();
                isInitialized = true;
            }
           
        }

        if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
        {
            Plugin.OnPlayerJoin();
        }
        else if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.mKey.wasPressedThisFrame)
        {
            Debug.Log("Ctrl key held down and 'm' key pressed - Triggering Method2");
            ToggleMenu();
        }
    }

    private void InitializeFunctions()
    {
        _isMenuOpen = true;
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