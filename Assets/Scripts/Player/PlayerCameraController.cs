using Cinemachine;
using UnityEngine;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource impulseSource;

        public void ShankCamera(Vector3 snakeAmount)
        {
            impulseSource.GenerateImpulse(snakeAmount);
        }
    }
}