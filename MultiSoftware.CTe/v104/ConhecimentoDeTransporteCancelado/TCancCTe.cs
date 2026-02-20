namespace MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute("cancCTe", Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class TCancCTe
    {

        private TCancCTeInfCanc infCancField;

        private Assinatura.SignatureType signatureField;

        private string versaoField;

        /// <remarks/>
        public TCancCTeInfCanc infCanc
        {
            get
            {
                return this.infCancField;
            }
            set
            {
                this.infCancField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Assinatura.SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string versao
        {
            get
            {
                return this.versaoField;
            }
            set
            {
                this.versaoField = value;
            }
        }
    }
}
