using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace JsonSchemaTask.Providers
{
    /// <summary>
    /// JsonSchema property specific generator
    /// </summary>
    public class CustomSchemaProvider : JSchemaGenerationProvider
    {
        public override JSchema GetSchema(JSchemaTypeGenerationContext context)
        {
            if (context.MemberProperty != null)
            {
                var attributes = context.MemberProperty.AttributeProvider.GetAttributes(false);
                if (attributes.Count > 0)
                {                   
                    // "readonly" metadata
                    if (attributes.Any(x => x.GetType() == typeof(ReadOnlyAttribute)))
                    {
                        return CreateSchemaWithReadOnly(context.ObjectType, context.Required);
                    }
                    // "hidden" metadata
                    else if (attributes.Any(x => x.GetType() == typeof(BrowsableAttribute)))
                    {
                        return CreateSchemaWithFormat(context.ObjectType, context.Required, "hidden");
                    }
                    // "obfuscation" metadata
                    else if (attributes.Any(x => x.GetType() == typeof(PasswordPropertyTextAttribute)))
                    {
                        return CreateSchemaWithFormat(context.ObjectType, context.Required, "obfuscation");
                    }
                    // "title" metadata is now handled by default using the DisplayNameAttribute                   
                }

                if (context.ObjectType == typeof(CultureInfo))
                {
                    return CreateSchemaWithFormat(context.ObjectType, context.Required, "culture");
                }
            }

            // use default schema generation for all other types
            return new JSchemaGenerator().Generate(context.ObjectType, context.Required != Required.Always);
        }

        public override bool CanGenerateSchema(JSchemaTypeGenerationContext context)
        {
            return base.CanGenerateSchema(context) && context.Generator.ContractResolver.ResolveContract(context.ObjectType) is JsonObjectContract;
        }

        private JSchema CreateSchemaWithFormat(Type type, Required required, string format)
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(type, required != Required.Always);
            schema.Format = format;

            return schema;
        }

        private JSchema CreateSchemaWithReadOnly(Type type, Required required)
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(type, required != Required.Always);
            schema.ReadOnly = true;

            return schema;
        }

        private JSchema CreateSchemaWithTitle(Type type, Required required, string title)
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(type, required != Required.Always);
            schema.Title = title;

            return schema;
        }
    }
}
