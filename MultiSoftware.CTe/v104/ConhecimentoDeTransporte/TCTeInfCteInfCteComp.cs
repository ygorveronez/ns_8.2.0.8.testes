namespace MultiSoftware.CTe.v104.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfCteComp
    {

        private string chaveField;

        private TCTeInfCteInfCteCompVPresComp vPresCompField;

        private TCTeInfCteInfCteCompImpComp impCompField;

        /// <remarks/>
        public string chave
        {
            get
            {
                return this.chaveField;
            }
            set
            {
                this.chaveField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteInfCteCompVPresComp vPresComp
        {
            get
            {
                return this.vPresCompField;
            }
            set
            {
                this.vPresCompField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteInfCteCompImpComp impComp
        {
            get
            {
                return this.impCompField;
            }
            set
            {
                this.impCompField = value;
            }
        }
    }
}
