using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace JsonSchemaTask.Providers
{
    public class IncludeContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> defaultProperties = base.CreateProperties(type, memberSerialization);
            List<string> includedProperties = Utilities.GetPropertyNames(type);

            var final = defaultProperties.Where(p => includedProperties.Contains(p.PropertyName)).ToList();
            return final;
        }
    }

    public class Utilities
    {
        /// <summary>
        /// Gets a list of all public instance properties of a given class type
        /// excluding those belonging to or inherited by the given base type.
        /// </summary>
        /// <param name="type">The Type to get property names for</param>
        /// <param name="stopAtType">A base type inherited by type whose properties should not be included.</param>
        /// <returns></returns>
        public static List<string> GetPropertyNames(Type type)
        {
            var propertyNames = new List<string>();

            if (type == null) return propertyNames;

            // MetadataTypeAttribute is used to associate a class with a metadata class.
            var metadataType = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true)
             .OfType<MetadataTypeAttribute>().FirstOrDefault();

            if (metadataType != null)
            {
                type = metadataType.MetadataClassType;
                //var include = member.GetCustomAttributes(typeof(FrameworkCore.Data.Model.JsonSchemaIncludeAttribute), true).Any();
            }

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(JsonSchemaIncludeAttribute), true).Any())
                    if (!propertyNames.Contains(property.Name))
                        propertyNames.Add(property.Name);
            }

            return propertyNames;
        }
    }
}
