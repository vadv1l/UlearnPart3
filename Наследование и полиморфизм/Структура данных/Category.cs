using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        public string Symbol { get; set; }
        public MessageType MessageTyp { get; set; }
        public MessageTopic MessageTop { get; set; }
        public Category(string symbol, MessageType messageType, MessageTopic messageTopic)
        {
            Symbol = symbol;
            MessageTyp = messageType;
            MessageTop = messageTopic;
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (obj.GetType() != typeof(Category))
            {
                return 1;
            }
            var objConvert = obj as Category;
            return (Symbol, MessageTyp, MessageTop).CompareTo((objConvert.Symbol,objConvert.MessageTyp,objConvert.MessageTop));
        }
        public override string ToString()
        {
            string tostr = $"{Symbol}.{MessageTyp}.{MessageTop}";
            return tostr;
        }
        public override bool Equals(object obj)
        {
            if (obj == null||obj.GetType() != typeof(Category))
            {
                return false;
            }
            Category objConvert = obj as Category;
            return (objConvert.Symbol == Symbol && objConvert.MessageTyp == MessageTyp && objConvert.MessageTop == MessageTop);
        }
        public override int GetHashCode()
        {
            var hash = 1149;
            return Symbol.GetHashCode() + hash*MessageTyp.GetHashCode() +hash*hash*MessageTop.GetHashCode();
        }
        public static bool operator >(Category cat1, Category cat2)
        {
            return cat1.CompareTo(cat2) == 1;
        }
        public static bool operator <(Category cat1, Category cat2)
            {
                return cat1.CompareTo(cat2)==-1;
            }
        public static bool operator >=(Category cat1, Category cat2)
        {
            return cat1.CompareTo(cat2) >= 0;
        }
        public static bool operator <=(Category cat1, Category cat2)
        {
            return cat1.CompareTo(cat2) <= 0;
        }
        public static bool operator ==(Category cat1, Category cat2)
        {
            return cat1.CompareTo(cat2) == 0;
        }
        public static bool operator !=(Category cat1, Category cat2)
        {
            return cat1.CompareTo(cat2) != 0;
        }
    }
}
