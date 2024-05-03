using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Ccom.Xml.Serialization
{
    /// <summary>
    /// XML Serializer that executes callbacks (IDeserializationCallback) and/or
    /// annotated methods during serialization and deserialization similar to the
    /// BinaryFormatter serializer.
    /// </summary>
    /// <remarks>
    /// NB. unlike BinaryFormatter, the OnDeserializing attribute is not supported
    /// as we cannot create the blank object, run the OnDeserializing methods, 
    /// then overlay the deserialized data like BinaryFormatter does.
    /// </remarks>
    /// <remarks>
    /// Consider adding a parameter to disable the callbacks to improve performance
    /// when deserializing objects that do not need to have any callbacks performed.
    /// As it is, it walks the object tree recursively so that not only the top-level
    /// object has its callbacks performed.
    /// Of course, in such case, the standard XmlSerializer can be used, so may not be necessary.
    /// </remarks>
    /// <remarks>
    /// An alternative may be to have the data model classes implement IXmlSerializable
    /// to take full control of the XML serialization/deserialization. But that is much
    /// more effort than handling it centrally for the moment.
    /// </remarks>
    /// <remarks>
    /// Based on the answers at https://stackoverflow.com/questions/1266547/how-do-you-find-out-when-youve-been-loaded-via-xml-serialization
    /// In particular thanks to:
    /// - HotN, who provided the complete skeleton.
    /// - Norritt who added the OnDeserializedAttribute usage (and restriction to XML annotated proeprties).
    /// - Itsstar for correcting an issue
    /// - user17283045 who added OnSerializing and OnSerialized, which we will make use of later.
    /// </remarks>
    public class XmlCallbackSerializer : XmlSerializer
    {
        public XmlCallbackSerializer(Type type) : base(type)
        {
        }

        public XmlCallbackSerializer(XmlTypeMapping xmlTypeMapping) : base(xmlTypeMapping)
        {
        }

        public XmlCallbackSerializer(Type type, string defaultNamespace) : base(type, defaultNamespace)
        {
        }

        public XmlCallbackSerializer(Type type, Type[] extraTypes) : base(type, extraTypes)
        {
        }

        public XmlCallbackSerializer(Type type, XmlAttributeOverrides overrides) : base(type, overrides)
        {
        }

        public XmlCallbackSerializer(Type type, XmlRootAttribute root) : base(type, root)
        {
        }

        public XmlCallbackSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes,
            XmlRootAttribute root, string defaultNamespace) : base(type, overrides, extraTypes, root, defaultNamespace)
        {
        }

        public XmlCallbackSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes,
            XmlRootAttribute root, string defaultNamespace, string location)
            : base(type, overrides, extraTypes, root, defaultNamespace, location)
        {
        }

        public new object? Deserialize(Stream stream)
        {
            var result = base.Deserialize(stream);
            CheckForDeserializationCallbacks(result);
            return result;
        }

        public new object? Deserialize(TextReader textReader)
        {
            var result = base.Deserialize(textReader);
            CheckForDeserializationCallbacks(result);
            return result;
        }

        public new object? Deserialize(XmlReader xmlReader)
        {
            var result = base.Deserialize(xmlReader);
            CheckForDeserializationCallbacks(result);
            return result;
        }

        public new object? Deserialize(XmlSerializationReader reader)
        {
            var result = base.Deserialize(reader);
            CheckForDeserializationCallbacks(result);
            return result;
        }

        public new object? Deserialize(XmlReader xmlReader, string encodingStyle)
        {
            var result = base.Deserialize(xmlReader, encodingStyle);
            CheckForDeserializationCallbacks(result);
            return result;
        }

        public new object? Deserialize(XmlReader xmlReader, XmlDeserializationEvents events)
        {
            var result = base.Deserialize(xmlReader, events);
            CheckForDeserializationCallbacks(result);
            return result;
        }

        public new object? Deserialize(XmlReader xmlReader, string encodingStyle, XmlDeserializationEvents events)
        {
            var result = base.Deserialize(xmlReader, encodingStyle, events);
            CheckForDeserializationCallbacks(result);
            return result;
        }

        private void CheckForDeserializationCallbacks(object? deserializedObject, StreamingContext? context = null)
        {
            if (deserializedObject is null) return;

            if (deserializedObject is IDeserializationCallback deserializationCallback)
            {
                deserializationCallback.OnDeserialization(this);
            }

            context ??= new StreamingContext(StreamingContextStates.Other); // We do not make much use of this yet

            Type type = deserializedObject.GetType();

            // TODO: cache methods by Type, so we do not have to re-search them
            // TODO: should include non-public too?
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod)
                .Where(m => m.GetCustomAttribute<OnDeserializedAttribute>(true) is not null);

            foreach (var method in methods)
            {
                method.Invoke(deserializedObject, new object[] { context });
            }

            // Recursively execute callbacks on Properties, but restricted to only
            // XmlElement and XmlAttribute properties and non-indexed properties.
            // TODO: cache the properties by Type, so we do not have to re-search them
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(true).Any(a => a is XmlElementAttribute || a is XmlAttributeAttribute))
                .Where(p => p.GetIndexParameters().Length == 0);

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType.GetInterface(nameof(IEnumerable)) == null)
                {
                    CheckForDeserializationCallbacks(propertyInfo.GetValue(deserializedObject), context);
                }
                else
                {
                    if (propertyInfo.GetValue(deserializedObject) is IEnumerable collection)
                    {
                        foreach (var item in collection)
                        {
                            CheckForDeserializationCallbacks(item, context);
                        }
                    }
                }
            }
        }

        // TODO: implement the serialization half
        public new void Serialize(object o, XmlSerializationWriter xmlSerializationWriter)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(o, xmlSerializationWriter);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(Stream stream, object o)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(stream, o);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(TextWriter textWriter, object o)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(textWriter, o);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(XmlWriter xmlWriter, object o)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(xmlWriter, o);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(Stream stream, object o, XmlSerializerNamespaces namespaces)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(stream, o, namespaces);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(TextWriter textWriter, object o, XmlSerializerNamespaces namespaces)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(textWriter, o, namespaces);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(xmlWriter, o, namespaces);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(xmlWriter, o, namespaces, encodingStyle);
            // EachWith<OnSerializedAttribute>(o);
        }

        public new void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
        {
            // EachWith<OnSerializingAttribute>(o);
            base.Serialize(xmlWriter, o, namespaces, encodingStyle, id);
            // EachWith<OnSerializedAttribute>(o);
        }
    }
}