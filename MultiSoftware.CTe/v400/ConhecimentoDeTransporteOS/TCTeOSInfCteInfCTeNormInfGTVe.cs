namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteOS
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteInfCTeNormInfGTVe
    {

        private string chCTeField;

        private TCTeOSInfCteInfCTeNormInfGTVeComp[] compField;

        /// <remarks/>
        public string chCTe
        {
            get
            {
                return this.chCTeField;
            }
            set
            {
                this.chCTeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Comp")]
        public TCTeOSInfCteInfCTeNormInfGTVeComp[] Comp
        {
            get
            {
                return this.compField;
            }
            set
            {
                this.compField = value;
            }
        }
    }
}
