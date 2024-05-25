using Fusion;
using UnityEngine;

namespace Player
{
    public struct PlayerData : INetworkInput
    {
        public float horizontalInput;
        public Quaternion gunPivotRotation;
        public NetworkButtons networkButtons;
    }
}