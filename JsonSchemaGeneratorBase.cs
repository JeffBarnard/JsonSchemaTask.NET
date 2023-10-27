using Microsoft.Build.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JsonSchemaTask
{
    public class JsonSchemaGeneratorBase
    {   
        protected string _workingdir;
        protected string _outputdirectory = "JsonSchema";

        public event EventHandler<(MessageImportance, string)> OnLog;

        public JsonSchemaGeneratorBase()
        {
            AssemblyResolver.Enable();
        }

        //The event-invoking method that derived classes can override.
        protected virtual void OnLogMessage((MessageImportance, string) e)
        {
            // Safely raise the event for all subscribers
            this.OnLog?.Invoke(this, e);
        }

        /// <summary>
        /// Generate json schema and save to file
        /// </summary>
        /// <typeparam name="T">Used to find asembly</typeparam>
        /// <param name="typeName">The class name</param>
        public virtual void GenerateSchemaForPath<T>(string typeName)
        {

        }
    }
}