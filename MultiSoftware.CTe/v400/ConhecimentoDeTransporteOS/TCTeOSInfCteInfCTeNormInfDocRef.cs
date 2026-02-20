namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteOS
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteInfCTeNormInfDocRef
    {

        private string[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("chBPe", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("dEmi", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("nDoc", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("serie", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("subserie", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("vDoc", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public string[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }
}
