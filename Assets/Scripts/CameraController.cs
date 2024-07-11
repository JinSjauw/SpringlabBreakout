using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameEventChannel gameEventChannel;
    
    [SerializeField][Range(0, 1)] private float cameraShakePower;
    
    private Spring2DComponent cameraSpring;

    private void Awake()
    {
        cameraSpring = GetComponent<Spring2DComponent>();
        cameraSpring.SetEquilibriumPosition(0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        //For Debugging
        inputHandler.SpawnBallEvent += Shake;
        gameEventChannel.BallBouncedOnWallEvent += Shake;
    }

    private void Shake(object sender, EventArgs e)
    {
        ShakeCamera();    
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cameraSpring.SpringValueX, cameraSpring.SpringValueY, -1);
    }

    public void ShakeCamera()
    {
        Vector2 randomShakeDirection = new Vector2(
            Random.Range(-cameraShakePower, cameraShakePower), 
            Random.Range(-cameraShakePower, cameraShakePower));
        cameraSpring.Nudge(randomShakeDirection.x, randomShakeDirection.y);
    }
}
