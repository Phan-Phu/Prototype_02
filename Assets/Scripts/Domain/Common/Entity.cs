namespace Domain
{
    public abstract class Entity<TId>
    {
        public TId Id { get; }

        protected Entity(TId id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity<TId> other && Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}
