using LethalCompanyScalingMaster;
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
                Debug.Log("Playercount: " + Plugin.GetConnectedPlayers());
            }
            if (!isInitialized && StartOfRound.Instance != null && (Plugin.GetConnectedPlayers() == 1))
            {
                InitializeFunctions();
                isInitialized = true;
            }
           
        }
        
        if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.mKey.wasPressedThisFrame && Plugin.Host)
        {
            Debug.Log("Ctrl key held down and 'm' key pressed - Triggering Method2");
            Debug.Log("Plugin Host: " + Plugin.Host);
            ToggleMenu();
        }
        if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.oKey.wasPressedThisFrame && !Plugin.Host)
        {
            Debug.Log("Ctrl key held down and 'm' key pressed - Triggering Method2");
            Debug.Log("Plugin Host: " + Plugin.Host);
            ToggleMenu();
        }
    }

    private void InitializeFunctions()
    {
        _isMenuOpen = false;
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