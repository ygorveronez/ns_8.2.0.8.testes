namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteOS
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteInfCTeNormSeg
    {

        private TCTeOSInfCteInfCTeNormSegRespSeg respSegField;

        private string xSegField;

        private string nApolField;

        /// <remarks/>
        public TCTeOSInfCteInfCTeNormSegRespSeg respSeg
        {
            get
            {
                return this.respSegField;
            }
            set
            {
                this.respSegField = value;
            }
        }

        /// <remarks/>
        public string xSeg
        {
            get
            {
                return this.xSegField;
            }
            set
            {
                this.xSegField = value;
            }
        }

        /// <remarks/>
        public string nApol
        {
            get
            {
                return this.nApolField;
            }
            set
            {
                this.nApolField = value;
            }
        }
    }
}
