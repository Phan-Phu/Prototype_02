namespace Domain
{
    // Marker for Domain Value Objects: immutable, no identity, compared by structural value equality.
    // A Value Object here is typically a struct (which cannot inherit an abstract base class in C#),
    // so this is an interface rather than a base class — implementers still provide their own
    // IEquatable<T>/Equals/GetHashCode for the actual structural comparison.
    public interface IValueObject
    {
    }
}
