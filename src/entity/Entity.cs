namespace BesmashContent  {
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Content;
    
    [DataContract]
    public class Entity : MapObject  {
        /// The title of this entity
        [DataMember]
        [ContentSerializer(Optional = true)]
        public string Title {get; set;}

        public Entity() {
            // default title
            Title = GetType().Name;
        }
    }
}