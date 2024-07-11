using UnityEngine;
using UnityEngine.Serialization;

public class SpringComponent : MonoBehaviour
    {
        [Header("Spring Params")] 
        [SerializeField] private float angularFrequency;
        [SerializeField] private float dampingRatio;

        [FormerlySerializedAs("position")] [SerializeField] private float springValue;
        [SerializeField] private float velocity;
        private float equilibriumPosition;
        
        private SpringMotionParams springMotionParams;

        public float SpringValue => springValue;
        
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
            SpringUtils.UpdateDampedSpringMotion(ref springValue, ref velocity, equilibriumPosition, springMotionParams);
        }

        public void SetEquilibriumPosition(float target)
        {
            equilibriumPosition = target;
        }

        public void Nudge(float value)
        {
            springValue += value;
        }
    }


