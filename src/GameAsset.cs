namespace BesmashContent {
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Reflection;
    using System.Linq;

    /// Represents an object which contains a loadable resource
    [DataContract(IsReference = true)]
    public class GameAsset<T> : ICloneable where T : ICloneable, new() {
        /// The preset resource that this asset is loaded from
        [DataMember]
        [ContentSerializer(Optional = true)]
        public string Preset {get; set;}

        /// The underlying object of the asset
        [DataMember]
        [ContentSerializer(ElementName = "Properties", Optional = true)]
        public T Object {get; set;}

        /// Loads the preset resource file and assigns it to the object.
        /// In case 'Object' was already defined within the underlying
        /// resource of this asset, any of its set properties will
        /// remain the same after this operation completed
        public void load(ContentManager content) {
            if(Preset != null) {
                T foo = new T();
                T preset = content.Load<T>(Preset);

                if(Object != null) { // modifications
                    T def = new T(); // default object
                    object objVal, preVal, defVal;
                    preset.GetType().GetTypeInfo().GetProperties()
                        .ToList().ForEach(pi => {
                            defVal = pi.GetValue(def);
                            preVal = pi.GetValue(preset);
                            objVal = pi.GetValue(Object);

                            if(pi.CanWrite) {
                                if(objVal != null && !objVal.Equals(defVal))
                                    pi.SetValue(def, objVal);
                                else if(preVal != null && !preVal.Equals(defVal))
                                    pi.SetValue(def, preVal);
                            }
                        });

                    Object = def; // new instance
                } else Object = preset;
            }
        }

        public object clone() {
            GameAsset<T> copy = MemberwiseClone() as GameAsset<T>;
            if(Object != null) Object = (T)Object.clone();
            return copy;
        }
    }
}