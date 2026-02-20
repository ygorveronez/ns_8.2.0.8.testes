namespace MultiSoftware.NFe.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetImpostoPIS
    {

        private object itemField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PISAliq", typeof(TNFeInfNFeDetImpostoPISPISAliq))]
        [System.Xml.Serialization.XmlElementAttribute("PISNT", typeof(TNFeInfNFeDetImpostoPISPISNT))]
        [System.Xml.Serialization.XmlElementAttribute("PISOutr", typeof(TNFeInfNFeDetImpostoPISPISOutr))]
        [System.Xml.Serialization.XmlElementAttribute("PISQtde", typeof(TNFeInfNFeDetImpostoPISPISQtde))]
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
    }
}
