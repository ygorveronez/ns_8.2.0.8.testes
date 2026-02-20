namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteVPrest
    {

        private string vTPrestField;

        private string vRecField;

        private TCTeOSInfCteVPrestComp[] compField;

        /// <remarks/>
        public string vTPrest
        {
            get
            {
                return this.vTPrestField;
            }
            set
            {
                this.vTPrestField = value;
            }
        }

        /// <remarks/>
        public string vRec
        {
            get
            {
                return this.vRecField;
            }
            set
            {
                this.vRecField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Comp")]
        public TCTeOSInfCteVPrestComp[] Comp
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
