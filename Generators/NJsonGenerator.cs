using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using NJsonSchema;

namespace JsonSchemaTask
{
    public class NJsonGenerator : JsonSchemaGeneratorBase
    {
        /// <summary>
        /// Task entry point
        /// </summary>
        /// <returns></returns>
        public bool Generate(string workingdirectory, string modeldirectory, string outputdirectory, string assemblyname, string assemblypath)
        {
            if (!string.IsNullOrEmpty(outputdirectory))
                _outputdirectory = outputdirectory;

            _workingdir = workingdirectory;

            try
            {
                //System.Diagnostics.Debugger.Launch();
                OnLogMessage((Microsoft.Build.Framework.MessageImportance.High, $"Working directory {_workingdir}"));

                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles($"{_workingdir}\\{modeldirectory}");
                var assembly = Assembly.LoadFrom($"{assemblypath}{assemblyname}.dll");
                var assemblyTypes = assembly.GetTypes();
                foreach (string fileName in fileEntries)
                {
                    string filePart = Path.GetFileName(fileName);
                    string typeName = filePart.Substring(0, filePart.LastIndexOf('.'));
                    var type = assemblyTypes.Where(t => t.Name == typeName).FirstOrDefault();

                    OnLogMessage((Microsoft.Build.Framework.MessageImportance.High, $"Processing filename {fileName} - Typename will be {typeName}"));

                    // When using reflection to call a generic method, we must first use reflection to get the method itself
                    // https://stackoverflow.com/questions/232535/how-do-i-use-reflection-to-call-a-generic-method
                    MethodInfo method = typeof(NJsonGenerator).GetMethod(nameof(GenerateSchemaForPath));
                    MethodInfo generic = method.MakeGenericMethod(type);
                    generic.Invoke(this, new object[] { typeName });
                }
            }
            catch (Exception ex)
            {
                OnLogMessage((Microsoft.Build.Framework.MessageImportance.High, ex.Message));
            }

            return true;
        }

        /// <summary>
        /// Generate json schema and save to file
        /// </summary>
        /// <typeparam name="T">Used to find asembly</typeparam>
        /// <param name="typeName">The class name</param>
        public override void GenerateSchemaForPath<T>(string typeName)
        {
            try
            {
                Type type = typeof(T);
                if (type != null)
                {
                    var schema = JsonSchema.FromType<T>();
                    ApplyMetadataToJson(schema, type);
                    File.WriteAllText($"{_workingdir}\\{_outputdirectory}\\{typeName}.json", schema.ToJson());
                }
            }
            catch (Exception ex)
            {
                OnLogMessage((Microsoft.Build.Framework.MessageImportance.High, ex.Message));
            }
        }

        /// <summary>
        /// Reads attributes from metadata class and applies them to the JsonSchema.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="type"></param>
        public static void ApplyMetadataToJson(JsonSchema schema, Type type)
        {
            var metadataType = type.GetCustomAttribute<MetadataTypeAttribute>()?.MetadataClassType;
            var properties = metadataType?.GetProperties();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(true);

                    if (!schema.Properties.ContainsKey(property.Name))                    
                        continue;
                    
                    var schemaProperty = schema.Properties[property.Name];

                    foreach (var attribute in attributes)
                    {
                        switch (attribute)
                        {
                            case RequiredAttribute requiredAtrribute:
                                schemaProperty.IsRequired = true;
                                break;                          
                            case DescriptionAttribute descriptionAttribute:
                                schemaProperty.Description = descriptionAttribute.Description;
                                break;
                            case DisplayNameAttribute displayNameAttribute:
                                schemaProperty.Title = displayNameAttribute.DisplayName;
                                break;
                            case DefaultValueAttribute defaultValueAttribute:
                                schemaProperty.Default = defaultValueAttribute.Value;
                                break;
                            case ReadOnlyAttribute readOnlyAttribute:
                                schemaProperty.IsReadOnly = readOnlyAttribute.IsReadOnly;
                                break;
                            case MinLengthAttribute minLengthAttribute:
                                schemaProperty.MinLength = minLengthAttribute.Length;
                                break;
                            case MaxLengthAttribute maxLengthAttribute:
                                schemaProperty.MaxLength = maxLengthAttribute.Length;
                                break;
                            case RangeAttribute rangeAttribute:
                                schemaProperty.Minimum = (int)rangeAttribute.Minimum;
                                schemaProperty.Maximum = (int)rangeAttribute.Maximum;
                                break;
                            case RegularExpressionAttribute regularExpressionAttribute:
                                schemaProperty.Pattern = regularExpressionAttribute.Pattern;
                                break;
                            case UrlAttribute urlAttribute:
                                schemaProperty.Format = JsonFormatStrings.Uri;
                                break;
                        }
                    }
                }
            }
        }

    }

}
