# JsonSchemaTask.NET
An MSBuild task to generate json schema validation for .NET POCO classes

# Usage

```
<UsingTask TaskName = "JsonSchemaTask" AssemblyFile="..\JsonSchemaTask.NET\bin\Debug\netstandard2.0\JsonSchemaTask.dll" />
<Target Name = "SchemaTarget" AfterTargets="Build">
	 <JsonSchemaTask AssemblyName = "$(AssemblyName)" AssemblyPath="$(OutputPath)" ModelDirectory="Models\" ConvertDirectory="JsonSchema\" />
</Target>
```

Models can be annotated for validation rules using standard System.ComponentModel.DataAnnotations attributes.

Model properties can be opt-in included for json schema generation by adding the [JsonSchemaInclude] attribute annotation.


# TODO
Create a nuget package for easy integration into projects
