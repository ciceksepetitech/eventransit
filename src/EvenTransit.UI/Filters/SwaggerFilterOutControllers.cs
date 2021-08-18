using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EvenTransit.UI.Filters
{
    public class SwaggerFilterOutControllers : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var item in swaggerDoc.Paths.ToList())
            {
                if (!item.Key.ToLower().Contains("/api/v1"))
                {
                    swaggerDoc.Paths.Remove(item.Key);
                }
            }
        }
    }
}