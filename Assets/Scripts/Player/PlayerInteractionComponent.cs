using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{
    
    private HashSet<IInteractable> _availableInteractables = new();
    int _ownerPlayerNumber;

    private void Start()
    {
        PlayerController owner = GetComponent<PlayerController>();
        _ownerPlayerNumber = owner.GetPlayerNumber();
    }

    public void RegisterInteractable(IInteractable interactable)
    {
        if(_availableInteractables.Contains(interactable))
            Debug.LogWarning("Interactable already registered");
        else
            _availableInteractables.Add(interactable);
    }
    public void DeRegisterInteractable(IInteractable interactable)
    {
        if(_availableInteractables.Contains(interactable))
            _availableInteractables.Remove(interactable);
        else
            Debug.LogWarning("Interactable not registered");
    }

    public void ReceiveInteractionInput()
    {
        if(GetComponent<PlayerController>()==PlayerController.Player1)
            GetClosestInteractable()?.Interact(1);
        else
            GetClosestInteractable()?.Interact(2);
    }
    
    private IInteractable GetClosestInteractable(){
        if(_availableInteractables.Count==0){
            return null;
        }
        IInteractable closestInteractable = null;
        float minDistance = float.MaxValue;
        foreach(IInteractable interactable in _availableInteractables){
            if(!interactable.IsInteractable(_ownerPlayerNumber)) continue;
            Component interactableComponent = interactable as Component;
            if(!interactableComponent) continue; //handle possible NullRefException
            
            float distance=Vector2.Distance(transform.position,interactableComponent.transform.position);
            if(distance<minDistance){
                minDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }
}
