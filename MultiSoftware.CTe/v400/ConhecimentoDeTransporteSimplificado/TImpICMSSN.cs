namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TImpICMSSN
    {

        private TImpICMSSNCST cSTField;

        private TImpICMSSNIndSN indSNField;

        /// <remarks/>
        public TImpICMSSNCST CST
        {
            get
            {
                return this.cSTField;
            }
            set
            {
                this.cSTField = value;
            }
        }

        /// <remarks/>
        public TImpICMSSNIndSN indSN
        {
            get
            {
                return this.indSNField;
            }
            set
            {
                this.indSNField = value;
            }
        }
    }
}