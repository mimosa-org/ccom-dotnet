﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace Oagis.QualifiedDataTypes {
    using System.Xml.Serialization;
    using System.Xml.Linq;

    public interface Namespace
    {
        const string URI = "http://www.openapplications.org/oagis/9/qualifieddatatypes/1.1";
        public readonly static XNamespace XNAMESPACE;

        static Namespace() {
            XNAMESPACE = XNamespace.Get(URI);
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.openapplications.org/oagis/9/qualifieddatatypes/1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.openapplications.org/oagis/9/qualifieddatatypes/1.1", IsNullable=false)]
    public partial class GenSupportQualifiedDataTypes {
        
        private byte[] hexBinaryObjectTypeSupportField;
        
        private string yearDateTypeSupportField;
        
        private string yearMonthDateTypeSupportField;
        
        private float floatNumericTypeSupportField;
        
        private double doubleNumericTypeSupportField;
        
        private string integerNumericTypeSupportField;
        
        private string positiveIntegerNumericTypeSupportField;
        
        private string negativeIntegerNumericTypeSupportField;
        
        private string nonPositiveIntegerNumericTypeSupportField;
        
        private string nonNegativeIntegerNumericTypeSupportField;
        
        private string durationMeasureTypeSupportField;
        
        private string stringTypeSupportField;
        
        private string normalizedStringTypeSupportField;
        
        private string tokenTypeSupportField;
        
        private string uRITypeSupportField;
        
        private string languageCodeTypeSupportField;
        
        private MonthDateType monthDateTypeSupportField;
        
        private DayDateType dayDateTypeSupportField;
        
        private string monthDayDateTypeSupportField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="hexBinary")]
        public byte[] HexBinaryObjectTypeSupport {
            get {
                return this.hexBinaryObjectTypeSupportField;
            }
            set {
                this.hexBinaryObjectTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="gYear")]
        public string YearDateTypeSupport {
            get {
                return this.yearDateTypeSupportField;
            }
            set {
                this.yearDateTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="gYearMonth")]
        public string YearMonthDateTypeSupport {
            get {
                return this.yearMonthDateTypeSupportField;
            }
            set {
                this.yearMonthDateTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        public float FloatNumericTypeSupport {
            get {
                return this.floatNumericTypeSupportField;
            }
            set {
                this.floatNumericTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        public double DoubleNumericTypeSupport {
            get {
                return this.doubleNumericTypeSupportField;
            }
            set {
                this.doubleNumericTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string IntegerNumericTypeSupport {
            get {
                return this.integerNumericTypeSupportField;
            }
            set {
                this.integerNumericTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="positiveInteger")]
        public string PositiveIntegerNumericTypeSupport {
            get {
                return this.positiveIntegerNumericTypeSupportField;
            }
            set {
                this.positiveIntegerNumericTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="negativeInteger")]
        public string NegativeIntegerNumericTypeSupport {
            get {
                return this.negativeIntegerNumericTypeSupportField;
            }
            set {
                this.negativeIntegerNumericTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="nonPositiveInteger")]
        public string NonPositiveIntegerNumericTypeSupport {
            get {
                return this.nonPositiveIntegerNumericTypeSupportField;
            }
            set {
                this.nonPositiveIntegerNumericTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
        public string NonNegativeIntegerNumericTypeSupport {
            get {
                return this.nonNegativeIntegerNumericTypeSupportField;
            }
            set {
                this.nonNegativeIntegerNumericTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="duration")]
        public string DurationMeasureTypeSupport {
            get {
                return this.durationMeasureTypeSupportField;
            }
            set {
                this.durationMeasureTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        public string StringTypeSupport {
            get {
                return this.stringTypeSupportField;
            }
            set {
                this.stringTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString")]
        public string NormalizedStringTypeSupport {
            get {
                return this.normalizedStringTypeSupportField;
            }
            set {
                this.normalizedStringTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string TokenTypeSupport {
            get {
                return this.tokenTypeSupportField;
            }
            set {
                this.tokenTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI")]
        public string URITypeSupport {
            get {
                return this.uRITypeSupportField;
            }
            set {
                this.uRITypeSupportField = value;
            }
        }
        
        /// <remarks/>
        public string LanguageCodeTypeSupport {
            get {
                return this.languageCodeTypeSupportField;
            }
            set {
                this.languageCodeTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        public MonthDateType MonthDateTypeSupport {
            get {
                return this.monthDateTypeSupportField;
            }
            set {
                this.monthDateTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        public DayDateType DayDateTypeSupport {
            get {
                return this.dayDateTypeSupportField;
            }
            set {
                this.dayDateTypeSupportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string MonthDayDateTypeSupport {
            get {
                return this.monthDayDateTypeSupportField;
            }
            set {
                this.monthDayDateTypeSupportField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.openapplications.org/oagis/9/qualifieddatatypes/1.1")]
    public enum MonthDateType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("09")]
        Item09,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Item10,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Item11,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        Item12,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.openapplications.org/oagis/9/qualifieddatatypes/1.1")]
    public enum DayDateType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("09")]
        Item09,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Item10,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Item11,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        Item12,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("13")]
        Item13,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("14")]
        Item14,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("15")]
        Item15,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("16")]
        Item16,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("17")]
        Item17,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("18")]
        Item18,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("19")]
        Item19,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20")]
        Item20,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        Item21,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("22")]
        Item22,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("23")]
        Item23,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("24")]
        Item24,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("25")]
        Item25,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("26")]
        Item26,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("27")]
        Item27,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("28")]
        Item28,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("29")]
        Item29,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("30")]
        Item30,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("31")]
        Item31,
    }
}
