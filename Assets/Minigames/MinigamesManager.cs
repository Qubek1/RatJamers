using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigamesManager : MonoSingleton<MinigamesManager>
{
    
    //Just fucking send it
    private const float MINIGAME_OFFSET = 100f;
    
    /// <summary>
    /// Minigame entered, on side of player id, by player id
    /// </summary>
    public static event Action<MinigameController, int, int> MinigameEnteredAction;
    public static Action<int> MinigameLeftAction;
    
    //public bool IsInMinigame { get; private set; } = false;
    
    [SerializeField] private List<MinigameData> m_MinigameDatas;
    
    [SerializeField] private MashingPVPMinigameController m_PVPMinigameInstance;
    private Dictionary<string,MinigameController> _minigamesDict=new();
    
    private MinigameController _player1Minigame;
    private MinigameController _player2Minigame;

    private void Start()
    {
        foreach (var data in m_MinigameDatas)
        {
            MinigameController minigameInstance=CreateMinigameInstance(data.Prefab);
            _minigamesDict.Add(data.MinigameName, minigameInstance);
        }
    }

    private MinigameController CreateMinigameInstance(GameObject minigamePrefab)
    {
        MinigameController minigameInstance= 
            Instantiate(minigamePrefab).GetComponent<MinigameController>();
        
        minigameInstance.transform.position=new Vector3((_minigamesDict.Count+1)*MINIGAME_OFFSET,0,0);

        return minigameInstance;
    }

    public void LaunchMinigame(string minigameName, int onSideOf, int byPlayer)
    {
        
        if (_minigamesDict.TryGetValue(minigameName, out MinigameController instance))
        {
           //Debug.Log($"Launching {minigameName} minigame");
           MinigameEnteredAction?.Invoke(instance, onSideOf, byPlayer);
           instance.Launch(byPlayer,onSideOf);
        }
        else
        {
            Debug.LogWarning($"Attempted to launch {minigameName} that is not in the list!!!");
        }
    }

    public void LaunchPVPMinigame()
    {
        PlayerController.Player1.transform.position = m_PVPMinigameInstance.Player1Pos.position;
        PlayerController.Player2.transform.position = m_PVPMinigameInstance.Player2Pos.position;
        PlayerController.Player1.OnPVPMinigameEntered(m_PVPMinigameInstance);
        PlayerController.Player2.OnPVPMinigameEntered(m_PVPMinigameInstance);
        m_PVPMinigameInstance.LaunchGame();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }
}


[Serializable]
public struct MinigameData
{
    public GameObject Prefab;
    public string MinigameName;
}
