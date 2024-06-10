using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using utilities;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private CinemachineConfiner2D confiner2D;

        private void Start()
        {
            confiner2D.m_BoundingShape2D = GlobalManager.Instance.gameManager.cameraBound;
        }

        public void ShankCamera(Vector3 snakeAmount)
        {
            impulseSource.GenerateImpulse(snakeAmount);
        }
    }
}