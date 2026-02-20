namespace MultiSoftware.CTe.v400.ModalRodoviario
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
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

        private rodoOSInfFretamento infFretamentoField;

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

        /// <remarks/>
        public rodoOSInfFretamento infFretamento
        {
            get
            {
                return this.infFretamentoField;
            }
            set
            {
                this.infFretamentoField = value;
            }
        }
    }
}
