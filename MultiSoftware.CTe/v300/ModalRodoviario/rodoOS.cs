namespace MultiSoftware.CTe.v300.ModalRodoviario
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class rodoOS
    {

        private string itemField;

        private ItemChoiceType itemElementNameField;

        private rodoOSVeic veicField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NroRegEstadual", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("TAF", typeof(string))]
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
        public ItemChoiceType ItemElementName
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
        public rodoOSVeic veic
        {
            get
            {
                return this.veicField;
            }
            set
            {
                this.veicField = value;
            }
        }
    }
}
