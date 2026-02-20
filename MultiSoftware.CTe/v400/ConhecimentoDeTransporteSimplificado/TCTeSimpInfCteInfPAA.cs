namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCteInfPAA
    {

        private string cNPJPAAField;

        private TCTeSimpInfCteInfPAAPAASignature pAASignatureField;

        /// <remarks/>
        public string CNPJPAA
        {
            get
            {
                return this.cNPJPAAField;
            }
            set
            {
                this.cNPJPAAField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteInfPAAPAASignature PAASignature
        {
            get
            {
                return this.pAASignatureField;
            }
            set
            {
                this.pAASignatureField = value;
            }
        }
    }
}