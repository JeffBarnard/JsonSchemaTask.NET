# JsonSchemaTask.NET
 A .NET build task to generate json schema valiation for .NET POCO classes

'<UsingTask TaskName="JsonSchemaGeneratorTask" AssemblyFile="..\JsonSchemaGeneratorTask\bin\Debug\netstandard2.0\JsonSchemaGeneratorTask.dll" />
'<Target Name="SchemaGeneratorTarget" AfterTargets="Build">
'    <JsonSchemaGeneratorTask AssemblyName="$(AssemblyName)" AssemblyPath="$(OutputPath)" ModelDirectory="Models\" ConvertDirectory="JsonSchema\" />
'</Target>