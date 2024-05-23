using Fusion;

namespace Player
{
    public struct PlayerData : INetworkInput
    {
        public float horizontalInput;
        public NetworkButtons networkButtons;
    }
}