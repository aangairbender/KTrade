using System.Runtime.Serialization;

namespace KTrade.Core
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Name { get; set; }

        public IPlayerController PlayerController { get; set; }
        public IPlayerControllerCallback PlayerControllerCallback { get; set; }
    }
}