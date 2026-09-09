using Reinforced.Typings.Attributes;

namespace FastFood.ReinforcedTypings.Generator;

public class AngularMethodAttribute : TsFunctionAttribute
{
    public AngularMethodAttribute(Type returnType)
    {
        // Here we override method return type for TypeScript export
        StrongType = returnType;

        // Here we are specifying code generator for particular method
        CodeGeneratorType = typeof(AngularActionCallGenerator);
    }

    public bool isArrayBuffer { get; set; }
}
