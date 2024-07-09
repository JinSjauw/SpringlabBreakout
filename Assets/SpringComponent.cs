using UnityEngine;

    public class SpringComponent : MonoBehaviour
    {
        [Header("Spring Params")] 
        [SerializeField] private float angularFrequency;
        [SerializeField] private float dampingRatio;

        [SerializeField] private float position;
        [SerializeField] private float velocity;
        private float equilibriumPosition;
        
        private SpringMotionParams springMotionParams;

        public float Position => position;
        
        private void Awake()
        {
            springMotionParams = new SpringMotionParams();
        }
        
        private void Update()
        {
            UpdateSpring();
        }

        private void UpdateSpring()
        {
            SpringUtils.CalcDampedSpringMotionParams(springMotionParams, Time.deltaTime, angularFrequency, dampingRatio);
            SpringUtils.UpdateDampedSpringMotion(ref position, ref velocity, equilibriumPosition, springMotionParams);
        }

        public void SetEquilibriumPosition(float target)
        {
            equilibriumPosition = target;
        }

        public void Nudge(float value)
        {
            position += value;
        }
    }


