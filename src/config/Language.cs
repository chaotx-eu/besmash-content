namespace BesmashContent {
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;

    // Represents a natural language
    public class Language {
        /// The id of the language
        public string ID {get; set;}
        
        /// The title of the language
        public string Title {get; set;}

        /// Dictionary which holds the english words
        /// and their translations to this language
        [ContentSerializer(CollectionItemName="Word")]
        public Dictionary<string, string> Words {get;}

        /// Path to the .lang file
        [ContentSerializerIgnore]
        public string File {get; set;}

        /// Creates a new language object not referencing
        /// any file, main purpose is for deserialization
        public Language() : this(null, null, null) {}

        /// Creates a new language object which will
        /// be parsed from the langFile during load
        public Language(string id, string title, string langFile) {
            ID = id;
            Title = title;
            File = langFile;
            Words = new Dictionary<string, string>();
        }

        /// Array of special symbols to be ignored
        /// during translation
        protected char[] specialSymbols = {
            ' ', ',', '.', '!', '?', ';', ':', '#', '-', '+', '/', '\\',
            '<', '>', '\"', '§', '%', '&', '(', ')', '\'', '*', '°', '|',
            '^', '{', '}', '[', ']', '`', '´', '~', '_'
        };

        /// Returns the translation of the passed
        /// word which is expected to be english
        /// or translates the word as a whole
        /// sentence (see below) if not found.
        ///
        /// If isSentence is set to true the word string
        /// will be split between specialSymbols and the
        /// results will be translated independently.
        ///
        /// Capitalization is irrelevant only for the first
        /// letter (TODO) and special characters (e.g. '.,!?+-/=')
        /// will be ignored if isSentence is true.
        public string translate(string word, bool isSentence) {
            return translate(word, isSentence, isSentence);
        }

        /// Overload without public access to prevent infinite recursion
        private string translate(string word, bool isSentence, bool wasSentence) {
            string translation = word;
            if(isSentence) word.Trim()
                .Split(specialSymbols)
                .Distinct().ToList()
                .ForEach(w => translation.Replace(w, translate(w, false, true)));
            else {
                string upper = char.ToUpper(word[0]) + word.Substring(1);
                string lower = char.ToLower(word[0]) + word.Substring(1);
                translation = Words.ContainsKey(word) ? Words[word]
                    : Words.ContainsKey(upper) ? Words[upper]
                    : Words.ContainsKey(lower) ? Words[lower] : word;

                // try to translate as sentence if word not found
                if(translation.Equals(word) && !wasSentence)
                    return translate(word, true, true);
            }

            return translation;
        }

        /// Overload which translates the whole string as a
        /// single word. For more info see translate(string, bool)
        public string translate(string word) {
            return translate(word, false);
        }
        
        /// Languages are considered equal if their id is equal
        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != this.GetType())
                return false;
                
            return ID.Equals(((Language)obj).ID);
        }
    }
}