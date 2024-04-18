using System.Reflection;

namespace NHIABackendService.Entities.Common
{
    public interface IEntity<Key>
    {
        Key Id { get; set; }
        bool IsTransient();
    }

    public interface IEntity : IEntity<int>
    {
    }

    [Serializable]
    public abstract class Entity : Entity<int>, IEntity
    {
    }

    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }

        public virtual bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default)) return true;

            if (typeof(TPrimaryKey) == typeof(int)) return Convert.ToInt32(Id) <= 0;

            if (typeof(TPrimaryKey) == typeof(long)) return Convert.ToInt64(Id) <= 0;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity<TPrimaryKey>)) return false;

            if (ReferenceEquals(this, obj)) return true;

            var other = (Entity<TPrimaryKey>)obj;
            if (IsTransient() && other.IsTransient()) return false;

            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) &&
                !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis)) return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            if (Id == null) return 0;

            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{GetType().Name} {Id}]";
        }
    }
}
