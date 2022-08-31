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

## Copyright

Copyright (c) 2022 Matt Selway. All Rights Reserved.

The included CCOM XML Schema and CCOM BOD Schemas are Copyright MIMOSA: see [XSD/License.txt](XSD/License.txt) for details.

The included OAGIS BOD Schemas are Copyright OAGi: see [XSD/BOD/OAGIS/OAGi License Agreement.txt](XSD/BOD/OAGIS/OAGi%20License%20Agreement.txt) for details.
