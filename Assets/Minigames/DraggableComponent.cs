using UnityEngine;

public class DraggableComponent : MonoBehaviour
{
    [SerializeField] private float m_DragSpeed;

    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    public void Move(Vector2 moveDir)
    {
        transform.position += (Vector3)moveDir * m_DragSpeed;
    }
}
