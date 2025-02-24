# CCOM.Net

Really simple example of using MIMOSA CCOM v4.1 in C#/.Net. Uses XML Serialization 
framework with mostly auto-generated classes. Some modifications were required to 
resolve issue of 'unsupported' elements occurring in the generated classes initially, 
which were only detected at runtime.

Works with .Net Core v6.

Includes a copy of CCOM Reference Data and some example data files (copyright MIMOSA)
for playing around with.

There is lots of work left to be done if this is to be turned into an SDK for CCOM.
- Code needs to be refactored into reusable modules, particularly BOD processing
- Unit Tests need to be added
  - e.g., serialisation/deserialisation of various examples in different, yet 
    conformant, forms such as default namespace, explicit namespace prefixes, etc.
- ToString implementations that make the CCOM data objects more easily understandable
- Helper methods for instantiation, navigation, and processing as necessary.
- etc.

## Generating the classes

Class generation performed by the MS 'xsd' tool:
  https://learn.microsoft.com/en-us/dotnet/standard/serialization/xml-schema-definition-tool-xsd-exe

Instructions on how to use the tool are at:
  https://learn.microsoft.com/en-us/dotnet/standard/serialization/xml-schema-def-tool-gen

```
xsd /parameters:config/GenerateCCOMClasses.xml 
```

```
xsd /parameters:config/GenerateOAGISClasses.xml
```

## Getting/Publishing the NuGet package

The NuGet package for the CCOM.Net library is currently being published via GitHub
rather than [NuGet.org](https://www.nuget.org). To read or publish the package you
will need to configure the NuGet registry for the CCOM.Net package.

If you want to use the CCOM.Net package in your project:

1. Copy the `github-mimosa-org` package source from [nuget.config](./nuget.config)
   into the `nuget.config` file in your project.

2. Note that the package should be publicly available so auth credentials should
   not be required. However, if credentials are required:
    1. Set the environment variable `GH_USERNAME` to your GitHub username
    2. Set the environment variable `GH_TOKEN` to a GitHub Personal Access Token (classic)
       that you have generated with `read:packages` permission.


If you want to publish a new version of the CCOM.Net package to the NuGet registry:

1. Set the environment variable `GH_USERNAME` to your GitHub username
2. Set the environment variable `GH_TOKEN` to a GitHub Personal Access Token (classic)
   that you have generated with `write:packages` permission.
3. ```bat
   dotnet pack --configuration Release
   ```
4. ```bat
   dotnet nuget push "src/CCOM.Net/bin/Release/CCOM.Net.VERSION.nupkg" --api-key %GH_TOKEN% --source "github-mimosa-org"
   ```
   where `VERSION` is replaced with the version number being published.

## Contributing

The main repository for CCOM.Net is https://github.com/mimosa-org/ccom-dotnet

If you would like to contribute to development, please:
1. Fork the repository
2. Make your changes in a branch
3. Create a pull request with a description of the new feature, bugfix, etc.
   of the contribution.

   > Note that not all pull requests will necessarily be merged into the main
   > repository. If you like, you could first open an issue discussing the
   > proposed change before implementing it. This will help ensure that any
   > contributions align with the overall goals of the project.

You may also open an issue, feature request, discussion, etc., in the repository's
issue tracker if you like.

## Copyright

Copyright (c) 2025 Matt Selway. All Rights Reserved.

The included CCOM XML Schema and CCOM BOD Schemas are Copyright MIMOSA: see [XSD/License.txt](XSD/License.txt) for details.

The included OAGIS BOD Schemas are Copyright OAGi: see [XSD/BOD/OAGIS/OAGi License Agreement.txt](XSD/BOD/OAGIS/OAGi%20License%20Agreement.txt) for details.
