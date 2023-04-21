# JsonSchemaTask.NET
 An MSBuild task to generate json schema valiation for .NET POCO classes

# Usage

'<UsingTask TaskName="JsonSchemaGeneratorTask" AssemblyFile="..\JsonSchemaGeneratorTask\bin\Debug\netstandard2.0\JsonSchemaGeneratorTask.dll" />
'<Target Name="SchemaGeneratorTarget" AfterTargets="Build">
'    <JsonSchemaGeneratorTask AssemblyName="$(AssemblyName)" AssemblyPath="$(OutputPath)" ModelDirectory="Models\" ConvertDirectory="JsonSchema\" />
'</Target>

# TODO
Create a nuget package for easy integration into projects