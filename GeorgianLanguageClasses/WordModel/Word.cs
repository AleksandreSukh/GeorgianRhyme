using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeorgianLanguageClasses.WordModel
{
    public struct Word : IEquatable<Word>
    {
        public Word(params TypedWordPart[] parts)
        {
            if (parts.Count(p => p.Type == TypedWordPart.WordPartType.Stem) != 1)
            { throw new InvalidOperationException($"One and single {nameof(TypedWordPart.WordPartType.Stem)} is required to create {nameof(Word)}"); }
            if (parts.Length > 1)
            {
                var partsList = parts.ToList();
                var indexOfStem = partsList.FindIndex(p => p.Type == TypedWordPart.WordPartType.Stem);
                var indexOfPrefix = partsList.FindIndex(p => p.Type == TypedWordPart.WordPartType.Prefix);
                var indexOfSuffix = partsList.FindIndex(p => p.Type == TypedWordPart.WordPartType.Suffix);

                if (indexOfPrefix > indexOfStem || (indexOfSuffix != -1 && indexOfSuffix < indexOfStem))
                    throw new InvalidOperationException($"{nameof(TypedWordPart)}s should be passed with following type order Prefix,Stem,Suffix");
            }
            var val = string.Empty;
            foreach (var part in parts)
            {
                val += part.WordPart.TextValue.StringValue;
            }
            TextValue = new Text(val);
        }

        public Text TextValue { get; }

        public bool Equals(Word other)
        {
            return Equals(TextValue, other.TextValue);
        }

        
    }

    public struct TypedWordPart : IEquatable<TypedWordPart>
    {
        public WordPart WordPart { get; }
        public WordPartType Type { get; }

        public TypedWordPart(WordPart wordPart, WordPartType type)
        {
            WordPart = wordPart;
            Type = type;
        }

        public enum WordPartType
        {
            Prefix = 1,
            Stem = 2,
            Suffix = 3
        }

        public bool Equals(TypedWordPart other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypedWordPart)obj);
        }

        public override int GetHashCode()
        {
            return (int)Type;
        }
    }

    public struct WordPart
    {
        public Text TextValue { get; }

        public WordPart(Text textValue)
        {
            if (textValue.Any(char.IsWhiteSpace))
                throw new InvalidOperationException($"{nameof(WordPart)} can't be created from a string which contains white space characters in it");
            TextValue = textValue;
        }
    }

    public struct Text : IEquatable<Text>, IEnumerable<char>
    {
        public Text(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                throw new InvalidOperationException($"{nameof(Text)} can't be created by an empty string");
            StringValue = stringValue;
        }

        public string StringValue { get; }

        #region Overloads

        public bool Equals(Text other)
        {
            return StringValue.Equals(other.StringValue);
        }


        public static implicit operator Text(string value) => new Text(value);

        public static Text operator +(Text first, Text second)
        {
            return new Text(first.StringValue + second.StringValue);
        }

        #endregion
        /// <summary>
        /// Just for syntax, You can use operator + instead
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public Text AddAtBeginnig(Text second) => second + this;

        /// <summary>
        /// Just for syntax, You can use operator + instead
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public Text AddToTheEnd(Text second) => this + second;

        public Text RemoveAtTheEnd(Text second)
        {
            if (!StringValue.EndsWith(second.StringValue))
                throw new InvalidOperationException("In order to cut part of text the text must contain the part");
            return new Text(StringValue.Remove(StringValue.Length - second.StringValue.Length));
        }

        public Text RemoveAtTheBeginning(Text second)
        {
            if (!StringValue.StartsWith(second.StringValue))
                throw new InvalidOperationException("In order to cut part of text the text must contain the part");
            return new Text(StringValue.Remove(0, second.StringValue.Length));
        }

        public IEnumerator<char> GetEnumerator()
        {
            return StringValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
