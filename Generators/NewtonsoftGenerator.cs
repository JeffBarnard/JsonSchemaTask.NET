using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JsonSchemaTask.Providers;
using Microsoft.Build.Framework;
using Newtonsoft.Json.Schema.Generation;

namespace JsonSchemaTask
{
    public class NewtonsoftGenerator : JsonSchemaGeneratorBase
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
                System.Diagnostics.Debugger.Launch();
                OnLogMessage((MessageImportance.High, $"Working directory {_workingdir}"));

                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles($"{_workingdir}\\{modeldirectory}");
                var assembly = Assembly.LoadFrom($"{assemblypath}{assemblyname}.dll");
                var assemblyTypes = assembly.GetTypes();
                foreach (string fileName in fileEntries)
                {
                    string filePart = Path.GetFileName(fileName);
                    string typeName = filePart.Substring(0, filePart.LastIndexOf('.'));
                    var type = assemblyTypes.Where(t => t.Name == typeName).FirstOrDefault();

                    OnLogMessage((MessageImportance.High, $"Processing filename {fileName} - Typename will be {typeName}"));

                    // When using reflection to call a generic method, we must first use reflection to get the method itself
                    // https://stackoverflow.com/questions/232535/how-do-i-use-reflection-to-call-a-generic-method
                    MethodInfo method = typeof(NewtonsoftGenerator).GetMethod(nameof(GenerateSchemaForPath));
                    MethodInfo generic = method.MakeGenericMethod(type);
                    generic.Invoke(this, new object[] { typeName });
                }

                OnLogMessage((MessageImportance.High, $"Finished {nameof(NewtonsoftGenerator)}"));
            }
            catch (Exception ex)
            {
                OnLogMessage((MessageImportance.High, ex.Message));
            }

            return true;
        }

        /// <summary>
        /// Generate json schema and save to file
        /// </summary>
        /// <typeparam name="T">Used to find asembly</typeparam>
        /// <param name="typeName">The class name</param>
        public void GenerateSchemaForPath<T>(string typeName)
        {
            try
            {
                Type type = typeof(T);
                if (type != null)
                {
                    // Json schema generation
                    var generator = new JSchemaGenerator()
                    {
                        SchemaLocationHandling = SchemaLocationHandling.Definitions,
                        DefaultRequired = Newtonsoft.Json.Required.Default,
                    };
                    generator.GenerationProviders.Add(new StringEnumGenerationProvider());
                    //generator.GenerationProviders.Add(new CustomSchemaProvider());
                    generator.ContractResolver = new IncludeContractResolver();

                    var schema = generator.Generate(type, false);
                    File.WriteAllText($"{_workingdir}\\{_outputdirectory}\\{typeName}.json", schema.ToString());
                }
            }
            catch (Exception ex)
            {
                OnLogMessage((MessageImportance.High, ex.Message));
            }
        }
    }
}
