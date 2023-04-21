using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using Newtonsoft.Json.Schema.Generation;

namespace JsonSchemaTask.Build.Utilities
{
    /// <summary>
    /// Example usage:
    ///<UsingTask TaskName = "JsonSchemaTask" AssemblyFile="..\JsonSchemaTask.NET\bin\Debug\netstandard2.0\JsonSchemaTask.dll" />
	///<Target Name = "SchemaTarget" AfterTargets="Build">
	///	 <JsonSchemaTask AssemblyName = "$(AssemblyName)" AssemblyPath="$(OutputPath)" ModelDirectory="Models\" ConvertDirectory="JsonSchema\" />
	///</Target>
    /// </summary>
    public class JsonSchemaTask : Microsoft.Build.Utilities.Task, ITask
    {
        [Required]
        public string AssemblyName { get; set; }
        [Required]
        public string AssemblyPath { get; set; }
        [Required]
        public string ModelDirectory { get; set; }
        [Required]
        public string ConvertDirectory { get; set; }

        /// <summary>
        /// Task entry point
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Debugger.Launch();
            Log.LogMessage(MessageImportance.High, $"Starting {nameof(JsonSchemaTask)}");
            
            try
            {
                string workingDirectory = Environment.CurrentDirectory;
                Log.LogMessage(MessageImportance.High, $"Working directory {workingDirectory}");
                
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles($"{workingDirectory}\\{ModelDirectory}");
                var assembly = Assembly.LoadFrom($"{AssemblyPath}{AssemblyName}.dll");
                var assemblyTypes = assembly.GetTypes();
                foreach (string fileName in fileEntries)
                {
                    string filePart = Path.GetFileName(fileName);
                    string typeName = filePart.Substring(0, filePart.LastIndexOf('.'));
                    var type = assemblyTypes.Where(t => t.Name == typeName).FirstOrDefault();

                    Log.LogMessage(MessageImportance.High, $"Processing filename {fileName} - Typename will be {typeName}");

                    // When using reflection to call a generic method, we must first use reflection to get the method itself
                    // https://stackoverflow.com/questions/232535/how-do-i-use-reflection-to-call-a-generic-method
                    MethodInfo method = typeof(JsonSchemaTask).GetMethod(nameof(GenerateSchemaForPath));
                    MethodInfo generic = method.MakeGenericMethod(type);
                    generic.Invoke(this, new object[] { typeName, workingDirectory });                    
                }
            }
            catch(Exception ex)
            {
                Log.LogErrorFromException(ex);
            }

            Log.LogMessage(MessageImportance.High, $"Finished {nameof(JsonSchemaTask)}");
            return true;
        }

        /// <summary>
        /// Generate json schema and save to file
        /// </summary>
        /// <typeparam name="T">Used to find asembly</typeparam>
        /// <param name="typeName">The class name</param>
        public void GenerateSchemaForPath<T>(string typeName, string workingdir)
        {   
            Type type = typeof(T);
            if (type != null)
            {
                // Json schema generation
                var generator = new JSchemaGenerator();
                generator.GenerationProviders.Add(new StringEnumGenerationProvider());
                var schema = generator.Generate(type);
                File.WriteAllText($"{workingdir}\\{ConvertDirectory}\\{typeName}.json", schema.ToString());
            }
        }
    }

}
