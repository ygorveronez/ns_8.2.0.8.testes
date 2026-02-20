namespace MultiSoftware.CTe.v104.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfCteCompVPresComp
    {

        private string vTPrestField;

        private TCTeInfCteInfCteCompVPresCompCompComp[] compCompField;

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
        [System.Xml.Serialization.XmlElementAttribute("compComp")]
        public TCTeInfCteInfCteCompVPresCompCompComp[] compComp
        {
            get
            {
                return this.compCompField;
            }
            set
            {
                this.compCompField = value;
            }
        }
    }
}
