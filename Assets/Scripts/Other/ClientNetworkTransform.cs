using Unity.Netcode.Components;

namespace MultiplayerTask
{
    public class ClientNetworkTransform : NetworkTransform {
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }   
}