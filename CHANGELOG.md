# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

(Sections: Security, Fixed, Added, Changed, Deprecated, Removed)


## [0.1.0] - 2024-05-10

### Added

- Initial data model generated from XML schemas
- Implicit casts for most of the core component types/primitive value types
- Basic `BODReader` class to address handling of BOD XML files without processing
  every BOD XSD file separately.
- Example program demonstrating reading/writing CCOM XML files.
- Helpers to instantiate `PropertySetDefinition`s into `PropertySet`
- `XmlCallbackSerializer` custom serializer that runs the callbacks using the
  `OnDeserialized` attribute, which it turns out the XmlSerializer does not do
  in contrast to the BinaryFormatter.
  This is used, in particular, for reconciling the parents of child elements
  in composition structures, such as PropertySet/Group/Property hierarchies.
  See `ICompositionParent` and `ICompositionChild`.
