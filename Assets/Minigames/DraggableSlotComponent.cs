
using System.Collections.Generic;
using UnityEngine;

public class DraggableSlotComponent : MonoBehaviour
{
    private static List<DraggableSlotComponent> _instances = new();

    public static DraggableSlotComponent IsWithin(Vector2 pos, float threshold)
    {
        foreach (DraggableSlotComponent slot in _instances)
            if (Vector2.Distance(pos, slot.transform.position) < threshold)
                return slot;

        return null;
    }

    [SerializeField] private DraggableComponent m_AssignedDraggable;
    public bool IsAnyAssigned(DraggableComponent draggable)
        => m_AssignedDraggable != null && m_AssignedDraggable == draggable;

    public bool IsCorrectAssigned() => _usedBy != null && _usedBy == m_AssignedDraggable;
    
    private DraggableComponent _usedBy;

    private void Awake()
    {
        DraggableMinigameController.ResetAction += HandleReset;
        _instances.Add(this);
        if (m_AssignedDraggable == null)
        {
            Debug.LogWarning($"{gameObject.name} draggable slot has no draggable assigned");
        }
    }

    private void OnDestroy()
    {
        _instances.Remove(this);
        DraggableMinigameController.ResetAction -= HandleReset;
    }

    public void UseSlot(DraggableComponent draggable)
    {
        _usedBy = draggable;
    }

    public void FreeSlot()
    {
        if (_usedBy == null)
        {
            Debug.LogWarning($"Trying to free {gameObject.name} slot that is not used!");
        }
        _usedBy = null;
    }
    
    public bool IsUsed()=> _usedBy != null;

    public void HandleReset()
    {
        _usedBy = null;
    }
}
