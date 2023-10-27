using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;

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
            try
            {
#if DEBUG
                Debugger.Launch();
#endif
                Log?.LogMessage(MessageImportance.High, $"Starting {nameof(JsonSchemaTask)}");
                Log?.LogMessage(MessageImportance.High, $"{AssemblyPath}");

                var generator = new NewtonsoftGenerator();
                generator.OnLog += Generator_OnLog;
                generator.Generate(Environment.CurrentDirectory, ModelDirectory, ConvertDirectory, AssemblyName, AssemblyPath);

                Log?.LogMessage(MessageImportance.High, $"Finished {nameof(JsonSchemaTask)}");
            }
            catch (Exception ex)
            {
                Log?.LogErrorFromException(ex);
            }

            return true;
        }

        private void Generator_OnLog(object sender, (MessageImportance, string) e)
        {
            Log?.LogMessage(e.Item1, e.Item2);
        }
    }

}
