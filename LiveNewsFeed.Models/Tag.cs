using System;

namespace LiveNewsFeed.Models
{
    public class Tag
    {
        public string Name { get; }

        public Tag(string name)
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

            return Equals((Tag) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        
        protected bool Equals(Tag other)
        {
            return Name == other.Name;
        }
    }
}