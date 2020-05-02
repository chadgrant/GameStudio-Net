using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameStudio.Secrets
{
    public class Secret : IEquatable<Secret>
    {
        static readonly Regex _azureFriendly = new Regex("[A-Za-z0-9\\-]+");
        readonly string _separator;
        readonly string[] _parts;

        /// <summary>
        /// Need this abstraction since Azure
        /// had the bright idea of only allowing
        /// [A-Za-z0-9-]+
        /// </summary>
        public Secret(string separator, params string[] parts)
        {
            foreach (var p in parts)
            {
                if (!_azureFriendly.IsMatch(p))
                    throw new ArgumentException($"Invalid characters in secret name {p}");
            }

            _separator = separator;
            _parts = parts;
        }
        
        public override string ToString()
        {
            return string.Join(_separator, _parts);
        }

        public bool Equals(Secret other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_separator, other._separator) && _parts.SequenceEqual(other._parts);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Secret) obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
