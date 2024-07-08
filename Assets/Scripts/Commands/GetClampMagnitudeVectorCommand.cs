using QFramework;
using UnityEngine;

namespace Commands
{
    public class GetClampMagnitudeVectorCommand : AbstractCommand<Vector3>
    {
        private Vector3 _offset;
        private ConfigGame _configGame;

        public GetClampMagnitudeVectorCommand(Vector3 offset)
        {
            _offset = offset;
        }

        protected override Vector3 OnExecute()
        {
            _configGame = ConfigGame.Instance;
            return GetClampMagnitudeVector(_offset);
        }

        private Vector3 GetClampMagnitudeVector(Vector3 offset)
        {
            if ((Mathf.Abs(offset.x) < _configGame.MinSensitivity &&
                 Mathf.Abs(offset.y) < _configGame.MinSensitivity) ||
                Mathf.Abs(Mathf.Abs(offset.x) - Mathf.Abs(offset.y)) < _configGame.MinSensitivity)
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