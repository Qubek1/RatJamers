using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class DraggableComponent : MonoBehaviour
{
    [SerializeField] private float m_DragSpeed;
    [SerializeField] private float m_RotateSpeed=0.1f;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    private Vector2 _initScale;
    private Vector2 _initPos;

    private float _currentRotDir;

    public DraggableSlotComponent LastSlot
    {
        get;
        private set;
    }

    public bool IsInSlot
    {
        get;
        private set;
    }

    private void Awake()
    {
        DraggableMinigameController.ResetAction+= HandleReset;
        
        _initScale= transform.localScale;
        _initPos= transform.position;
        _canBeMoved = true;
    }

    private void OnDestroy()
    {
        DraggableMinigameController.ResetAction-= HandleReset;
    }

    public void OnSelected()
    {
        m_SpriteRenderer.color = GetColorWithAlpha(m_SpriteRenderer.color, 1.0f);
    }

    public void OnDeselected()
    {
        m_SpriteRenderer.color = GetColorWithAlpha(m_SpriteRenderer.color, 0.5f);
    }

    private void Update()
    {
        //rotation
        
        if((!IsInSlot)&&(!Mathf.Approximately(_currentRotDir, 0)))
            transform.Rotate(new Vector3(0,0, _currentRotDir * m_RotateSpeed * Time.deltaTime));
        
        
        //allowing draggable to be removed from current slot
        if(IsInSlot||LastSlot==null) return;
        float distanceFromLastSlot=Vector2.Distance(transform.position, LastSlot.transform.position);
        //can be considered to be attached to a slot again
        if (distanceFromLastSlot > DraggableMinigameController.DRAGGABLE_POSITION_THRESHOLD*1.1f)
        {
            LastSlot = null;
        }
        
    }

    private bool _canBeMoved;

    private IEnumerator AllowForMovementAfterUsingSlot()
    {
        _canBeMoved = false;
        yield return new WaitForSeconds(0.2f);
        _canBeMoved = true;
    }

    public void Move(Vector2 moveDir)
    {
        if(!_canBeMoved) return;
        
        if (IsInSlot)
        {
            LastSlot.FreeSlot();
            IsInSlot = false;
        }
        transform.position += (Vector3)moveDir * m_DragSpeed;
    }

    public void StartRotate(float dir)
    {
        _currentRotDir = dir;
        //transform.Rotate(new Vector3(0,0, dir * m_RotateSpeed * Time.deltaTime));
    }

    public void StopRotate()
    {
        _currentRotDir = 0;
    }

    public void OnPutIntoSlot(DraggableSlotComponent slot)
    {
        LastSlot = slot;
        IsInSlot = true;
        m_SpriteRenderer.color = GetColorWithAlpha(m_SpriteRenderer.color, 1.0f);
        transform.localScale = _initScale * 1.2f;
        StopAllCoroutines();
        StartCoroutine(AllowForMovementAfterUsingSlot());
    }

    public void HandleReset()
    {
        StopAllCoroutines();
        IsInSlot = false;
        transform.position = _initPos;
        transform.localScale = _initScale;
        LastSlot = null;
        _canBeMoved = true;
        _currentRotDir = 0;
    }
    
    private static Color GetColorWithAlpha(Color color, float newAlpha)
        => new(color.r, color.g, color.b, newAlpha);
}
