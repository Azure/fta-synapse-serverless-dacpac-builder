using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.Dac;
using System.CommandLine;

var rootCommand = new RootCommand("SynapseServerlessDacpacBuilder");

var sourceOption = new Option<string>(
    name: "--source",
    description: "Root path of the SQL source code.",
    getDefaultValue: () => "."
);
sourceOption.AddAlias("-s");

var outputOption = new Option<string>(
    name: "--output",
    description: "Path of the output DACPAC file.",
    getDefaultValue: () => "./package.dacpac"
);
outputOption.AddAlias("-o");

var nameOption = new Option<string>(
    name: "--name",
    description: "Name of the package to be included in its metadata.",
    getDefaultValue: () => "package"
);
nameOption.AddAlias("-n");

rootCommand.AddOption(sourceOption);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(nameOption);

rootCommand.SetHandler((sourceOptionValue, outputOptionValue, nameOptionValue) =>
    {
        var model = new TSqlModel(SqlServerVersion.SqlServerless, default);

        foreach(var fileName in Directory.GetFiles(
            sourceOptionValue,
            "*.sql",
            SearchOption.AllDirectories))
        {
            Console.WriteLine($"Adding {fileName}");
            model.AddObjects(File.ReadAllText(fileName));
        }

        DacPackageExtensions.BuildPackage(
            outputOptionValue,
            model,
            new() {Name = nameOptionValue}
        );
    },
    sourceOption,
    outputOption,
    nameOption);

await rootCommand.InvokeAsync(args);

