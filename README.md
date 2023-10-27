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
```
[Required]
[ReadOnly]
[Description]
[DisplayName]
[DefaultValue]
[MaxLength]
[Range]
[RegularExpression]
[Url]
[EnumDataType]
```

Model properties can be opt-in for json schema generation by adding the [JsonSchemaInclude] annotation.
```
[JsonSchemaInclude]
string Name { get; set; }
```

# TODO
Create a nuget package for easy integration into projects

https://learn.microsoft.com/en-us/visualstudio/msbuild/tutorial-custom-task-code-generation?view=vs-2022
