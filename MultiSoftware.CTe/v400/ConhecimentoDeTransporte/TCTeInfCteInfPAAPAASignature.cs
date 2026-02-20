namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporte
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfPAAPAASignature
    {

        private byte[] signatureValueField;

        private TRSAKeyValueType rSAKeyValueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] SignatureValue
        {
            get
            {
                return this.signatureValueField;
            }
            set
            {
                this.signatureValueField = value;
            }
        }

        /// <remarks/>
        public TRSAKeyValueType RSAKeyValue
        {
            get
            {
                return this.rSAKeyValueField;
            }
            set
            {
                this.rSAKeyValueField = value;
            }
        }
    }
}
