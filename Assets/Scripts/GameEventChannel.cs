using System;
using UnityEngine;
/// <summary>
/// ScriptableObject to act as an intermediary layer between subscribers and invokers
/// </summary>
[CreateAssetMenu(menuName = "Game Event Channel")]
public class GameEventChannel : ScriptableObject
{
    public event EventHandler BallBouncedOnWallEvent;
    public event EventHandler<BallController> BallSpawnedEvent; 
    public event EventHandler BallLostEvent;
    public event EventHandler BricksClearedEvent;
    
    public void BallBouncedOnWall()
    {
        BallBouncedOnWallEvent?.Invoke(this, EventArgs.Empty);
    }

    public void BallSpawned(BallController spawnedBallController)
    {
        BallSpawnedEvent?.Invoke(this, spawnedBallController);
    }
    
    public void BallLost()
    {
        BallLostEvent?.Invoke(this, EventArgs.Empty);
    }

    public void BricksCleared()
    {
        BricksClearedEvent?.Invoke(this, EventArgs.Empty);
    }
    
    
}
