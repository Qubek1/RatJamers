using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact(PlayerController interactingPlayer);
    public bool IsInteractable(PlayerController interactingPlayer);
}
