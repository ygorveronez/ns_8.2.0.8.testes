namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporte
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfPAA
    {

        private string cNPJPAAField;

        private TCTeInfCteInfPAAPAASignature pAASignatureField;

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
        public TCTeInfCteInfPAAPAASignature PAASignature
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
