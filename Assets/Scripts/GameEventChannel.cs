using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event Channel")]
public class GameEventChannel : ScriptableObject
{
    public event EventHandler BallBouncedOnWallEvent;

    public void BallBouncedOnWall()
    {
        BallBouncedOnWallEvent?.Invoke(this, EventArgs.Empty);
    }
}
