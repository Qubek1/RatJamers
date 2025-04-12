using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenManager : MonoBehaviour
{
    [SerializeField] private Camera[] cameras; // Drag & drop two scene cameras
    private int playerCount = 0;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerCount >= cameras.Length)
        {
            Debug.LogWarning("Too many players for available cameras!");
            return;
        }

        Camera assignedCamera = cameras[playerCount];

        // 1. Assign the camera to follow this player
        var followScript = assignedCamera.GetComponent<PlayerCameraController>();
        followScript.target = playerInput.transform;

        // 2. Link camera to PlayerInput (so UI works per camera)
        playerInput.camera = assignedCamera;

        playerCount++;
    }
}