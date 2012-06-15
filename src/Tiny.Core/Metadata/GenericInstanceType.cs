using System.Collections.Generic;
using System.Text;

namespace Tiny.Metadata
{
    //# Represents an instanstation of a generic type, such as IEnumerable<Sting> or
    //# List<int>.
    public class GenericInstanceType : Type
    {
        readonly TypeDefinition m_baseType;
        readonly IReadOnlyList<Type> m_parameters;

        public GenericInstanceType(TypeKind kind, TypeDefinition baseType, IReadOnlyList<Type> parameters) : base(kind)
        {
            m_baseType = baseType.CheckNotNull("baseType");
            m_parameters = parameters.CheckNotNull("parameters");
        }

        public TypeDefinition BaseType
        {
            get { return m_baseType; }
        }

        public IReadOnlyList<Type> Parameters
        {
            get { return m_parameters; }
        }

        internal override void GetFullName(StringBuilder b)
        {
            BaseType.GetFullName(b);
            b.Append("<");
            var first = true;
            foreach (var p in Parameters) {
                if (first) {
                    first = false;
                }
                else {
                    b.Append(",");
                }
                p.GetFullName(b);
            }
            b.Append(">");
        }
    }
}
