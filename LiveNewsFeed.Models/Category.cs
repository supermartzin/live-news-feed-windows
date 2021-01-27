using System;

namespace LiveNewsFeed.Models
{
    public class Category
    {
        public string Name { get; }

        public Category(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((Category) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        
        protected bool Equals(Category other)
        {
            return Name == other.Name;
        }
    }
}