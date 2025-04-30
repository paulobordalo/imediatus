using System.Reflection;
using Ardalis.SmartEnum;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace imediatus.Framework.Infrastructure.Filters;

/// <summary>
/// A Swashbuckle Schema filter for SmartEnum
/// </summary>
public class SmartEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        Type? type = context.Type;

        if (!IsTypeDerivedFromGenericType(type, typeof(SmartEnum<>)) && !IsTypeDerivedFromGenericType(type, typeof(SmartEnum<,>)))
        {
            return;
        }

        // Para simplificar, o método select pode ser assim: .Select(d => d.Name)
        IEnumerable<string>? enumValues = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .Select(field =>
             {
                 var instance = field.GetValue(null);
                 var name = field.Name;
                 var valueProperty = instance?.GetType().GetProperty("Value");
                 var value = valueProperty?.GetValue(instance)?.ToString();
                 return $"{name} ({value})";
             });

        var openApiValues = new OpenApiArray();
        openApiValues.AddRange(enumValues.Select(d => new OpenApiString(d)));

        // See https://swagger.io/docs/specification/data-models/enums/
        schema.Type = "string";
        schema.Enum = openApiValues;
        schema.Properties = null;
    }

    private static bool IsTypeDerivedFromGenericType(Type? typeToCheck, Type genericType)
    {
        while (true)
        {
            if (typeToCheck == typeof(object))
            {
                return false;
            }

            if (typeToCheck == null)
            {
                return false;
            }

            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            typeToCheck = typeToCheck.BaseType;
        }
    }
}
