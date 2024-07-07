using QFramework;
using UnityEngine;

namespace Commands
{
    public class GetClampMagnitudeVectorCommand:AbstractCommand<Vector3>
    {
        private Vector3 _offset;
        private float _minSensitivity;

        public GetClampMagnitudeVectorCommand(Vector3 offset, float minSensitivity)
        {
            _offset = offset;
            _minSensitivity = minSensitivity;
        }

        protected override Vector3 OnExecute()
        {
           return GetClampMagnitudeVector(_offset);
        }
        private Vector3 GetClampMagnitudeVector(Vector3 offset)
        {
            if ((Mathf.Abs(offset.x) < _minSensitivity && Mathf.Abs(offset.y) < _minSensitivity) ||
                Mathf.Abs(Mathf.Abs(offset.x) - Mathf.Abs(offset.y)) < _minSensitivity)
            {
                return Vector3.zero;
            }

            var min = Mathf.Abs(offset.x) - Mathf.Abs(offset.y) > 0 ? offset.y : offset.x;

            var directionAxis = offset - new Vector3(min, min);
            return new Vector3(GetClampMagnitude(directionAxis.x), GetClampMagnitude(directionAxis.y));
        }

        private float GetClampMagnitude(float value)
        {
            return value switch
            {
                > 1 => 1,
                < -1 => -1,
                _ => value
            };
        }
    }
}