using Reinforced.Typings;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Generators;

namespace FastFood.ReinforcedTypings.Generator;

public class AngularControllerGenerator : ClassCodeGenerator
{
    public override RtClass GenerateNode(Type element, RtClass result, TypeResolver resolver)
    {
        result = base.GenerateNode(element, result, resolver);
        if (result == null)
        {
            return null;
        }

        result.Decorators.Add(new RtDecorator(@"Injectable()"));

        var httpServiceType = new RtSimpleTypeName("HttpClient");
        //var settingsService = new RtSimpleTypeName("SettingsService");

        RtConstructor constructor = new RtConstructor();
        constructor.Arguments.Add(new RtArgument() { Type = httpServiceType, Identifier = new RtIdentifier("protected http") });
        //constructor.Arguments.Add(new RtArgument() { Type = settingsService, Identifier = new RtIdentifier("protected settingsService") });

        result.Members.Add(constructor);

        return result;
    }
}
