using UnityEngine;

public interface ITouchObject
{
    void OnStart(Vector3 pos);
    
    void OnMoving(Vector3 pos);
    
    void OnStop();
}
