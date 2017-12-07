using System;
using System.Collections.Generic;
using System.Linq;
using MyApp.Types.Extensions;

namespace MyApp.Types.Models
{
    public class Pizza : IEquatable<Pizza>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public double BasePrice { get; set; }
        public List<Topping> Toppings { get; } = new List<Topping>();
        
        public double Price => BasePrice + Toppings.Select(t => t.Price).Sum();

        public bool Equals(Pizza other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) &&
                   string.Equals(Name, other.Name) &&
                   string.Equals(Description, other.Description) &&
                   BasePrice.Equals(other.BasePrice) &&
                   Toppings.OrderlessSequenceEquals(other.Toppings);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pizza) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ BasePrice.GetHashCode();
                hashCode = (hashCode * 397) ^ (Toppings != null ? Toppings.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}