using Reinforced.Typings;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Generators;
using System.Reflection;
using System.Text;

namespace FastFood.ReinforcedTypings.Generator;

public class AngularActionCallGenerator : MethodCodeGenerator
{
    public override RtFunction GenerateNode(MethodInfo element, RtFunction result, TypeResolver resolver)
    {
        result = base.GenerateNode(element, result, resolver);
        if (result == null)
        {
            return null;
        }

        var returnType = result.ReturnType;
        if (returnType is RtSimpleTypeName && ((RtSimpleTypeName)returnType).TypeName == "void")
        {
            returnType = resolver.ResolveTypeName(typeof(object));
        }

        result.ReturnType = new RtSimpleTypeName("Observable", new[] { returnType });

        var parameters = element.GetParameters().Select(c => c.Name).ToList();
        var parameterTypes = element.GetParameters().Select(c => c.ParameterType).ToList();

        var controller = element.DeclaringType.Name.Replace("Controller", string.Empty);
        var path = $"{controller}/{element.Name}";

        var angularAttribute = (AngularMethodAttribute)Attribute.GetCustomAttribute(element, typeof(AngularMethodAttribute));

        var httpPostAttribute = Attribute.GetCustomAttribute(element, typeof(Microsoft.AspNetCore.Mvc.HttpPostAttribute));

        var allowAnonymousAttribute = Attribute.GetCustomAttribute(element, typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute));

        var code = new StringBuilder();

        if (parameters.Count == 1 && httpPostAttribute != null)
        {
            code.AppendLine($"const body = <any>{parameters.Single()};");
        }
        else if (parameters.Any())
        {
            var joinedParameters = string.Join(",", parameters.Select(p => $"'{p}': {p}"));
            code.AppendLine($"const body = <any>{{ {joinedParameters} }};");
        }

        if (httpPostAttribute == null)
        {
            code.AppendLine($"return this.http.get<{returnType}>(`${{environment.apiUrl}}/{path}`,");
            code.AppendLine("{");
            code.AppendLine($"\tresponseType: '{(angularAttribute?.isArrayBuffer == true ? "arraybuffer" : "json")}',");
            code.AppendLine("\tobserve: 'response',");
            if (parameters.Any())
            {
                //code.AppendLine($"\twithCredentials: {(allowAnonymousAttribute == null).ToString().ToLower()},");
                code.AppendLine("\tparams: new HttpParams({ fromObject: body })");
            }
            else
            {
                //code.AppendLine($"\twithCredentials: {(allowAnonymousAttribute == null).ToString().ToLower()}");
            }
            code.AppendLine("})");
            code.AppendLine(".pipe(map(response => response.body));");
        }
        else
        {
            code.AppendLine($"return this.http.post<{returnType}>(`${{environment.apiUrl}}/{path}`,");

            if (parameters.Any())
            {
                code.AppendLine("body,");
            } 
            else
            {
                code.AppendLine("null,");
            }

            code.AppendLine("{");
            code.AppendLine($"\tresponseType: '{(angularAttribute?.isArrayBuffer == true ? "arraybuffer" : "json")}',");
            code.AppendLine("\tobserve: 'response'");
            //code.AppendLine($"\twithCredentials: {(allowAnonymousAttribute == null).ToString().ToLower()}");
            code.AppendLine("})");
            code.AppendLine(".pipe(map(response => response.body));");
        }

        result.Body = new RtRaw(code.ToString());

        return result;
    }
}
