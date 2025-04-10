using UnityEngine;

public class DraggableComponent : MonoBehaviour
{
    [SerializeField] private float m_DragSpeed;
    
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    public void Move(Vector2 moveDir)
    {
        transform.position += (Vector3)moveDir * m_DragSpeed;
    }
    /*
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_SpriteRenderer.color = Color.blue;
        //throw new System.NotImplementedException();
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Plane plane = new Plane(Vector3.up, transform.position);
        m_SpriteRenderer.color = Color.green;
        //Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
        //if()
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_SpriteRenderer.color = Color.red;
        //throw new System.NotImplementedException();
    }
    */
}
