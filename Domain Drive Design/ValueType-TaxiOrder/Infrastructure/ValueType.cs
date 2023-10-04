using System;
using System.CodeDom;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Ddd.Taxi.Domain;

namespace Ddd.Infrastructure
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public class ValueType<T>
    {
        protected static PropertyInfo[] PropertyInfos;
        protected const int HashPrime = 0x01000193;
        protected const int Offset = unchecked((int)0x811c9dc5);

        static ValueType() => 
            PropertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (!(obj is T objValue)) return false;
            return PropertyInfos.All(property =>
                Equals(property.GetValue(this), (property.GetValue(objValue))));
        }

        public bool Equals(PersonName personName)
        {
            if (!(this is ValueType<PersonName>)) return false;
            if (personName == null) return false;
            var firstName = personName.FirstName;
            var lastName = personName.LastName;
            return PropertyInfos[0].GetValue(this).Equals(firstName)
                   && PropertyInfos[1].GetValue(this).Equals(lastName);
        }

        public override int GetHashCode()
        {
            var hashcode = Offset;
            foreach (var property in PropertyInfos)
            {
                if (property.GetValue(this) == null) continue;
                hashcode ^= property.GetValue(this).GetHashCode();
                hashcode = unchecked(hashcode * HashPrime);
            }
            return hashcode;
        }

        public override string ToString()
        {
            var sortedProperties = PropertyInfos.Select(property =>
                    Tuple.Create(property.Name, property.GetValue(this)))
                .OrderBy(property => property.Item1).ToArray();
            var sBuilder = new StringBuilder($"{typeof(T).Name}(");
            foreach (var propertyTuple in sortedProperties.Take(sortedProperties.Length - 1))
                sBuilder.Append($"{propertyTuple.Item1}: {propertyTuple.Item2}; ");
            var lastTuple = sortedProperties.Last();
            sBuilder.Append($"{lastTuple.Item1}: {lastTuple.Item2})");
            return sBuilder.ToString();
        }
    }
}