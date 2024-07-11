using UnityEngine;

public class Spring2DComponent : MonoBehaviour
{
    [Header("Spring Params X")] 
    [SerializeField] private float angularFrequencyX;
    [SerializeField] private float dampingRatioX;

    [Header("Spring Params Y")] 
    [SerializeField] private float angularFrequencyY;
    [SerializeField] private float dampingRatioY;
    
    private float springValueX;
    private float velocityX;
    
    private float springValueY;
    private float velocityY;
    
    private float equilibriumPositionX;
    private float equilibriumPositionY;
        
    private SpringMotionParams springMotionParamsX;
    private SpringMotionParams springMotionParamsY;

    public float SpringValueX => springValueX;
    public float SpringValueY => springValueY;
        
    private void Awake()
    {
        springMotionParamsX = new SpringMotionParams();
        springMotionParamsY = new SpringMotionParams();
    }
        
    private void Update()
    {
        UpdateSpring();
    }

    private void UpdateSpring()
    {
        SpringUtils.CalcDampedSpringMotionParams(springMotionParamsX, Time.deltaTime, angularFrequencyX, dampingRatioX);
        SpringUtils.UpdateDampedSpringMotion(ref springValueX, ref velocityX, equilibriumPositionX, springMotionParamsX);
        
        SpringUtils.CalcDampedSpringMotionParams(springMotionParamsY, Time.deltaTime, angularFrequencyY, dampingRatioY);
        SpringUtils.UpdateDampedSpringMotion(ref springValueY, ref velocityY, equilibriumPositionY, springMotionParamsY);
    }

    public void SetEquilibriumPosition(float targetX, float targetY)
    {
        equilibriumPositionX = targetX;
        equilibriumPositionY = targetY;
    }

    public void Nudge(float valueX, float valueY)
    {
        springValueX += valueX;
        springValueY += valueY;
    }
}
