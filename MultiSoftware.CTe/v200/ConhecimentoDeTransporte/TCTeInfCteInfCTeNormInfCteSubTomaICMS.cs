namespace MultiSoftware.CTe.v200.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfCTeNormInfCteSubTomaICMS
    {

        private object itemField;

        private ItemChoiceType8 itemElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("refCte", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("refNF", typeof(TCTeInfCteInfCTeNormInfCteSubTomaICMSRefNF))]
        [System.Xml.Serialization.XmlElementAttribute("refNFe", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public object Item
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
        public ItemChoiceType8 ItemElementName
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
    }
}
