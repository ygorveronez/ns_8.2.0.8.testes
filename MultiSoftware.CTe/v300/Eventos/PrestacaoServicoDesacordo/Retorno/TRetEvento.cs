namespace MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Retorno
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute("retEventoCTe", Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class TRetEvento
    {

        private TRetEventoInfEvento infEventoField;

        private MultiSoftware.CTe.v300.Assinaturas.SignatureType signatureField;

        private string versaoField;

        /// <remarks/>
        public TRetEventoInfEvento infEvento
        {
            get
            {
                return this.infEventoField;
            }
            set
            {
                this.infEventoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public MultiSoftware.CTe.v300.Assinaturas.SignatureType Signature
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
