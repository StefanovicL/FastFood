using FastFood.Controllers;
using FastFood.ReinforcedTypings.Generator;
using Reinforced.Typings.Fluent;
using System.Reflection;

namespace FastFood.ReinforcedTypings;

public static class FluentConfiguration
{
    private static TBuilder BaseConfiguration<TBuilder>(this TBuilder conf) where TBuilder : ClassOrInterfaceExportBuilder
    {
        conf
            .WithPublicProperties(i => i.CamelCase())
            .WithProperties(i => i.PropertyType == typeof(DateTime) || i.PropertyType == typeof(DateTime?), i => i.Type("Date"));

        return conf;
    }

    //private static readonly Action<ClassExportBuilder> _classConfiguration = conf => conf
    //    .BaseConfiguration()
    //    .ExportTo("models.ts");

    private static readonly Action<ClassExportBuilder> _serviceConfiguration = conf => conf
        .AddImport("{ Injectable }", "@angular/core")
        .AddImport("{ HttpParams, HttpClient }", "@angular/common/http")
        //.AddImport("{ SettingsService }", "./settings.service")
        .AddImport("{ Observable }", "rxjs")
        .AddImport("{ map }", "rxjs/operators")
        .AddImport("{ environment }", "src/environments/environment")
        .ExportTo("fastfood.service.ts")
        .WithCodeGenerator<AngularControllerGenerator>();

    public static void Configure(Reinforced.Typings.Fluent.ConfigurationBuilder builder)
    {
        builder.Global(i =>
        {
            i.UseModules();
        });

        builder.ConfigureTypes();
    }

    private static void ConfigureTypes(this Reinforced.Typings.Fluent.ConfigurationBuilder builder)
    {
        builder.ExportAsClasses(
            Assembly.GetAssembly(typeof(BaseController)).ExportedTypes
                .Where(i => i.Namespace.StartsWith("FastFood.Controllers"))
                .OrderBy(i => i.Name)
                .OrderBy(i => i.Name != nameof(BaseController))
                .ToArray(),
            _serviceConfiguration
        );
    }
}
