namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TCredPres
    {

        private object cCredPresField;

        private string pCredPresField;

        private string itemField;

        private ItemChoiceType7 itemElementNameField;

        /// <remarks/>
        public object cCredPres
        {
            get
            {
                return this.cCredPresField;
            }
            set
            {
                this.cCredPresField = value;
            }
        }

        /// <remarks/>
        public string pCredPres
        {
            get
            {
                return this.pCredPresField;
            }
            set
            {
                this.pCredPresField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("vCredPres", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("vCredPresCondSus", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public string Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType7 ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe", IncludeInSchema = false)]
        public enum ItemChoiceType7
        {

            /// <remarks/>
            vCredPres,

            /// <remarks/>
            vCredPresCondSus,
        }
    }
}
