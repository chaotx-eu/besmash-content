namespace BesmashContent {
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Collections;
    using System.Reflection;
    using System.Linq;
    using System;

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
        /// remain the same after this operation completed. In case no
        /// preset is defined a new object will be created using the
        /// object types default parameters as preset
        public void load(ContentManager content) {
            T preset = Preset == null ? new T()
                : content.Load<T>(Preset);

            if(Object != null) { // modifications
                T def = new T(); // default object
                object objVal, preVal, defVal;
                preset.GetType().GetTypeInfo().GetProperties()
                    .ToList().ForEach(pi => {
                        defVal = pi.GetValue(def);
                        preVal = pi.GetValue(preset);
                        objVal = pi.GetValue(Object);

                        if(pi.CanWrite) {
                            // TODO test copy list
                            // if(preVal is IList || objVal is IList) {
                            //     List<object> list = new List<object>();

                            //     if(preVal != null) foreach(var val in (preVal as IList))
                            //         list.Add(val);
                            //     if(objVal != null) foreach(var val in (objVal as IList))
                            //         list.Add(val);
                                    
                            //     pi.SetValue(def, list); // TODO fix crash => cannot cast List<object>
                            // } // else if

                            // TODO alternative solution (logic is flawed, list is not copied)
                            if(preVal is IList || objVal is IList) {
                                IList list = objVal as IList;

                                if(preVal != null) foreach(var val in (preVal as IList))
                                    list.Add(val);
                            }
                            
                            if(objVal != null && !objVal.Equals(defVal)) {
                                pi.SetValue(def, objVal);
                            } else if(preVal != null && !preVal.Equals(defVal)) {
                                pi.SetValue(def, preVal);
                            }
                        }
                    });

                Object = def; // new instance
            } else Object = preset;

            // TODO constaint for: T is GameObject/Loadable ?
            // Object.load(content);
        }

        public static IList createList(Type myType) {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public object clone() {
            GameAsset<T> copy = MemberwiseClone() as GameAsset<T>;
            if(Object != null) Object = (T)Object.clone();
            return copy;
        }
    }
}