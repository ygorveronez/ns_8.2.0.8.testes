using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace SGT.WebService.REST.Utils
{
    public class AutoRestSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema is null)
            {
                throw new System.ArgumentNullException(nameof(schema));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            schema.Title = context.Type.Name;
        }
    }
}
