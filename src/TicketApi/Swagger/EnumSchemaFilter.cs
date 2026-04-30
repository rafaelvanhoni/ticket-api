using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
            return;

        schema.Type = "string";
        schema.Format = null;

        schema.Enum.Clear();

        foreach (var enumName in Enum.GetNames(context.Type))
        {
            schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiString(enumName));
        }
    }
}