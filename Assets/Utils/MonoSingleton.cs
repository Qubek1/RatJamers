using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (!Instance)
            Instance = this as T;
        else
            Destroy(this);
    }
    
    public bool IsTheOne()=>this==Instance;
}