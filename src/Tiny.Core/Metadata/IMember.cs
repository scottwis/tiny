namespace Tiny.Metadata
{
    public interface IMemberDefinition
    {
        string Name { get;  }
        string FullName { get; }
        TypeDefinition DeclaringType { get;  }
    }
}
