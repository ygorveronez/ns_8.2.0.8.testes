namespace MultiSoftware.CTe.v400.Eventos
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TEventoInfEventoInfPAA
    {

        private string cNPJPAAField;

        private TEventoInfEventoInfPAAPAASignature pAASignatureField;

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
        public TEventoInfEventoInfPAAPAASignature PAASignature
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
